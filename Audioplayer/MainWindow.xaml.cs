using Audioplayer.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _volumeValue;
        private bool _isFileSelected = false;
        private bool _isFilePlaying = false;
        private string _searchText = "";
        private ObservableCollection<MusicTrack> _trackList = new ObservableCollection<MusicTrack>();
        private MusicTrack _selectedTrack = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            VolumeValue = (int)(mediaPlayer.Volume * 100);


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
        public ObservableCollection<MusicTrack> FavoriteTrackList
        {
            get { return new ObservableCollection<MusicTrack>(TrackList.Where(x=>x.IsFavorite)); }
        }
        public MusicTrack SelectedTrack
        {
            get { return _selectedTrack; }
            set 
            { 
                _selectedTrack = value;
                RaisePropertyChange("SelectedTrack");
            }
        }
        private Bitmap _artwork;
        public Bitmap Artwork
        {
            get { return _artwork; }
            set 
            {
                _artwork = value;
                RaisePropertyChange("Artwork");
            }
        }
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; }
        }
        
        public int VolumeValue
        {
            get { return _volumeValue; }
            set
            {
                _volumeValue = value;
                RaisePropertyChange("VolumeValue");
                RaisePropertyChange("VolumeValueText");
            }
        }
        public string VolumeValueText
        {
            get { return $"Громксоть: {_volumeValue}%"; }
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
                var tagFile = TagLib.File.Create(ofd.FileName);
                MetaData meta = new MetaData
                {
                    Title = tagFile.Tag.Title,
                    Album = tagFile.Tag.Album,
                    Singer = String.Join(", ", tagFile.Tag.Performers),
                    Year = tagFile.Tag.Year,
                    Duration = tagFile.Properties.Duration
                };
                if (tagFile.Tag.Pictures.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(tagFile.Tag.Pictures.First().Data.Data);
                    meta.Artwork = (Bitmap)System.Drawing.Image.FromStream(ms);
                }

                var track = new MusicTrack() { FilePath = ofd.FileName, Name = ofd.SafeFileName , MetaData = meta };
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
            VolumeValue = (int)(mediaPlayer.Volume * 100);
        }
        private void IncreaseVolume_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (mediaPlayer.Volume < 1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void DecreaseVolume_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (mediaPlayer.IsMuted)
            {
                mediaPlayer.IsMuted = false;
            }
            mediaPlayer.Volume -= 0.05;
            VolumeValue = (int)(mediaPlayer.Volume * 100);
        }
        private void DecreaseVolume_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Из-за погрешности double нужно сравнивать очень маленьким числом, но не 0
            if (mediaPlayer.Volume > 0.001)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
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
        private void RaisePropertyChange(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RaisePropertyChange("FavoriteTrackList");
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RaisePropertyChange("FavoriteTrackList");
        }
    }
}
