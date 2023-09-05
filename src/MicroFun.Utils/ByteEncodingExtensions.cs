namespace MicroFun.Utils;

public enum ByteEncoding
{
    HexUpper,
    HexLower,
    Base64,
    Base64Unpadded,
}

public static class ByteEncodingExtensions
{
    public static string EncodeBytes(this ByteEncoding encoding, byte[] bytes)
    {
        return encoding switch
        {
            ByteEncoding.HexUpper => BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant(),
            ByteEncoding.HexLower => BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant(),
            ByteEncoding.Base64Unpadded => Convert.ToBase64String(bytes).TrimEnd('='),
            ByteEncoding.Base64 => Convert.ToBase64String(bytes),
            _ => throw new ArgumentException("Invalid ByteEncoding", nameof(encoding)),
        };
    }
}
