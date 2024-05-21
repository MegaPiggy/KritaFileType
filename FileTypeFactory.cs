using PaintDotNet;

namespace KritaFileType
{
    public class FileTypeFactory : IFileTypeFactory
    {
        public FileType[] GetFileTypeInstances()
        {
            return new FileType[]
            {
                new KraFileType()
            };
        }
    }
}
