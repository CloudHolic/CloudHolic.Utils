using System.IO;

namespace CloudHolic.Utils.Extensions;

public static class BinaryReaderExtensions
{
    public static bool Eof(this BinaryReader reader)
    {
        var bs = reader.BaseStream;
        return bs.Position == bs.Length;
    }
}
