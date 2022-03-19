using System;
using System.Collections.Generic;
using System.Text;
using TagLib;

namespace Audioplayer.Models
{
    public class MusicTrack
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public MetaData MetaData { get; set; }
        public bool IsFavorite { get; set; }
    }
}
