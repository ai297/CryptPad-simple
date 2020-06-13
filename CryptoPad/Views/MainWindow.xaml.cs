using CryptoPad.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CryptoPad.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        public MainWindow()
        {
            // load user's app settings
            var editorFontFamily = new FontFamily(Properties.Settings.Default.EditorFontFamily);
            var editorFontSize = Properties.Settings.Default.EditorFontSize;

            InitializeComponent();
            // set data
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(font => font.Source);

            // init start state
            txtEditor.FontFamily = editorFontFamily;
            txtEditor.FontSize = editorFontSize;
            cmbFontFamily.SelectedItem = txtEditor.FontFamily;
            cmbFontSize.Text = txtEditor.FontSize.ToString();
            txtEditor.Focus();
        }


        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
            {
                txtEditor.FontFamily = cmbFontFamily.SelectedItem as FontFamily;
                Properties.Settings.Default.EditorFontFamily = txtEditor.FontFamily.Source;
            }
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            double fontSize;
            if (Double.TryParse(cmbFontSize.Text, out fontSize))
            {
                txtEditor.FontSize = fontSize;
                Properties.Settings.Default.EditorFontSize = fontSize;
            }
            else
            {
                cmbFontSize.Text = txtEditor.FontSize.ToString();
                e.Handled = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            menuClose.Command.Execute(sender);
        }
    }
}
