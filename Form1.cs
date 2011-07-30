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
        private const double currentVersion = 1.1;

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
            updateThread = new Thread(new ThreadStart(checkUpdate));
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

        private void checkUpdate()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load("http://wiki.hackerzhou.me/GPATool");
                XmlNodeList list = doc.SelectNodes("//table[@id='updateData']/tr");
                double versionValue = 0;
                String downloadUrl = null;
                foreach (XmlNode n in list)
                {
                    String version = n.ChildNodes[0].InnerText.Replace("v", "");
                    String downloadTemp = n.ChildNodes[1].InnerText;
                    double versionTemp = 0;
                    double.TryParse(version, out versionTemp);
                    if (versionTemp > versionValue)
                    {
                        versionValue = versionTemp;
                        downloadUrl = HttpUtility.UrlDecode(downloadTemp);
                    }
                }
                if (versionValue > currentVersion && downloadUrl != null && downloadUrl.StartsWith("http://hackerzhou.googlecode.com"))
                {
                    String upgradeFile = HTTPUtil.GetFileNameFromUrl(downloadUrl);
                    String filePath = Environment.CurrentDirectory + "\\" + upgradeFile;
                    bool success = HTTPUtil.SaveUrlContentToFile(downloadUrl, filePath);
                    if (success)
                    {
                        MessageBox.Show("已下载更新版 v" + versionValue.ToString("0.00") + " 到 " + upgradeFile + "\n请解压覆盖旧版本应用更新", "更新");
                    }
                }
            }
            catch
            {
            }
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
