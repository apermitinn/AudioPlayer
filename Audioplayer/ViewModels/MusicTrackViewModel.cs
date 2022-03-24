using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Audioplayer.Models;

namespace Audioplayer.ViewModels
{
    public class MusicTrackViewModel : NotifyPropertyChanged
    {
        private MusicTrack track;
        private Visibility _visibility;

        public MusicTrackViewModel(MusicTrack track)
        {
            this.track = track;
            Visibility = Visibility.Visible;
        }

        public MusicTrack MusicTrack { get; set; }
        public bool IsFavorite { get; set; }
        public Visibility Visibility 
        {
            get { return _visibility; }
            set 
            {
                _visibility = value;
                var b = track.MetaData.Year > 0 ? track.MetaData.Year : 0;
                RaisePropertyChange("Visibility");
            }
        }
        public BitmapImage Artwork => track.MetaData.Artwork;
        public string Name => track.Name;
        public string FilePath => track.FilePath;
        public string Album => $"Альбом: {track.MetaData.Album}";
        public string Title => $"Название: {track.MetaData.Title}";
        public string Singer => $"Исполнитель: {track.MetaData.Singer}";
        public string Year
        {
            get
            {
                string year = track.MetaData.Year > 0 ? track.MetaData.Year.ToString() : "";
                return $"Год: {year}";
            }
        }
        public string Duration => $"Длительность: {track.MetaData.Duration.ToString(@"mm\:ss")}";
    }
}
