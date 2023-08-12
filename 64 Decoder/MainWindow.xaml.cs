using _64_Decoder.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _64_Decoder
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Messenger.Default.Register<string>(this, "VIEW_MODEL", ShowMsg);
            Messenger.Default.Register<bool>(this, "VIEW_MODEL", CloseApp);

            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void CloseApp(bool close)
        {
            if (close)
                this.Close();
        }

        private void ShowMsg(string strMsg)
        {
            MessageBoxImage mboxImg;

            if (strMsg.ToUpper().Contains("ERROR"))
                mboxImg = MessageBoxImage.Error;
            else if (strMsg.ToUpper().Contains("WARNING"))
                mboxImg = MessageBoxImage.Warning;
            else
                mboxImg = MessageBoxImage.Information;

            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(this, strMsg, "Base64 Encoder/Decoder", MessageBoxButton.OK, mboxImg);
            });
        }


        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                var data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (data == null)
                {
                    ShowMsg("Error: Invalid Drop Content.");
                    return;
                }

                var fileName = data.FirstOrDefault();
                if (string.IsNullOrEmpty(fileName))
                {
                    ShowMsg("Error: Invalid File Path.");
                    Messenger.Default.Send("", "VIEW");
                    return;
                }

                if (!fileName.ToUpper().EndsWith("JAR"))
                {
                    ShowMsg("Error: Invalid File Type.");
                    Messenger.Default.Send("", "VIEW");
                    return;
                }

                Messenger.Default.Send(fileName, "VIEW");
            }
            catch (Exception es)
            {
                ShowMsg("Error: Fail to get drop content.");
            }
            

        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                var fileName = (sender as TextBox).Text.Trim().Replace("\"","");
                if (string.IsNullOrEmpty(fileName))
                {
                    ShowMsg("Error: Invalid File Path.");
                    Messenger.Default.Send("", "VIEW");
                    return;
                }

                if (!fileName.ToUpper().EndsWith("JAR"))
                {
                    ShowMsg("Error: Invalid File Type.");
                    Messenger.Default.Send("", "VIEW");
                    return;
                }

                Messenger.Default.Send(fileName, "VIEW");
            }
        }
    }
}