using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using FTP_Browser.Objects;

namespace FTP_Browser.Controllers
{
    public class RemoteBrowserController
    {
        public static readonly RemoteDirInfo DirInfo = new RemoteDirInfo();
        public static readonly FtpCredentials Credentials = new FtpCredentials();
        public static readonly UriObject MyUriObject = new UriObject();

        public static IEnumerable<FtpItem> RemoteGoTo()
        {
            // Test if goTo booleans are both true or false at same time, if yes, return false.
            var sameSame = (MyUriObject.GoNext == MyUriObject.GoBack);

            if (sameSame)
            {
                MessageBox.Show("Conflict information was given to the UriHandler", "ERROR in code");
                throw new Exception("Error, conflict information was given to the UriHandler");
            }

            // Reorganize MyUriObject
            var next = MyUriObject.NextUri;
            var current = MyUriObject.CurrentUri;
            var previous = MyUriObject.PreviousUri;

            if (MyUriObject.GoNext)
            {
                MyUriObject.CurrentUri = next;
                MyUriObject.PreviousUri = current;

                MyUriObject.GoNext = false;
            }
            else
            {
                MyUriObject.CurrentUri = previous;
                MyUriObject.NextUri = current;

                MyUriObject.GoBack = false;
            }

            // ReSharper disable once InconsistentNaming
            var RB = new RemoteBrowserController();
            return RB.GetRemoteItems(Credentials, MyUriObject.CurrentUri);
        }

        public IEnumerable<FtpItem> GetRemoteItems(FtpCredentials credentials, Uri myUri)
        {
            var request = (FtpWebRequest) WebRequest.Create(myUri);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(credentials.User, credentials.Password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            var returnList = new List<FtpItem>();

            using var response = (FtpWebResponse)request.GetResponse();
            using var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
            var list = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in list)
            {
                var parts = line.Split(" ").ToList();
                //parts = parts.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToList();

                static bool IsDirectory(string str)
                {
                    return str.StartsWith("d") || str.StartsWith("l");
                }

                static string ParseDate(string str1, string str2, string str3)
                {
                    if (str3.Contains(":"))
                    {
                        var myMonth = (int)(DateTimeController.Month) Enum.Parse(typeof(DateTimeController.Month), str1);
                        var myDays = Convert.ToInt32(str2);
                        var myTime = str3.Split(":");
                        var myHours = Convert.ToInt32(myTime[0]);
                        var myMinutes = Convert.ToInt32(myTime[1]);

                        var date1 = new DateTime(year: 2020 , myMonth, myDays, myHours, myMinutes, 15);

                        var myDate = date1.ToString("dd/MM H:mm");
                        return myDate;
                    }
                    return default;
                }

                static string Type(bool isDir, string name)
                {
                    if (isDir)
                    {
                        return "File folder";
                    }

                    var thisType = name.Split(".");
                    try
                    {
                        return thisType[1] + " File".ToUpper();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return "File";
                    }
                }
                
                var permission = parts[0];
                //var weirdPart = parts[1];
                var group = parts[2];
                var owner = parts[3];
                var size = parts[4];
                var name = parts[8];
                var isDirectory = IsDirectory(permission);
                var type = Type(isDirectory, name);

                string dateTime;
                try
                {
                    if (parts[7].Contains(":"))
                    {
                        dateTime = ParseDate(parts[5], parts[6], parts[7]);
                    }
                    else
                    {
                        dateTime = DateTime.Parse($"{parts[5]} {parts[6]} {parts[7]}").ToString(CultureInfo.CurrentCulture);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("The DateTime represented by the string"))
                    {
                        throw new Exception(ex.Message);
                    }
                    dateTime = "Error";
                }
                
                // Total counters
                if (isDirectory && name != "." && name != "..")
                {
                    DirInfo.DirectoryCount++;
                    DirInfo.DirectorySize += Convert.ToInt32(size);
                }
                else if (!isDirectory && name != "." && name != "..")
                {
                    DirInfo.FileCount++;
                    DirInfo.DirectorySize += Convert.ToInt32(size);
                }

                FtpItem item;

                if (name != "..")
                {
                    item = new FtpItem
                    {
                        BaseUri = new Uri(credentials.Host),
                        FileUri = new Uri(credentials.Host + "/" + name),
                        Permission = permission,
                        Group = group,
                        Owner = owner,
                        Size = long.Parse(size),
                        DateCreated = dateTime.ToString(CultureInfo.CurrentCulture),
                        IsDirectory = isDirectory,
                        Type = type,
                        Name = name,
                        IsEnabled = true
                    };
                }
                else
                {
                    item = new FtpItem
                    {
                        Name = name,
                        IsDirectory = isDirectory,
                        DateCreated = null
                    };
                }

                if (name != ".")
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }
    }
}
