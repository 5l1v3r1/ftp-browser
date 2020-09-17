namespace FTP_Browser.Interfaces
{
    public interface IFtpCredentials
    {
        string Host { get; set; }
        string User { get; set; }
        string Password { get; set; }
    }
}