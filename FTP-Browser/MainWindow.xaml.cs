using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTP_Browser.Controllers;
using FTP_Browser.Objects;
// ReSharper disable RedundantExtendsListEntry

namespace FTP_Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Create a function to handle copy commands from remote form.
        /// </summary>
        private readonly List<FtpItem> _copyRemoteList = new List<FtpItem>();

        /// <summary>
        /// Create a function to handle copy commands from local form.
        /// </summary>
        private readonly List<LocalItem> _copyLocalList = new List<LocalItem>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _copyRemoteList.Clear();

            var myItem = lbFTPBrowse.SelectedItems;

            foreach (var t in myItem)
            {
                var copiedItem = t as FtpItem;
                _copyRemoteList.Add(copiedItem);
            }
        }

        private void Lbi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // This one is for the event handler, it indicates that the event has been completed
            e.Handled = true;

            // If a double click is not made on FtpItem in the ListView, this function will be suspended
            if (!(lbFTPBrowse.SelectedItem is FtpItem myItem)) return;

            var remoteFileBrowser = new RemoteBrowserController();
           
            // This local little function is for update the info label of remote browser
            static string UpdateLabel()
            {
                return $"{RemoteBrowserController.DirInfo.FileCount} files and {RemoteBrowserController.DirInfo.DirectoryCount} directories. Total size: {RemoteBrowserController.DirInfo.DirectorySize} bytes";
            }

            // Move remote file browser to next page if double-clicking on the correct FtpItem
            if (myItem.Name != "..")
            {
                RemoteBrowserController.MyUriObject.NextUri = new Uri(RemoteBrowserController.MyUriObject.CurrentUri + myItem.Name + "/");
                lbFTPBrowse.ItemsSource = remoteFileBrowser.GetRemoteItems(RemoteBrowserController.Credentials, RemoteBrowserController.MyUriObject.NextUri);
                RemoteBrowserController.MyUriObject.CurrentUri = RemoteBrowserController.MyUriObject.NextUri;

                LabelRemoteDirInfo.Content = UpdateLabel();
            }
            else // Move remote file browser back to previous page if it possible if double-clicking on the FtpItem named by ".."
            {
                var segmentCount = RemoteBrowserController.MyUriObject.CurrentUri.Segments.Count();

                if (segmentCount <= 1) return;

                // remove last segment from current Uri
                var current = RemoteBrowserController.MyUriObject.CurrentUri.ToString();
                current = current.EndsWith("/") ? current.Remove(current.Length - 1, 1) : current;
                var previous = current.Substring(0, current.LastIndexOf("/", StringComparison.Ordinal)) + "/";
                RemoteBrowserController.MyUriObject.PreviousUri = new Uri(previous);

                // update remote browser data
                lbFTPBrowse.ItemsSource = remoteFileBrowser.GetRemoteItems(RemoteBrowserController.Credentials, RemoteBrowserController.MyUriObject.PreviousUri);
                RemoteBrowserController.MyUriObject.CurrentUri = RemoteBrowserController.MyUriObject.PreviousUri;

                LabelRemoteDirInfo.Content = UpdateLabel();
            }
        }

        /// <summary>
        /// Initialize FTP Credentials to memory
        /// </summary>
        /// <returns>Returns True if Object not contains any empty strings</returns>
        private bool InitializeFtpCredentials()
        {
            // Initialize provided information to FTP credentials object
            RemoteBrowserController.Credentials.Host = TbHost.Text;
            RemoteBrowserController.Credentials.User = TbUser.Text;
            RemoteBrowserController.Credentials.Password = TbPassword.Text;

            // return false if some value on FTP credentials object is null or WhiteSpace 
            return !RemoteBrowserController.Credentials.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType == typeof(string))
                .Select(pi => (string) pi.GetValue(RemoteBrowserController.Credentials))
                .Where(string.IsNullOrWhiteSpace)
                .Any();
        }

        /// <summary>
        /// The "Go" button in form. Start new FTP "session" on click if "InitializeFtpCredentials" method returns true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Interrupt this function if the required information has not been provided
            if (!InitializeFtpCredentials()) return;

            // Bind local items to form
            lbLocalBrowse.ItemsSource = LocalFileBrowserController.GetLocalItemInfo();

            // Save current Uri to FTP credentials object
            RemoteBrowserController.MyUriObject.CurrentUri = new Uri(RemoteBrowserController.Credentials.Host);

            // Bind ftp items to form
            //var remoteFileBrowser = new RemoteBrowserController();
            //lbFTPBrowse.ItemsSource = remoteFileBrowser.GetRemoteItems(RemoteBrowserController.Credentials, RemoteBrowserController.MyUriObject.CurrentUri);

            // Data binding test
            // Initialize MyUriObject
            RemoteBrowserController.MyUriObject.NextUri = new Uri(RemoteBrowserController.Credentials.Host);
            RemoteBrowserController.MyUriObject.GoNext = true;
            lbFTPBrowse.ItemsSource = RemoteBrowserController.RemoteGoTo();

            // Update Remote browsers info label
            LabelRemoteDirInfo.Content =
                $"{RemoteBrowserController.DirInfo.FileCount} files and {RemoteBrowserController.DirInfo.DirectoryCount} directories. Total size: {RemoteBrowserController.DirInfo.DirectorySize} bytes";

            // Update remote site to address bar
            tbRemoteSite.Text = "/";
        }
    }
}
