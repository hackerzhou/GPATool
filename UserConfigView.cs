using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using DevComponents.DotNetBar;
using GPATool.Util;

namespace GPATool
{
    public partial class UserConfigView : UserControl
    {
        private Thread fetchRSSThread = null;
        private int savedStyleIndex;

        public UserConfigView()
        {
            InitializeComponent();
        }

        public void StopAllThreads()
        {
            try
            {
                if (fetchRSSThread != null && fetchRSSThread.ThreadState != ThreadState.Stopped)
                {
                    fetchRSSThread.Interrupt();
                }
            }
            catch
            {
            }
        }

        private void UserConfigView_Load(object sender, EventArgs e)
        {
            XMLConfig.LoadConfig();
            panel2.DataBindings.Add("Enabled", checkBox3, "Checked");
            panel3.DataBindings.Add("Enabled", checkBox4, "Checked");
            checkBox3.Checked = XMLConfig.useProxy;
            checkBox4.Checked = XMLConfig.proxyUseAuth;
            textBox3.Text = XMLConfig.proxyHost;
            textBox4.Text = XMLConfig.proxyPort.ToString();
            textBox6.Text = XMLConfig.proxyUsername;
            textBox5.Text = XMLConfig.proxyPassword;
            savedStyleIndex = comboBox2.SelectedIndex = XMLConfig.styleIndex;
            fetchRSSThread = new Thread(new ThreadStart(updateRSSHandler));
            fetchRSSThread.Start();
        }

        private delegate void UpdateRSSListViewDelegate(List<RSSSpider.RSSItem> list);

        private void updateRSSListView(List<RSSSpider.RSSItem> list)
        {
            if (this.InvokeRequired)
            {
                UpdateRSSListViewDelegate cb = new UpdateRSSListViewDelegate(updateRSSListView);
                this.Invoke(cb, list);
            }
            else
            {
                listView2.Items.Clear();
                foreach (RSSSpider.RSSItem i in list)
                {
                    ListViewItem listItem = new ListViewItem(new String[] { i.Name, i.UpdateDate });
                    listItem.Tag = i.Url;
                    listView2.Items.Add(listItem);
                }
            }
        }

        private void updateRSSHandler()
        {
            List<RSSSpider.RSSItem> list = RSSSpider.RSSUtil.GetRSSItemList("http://hackerzhou.me/feed");
            updateRSSListView(list);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (savedStyleIndex != comboBox2.SelectedIndex)
            {
                button7_Click(sender, null);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            XMLConfig.useProxy = checkBox3.Checked;
            XMLConfig.proxyUseAuth = checkBox4.Checked;
            XMLConfig.proxyHost = textBox3.Text;
            int port = 8080;
            int.TryParse(textBox4.Text, out port);
            XMLConfig.proxyPort = port;
            XMLConfig.proxyUsername = textBox6.Text;
            XMLConfig.proxyPassword = textBox5.Text;
            XMLConfig.WriteConfig();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            String style = comboBox2.SelectedItem.ToString();
            Form1 f = Form1.getInstance();
            if (f == null)
            {
                return;
            }
            if ("Office2007Black".Equals(style))
            {
                f.ChangeStyle(eStyle.Office2007Black);
            }
            else if ("Office2007Blue".Equals(style))
            {
                f.ChangeStyle(eStyle.Office2007Blue);
            }
            else if ("Office2007Silver".Equals(style))
            {
                f.ChangeStyle(eStyle.Office2007Silver);
            }
            else if ("Office2007VistaGlass".Equals(style))
            {
                f.ChangeStyle(eStyle.Office2007VistaGlass);
            }
            else if ("Office2010Silver".Equals(style))
            {
                f.ChangeStyle(eStyle.Office2010Silver);
            }
            else if ("Windows7Blue".Equals(style))
            {
                f.ChangeStyle(eStyle.Windows7Blue);
            }
            if (e != null)
            {
                XMLConfig.styleIndex = comboBox2.SelectedIndex;
                XMLConfig.WriteConfig();
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://bbs.fudan.sh.cn/bbs/qry?u=hackerzhou");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://hackerzhou.me");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/hackerzhou/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://twitter.com/hackerzhou");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:hackerzhou@hackerzhou.me");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://weibo.com/hackerzhou");
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                System.Diagnostics.Process.Start(listView2.SelectedItems[0].Tag.ToString());
            }
        }
    }
}
