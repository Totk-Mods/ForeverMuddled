using CommunityToolkit.HighPerformance.Buffers;
using Revrs.Buffers;
using TotkCommon;

namespace ForeverMuddled.Helpers;

static class DataHelper
{
    public static ArraySegmentOwner<byte> ReadAndDecompress(string path, out int dictioanryId)
    {
        using FileStream fs = File.OpenRead(path);
        int size = Convert.ToInt32(fs.Length);
        using SpanOwner<byte> raw = SpanOwner<byte>.Allocate(size);
        fs.Read(raw.Span);
        ArraySegmentOwner<byte> decompressed = ArraySegmentOwner<byte>.Allocate(Zstd.GetDecompressedSize(raw.Span));
        Totk.Zstd.Decompress(raw.Span, decompressed.Segment, out dictioanryId);
        return decompressed;
    }
}
