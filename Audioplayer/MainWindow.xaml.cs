using Audioplayer.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _isFileSelected = false;
        private bool _isFilePlaying = false;
        private string _searchText = "";
        private ObservableCollection<MusicTrack> _trackList = new ObservableCollection<MusicTrack>();
        private MusicTrack _selectedTrack = null;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            //_trackList.Add(new MusicTrack { Name = "Track1" });
            //_trackList.Add(new MusicTrack { Name = "Track2" });
            //_trackList.Add(new MusicTrack { Name = "Track3" });
            //_trackList.Add(new MusicTrack { Name = "Track4" });
            //_trackList.Add(new MusicTrack { Name = "Track5" });
        }

        public ObservableCollection<MusicTrack> TrackList
        {
            get { return _trackList; }
        }
        public MusicTrack SelectedTrack
        {
            get { return _selectedTrack; }
            set { _selectedTrack = value; }
        }
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; }
        }
        
        public string VolumeValue
        {
            get { return "Громкость " + (mediaPlayer.Volume * 100) + "%"; }
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
                var track = new MusicTrack() { FilePath = ofd.FileName, Name = ofd.SafeFileName };
                TrackList.Add(track);
            }
        }

        private void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Play();
            _isFilePlaying = true;
        }
        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isFileSelected && !_isFilePlaying;
        }
        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Pause();
            _isFilePlaying = false;
        }
        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isFilePlaying;
        }
        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Stop();
            _isFilePlaying = false;
        }
        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isFilePlaying;
        }
        private void Rewind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int shift = Int32.Parse(e.Parameter.ToString());
            mediaPlayer.Position += TimeSpan.FromSeconds(shift);
        }
        private void Rewind_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isFilePlaying;
        }
        private void IncreaseVolume_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (mediaPlayer.IsMuted)
            {
                mediaPlayer.IsMuted = false;
            }
            mediaPlayer.Volume += 0.05;
        }
        private void IncreaseVolume_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void DecreaseVolume_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (mediaPlayer.IsMuted)
            {
                mediaPlayer.IsMuted = false;
            }
            mediaPlayer.Volume -= 0.05;
        }
        private void DecreaseVolume_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void MuteVolume_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.IsMuted = !mediaPlayer.IsMuted;
        }
        private void MuteVolume_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedTrack != null)
            {
                _isFileSelected = true;
                mediaPlayer.Source = new Uri(SelectedTrack.FilePath);
            }
            else
            {
                _isFileSelected = false;
            }
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchText.Length > 5)
            {
                MessageBox.Show("Ищу песню " + SearchText);
            }
        }
    }
}
