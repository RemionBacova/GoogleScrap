using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace GoogleAPI_Search
{
    public partial class Form1 : Form
    {
        private String FileToOpen = "";
        private List<String> SKUs = new List<string>();
        private Timer timer = new Timer();
        private int TimerTicks = 1;
        public Form1()
        {
            InitializeComponent();
            label2.Text = "TimerTicks : 0";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadRandomly();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

                //Get the path of specified file
                FileToOpen = openFileDialog1.FileName;
                ReadFromFile readFromFile = new ReadFromFile();
                SKUs = readFromFile.ReadTxtFile(FileToOpen);
                label1.Text = "Loaded from file : " + FileToOpen;

        }


        private void ReadRandomly()
        {
            Random a = new Random(DateTime.Now.Millisecond);
            timer.Interval = a.Next(1, 10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            TimerTicks++;
            label2.Text = "TimerTicks : " + TimerTicks;

            ImageDownloader imageDownloader = new ImageDownloader();
            string search = SKUs[0];
            SKUs.Remove(search);
            imageDownloader.findAllLinks(search, 4);

            Random a = new Random(DateTime.Now.Millisecond);
            timer.Interval = a.Next(1, 10);
            timer.Start();
        }
    }

    public class ReadFromFile
    {
        public List<String> ReadTxtFile(string FileName)
        {
            List<String> Rows = new List<String>();

            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(FileName);
            while ((line = file.ReadLine()) != null)
            {
                Rows.Add(line);
            }
            file.Close();
            return Rows;
        }
    }


    public class ImageDownloader
    {
       public void findAllLinks(string Product,int qtyImages)
        {

            //   string Url = "https://canyon.eu/product/" + Product + "/";
            //url standard

            string Url = @"https://www.google.com/search?q=" + Product + "&tbm=isch";
            //url google


            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = hw.Load(Url);


            int i = 1;
            foreach (HtmlNode link in doc.DocumentNode.Descendants("a"))
            {
                /*google Searc*/
                string srcValue = link.GetAttributeValue("href", string.Empty);
                if (srcValue.Contains("imgres?imgurl=") && srcValue.Contains(".png"))
                {
                    using (var client = new WebClient())
                    {
                        string SrcToDownload = srcValue.Replace(@"/imgres?imgurl=", "");
                        SrcToDownload = SrcToDownload.Substring(0, SrcToDownload.IndexOf('&'));
                        try
                        {
                            if (!File.Exists(@"E:\images\" + Product + "_" + i.ToString() + ".png"))
                            {
                                client.DownloadFile(SrcToDownload, @"E:\images\" + Product + "_" + i.ToString() + ".png");
                                i++;
                            }
                        }
                        catch
                        {

                        }
                        
                    }

                }
                if (i > qtyImages)
                {
                    break;
                }

                /* normal searc */
                    //string srcValue = link.GetAttributeValue("src", string.Empty);
                    //if (srcValue.ToLower().Contains(Product) && srcValue.ToLower().Contains(".png"))
                    //{
                    //    using (var client = new WebClient())
                    //    {
                    //        client.DownloadFile(srcValue, @"E:\images\"+ Product +"_"+ i.ToString()+ ".png");
                    //        i++;
                    //    }
                    //}
                    //if (i > 2)
                    //    break;
            }
        }
    }
}
