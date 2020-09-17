using System;

namespace FTP_Browser.Interfaces
{
    internal interface IFileItem
    {
        string Name { get; set; }
        long? Size { get; set; }
        string CurrentPath { get; set; }
        string Type { get; set; }
        string Permission { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        bool IsDirectory { get; set; }
        bool IsEnabled { get; set; }
        Uri BaseUri { get; set; }
        Uri FileUri { get; set; }
        string DateCreated { get; set; }
    }
}