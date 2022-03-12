using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Audioplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isFileSelected = false;
        private bool isFilePlaying = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position -= TimeSpan.FromSeconds(5);
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position += TimeSpan.FromSeconds(5);
        }

        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Media files (*.mp3)|*.mp3"
            };
            if (ofd.ShowDialog() == true)
            {
                mediaPlayer.Source = new Uri(ofd.FileName);
                isFileSelected = true;
            }
        }

        private void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Play();
            isFilePlaying = true;
        }
        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isFileSelected && !isFilePlaying;
        }
        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Pause();
            isFilePlaying = false;
        }
        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isFilePlaying;
        }
        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Stop();
            isFilePlaying = false;
        }
        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isFilePlaying;
        }
        private void Rewind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int shift = Int32.Parse(e.Parameter.ToString());
            mediaPlayer.Position += TimeSpan.FromSeconds(shift);
        }
        private void Rewind_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isFilePlaying;
        }
    }
}
