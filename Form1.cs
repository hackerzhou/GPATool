using System;
using DevComponents.DotNetBar;
using GPATool.Util;
using System.Threading;
using System.Net;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Web;
using System.IO;

namespace GPATool
{
    public partial class Form1 :DevComponents.DotNetBar.Office2007RibbonForm
    {
        private static Form1 instance = null;
        private static bool isAdmin = false;
        private static Thread updateThread;

        public static Form1 getInstance()
        {
            return instance;
        }

        public Form1()
        {
            instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XMLConfig.LoadConfig();
            isAdmin = XMLConfig.isAdmin;
            if (isAdmin)
            {
                this.Text = "复旦大学绩点查询工具 v1.10 专业版 by hackerzhou";
            }
            ribbonTabItem4.Checked = ribbonTabItem1.Checked = ribbonTabItem2.Checked = true;
            updateThread = new Thread(new ThreadStart(UpdateUtil.CheckUpdate));
            updateThread.Start();
        }

        public void ChangeStyle(eStyle style)
        {
            styleManager1.ManagerStyle = style;
        }

        void Form1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            StopAllThreads();
            scoreListView1.StopAllThreads();
            userConfigView1.StopAllThreads();
            scoreDistribution2.StopAllThreads();
        }

        

        public void StopAllThreads()
        {
            try
            {
                if (updateThread != null && updateThread.ThreadState != ThreadState.Stopped)
                {
                    updateThread.Interrupt();
                }
            }
            catch
            {
            }
        }
    }
}
