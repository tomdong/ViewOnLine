using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveDesign.Utilities;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using QuickMark;

namespace ViewOnLine
{
    public partial class MainForm : Form
    {
        private string mPathToUpload = "";
        public MainForm()
        {
            InitializeComponent();
            this.uploadBtn.Visible = false;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            path = path.ToLower();
            if (path.EndsWith(".png"))
            {
                this.pictureBox.Image = Image.FromFile(path);
                this.uploadBtn.Visible = true;
                mPathToUpload = path;
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void uploadBtn_Click(object sender, EventArgs e)
        {
            string url = "http://services.sketchbook.cn/sandbox/viewonline/index.php";
            //string fileName1 = @"d:\data\test1.psd";
            //string fileName2 = @"d:\data\test1.png";
            string fileName = mPathToUpload;
            //string fileName2 = mPathToUpload;
            UploadFile[] files = new UploadFile[] 
            { 
                //new UploadFile(fileName, "design_psd", "image/x-photoshop"),
                new UploadFile(fileName, "preview", "image/png")
            };

            NameValueCollection form = new NameValueCollection();

            //form["ownerid"] = "tom's id";
            //form["comments"] = "tom's comments";
            //form["design_guid"] = "test111";

            try
            {
                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

                // set credentials/cookies etc.

                HttpWebResponse resp = HttpUploadHelper.Upload(req, files, form);

                using (Stream s = resp.GetResponseStream())
                using (StreamReader sr = new StreamReader(s))
                {
                    string response = sr.ReadToEnd();
                    if (response.Length < 10)
                    {
                        MessageBox.Show("Failed to upload, report to Tom.");
                        return;
                    }

                    CreateTwoCode ctc = new CreateTwoCode();
                    string onlinePath = "http://services.sketchbook.cn/sandbox/viewonline/images/" + response + ".png";
                    this.pictureBox.Image = ctc.CreateCode(onlinePath, CreateTwoCode.CodeType.Byte, CreateTwoCode.Correct.H, 0, 10);
                    //ctc.CreateNewCode(onlinePath, "c:\\123.bmp");
                    //this.pictureBox.Image = Image.FromFile("c:\\123.bmp");
                    this.uploadBtn.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
