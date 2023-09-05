namespace MicroFun.Utils;

public enum StatsBinField
{
    Count,
    Sum,
    Max,
    Min,
    Mean,
    StDev,
}

public record StatsBin(int Count, double Sum, double Min, double Max, double SumSq)
{
    public static IEnumerable<string> TableHeaders = new[] { "Count", "Sum", "Max", "Min", "Mean", "StDev" };

    public static StatsBin Empty { get; } = new(0, 0, 0, 0, 0);

    public static StatsBin One(double value)
    {
        CheckValue(value);

        return new StatsBin(1, value, value, value, value * value);
    }

    public static void CheckValue(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentException($"Invalid non-finite value: {value}", nameof(value));
    }

    public bool IsEmpty => Count == 0;

    public double Mean => Count == 0 ? 0 : Sum / Count;

    public double StDev => Math.Sqrt(Math.Max(SumSq / Count - Mean * Mean, 0));

    public StatsBin Add(double value) => Add(One(value));

    public StatsBin Add(StatsBin other)
    {
        if (other.IsEmpty) return this;
        if (IsEmpty) return other;

        var count = Count + other.Count;
        var sum = Sum + other.Sum;
        var min = Math.Min(Min, other.Min);
        var max = Math.Max(Max, other.Max);
        var sumSq = SumSq + other.SumSq;
        return new StatsBin(count, sum, min, max, sumSq);
    }

    public string ToString(string format, string unit)
    {
        string Fmt(double x) => $"{x.ToString(format)}{unit}";
        return $"count = {Count}; sum = {Fmt(Sum)}; max = {Fmt(Max)}; min = {Fmt(Min)}; avg = {Fmt(Mean)} \u00b1 {Fmt(StDev)}";
    }

    public IEnumerable<string> GetTableFields(string format, string unit)
    {
        string Fmt(double x) => $"{x.ToString(format)}{unit}";

        yield return Count.ToString();
        yield return Fmt(Sum);
        yield return Fmt(Max);
        yield return Fmt(Min);
        yield return Fmt(Mean);
        yield return Fmt(StDev);
    }

    public class FieldComparer : IComparer<StatsBin>
    {
        private readonly StatsBinField field;

        public FieldComparer(StatsBinField field = StatsBinField.Count)
        {
            this.field = field;
        }

        public int Compare(StatsBin? x, StatsBin? y)
        {
            if (x is null) return y is null ? 0 : -1;
            if (y is null) return 1;

            return field switch
            {
                StatsBinField.Count => x.Count.CompareTo(y.Count),
                StatsBinField.Sum => x.Sum.CompareTo(y.Sum),
                StatsBinField.Max => x.Max.CompareTo(y.Max),
                StatsBinField.Min => x.Min.CompareTo(y.Min),
                StatsBinField.Mean => x.Mean.CompareTo(y.Mean),
                StatsBinField.StDev => x.StDev.CompareTo(y.StDev),
                _ => throw new ArgumentException($"Invalid field: {field}", nameof(field)),
            };
        }
    }
}
