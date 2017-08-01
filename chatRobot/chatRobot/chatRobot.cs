using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.IO;

namespace chatRobot
{
    public partial class chatRobot : Form
    {
        public chatRobot()
        {
            InitializeComponent();
            this.webBrowser1.Navigate(index);
        }

        string index = Application.StartupPath + "../../htmlview/index.html";
        UTF8Encoding utf8 = new UTF8Encoding();

        string appid = ConfigurationManager.AppSettings["appid"];

        public void getAnswer(string question)
        {
            string url = "http://www.tuling123.com/openapi/api";
            string postData = "key=" + appid + "&info=" + question;
            byte[] byteResquest = Encoding.GetEncoding("utf-8").GetBytes(postData);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "Post";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteResquest.Length;

            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.Write(postData);
            sw.Flush();

            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));

            //得到json
            string json = sr.ReadToEnd();
            MessageBox.Show(json);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            getAnswer("你好");
        }

    }
}
