using FTP_Browser.Interfaces;

namespace FTP_Browser.Objects
{
    public class FtpCredentials : IFtpCredentials
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}