using Spectre.Console.Cli;
using System.ComponentModel;
using MicroFun.Utils.FileSystem;
using Spectre.Console;

namespace MicroFun.Utils.Commands.FileSystem;

public class FileStatsCommand : Command<FileStatsCommand.Settings>
{
    public class Settings : OutputSettings
    {
        [CommandArgument(0, "[folder]")]
        [Description("Folder to scan. Default is current folder.")]
        [DefaultValue(null)]
        public string? Folder { get; set; } = null;

        [CommandArgument(1, "[pattern]")]
        [Description("File pattern to match. Default is *.*")]
        [DefaultValue("*.*")]
        public string Pattern { get; set; } = "*.*";

        [CommandOption("--no-recurse")]
        [Description("Do not recurse into subfolders.")]
        [DefaultValue(false)]
        public bool NoRecurse { get; set; } = false;

        [CommandOption("-d|--depth")]
        [Description("Maximum depth to recurse into subfolders. Default is infinite.")]
        [DefaultValue(int.MaxValue)]
        public int MaxDepth { get; set; } = int.MaxValue;

        [CommandOption("--no-hidden")]
        [Description("Do not count hidden files")]
        [DefaultValue(false)]
        public bool NoHidden { get; set; } = false;

        [CommandOption("--no-system")]
        [Description("Do not count system files")]
        [DefaultValue(false)]
        public bool NoSystem { get; set; } = false;

        [CommandOption("-s|--sort")]
        [Description("Field to sort results. By default is by Sum")]
        [DefaultValue(BucketStatsBinField.Sum)]
        public BucketStatsBinField SortBy { get; set; } = BucketStatsBinField.Sum;

        [CommandOption("-a|--asc|--ascending")]
        [Description("Sort results in ascending order. By default is descending.")]
        [DefaultValue(false)]
        public bool Ascending { get; set; } = false;

        public override ValidationResult Validate()
        {
            if (Folder is null) Folder = Environment.CurrentDirectory;

            if (!Directory.Exists(Folder))
            {
                return ValidationResult.Error($"Folder '{Folder}' does not exist");
            }

            Folder = Path.GetFullPath(Folder);

            return base.Validate();
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var input = ToInput(settings);

        var output = FileStats.Execute(input);

        settings.Print(output);

        return 0;
    }

    private FileStatsInput ToInput(Settings settings)
    {
        return new FileStatsInput
        {
            Folder = settings.Folder!,
            Pattern = settings.Pattern,
            Recurse = !settings.NoRecurse,
            MaxDepth = settings.MaxDepth,
            NoHidden = settings.NoHidden,
            NoSystem = settings.NoSystem,
            SortBy = settings.SortBy,
            Ascending = settings.Ascending,
        };
    }
}
