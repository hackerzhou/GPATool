using System;
using DevComponents.DotNetBar;
using GPATool.Util;

namespace GPATool
{
    public partial class Form1 :DevComponents.DotNetBar.Office2007RibbonForm
    {
        private static Form1 instance = null;
        private static bool isAdmin = false;
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
                this.Text = "复旦大学绩点查询工具 v1.01 专业版 by hackerzhou";
            }
            ribbonTabItem4.Checked = ribbonTabItem1.Checked = ribbonTabItem2.Checked = true;
        }

        public void ChangeStyle(eStyle style)
        {
            styleManager1.ManagerStyle = style;
        }

        void Form1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            scoreListView1.StopAllThreads();
            userConfigView1.StopAllThreads();
        }
    }
}
