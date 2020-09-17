using System.Windows;
using System.Windows.Input;

namespace FTP_Browser.Commands
{
    public class WpfCommands
    {
        public void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Invoked Cut Command");
        }
    }
}
