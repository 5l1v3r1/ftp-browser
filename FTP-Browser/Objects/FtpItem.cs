using System;
using FTP_Browser.Interfaces;

namespace FTP_Browser.Objects
{
    public class FtpItem : IFileItem
    {
        public string Name { get; set; }
        public long? Size { get; set; }
        public string CurrentPath { get; set; }
        public string Type { get; set; }
        public string Permission { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsEnabled { get; set; }
        public Uri BaseUri { get; set; }
        public Uri FileUri { get; set; }
        public string DateCreated { get; set; }
    }
}
