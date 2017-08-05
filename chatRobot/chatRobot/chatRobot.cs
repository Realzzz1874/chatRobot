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
using Speak;
using System.Threading;

namespace chatRobot
{
    public partial class chatRobot : Form
    {
        public chatRobot()
        {
            InitializeComponent();
            this.webBrowser1.Navigate(index);
        }

        string index = Application.StartupPath + "../htmlview/index.html";
        UTF8Encoding utf8 = new UTF8Encoding();

        string appid = ConfigurationManager.AppSettings["appid"];
        string json = "";
        string question = "";
        string text = "";
        int userInsertFlag = 0;
        int robotInsertFlag = 0;
        static string receiveData;

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

        public void insertUserMsg(string question)
        {
            string q = question;
            if (q == "")
            {
                return;
            }
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(index, utf8);
                var node = doc.DocumentNode.SelectSingleNode("//body//div//ul");
                HtmlNode newUserMsg = HtmlNode.CreateNode("<li class='user'><div>" + q + "</div></li>" + "\n");
                node.AppendChild(newUserMsg);
                doc.Save(index, utf8);
                webBrowser1.Refresh();
                System.Windows.Forms.Application.DoEvents();
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollTop, webBrowser1.Document.Body.ScrollRectangle.Bottom);
                userInsertFlag = 1;
            }
            catch
            {
                userInsertFlag = 0;
            }
        }

        public void insertRobotMsg(string text)
        {
            string t = text;
            if (t == "")
            {
                return;
            }
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(index, utf8);
                var node = doc.DocumentNode.SelectSingleNode("//body//div//ul");
                var robotNode = "<li class = 'robot'><div><span>" + t + "</span></div></li>";
                HtmlNode newRobotMsg = HtmlNode.CreateNode(robotNode);
                node.AppendChild(newRobotMsg);
                doc.Save(index, utf8);
                webBrowser1.Refresh();

                System.Windows.Forms.Application.DoEvents();
                webBrowser1.Document.Window.ScrollTo(webBrowser1.Document.Body.ScrollTop, webBrowser1.Document.Body.ScrollRectangle.Bottom);
                robotInsertFlag = 1;
            }
            catch
            {
                robotInsertFlag = 0;
            }
        }

        public void clearMsg()
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(index);
            var node = doc.DocumentNode.SelectSingleNode("//body//div/ul");
            node.RemoveAllChildren();
            doc.Save(index);
            webBrowser1.Refresh();
        }



        //语音
        static void receiverDataHandler(string str)
        {
            receiveData += str;
        }

        public void iatPlay()
        {
            try
            {
                SpeakFunction speaker = new SpeakFunction();
                speaker.DataReceive += receiverDataHandler;
                string login_configs = "dvc=Name32890, appid=58d328e1, work_dir =   .  ,timeout=1000";//登录参数
                string param1 = "sub=iat,ssm=1,auf=audio/L16;rate=16000,aue=speex-wb;7,ent=sms16k,rst=plain,rse=gb2312";
                speaker.Begin_Translate2(login_configs, param1);
                questionText.Text = receiveData;
            }
            catch
            {
                
            }
            finally
            {
                receiveData = "";
                pictureBox2.Enabled = true;
                pictureBox2.Image = Properties.Resources.speak;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            question = questionText.Text;
            questionText.Text = "";
            insertUserMsg(question);
            string json = getAnswer(question);
            text = parseAnswer(json);
            //MessageBox.Show(text);
            if(userInsertFlag == 1)
            {
                insertRobotMsg(text);
            }
            
        }

        private void chatRobot_Load(object sender, EventArgs e)
        {
            clearMsg();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox2.Enabled = false;
            pictureBox2.Image = Properties.Resources.speaking;
            ThreadStart threadStart = new ThreadStart(delegate()
                {
                    iatPlay();
                });
            Thread thread = new Thread(threadStart);
            Control.CheckForIllegalCrossThreadCalls = false;
            thread.Start();
            
        }

    }
}
