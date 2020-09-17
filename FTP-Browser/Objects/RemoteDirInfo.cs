using FTP_Browser.Interfaces;

namespace FTP_Browser.Objects
{
    public class RemoteDirInfo : IRemoteDirInfo
    {
        public int DirectoryCount { get; set; }
        public int FileCount { get; set; }
        public long DirectorySize { get; set; }
    }
}
