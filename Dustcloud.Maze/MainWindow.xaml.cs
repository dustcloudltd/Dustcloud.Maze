using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Dustcloud.Maze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.IsVisible)
            {
                textBox.ScrollToLine(textBox.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Length - 1);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            FilePathTextBox.Text = ofd.ShowDialog() == true ? ofd.FileName : null;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            FilePathTextBox.Text = sfd.ShowDialog() == true ?  sfd.FileName : null;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, $"aMaze-o by Greg Chelstowski, 14-16/12/2021{Environment.NewLine} ;)", "About", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Doc_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo{FileName = "https://github.com/dustcloudltd/Dustcloud.Maze/#readme",UseShellExecute = true});
        }
    }
}
