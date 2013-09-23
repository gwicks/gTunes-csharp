using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gTunes
{
    class Song
    {
        public string title = "";
        public string artist = "";
        public string album = "";
        public string year = "";
        public string genre = "";
        public System.Drawing.Image image = System.Drawing.Image.FromFile("noart.png");

        public Song()
        {

        }
    }
}
