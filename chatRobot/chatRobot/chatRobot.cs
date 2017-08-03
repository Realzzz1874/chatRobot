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
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

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
        string json = "";
        string question = "";
        string text = "";

        public String getAnswer(string question)
        {
            string url = "http://www.tuling123.com/openapi/api";
            string postData = "key=" + appid + "&info=" + question;
            try
            {
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
                json = sr.ReadToEnd();
            }
            catch
            {
                json = "";
            }
            return json;
        }

        public String parseAnswer(string json)
        {
            
            if (json == "")
            {
                text = "";
            }
            else
            {
                try
                {
                    JObject jsonObject = new JObject();
                    jsonObject = JObject.Parse(json);
                    JToken textToken = jsonObject["text"];
                    text = textToken.ToString();
                }
                catch
                {
                    text = "";
                }
            }
            return text;

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            question = questionText.Text;
            string json = getAnswer(question);
            text = parseAnswer(json);
            MessageBox.Show(text);
        }

    }
}
