using System;
using FTP_Browser.Interfaces;

namespace FTP_Browser.Objects
{
    public class UriObject : IUriObject
    {
        public Uri CurrentUri { get; set; }
        public Uri NextUri { get; set; }
        public Uri PreviousUri { get; set; }
        public bool GoNext { get; set; }
        public bool GoBack { get; set; }
    }
}
