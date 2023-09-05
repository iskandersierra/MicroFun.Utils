namespace MicroFun.Utils;

public enum BucketStatsBinField
{
    Name,
    Count,
    Sum,
    Max,
    Min,
    Mean,
    StDev,
}

public record BucketStatsBin(string Name, StatsBin Stats)
{
    public static readonly string[] TableHeaders = new[] { "Bucket" }.Concat(StatsBin.TableHeaders).ToArray();

    public string ToString(string format, string unit)
    {
        return $"{Name}: {Stats.ToString(format, unit)}";
    }

    public IEnumerable<string> GetTableFields(string format, string unit)
    {
        yield return Name;
        foreach (var field in Stats.GetTableFields(format, unit))
        {
            yield return field;
        }
    }

    public class FieldComparer : IComparer<BucketStatsBin>
    {
        private readonly BucketStatsBinField field;

        public FieldComparer(BucketStatsBinField field = BucketStatsBinField.Count)
        {
            this.field = field;
        }

        public int Compare(BucketStatsBin? x, BucketStatsBin? y)
        {
            if (x is null) return y is null ? 0 : -1;
            if (y is null) return 1;

            return field switch
            {
                BucketStatsBinField.Name => string.Compare(x.Name, y.Name, StringComparison.Ordinal),
                BucketStatsBinField.Count => x.Stats.Count.CompareTo(y.Stats.Count),
                BucketStatsBinField.Sum => x.Stats.Sum.CompareTo(y.Stats.Sum),
                BucketStatsBinField.Max => x.Stats.Max.CompareTo(y.Stats.Max),
                BucketStatsBinField.Min => x.Stats.Min.CompareTo(y.Stats.Min),
                BucketStatsBinField.Mean => x.Stats.Mean.CompareTo(y.Stats.Mean),
                BucketStatsBinField.StDev => x.Stats.StDev.CompareTo(y.Stats.StDev),
                _ => throw new ArgumentOutOfRangeException(nameof(field)),
            };
        }

        public static IComparer<BucketStatsBin> CreateComparer(BucketStatsBinField field, bool descending = false)
        {
            var comparer = new FieldComparer(field);
            return descending ? comparer.Reverse() : comparer;
        }
    }
}
