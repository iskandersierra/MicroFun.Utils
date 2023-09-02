using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MicroFun.Utils.Commands;

public class RandCommand : Command<RandCommand.Settings>
{
    public class Settings : ByteEncodingSettings
    {
        [CommandArgument(0, "[bytes]")]
        [Description("Number of bytes to generate. Default is 16.")]
        [DefaultValue(16)]
        public int BytesCount { get; set; }

        [CommandOption("-l|--max-length")]
        [Description("Maximum length of the output. Default is unlimited.")]
        [DefaultValue(0)]
        public int MaxLength { get; set; }

        public override ValidationResult Validate()
        {
            if (BytesCount < 1)
            {
                return ValidationResult.Error("BytesCount must be greater than 0");
            }

            if (MaxLength < 1)
            {
                MaxLength = int.MaxValue;
            }

            return base.Validate();
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var input = ToInput(settings);

        var output = Rand.Execute(input);

        settings.Print(output);

        return 0;
    }

    private RandInput ToInput(Settings settings)
    {
        return new RandInput
        {
            BytesCount = settings.BytesCount,
            MaxLength = settings.MaxLength,
            ByteEncoding = settings.ToByteEncoding(),
        };
    }
}
