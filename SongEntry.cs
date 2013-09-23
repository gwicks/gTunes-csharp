using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gTunes
{
    class SongEntry
    {
        public string title;
        public string path;
        public bool streamed = false;
        public Song relatedSong;

        public SongEntry(string ntitle,string npath)
        {
            title = ntitle;
            path = npath;
            
        }

        
        public override string ToString()
        {
            return title;
        }
    }
}
