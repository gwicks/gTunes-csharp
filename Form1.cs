using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NAudio;
using NAudio.Wave;
using System.Drawing.Drawing2D;
using System.Net;
using System.Collections.Specialized;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;




namespace gTunes
{
    public partial class Form1 : Form
    {
        String searchpath = "";
        String[] aacmusicfiles;
        String[] mp3musicfiles;
        bool paused = false;
        int seconds = 0;
        int minutesprefix = 0;
        float slidervalue = 0.4f;
        SongEntry globalcurrsong = null;

        public String USER_NAME = "";
        public String USER_PASS = "";

        public static String MIDDLEMAN_SERVER = "http://107.20.177.55/gm/";

        public Form1()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler(clocktick);

            ThumbnailToolBarButton infoButton = new ThumbnailToolBarButton(SystemIcons.Information, "Information");
            infoButton.Click += delegate
            {


            };

            TaskbarManager.Instance.ThumbnailToolBars.AddButtons(this.Handle, infoButton);
        }

        private void clocktick(object sender, EventArgs e)
        {

            seconds = seconds + 1;
            if (seconds <= progressBar1.Maximum)
            {
                progressBar1.Value = seconds;
            }
            label11.Text = integerToTimer(seconds);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            

        }

        private void button2_Click(object sender, EventArgs e)
        {

            SongEntry selectedsong = (SongEntry)listBox1.SelectedItem;
            globalcurrsong = selectedsong;

            if (!paused)
            {
                minutesprefix = 0;
                seconds = 0;
                label11.Text = "0:00";
            }
            timer1.Start();
            if (selectedsong.streamed)
            {
                if (selectedsong.path.Equals(""))
                {
                    selectedsong.path = getStreamURL("", "", selectedsong.relatedSong);
                    axQTControl1.URL = selectedsong.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }
                else if (!selectedsong.path.Equals("") && paused)
                {

                    axQTControl1.Movie.Play();
                }
                else if (!selectedsong.path.Equals("") && !paused)
                {
                    axQTControl1.URL = selectedsong.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }

            }
            else
            {
                if (!paused)
                {

                    axQTControl1.URL = selectedsong.path;
                }
                axQTControl1.Movie.AudioVolume = slidervalue;
                axQTControl1.Movie.Play();
                paused = false;
            }

        }

        public String integerToTimer(int seconds)
        {
            String timeString = "";
            if (seconds < 10)
            {
                timeString = "0:0" + seconds.ToString();
            }
            else if (seconds >= 10 && seconds < 60)
            {
                timeString = "0:" + seconds.ToString();
            }
            else
            {
                int remaindero = seconds % 60;
                if (remaindero == 0)
                {
                    minutesprefix++;
                    timeString = minutesprefix.ToString() + ":00";
                }
                else
                {
                    int remainder = seconds % 60;

                    if (remainder < 10)
                    {
                        String finalnumber = remainder.ToString();
                        timeString = minutesprefix.ToString() + ":0" + finalnumber;
                    }
                    else
                    {
                        String finalnumber = remainder.ToString();
                        timeString = minutesprefix.ToString() + ":" + finalnumber;
                    }
                }
            }
            return timeString;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        public String secondsToClock(int input)
        {
            TimeSpan s = new TimeSpan(0, 0, input);
            return s.ToString();
        }

        private static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public String getFileNameFromPath(String path)
        {
            String filename = "";
            int lastslash = path.LastIndexOf(@"\");
            filename = path.Substring(lastslash + 1);
            return filename;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        public void loadGooglePlay(string username, string password)
        {
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["uname"] = username;
                data["pass"] = password;
                

                var response = wb.UploadValues(MIDDLEMAN_SERVER + "list.php", "POST", data);
                
                string songstr = System.Text.Encoding.UTF8.GetString(response);
                
                Console.WriteLine(songstr);
                parseGooglePlay(songstr);
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void parseGooglePlay(string serverresponse)
        {
            Console.WriteLine("Server Respone: " + serverresponse);
            List<SongEntry> masterList = new List<SongEntry>();

            string[] sep = { "<>" };
            string[] songs = serverresponse.Split(sep, StringSplitOptions.None);
            foreach (string song in songs)
            {
                Song tempsong = new Song();
                string[] separ = { ";" };
                string[] metadata = song.Split(separ, StringSplitOptions.None);
                if (metadata.Length == 8)
                {
                    tempsong.title = metadata[0];
                    tempsong.artist = metadata[1];
                    tempsong.album = metadata[2];
                    tempsong.year = metadata[4];
                    tempsong.genre = metadata[3];
                    tempsong.image = resizeImage(tempsong.image, new Size(256, 256));
                    tempsong.id = metadata[5];
                    int duration = Int32.Parse(metadata[6]);
                    float nduration = duration / 1000;
                    tempsong.duration = Convert.ToInt32(nduration);
                    tempsong.artURL = "http:" + metadata[7];
                }
                else if (metadata.Length == 7)
                {
                    tempsong.title = metadata[0];
                    tempsong.artist = metadata[1];
                    tempsong.album = metadata[2];
                    tempsong.year = metadata[4];
                    tempsong.genre = metadata[3];
                    tempsong.image = resizeImage(tempsong.image, new Size(256, 256));
                    tempsong.id = metadata[5];
                    int duration = Int32.Parse(metadata[6]);
                    float nduration = duration / 1000;
                    tempsong.duration = Convert.ToInt32(nduration);
                }
                SongEntry tsentry = new SongEntry(tempsong.title, "");
                tsentry.streamed = true;
                tsentry.relatedSong = tempsong;
                //Console.WriteLine(tsentry.title);
                if (!tsentry.title.Equals(""))
                {
                    masterList.Add(tsentry);
                    
                }
                
            }
            var orderedList = masterList.OrderBy(x => x.title);
            foreach (SongEntry songitem in orderedList)
            {
                listBox1.Items.Add(songitem);
            }
        }

        private string getStreamURL(string uname, string pass, Song s)
        {
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["uname"] = uname;
                data["pass"] = pass;
                
                data["id"] = s.id;

                var response = wb.UploadValues(MIDDLEMAN_SERVER + "getstream.php", "POST", data);
                
                string songstr = System.Text.Encoding.ASCII.GetString(response);
                if (!songstr.StartsWith("http:"))
                {
                    songstr = "http:" + songstr;
                }
                return songstr;
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void GetAlbumArt_Click(object sender, EventArgs e)
        {
            
        }

        public static Image FromURL(string Url)
        {
            HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
            HttpWebResponse respone = request.GetResponse() as HttpWebResponse;
            return Image.FromStream(respone.GetResponseStream(), true);
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            paused = true;
            axQTControl1.Movie.Pause();
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

            
        }

        public void setLabelTimer(int time)
        {
            label11.Text = time.ToString();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            slidervalue = trackBar1.Value / 100.0f;
            Console.WriteLine(slidervalue);
            if (axQTControl1.Movie != null)
            {
                axQTControl1.Movie.AudioVolume = slidervalue;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            
            SongEntry selectedsong = (SongEntry)listBox1.SelectedItem;
            globalcurrsong = selectedsong;
            
        }

        private void updateInfo(SongEntry selectedsong)
        {
            TagLib.File tagfile = null;
            if (!selectedsong.streamed)
            {
                tagfile = TagLib.File.Create(selectedsong.path);

            }
            //axMetadataReader.URL = selectedsong.path;
            if (!selectedsong.streamed)
            {
                QTOLibrary.QTMovie mov = new QTOLibrary.QTMovie();

                //QTOLibrary.CFObject c = mov.Annotations;
                if (selectedsong.path.EndsWith(".m4a") || selectedsong.path.EndsWith(".mp3"))
                {
                    try
                    {
                        //byte[] o = (byte[])c.ChildItems.get_ItemByKey("artw").Value;
                        TagLib.IPicture[] pictures = tagfile.Tag.Pictures;
                        TagLib.IPicture ip = pictures[0];
                        byte[] tagpicture = ip.Data.Data;
                        Console.WriteLine("TagLib Data Array Size: " + tagpicture.Length);
                        //Console.WriteLine("Byte size: " + o.Length);
                        ImageConverter ic = new ImageConverter();
                        Image img = (Image)ic.ConvertFrom(tagpicture);
                        img = resizeImage(img, new Size(256, 256));
                        pictureBox1.Image = img;
                    }
                    catch (System.Exception exp)
                    {
                        pictureBox1.Image = resizeImage(Image.FromFile("noart.png"), new Size(256, 256));
                    }
                    //Bitmap bitmap1 = new Bitmap(img);

                }
                else
                {
                    Image noart = Image.FromFile("noart.png");
                    pictureBox1.Image = resizeImage(noart, new Size(256, 256));


                }

                double length = tagfile.Properties.Duration.TotalSeconds;
                int roundedlength = Convert.ToInt32(length);
                Console.WriteLine("Length: " + roundedlength);

                string title = tagfile.Tag.Title;

                string artist = tagfile.Tag.FirstAlbumArtist;

                string album = tagfile.Tag.Album;

                string year = tagfile.Tag.Year.ToString();

                string genre = tagfile.Tag.FirstGenre;

                //Console.WriteLine("Time: " + length);
                label10.Text = title;
                label9.Text = artist;
                label8.Text = album;
                label7.Text = year;
                label6.Text = genre;
                progressBar1.Maximum = roundedlength;
                label12.Text = secondsToClock(roundedlength).Substring(4);
            }
            else
            {
                label10.Text = selectedsong.relatedSong.title;
                label9.Text = selectedsong.relatedSong.artist;
                label8.Text = selectedsong.relatedSong.album;
                label7.Text = selectedsong.relatedSong.year;
                label6.Text = selectedsong.relatedSong.genre;
                pictureBox1.Image = selectedsong.relatedSong.image;
                progressBar1.Maximum = selectedsong.relatedSong.duration;
                label12.Text = secondsToClock(selectedsong.relatedSong.duration).Substring(4);
                if (selectedsong.relatedSong.artURL != "")
                {

                    Image art = FromURL(selectedsong.relatedSong.artURL);
                    pictureBox1.Image = resizeImage(art, new Size(256, 256));
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            SongEntry selectedsong = (SongEntry)listBox1.SelectedItem;
            globalcurrsong = selectedsong;
            updateInfo(globalcurrsong);
            if (!paused)
            {
                minutesprefix = 0;
                seconds = 0;
                label11.Text = "0:00";
            }
            timer1.Start();
            if (selectedsong.streamed)
            {
                if (selectedsong.path.Equals(""))
                {
                    selectedsong.path = getStreamURL(USER_NAME, USER_PASS, selectedsong.relatedSong);
                    selectedsong.path = selectedsong.path.Substring(0, selectedsong.path.Length - 1);
                    Console.WriteLine(selectedsong.path);
                    
                    axQTControl1.URL = selectedsong.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }
                else if (!selectedsong.path.Equals("") && paused)
                {

                    axQTControl1.Movie.Play();
                }
                else if (!selectedsong.path.Equals("") && !paused)
                {
                    axQTControl1.URL = selectedsong.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }

            }
            else
            {
                if (!paused)
                {
                    axQTControl1.URL = selectedsong.path;
                }
                axQTControl1.Movie.AudioVolume = slidervalue;
                axQTControl1.Movie.Play();
                paused = false;
            }

        }

        private void FolderAddBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                searchpath = folderBrowserDialog1.SelectedPath;
                aacmusicfiles = Directory.GetFiles(searchpath, "*.m4a", SearchOption.AllDirectories);
                mp3musicfiles = Directory.GetFiles(searchpath, "*.mp3", SearchOption.AllDirectories);
                List<SongEntry> masterList = new List<SongEntry>();

                foreach (String musicfile in aacmusicfiles)
                {
                    TagLib.File tagfile = TagLib.File.Create(musicfile);

                    if (!tagfile.Tag.Title.Equals(""))
                    {
                        SongEntry tempentry = new SongEntry(tagfile.Tag.Title, musicfile);
                        //listBox1.Items.Add(tempentry);
                        masterList.Add(tempentry);
                    }
                    else
                    {
                        SongEntry tempentry = new SongEntry(getFileNameFromPath(musicfile), musicfile);
                        //listBox1.Items.Add(tempentry);
                        masterList.Add(tempentry);
                    }
                    
                }
                foreach (String musicfile in mp3musicfiles)
                {
                    TagLib.File tagfile = TagLib.File.Create(musicfile);

                    if (tagfile.Tag.Title != null)
                    {
                        SongEntry tempentry = new SongEntry(tagfile.Tag.Title, musicfile);
                        //listBox1.Items.Add(tempentry);
                        masterList.Add(tempentry);
                    }
                    else
                    {
                        SongEntry tempentry = new SongEntry(getFileNameFromPath(musicfile), musicfile);
                        //listBox1.Items.Add(tempentry);
                        masterList.Add(tempentry);
                    }
                }
                var sortedList = masterList.OrderBy(x => x.title);
                foreach (SongEntry songitem in sortedList)
                {
                    listBox1.Items.Add(songitem);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (paused)
            {
                paused = false;
            }
            axQTControl1.Movie.Stop();
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            slidervalue = trackBar1.Value / 100.0f;
            Console.WriteLine(slidervalue);
            if (axQTControl1.Movie != null)
            {
                axQTControl1.Movie.AudioVolume = slidervalue;
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            timer1.Stop();
            axQTControl1.Movie.Stop();
            if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
            {
                listBox1.SelectedIndex = 0;
            }
            else
            {

                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
            }
            SongEntry sentry = (SongEntry)listBox1.SelectedItem;
            minutesprefix = 0;
            seconds = 0;
            timer1.Start();
            if (sentry.streamed)
            {
                if (sentry.path.Equals(""))
                {
                    sentry.path = getStreamURL("", "", sentry.relatedSong);
                    axQTControl1.URL = sentry.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }
                else
                {
                    axQTControl1.URL = sentry.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }
            }
            else
            {
                axQTControl1.URL = sentry.path;
                axQTControl1.Movie.AudioVolume = slidervalue;
                axQTControl1.Movie.Play();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            timer1.Stop();
            axQTControl1.Movie.Stop();
            if (listBox1.SelectedIndex == 0)
            {
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
            }
            SongEntry sentry = (SongEntry)listBox1.SelectedItem;
            minutesprefix = 0;
            seconds = 0;
            timer1.Start();
            if (sentry.streamed)
            {
                if (sentry.path.Equals(""))
                {
                    sentry.path = getStreamURL("", "", sentry.relatedSong);
                    axQTControl1.URL = sentry.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }
                else
                {
                    axQTControl1.URL = sentry.path;
                    axQTControl1.Movie.AudioVolume = slidervalue;
                    axQTControl1.Movie.Play();
                }
            }
            else
            {
                axQTControl1.URL = sentry.path;
                axQTControl1.Movie.AudioVolume = slidervalue;
                axQTControl1.Movie.Play();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            LoginForm lf = new LoginForm();
            lf.parent = this;
            lf.Show();
        }
    }

    public class SongTimer
    {
        public bool run = false;
        public int count = 0;
        public Thread sThread = null;
        public Form1 parent = null;

        public SongTimer(Form1 fparent)
        {
            sThread = new Thread(new ThreadStart(this.runThread));
            parent = fparent;
        }

        public void runThread()
        {
            while (run)
            {
                count = count + 1;
                parent.setLabelTimer(count);
                Thread.Sleep(1000);
            }
        }

        public void stop()
        {
            run = false;
            sThread.Join();
        }
    }
}

