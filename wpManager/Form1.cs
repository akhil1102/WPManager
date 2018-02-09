using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Forms;


namespace wpManager
{
    public partial class Form1 : Form
    {
        public string[] allFiles = new string[10];
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
        const uint SPI_SETDESKWALLPAPER = 0x14;
        const uint SPIF_UPDATEINIFILE = 0x01;
        const uint SPIF_SENDWININICHANGE = 0x02;
        public Form1()
        {
            InitializeComponent();
            SystemEvents.PowerModeChanged += OnPowerChange;
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(EventLock);

        }
        private void EventLock(object sender, SessionSwitchEventArgs e)
        {
            if(e.Reason == SessionSwitchReason.SessionUnlock)
            {
                string path = @"F:\others\wallpapers\";
                string[] files = Directory.GetFiles(path,"*jpg");
                var rand = new Random();
                string randomFile = files[rand.Next(files.Length)];
                var res = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, randomFile, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                //Console.WriteLine(GetWPPath());
            }
        }
        private void OnPowerChange(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    System.Timers.Timer myTimer = new System.Timers.Timer();
                    string selectedTime = comboBox1.SelectedItem.ToString();
                    myTimer.Interval = (Int32.Parse(selectedTime.Substring(0, 1))) * 60 * 60 * 1000;  //time in milliseconds
                    myTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
                    myTimer.AutoReset = false;
                    myTimer.Enabled = true;
                    //Console.WriteLine(result);
                    //string wp = GetWPPath();
                    //Console.WriteLine(wp);
                    break;
                //case PowerModes.Suspend:
                //    var res = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, @"F:\others\b9fe47a98226cffc682ffaafb0014a90f703eae5", SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                //    break;
            }

        }
        private void OnTimedEvent2(object source, ElapsedEventArgs e)
        {
            string file = @"F:\others\batman_v_superman-1366x768";
            var result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, file, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            Console.WriteLine("wallpaper set:" + result);
            Console.WriteLine(GetWPPath());
        }
        private static string GetWPPath()
        {
            RegistryKey wallpaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string wp_path = wallpaper.GetValue("WallPaper").ToString();
            wallpaper.Close();
            return wp_path;
        }

        private void button_browse_Click(object sender, EventArgs e)        //clicking button browse opens the dialog box to select image
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            dlg.Multiselect = true;
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                PictureBox[] picBoxes = new PictureBox[dlg.FileNames.Length];
                int i = 0;
                int x = 0;
                for (int j = 0; j < dlg.FileNames.Length; j++)
                {
                    allFiles[j] = dlg.FileNames[j];
                }
                foreach (string file in dlg.FileNames)
                {
                    picBoxes[i] = new PictureBox
                    {
                        Location = new System.Drawing.Point(30 + x, 106),
                        Size = new System.Drawing.Size(100, 117),
                        Name = "picturebox" + i,
                        Image = Image.FromFile(file),
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    this.Controls.Add(picBoxes[i]);
                    i++;
                    x += 130;
                }

            }
        }
        
        private void button_apply_Click(object sender, EventArgs e)
        {
            System.Timers.Timer myTimer = new System.Timers.Timer();
            string s = comboBox1.SelectedItem.ToString();
            myTimer.Interval = (Int32.Parse(s.Substring(0, 1))) * 60 * 60 * 1000;  //time in milliseconds
            myTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            myTimer.Enabled = true;
            
        }

        private void OnTimedEvent(object source,ElapsedEventArgs e)
        {
            SetWallpaper(allFiles);
        }

        static int i = 0;
        public void SetWallpaper(string[] files)
        {
            if (i <= files.Length)
            {
                string file = files[i];
                var result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, file, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                i++;
                if (i == files.Length)
                {
                    i = 0;
                }
            }
        }

        private void AddFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult res = fd.ShowDialog();
            if(res == DialogResult.OK && !string.IsNullOrWhiteSpace(fd.SelectedPath))
            {
                string path = fd.SelectedPath;
                var items = checkedListBox1.Items;
                items.Add(path, true);
            }

            string dir = Environment.CurrentDirectory;
            string wordfile = dir + "\\data.txt";
            if (File.Exists(wordfile))
            {
                var sw = new StreamWriter(wordfile, true);
                foreach(string path in checkedListBox1.Items)
                {
                    sw.WriteLine(path);
                }
            }


        }
    }
}


       
