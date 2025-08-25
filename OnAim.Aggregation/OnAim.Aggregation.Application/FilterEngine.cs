using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application;

public static class EventFilter
{
    public static bool Matches(object @event, List<FilterClause> filters)
    {
        if (filters.Count == 0) return false;
        foreach (var f in filters)
            if (!Evaluate(@event, f)) return false;
        return true;
    }

    private static bool Evaluate(object root, FilterClause f)
    {
        if (TryGetValueByPath(root, f.Field, out var fieldValue))
        {

            switch (f.Op)
            {
                case FilterOperator.Regex:
                    return RegexMatch(fieldValue, f.Value);
                case FilterOperator.Eq:
                    return EqualityMatch(fieldValue, f.Value, expectEqual: true);
                case FilterOperator.Ne:
                    return EqualityMatch(fieldValue, f.Value, expectEqual: false);
                case FilterOperator.Gt:
                case FilterOperator.Lt:
                    return RangeMatch(fieldValue, f.Value, f.Op);
                default:
                    return false;
            }
        }

        return false;
    }

    private static readonly Regex TokenRx = new(@"^(?<name>[^\[\]]+)(?:\[(?<idx>\d+)\])?$",
        RegexOptions.Compiled);

    private static bool TryGetValueByPath(object obj, string path, out object? value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(path)) return false;

        object? current = obj;
        foreach (var raw in path.Split('.'))
        {
            var m = TokenRx.Match(raw);
            if (!m.Success) return false;

            var name = m.Groups["name"].Value;
            int? idx = m.Groups["idx"].Success ? int.Parse(m.Groups["idx"].Value) : null;

            if (!TryGetMember(current, name, out current)) return false;

            if (idx.HasValue)
            {
                if (!TryIndex(current, idx.Value, out current)) return false;
            }
        }

        value = current;
        return true;
    }

    private static bool TryGetMember(object? obj, string name, out object? value)
    {
        value = null;
        if (obj is null) return false;

        if (obj is IDictionary<string, object> dict1)
            return dict1.TryGetValue(name, out value);

        if (obj is IDictionary dict)
        {
            foreach (DictionaryEntry de in dict)
                if (de.Key is string s && s == name) { value = de.Value; return true; }
            return false;
        }

        var t = obj.GetType();
        var prop = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (prop != null) { value = prop.GetValue(obj); return true; }

        var field = t.GetField(name, BindingFlags.Public | BindingFlags.Instance);
        if (field != null) { value = field.GetValue(obj); return true; }

        return false;
    }

    private static bool TryIndex(object? obj, int index, out object? value)
    {
        value = null;
        if (obj is null) return false;

        if (obj is IList list)
        {
            if (index < 0 || index >= list.Count) return false;
            value = list[index];
            return true;
        }

        if (obj is IEnumerable en && obj is not string)
        {
            int i = 0;
            foreach (var item in en)
            {
                if (i++ == index) { value = item; return true; }
            }
        }
        return false;
    }


    private static bool EqualityMatch(object? field, BsonValue expectedBson, bool expectEqual)
    {
        var expected = FromBson(expectedBson);

        // If field is an enumerable (not string), "Eq" passes if ANY element equals expected
        if (field is IEnumerable en and not string)
        {
            foreach (var el in en)
                if (LooseEquals(el, expected)) return expectEqual;
            return !expectEqual;
        }

        return expectEqual ? LooseEquals(field, expected) : !LooseEquals(field, expected);
    }

    private static bool InMatch(object? field, BsonValue optionsBson, bool wantContain)
    {
        if (optionsBson is not BsonArray set) return false;
        var opts = new List<object?>(set.Count);
        foreach (var v in set) opts.Add(FromBson(v));

        if (field is IEnumerable en && field is not string)
        {
            bool any = false;
            foreach (var el in en)
            {
                if (Contains(opts, el)) { any = true; break; }
            }
            return wantContain ? any : !any;
        }
        else
        {
            bool contains = Contains(opts, field);
            return wantContain ? contains : !contains;
        }
    }

    private static bool RegexMatch(object? field, BsonValue patternValue)
    {
        if (field is not string s) return false;

        string pattern;
        RegexOptions opts = RegexOptions.None;

        if (patternValue is BsonRegularExpression bre)
        {
            pattern = bre.Pattern;
            string o = (bre.Options ?? "").ToLowerInvariant();
            if (o.Contains("i")) opts |= RegexOptions.IgnoreCase;
            if (o.Contains("m")) opts |= RegexOptions.Multiline;
            if (o.Contains("s")) opts |= RegexOptions.Singleline;
        }
        else
        {
            pattern = patternValue?.ToString() ?? "";
        }

        return Regex.IsMatch(s, pattern, opts);
    }

    private static bool RangeMatch(object? field, BsonValue rhsBson, FilterOperator op)
    {
        var rhs = FromBson(rhsBson);
        if (!TryCompare(field, rhs, out int cmp)) return false;

        return op switch
        {
            FilterOperator.Gt  => cmp > 0,
            FilterOperator.Lt  => cmp < 0,
            _ => false
        };
    }

    private static object? FromBson(BsonValue v)
    {
        if (v.IsBsonNull) return null;
        if (v.IsBoolean)   return v.AsBoolean;
        if (v.IsString)    return v.AsString;
        if (v.IsInt32)     return v.AsInt32;
        if (v.IsDecimal128) return (decimal)v.AsDecimal128;
        if (v.IsValidDateTime) return v.ToUniversalTime();
        if (v is BsonDateTime bdt) return bdt.ToUniversalTime();
        if (v is BsonArray arr)
        {
            var list = new List<object?>(arr.Count);
            foreach (var x in arr) list.Add(FromBson(x));
            return list;
        }
        if (v is BsonRegularExpression bre) return bre; // handled in RegexMatch
        return v.ToString();
    }

    private static bool Contains(List<object?> set, object? x)
    {
        foreach (var o in set)
            if (LooseEquals(o, x)) return true;
        return false;
    }

    private static bool LooseEquals(object? a, object? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        // Numbers
        if (IsNumber(a) && IsNumber(b))
            return ToDecimal(a) == ToDecimal(b);

        // DateTime / DateTimeOffset
        if (TryToUtc(a, out var adt) && TryToUtc(b, out var bdt))
            return adt == bdt;

        // Guid convenience: allow string vs Guid equality
        if (a is Guid ga && TryToGuid(b, out var gb)) return ga == gb;
        if (b is Guid gb2 && TryToGuid(a, out var ga2)) return gb2 == ga2;

        // Bool
        if (a is bool ab && b is bool bb) return ab == bb;

        // String
        if (a is string sa && b is string sb) return string.Equals(sa, sb, StringComparison.Ordinal);

        // Fallback: IComparable equal?
        return a.Equals(b);
    }

    private static bool TryCompare(object? a, object? b, out int cmp)
    {
        cmp = 0;
        if (a is null || b is null) return false;

        // Numbers
        if (IsNumber(a) && IsNumber(b))
        {
            var da = ToDecimal(a);
            var db = ToDecimal(b);
            cmp = da.CompareTo(db);
            return true;
        }

        if (TryToUtc(a, out var adt) && TryToUtc(b, out var bdt))
        {
            cmp = adt.CompareTo(bdt);
            return true;
        }

        if (a is bool ab && b is bool bb)
        {
            cmp = ab.CompareTo(bb);
            return true;
        }

        if (a is string sa && b is string sb)
        {
            cmp = string.CompareOrdinal(sa, sb);
            return true;
        }

        // default comparer
        if (a.GetType() == b.GetType() && a is IComparable ic)
        {
            cmp = ic.CompareTo(b);
            return true;
        }

        return false;
    }


    // Assume int for now; extend if needed
    private static bool IsNumber(object o) =>
        o is int or decimal ;

    private static decimal ToDecimal(object o) => o switch
    {
        int v => v,
        float v => (decimal)v,
        double v => (decimal)v,
        decimal v => v,
        _ => throw new InvalidOperationException("Not numeric")
    };

    private static bool TryToUtc(object o, out DateTime dt)
    {
        if (o is DateTime d) { dt = d.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(d, DateTimeKind.Utc) : d.ToUniversalTime(); return true; }
        if (o is DateTimeOffset dto) { dt = dto.UtcDateTime; return true; }
        dt = default; return false;
    }

    private static bool TryToGuid(object o, out Guid g)
    {
        if (o is Guid gg) { g = gg; return true; }
        if (o is string s && Guid.TryParse(s, out var p)) { g = p; return true; }
        g = default; return false;
    }
}