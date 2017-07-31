using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    }
}
