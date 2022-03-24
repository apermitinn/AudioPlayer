using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;
using TagLib;

namespace Audioplayer.Models
{
    public class MetaData
    {
        public string Title { get; set; }
        public string Album { get; set; }
        public string Singer { get; set; }
        public uint Year { get; set; }
        public TimeSpan Duration { get; set; }
        public BitmapImage Artwork { get; set; }
    }
}
