using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CryptoPad.Views
{
    /// <summary>
    /// Логика взаимодействия для PasswordDialog.xaml
    /// </summary>
    public partial class InputPasswordDialog : Window
    {
        public string Value
        {
            get => passwordBox.Password;
        }
        
        public InputPasswordDialog()
        {
            InitializeComponent();
            passwordBox.Password = "";
            passwordBox.Focus();
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }
    }
}
