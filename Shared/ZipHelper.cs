using System.IO;
using System.IO.Compression;
using System.Text;

namespace ExistHelper.Shared
{
    public static class ZipHelper
    {
        public static byte[] CreateZipWithJson(string json, string jsonFileName)
        {
            using var memStream = new MemoryStream();
            using (var archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry(jsonFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                writer.Write(json);
            }
            return memStream.ToArray();
        }
    }
}