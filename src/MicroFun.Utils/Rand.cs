using System.Security.Cryptography;

namespace MicroFun.Utils;

public class RandInput
{
    public int BytesCount { get; set; }
    public int MaxLength { get; set; }

    public ByteEncoding ByteEncoding { get; set; }
}

public class RandOutput : ITableOutput
{
    public string Encoded { get; set; }

    public override string ToString()
    {
        return Encoded;
    }

    IEnumerable<string> ITableOutput.GetHeaders()
    {
        yield return "Encoded";
    }

    IEnumerable<IEnumerable<string>> ITableOutput.GetRows()
    {
        yield return new[] { Encoded };
    }
}

public static class Rand
{
    public static RandOutput Execute(RandInput input)
    {
        if (input.BytesCount < 1)
        {
            throw new ArgumentException("BytesCount must be greater than 0", nameof(input.BytesCount));
        }

        if (input.MaxLength < 1)
        {
            throw new ArgumentException("MaxLength must be greater than 0", nameof(input.MaxLength));
        }

        var bytes = RandomNumberGenerator.GetBytes(input.BytesCount);

        var encoded = input.ByteEncoding.EncodeBytes(bytes);

        if (encoded.Length > input.MaxLength)
        {
            encoded = encoded.Substring(0, input.MaxLength);
        }

        return new RandOutput
        {
            Encoded = encoded,
        };
    }
}
