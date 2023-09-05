using System.Text;
using System.Text.Json.Serialization;

namespace MicroFun.Utils.FileSystem;

public class FileStatsInput
{
    public string Folder { get; set; }
    public string Pattern { get; set; }
    public bool Recurse { get; set; }
    public int MaxDepth { get; set; }
    public bool NoHidden { get; set; }
    public bool NoSystem { get; set; }

    public BucketStatsBinField SortBy { get; set; }
    public bool Ascending { get; set; }

    public EnumerationOptions ToEnumerationOptions()
    {
        var attrToSkip = (FileAttributes)0;

        if (NoHidden) attrToSkip |= FileAttributes.Hidden;
        if (NoSystem) attrToSkip |= FileAttributes.System;

        return new EnumerationOptions
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = Recurse,
            MatchCasing = MatchCasing.PlatformDefault,
            MatchType = MatchType.Simple,
            MaxRecursionDepth = MaxDepth == 0 ? int.MaxValue : MaxDepth,
            AttributesToSkip = attrToSkip,
        };
    }
}

public class FileStatsOutput : ITableOutput
{
    private const string ValueFormat = "#,##0.0";

    [JsonIgnore]
    public string StatName { get; set; }

    [JsonIgnore]
    public string Unit { get; set; }

    public BucketStatsBin TotalBin { get; set; }
    public IEnumerable<BucketStatsBin> Bins { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendFormat("{0}", StatName).AppendLine();

        sb.AppendFormat("- {0}", TotalBin.ToString(ValueFormat, Unit)).AppendLine();

        foreach (var bucket in Bins)
        {
            sb.AppendFormat("- {0}", bucket.ToString(ValueFormat, Unit)).AppendLine();
        }

        return sb.ToString();
    }

    IEnumerable<string> ITableOutput.GetHeaders() => BucketStatsBin.TableHeaders;

    IEnumerable<IEnumerable<string>> ITableOutput.GetRows()
    {
        yield return TotalBin.GetTableFields(ValueFormat, Unit);

        foreach (var bucket in Bins)
        {
            yield return bucket.GetTableFields(ValueFormat, Unit);
        }
    }
}

public static class FileStats
{
    public static FileStatsOutput Execute(FileStatsInput input)
    {
        string GetFileBucket(FileInfo fileInfo)
        {
            return fileInfo.Extension.ToLowerInvariant();
        }

        double GetFileValue(FileInfo fileInfo)
        {
            return fileInfo.Length * 1.0 / 1024;
        }

        var totalBin = StatsBin.Empty;
        var bins = new Dictionary<string, StatsBin>();

        var files = Directory.EnumerateFiles(input.Folder, input.Pattern, input.ToEnumerationOptions());

        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);

            var value = GetFileValue(fileInfo);

            totalBin = totalBin.Add(value);

            var bucket = GetFileBucket(fileInfo);

            if (!bins.TryGetValue(bucket, out var bin))
            {
                bin = StatsBin.Empty;
                bins.Add(bucket, bin);
            }

            bin = bin.Add(value);
            bins[bucket] = bin;
        }

        var buckets = bins.Select(x => new BucketStatsBin(x.Key, x.Value));

        var bucketComparer = BucketStatsBin.FieldComparer.CreateComparer(input.SortBy, descending: !input.Ascending);

        buckets = buckets.OrderBy(x => x, bucketComparer).ToList();

        var totalBucket = new BucketStatsBin("Total", totalBin);

        return new FileStatsOutput
        {
            StatName = "File Size",
            Unit = "Kb",
            TotalBin = totalBucket,
            Bins = buckets,
        };
    }
}
