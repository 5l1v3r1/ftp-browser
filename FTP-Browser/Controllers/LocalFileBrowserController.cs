using System.Collections.Generic;
using System.IO;
using System.Linq;
using FTP_Browser.Objects;

namespace FTP_Browser.Controllers
{
    class LocalFileBrowserController
    {
        public static IEnumerable<LocalItem> GetLocalItemInfo()
        {
            var localItems = new List<LocalItem>();

            var di = new DirectoryInfo(@"C:\");
            var fiArr = di.GetFiles();
            var diArr = di.GetDirectories();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in fiArr)
            {
                var fileSize = file.Length;
                var fileSegment = file.ToString().Split(@"\").Last();
                var typeSegment = file.ToString().Split(".").Last() + " File";

                localItems.Add(new LocalItem() { Name = fileSegment, Type = typeSegment, Size = fileSize, IsDirectory = false });
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var dir in diArr)
            {
                var dirSegment = dir.ToString().Split(@"\").Last();
                localItems.Add(new LocalItem() { Name = dirSegment, Type = "dir", Size = null, IsDirectory = true });
            }

            return localItems;
        }
    }
}
