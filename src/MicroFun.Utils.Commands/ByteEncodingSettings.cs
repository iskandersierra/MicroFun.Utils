using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace MicroFun.Utils.Commands;

public abstract class ByteEncodingSettings : OutputSettings
{
    [CommandOption("--unpadded")]
    [Description("Output as base64 without padding.")]
    [DefaultValue(false)]
    public bool Unpadded { get; set; }

    [CommandOption("--hex|--hexadecimal")]
    [Description("Output as hexadecimal in lower case.")]
    [DefaultValue(false)]
    public bool HexLower { get; set; }

    [CommandOption("--HEX|--HEXADECIMAL")]
    [Description("Output as hexadecimal in upper case.")]
    [DefaultValue(false)]
    public bool HexUpper { get; set; }

    public override ValidationResult Validate()
    {
        var countEncodings = new[] { HexLower, HexUpper }.Count(x => x);

        if (countEncodings > 1)
        {
            return ValidationResult.Error("Only one of HexLower or HexUpper can be specified");
        }

        return base.Validate();
    }

    public ByteEncoding ToByteEncoding()
    {
        return HexLower
            ? ByteEncoding.HexLower
            : HexUpper
                ? ByteEncoding.HexUpper
                : Unpadded
                    ? ByteEncoding.Base64Unpadded
                    : ByteEncoding.Base64;
    }
}
