using System;

namespace FTP_Browser.Interfaces
{
    public interface IUriObject
    {
        Uri CurrentUri { get; set; }
        Uri NextUri { get; set; }
        Uri PreviousUri { get; set; }
        bool GoNext { get; set; }
        bool GoBack { get; set; }
    }
}
