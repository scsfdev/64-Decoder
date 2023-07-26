using GalaSoft.MvvmLight;
using _64_Decoder.Model;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;

namespace _64_Decoder.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {

        #region Binding

        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }


        private string titleVersion;
        public string TitleVersion
        {
            get { return titleVersion; }
            set { Set(ref titleVersion, value); }
        }


        private string encryptFile;
        public string EncryptFile
        {
            get { return encryptFile; }
            set { Set(ref encryptFile, value); }
        }


        private string decryptData;
        public string DecryptData
        {
            get { return decryptData; }
            set { Set(ref decryptData, value); }
        }


        private bool saveReady;
        public bool SaveReady
        {
            get { return saveReady; }
            set { Set(ref saveReady, value); }
        }


        public ICommand CmdJob { get; private set; }


        #endregion

        

        //public override void Cleanup()
        //{
        //    // Clean up if needed

        //    base.Cleanup();
        //}



        public MainViewModel(IDataService dataService)
        {
            Messenger.Default.Register<string>(this, "VIEW", GetDropContent);

            Title = "Base64 Encoder/Decoder";
            TitleVersion = Title + " - " + "Ver: " + GetVersion();
            EncryptFile = "";
            DecryptData = "";

            CmdJob = new RelayCommand<object>(Action_Job);
            SaveReady = false;
        }


        private string GetVersion()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
            return asm.GetName().Version.Major.ToString() + "." + asm.GetName().Version.Minor.ToString() + "." + asm.GetName().Version.Revision.ToString();
        }


        private void Action_Job(object obj)
        {
            string strTag = obj == null ? "" : obj.ToString().Trim();
            if (string.IsNullOrEmpty(strTag)) return;

            switch (strTag)
            {
                case "BROWSE": BrowseFile(); break;
                case "CLEAR": Reset(); break;
                case "CLOSE": CloseApp(); break;
                case "SAVE": SaveData(); break;
            }
        }

        


        private void BrowseFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = title,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "DLL|*.dll",

                InitialDirectory = @"C:\"
            };

            ofd.ShowDialog();

            if (string.IsNullOrEmpty(ofd.FileName))
                return;
            else
            {
                EncryptFile = ofd.FileName;
                ReadContents();
            }
        }

        private void Reset()
        {
            EncryptFile = "";
            DecryptData = "";
            SaveReady = false;
        }

        private void CloseApp()
        {
            Messenger.Default.Send(true, "VIEW_MODEL");
        }

        private void SaveData()
        {
            try
            {
                if (File.Exists(encryptFile))
                {
                    var contents = Base64Encode(decryptData);
                    File.WriteAllText(encryptFile, contents);

                    Messenger.Default.Send("Info: Successfully write the contents to file!", "VIEW_MODEL");
                }
                else
                {
                    Messenger.Default.Send("ERROR: Target file is missing!", "VIEW_MODEL");
                }
            }
            catch (Exception e)
            {
                Messenger.Default.Send("ERROR: Fail to write file contents!", "VIEW_MODEL");
            }
        }


        private void GetDropContent(string dropFilePath)
        {
            if (!string.IsNullOrEmpty(dropFilePath) && File.Exists(dropFilePath) && dropFilePath.ToUpper().EndsWith("DLL"))
            {
                EncryptFile = dropFilePath;
                ReadContents();
            }
            else
            {
                Reset();
            }
        }


        private void ReadContents()
        {
            SaveReady = false;
            try
            {
                if (string.IsNullOrEmpty(encryptFile) || !File.Exists(encryptFile) || !encryptFile.ToUpper().EndsWith("DLL"))
                {
                    Reset();
                }
                else
                {
                    var contents = File.ReadAllText(encryptFile);
                    DecryptData = Base64Decode(contents);
                    SaveReady = true;
                }
            }
            catch (Exception e)
            {
                Messenger.Default.Send("ERROR: Fail to read file contents!", "VIEW_MODEL");
            }
        }


        private string Base64Encode(string data)
        {
            var dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(dataBytes);
        }


        private string Base64Decode(string encodeData)
        {
            var dataBytes = Convert.FromBase64String(encodeData);
            return System.Text.Encoding.UTF8.GetString(dataBytes);
        }
    }
}