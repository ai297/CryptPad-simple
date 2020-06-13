using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CryptoPad.Models;
using CryptoPad.Views;
using Microsoft.Win32;

namespace CryptoPad.ViewModels
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private CryptFile file;
        private int newDocNum = 0;
        private string docName;

        private bool isDirty = false;
        public bool IsDirty
        { 
            get => isDirty;
            set
            {
                isDirty = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title { 
            get
            {
                return IsDirty ? $"*{docName}" : docName;
            }
        }

        public string TextContent
        { 
            get => file.Document.TextContent;
            set
            {
                file.Document.TextContent = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }

        public Encoding CurrentEncoding
        {
            get => file.Document.Encoding;
        }


        private string messageText = String.Empty;
        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged();

                Task.Delay(5000).ContinueWith( _ => {
                    messageText = string.Empty;
                    OnPropertyChanged(nameof(MessageText));
                });
            }
        }

        public Encoding EncodingANSI => Encoding.GetEncoding("windows-1251");
        public Encoding EncodingUTF => Encoding.UTF8;
        public bool UtfIsChecked
        {
            get => CurrentEncoding.EncodingName.Equals(EncodingUTF.EncodingName);
        }
        public bool AnsiIsChecked
        {
            get => CurrentEncoding.EncodingName.Equals(EncodingANSI.EncodingName);
        }


        private ICommand newCommand;
        public ICommand New {
            get
            {
                return newCommand ??= new RelayCommand(obj => NewDocument(), obj => true);
            }
        }
        
        private ICommand openCommand;
        public ICommand Open {
            get
            {
                return openCommand ??= new RelayCommand(obj =>
                {
                    var openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = $"{Properties.Resources.AllSupportedFiles}|*.txt;*{CryptFile.CryptExtension}";
                    openFileDialog.Title = Properties.Resources.OpenFileDilogTitle;

                    if (openFileDialog.ShowDialog() == true)
                    {
                        OpenDocument(openFileDialog.FileName);
                    }
                }
                , obj => true);
            }
        }

        private ICommand saveCommand;
        public ICommand Save
        {
            get
            {
                return saveCommand ??= new RelayCommand(obj =>
                {
                    if (!string.IsNullOrEmpty(file.FileName)) SaveDocument(file.FileName);
                    else SaveAs.Execute(obj);
                },
                obj => IsDirty);
            }
        }

        private ICommand saveAsCommand;
        public ICommand SaveAs {
            get
            {
                return saveAsCommand ??= new RelayCommand(obj => 
                {
                    var saveDialog = new SaveFileDialog();
                    saveDialog.Title = Properties.Resources.SaveFileDialogTitle;
                    saveDialog.Filter = $"{Properties.Resources.TextFileFormat}|*.txt|{Properties.Resources.CryptedTextFormat}|*{CryptFile.CryptExtension}";
                    if (!string.IsNullOrEmpty(file.FileName)) saveDialog.DefaultExt = Path.GetExtension(file.FileName);
                    saveDialog.FileName = docName;
                    if(saveDialog.ShowDialog() == true)
                    {
                        SaveDocument(saveDialog.FileName);
                    }

                }, obj => true);
            }
        }

        private ICommand closeCommand;
        public ICommand Close { 
            get
            {
                return closeCommand ??= new RelayCommand(obj =>
                {
                    if(IsDirty)
                    {
                        var confirm = MessageBox.Show(Properties.Resources.ConfirmExitMessage, Properties.Resources.ConfirmExitTitle,
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                        switch(confirm)
                        {
                            case MessageBoxResult.Yes:
                                Save.Execute(obj);
                                break;
                            case MessageBoxResult.Cancel:
                                return;
                            case MessageBoxResult.No:
                            default:
                                IsDirty = false;
                                break;
                        }
                    }
                    Application.Current.Shutdown();
                }, obj => true);
            }
        }

        private ICommand changeEncoding;
        public ICommand ChangeEncoding
        {
            get
            {
                return changeEncoding ??= new RelayCommand(obj =>
                {
                    file.Document.ChangeEncodingTo(obj as Encoding);
                    UpdateAllProperties();
                }, obj =>
                {
                    return !(obj as Encoding).EncodingName.Equals(CurrentEncoding.EncodingName);
                });
            }
        }

        public MainWindowModel()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) OpenDocument(args[1]);
            else NewDocument();
        }

        private void NewDocument()
        {
            newDocNum++;
            docName = $"New {newDocNum}";
            file = new CryptFile(GetSecurityKey);
            UpdateAllProperties();
            IsDirty = false;
        }

        private void OpenDocument(string fileName)
        {
            try
            {
                file = new CryptFile(fileName, GetSecurityKey);
                docName = Path.GetFileName(fileName);
                UpdateAllProperties();
                IsDirty = false;
            }
            catch(Exception)
            {
                MessageText = $"{Properties.Resources.OpenFileError} \"{fileName}\"...";
            }
        }

        private void SaveDocument(string fileName)
        {
            try
            {
                file.Save(fileName);
                docName = Path.GetFileName(fileName);
                UpdateAllProperties();
                IsDirty = false;
                MessageText = Properties.Resources.SaveFileSuccess;
            }
            catch (Exception)
            {
                MessageText = $"{Properties.Resources.SaveFileError} \"{fileName}\"...";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void UpdateAllProperties()
        {
            OnPropertyChanged(nameof(TextContent));
            OnPropertyChanged(nameof(CurrentEncoding));
            OnPropertyChanged(nameof(UtfIsChecked));
            OnPropertyChanged(nameof(AnsiIsChecked));
        }

        private string GetSecurityKey()
        {
            var passwordDialog = new InputPasswordDialog();
            if (passwordDialog.ShowDialog() == true) return passwordDialog.Value;
            else return null;
        }
    }
}
