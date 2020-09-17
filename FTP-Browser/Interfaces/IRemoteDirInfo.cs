namespace FTP_Browser.Interfaces
{
    public interface IRemoteDirInfo
    {
        int DirectoryCount { get; set; }
        int FileCount { get; set; }
        long DirectorySize { get; set; }
    }
}