using Audioplayer.Models;
using Audioplayer.ViewModels;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        private ObservableCollection<MusicTrackViewModel> _trackList = new ObservableCollection<MusicTrackViewModel>();
        private MusicTrackViewModel _selectedTrack = null;

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

        public ObservableCollection<MusicTrackViewModel> TrackList
        {
            get { return _trackList; }
        }
        public ObservableCollection<MusicTrackViewModel> FavoriteTrackList
        {
            get { return new ObservableCollection<MusicTrackViewModel>(TrackList.Where(x=>x.IsFavorite)); }
        }
        public MusicTrackViewModel SelectedTrack
        {
            get { return _selectedTrack; }
            set 
            { 
                _selectedTrack = value;
                RaisePropertyChange("SelectedTrack");
            }
        }
        public string SearchText
        {
            get { return _searchText; }
            set 
            { 
                _searchText = value;
                PerformSearch();
            }
        }

        private void PerformSearch()
        {
            if (SearchText.Length > 2)
            {
                foreach (var track in TrackList)
                {
                    if (track.Name.IndexOf(SearchText) > -1)
                    {
                        track.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        track.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                foreach (var track in TrackList)
                {
                    if (track.Visibility != Visibility.Visible)
                    {
                        track.Visibility = Visibility.Visible;
                    }
                }
            }
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
            get { return $"Громкость: {_volumeValue}%"; }
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
            if (e.Parameter?.ToString() == "folder")
            {
                AddTracksFromFolder();
            }
            else
            {
                AddTracks();
            }
            
        }

        private void AddTracksFromFolder()
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = false,
                AllowNonFileSystemItems = false,
                DefaultFileName = "Select Folder",
                Title = "Select Folder"
            };
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var dirPath = System.IO.Path.GetDirectoryName(ofd.FileName);
                var files = Directory.GetFiles(dirPath, "*.mp3");
                foreach (var track in files)
                {
                    AddTrack(track);
                }
            }
        }

        private void AddTracks()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Media files (*.mp3)|*.mp3"
            };
            if (ofd.ShowDialog() == true)
            {
                foreach (var track in ofd.FileNames)
                {
                    AddTrack(track);
                }
            }
        }
        private void AddTrack(string filePath)
        {
            var file = new FileInfo(filePath);
            var tagFile = TagLib.File.Create(filePath);
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
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
                meta.Artwork = image;
            }

            var track = new MusicTrack() { FilePath = filePath, Name = file.Name, MetaData = meta };
            var trackVM = new MusicTrackViewModel(track);
            TrackList.Add(trackVM);
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
