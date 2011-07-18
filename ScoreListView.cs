using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GPATool.Bean;
using GPATool.Util;

namespace GPATool
{
    public partial class ScoreListView : UserControl
    {
        public bool IsNormal { get; set; }
        private Thread thread = null;
        private Hashtable lessonsTable = new Hashtable();
        private List<Lesson> lessonBak = null;
        public GPAInfo GpaInfo { get; set; }
        private double majorCreditsDouble = 0;
        private double majorCreditsMultiScore = 0;
        private double majorFailCredits = 0;

        public ScoreListView()
        {
            InitializeComponent();
        }

        private void CourseListView_Load(object sender, EventArgs e)
        {
            XMLConfig.LoadConfig();
            numericUpDown1.DataBindings.Add("Enabled", checkBox1, "Checked");
            GpaInfo = new GPAInfo();
            label4.DataBindings.Add("Visible", this, "IsNormal");
            textBox2.DataBindings.Add("Visible", this, "IsNormal");
            checkBox2.DataBindings.Add("Visible", this, "IsNormal");
            textBox1.Text = XMLConfig.urpUsername;
            textBox2.Text = XMLConfig.urpPassword;
            checkBox1.Checked = XMLConfig.useAutoRefresh;
            numericUpDown1.Value = XMLConfig.autoRefreshInterval;
            IsNormal = !XMLConfig.isAdmin;
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || (IsNormal && String.IsNullOrEmpty(textBox2.Text)))
            {
                MessageBox.Show("学号和密码不能为空！", "检查输入" + IsNormal);
                return;
            }
            else
            {
                button1.Visible = false;
                panel1.Visible = true;
                thread = new Thread(refresh);
                thread.Start((Object)new String[] { textBox1.Text, textBox2.Text });
            }
        }

        private void filterSemester(ArrayList semester)
        {
            bool noFilter = false;
            if (semester.Count == 0)
            {
                noFilter = true;
            }
            listView1.Items.Clear();
            majorFailCredits = majorCreditsMultiScore = majorCreditsDouble = 0;
            majorGPA.Text = "0.00";
            majorCredits.Text = "0";
            foreach (Lesson l in lessonBak)
            {
                bool isNeed = (semester.BinarySearch(l.Semester) >= 0) ? true : false;
                isNeed |= noFilter;
                if (isNeed)
                {
                    listView1.Items.Add(new ListViewItem(new String[] { "否", l.Id + "", l.Semester, l.DetailCode, l.Name, l.Credit + "", l.ScoreString }));
                }
            }
        }

        private void listView1_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            listView1.ListViewItemSorter = new ListViewItemComparer(e.Column, listView1.Columns[e.Column]);
        }

        delegate void SetViewCallBack();
        public void enableView()
        {
            if (this.InvokeRequired)
            {
                SetViewCallBack cb = new SetViewCallBack(enableView);
                this.Invoke(cb);
            }
            else
            {
                listView1.Enabled = button1.Visible = button1.Enabled = true;
                panel1.Visible = false;
            }
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int step = (e.NewValue == CheckState.Checked) ? 1 : -1;
            listView1.Items[e.Index].SubItems[0].Text = (e.NewValue == CheckState.Checked) ? "是" : "否";
            listView1.Items[e.Index].BackColor = (e.NewValue == CheckState.Checked) ? Color.YellowGreen : Color.White;
            Lesson l = (Lesson)lessonsTable[listView1.Items[e.Index].SubItems[1].Text];
            if (!l.IsStar)
            {
                majorCreditsDouble += step * l.Credit;
                majorCreditsMultiScore += step * l.Credit * l.Score;
                if (l.Score == 0)
                {
                    majorFailCredits += step * l.Credit;
                }
            }
            majorCredits.Text = (majorCreditsDouble - majorFailCredits).ToString("#0.0");
            majorGPA.Text = (majorCreditsDouble == 0) ? "0.00" : (majorCreditsMultiScore / majorCreditsDouble).ToString("#0.00");
        }

        public void disableView()
        {
            if (this.InvokeRequired)
            {
                SetViewCallBack cb = new SetViewCallBack(disableView);
                this.Invoke(cb);
            }
            else
            {
                majorFailCredits = majorCreditsMultiScore = majorCreditsDouble = 0;
                majorCredits.Text = "0.0";
                majorGPA.Text = "0.00";
                comboBox1.Items.Clear();
                listBox1.Items.Clear();
                listView1.Items.Clear();
                listView1.Enabled = button1.Visible = groupBox4.Enabled = groupBox3.Enabled = groupBox2.Enabled = groupBox1.Enabled = false;
                panel1.Visible = true;
            }
        }

        private void refresh(Object o)
        {
            disableView();
            Object[] login = (Object[])o;
            bool showMsg = (login.Length == 3) ? (bool)login[2] : true;
            String html = null;
            WebProxy proxy = XMLConfig.GetProxy();
            try
            {
                html = ScoreHTMLUtil.TryGetHTMLString((String)login[0], (String)login[1], "9999", proxy, IsNormal);
                if (string.IsNullOrEmpty(html))
                {
                    throwError("无法抓取成绩信息，可能是服务器正忙或者是网络连接出错。", "网络错误", showMsg);
                }
                if (html.Contains("Course.jsp"))
                {
                    throwError("由于使用错误密码请求多次，请求被URP拒绝。\n请用浏览器重新登录URP输入正确的验证码后再使用本工具", "URP登录错误", showMsg);
                }
                else if (html.Contains("复旦大学统一身份认证服务"))
                {
                    throwError("学号密码不正确", "URP登录错误", showMsg);
                }
                List<Lesson> lessons = ParseHTML.TryParseHTML(html, GpaInfo, IsNormal);
                if (lessons == null)
                {
                    throwError("抓取的网页无法解析", "解析数据错误", showMsg);
                }
                refreshListView(lessons);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.HelpLink);
            }
            enableView();
        }

        private void throwError(String msg, String title, bool showMsg)
        {
            if (showMsg)
            {
                Exception e = new Exception(msg);
                e.HelpLink = title;
                throw e;
            }
        }

        delegate void SetListViewCallBack(List<Lesson> list);
        public void refreshListView(List<Lesson> list)
        {
            if (this.InvokeRequired)
            {
                SetListViewCallBack cb = new SetListViewCallBack(refreshListView);
                this.Invoke(cb, new object[] { list });
            }
            else
            {
                lessonsTable.Clear();
                listView1.Items.Clear();
                listBox1.Items.Clear();
                lessonBak = list;
                Hashtable hash = new Hashtable();
                Hashtable codeHash = new Hashtable();
                foreach (Lesson l in list)
                {
                    listView1.Items.Add(new ListViewItem(new String[] { "否", l.Id.ToString() , l.Semester, l.DetailCode, l.Name, l.Credit.ToString(), l.ScoreString }));
                    if (!hash.ContainsKey(l.Semester))
                    {
                        hash.Add(l.Semester, null);
                    }
                    if (!lessonsTable.ContainsKey(l.Id + ""))
                    {
                        lessonsTable.Add(l.Id + "", l);
                    }
                    String temp = null;
                    if ((temp = getEnglishCode(l.Code)) != null)
                    {
                        if (!codeHash.Contains(temp))
                        {
                            codeHash.Add(temp, 1);
                        }
                        else
                        {
                            codeHash[temp] = (int)codeHash[temp] + 1;
                        }
                    }
                }
                listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
                listView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize);
                listView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(4, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.HeaderSize);
                listView1.AutoResizeColumn(6, ColumnHeaderAutoResizeStyle.HeaderSize);
                ArrayList semestersList = new ArrayList(hash.Keys);
                semestersList.Sort();
                listBox1.Items.AddRange(semestersList.ToArray());
                List<String> codeList = new List<string>();
                IDictionaryEnumerator e = codeHash.GetEnumerator();
                int maxCount = 0;
                String maxCountCode = "";
                while (e.MoveNext())
                {
                    codeList.Add((String)e.Key);
                    if (maxCount <= (int)e.Value)
                    {
                        maxCountCode = (String)e.Key;
                        maxCount = (int)e.Value;
                    }
                }
                codeList.Sort();
                comboBox1.Items.AddRange(codeList.ToArray());
                if (!string.IsNullOrEmpty(maxCountCode))
                {
                    comboBox1.Text = maxCountCode;
                }
                label10.Text = GpaInfo.Name;
                label11.Text = GpaInfo.Major;
                label12.Text = GpaInfo.Gpa;
                label13.Text = GpaInfo.TotalCredit;
                groupBox4.Enabled = groupBox3.Enabled = groupBox2.Enabled = groupBox1.Enabled = listView1.Enabled = true;
            }
        }

        private String getEnglishCode(String code)
        {
            String buf = "";
            for (int i = 0; i < code.Length; i++)
            {
                if (char.IsLetter(code, i))
                {
                    buf += code[i];
                }
            }
            return string.IsNullOrEmpty(buf) ? null : buf;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void saveSettings()
        {
            XMLConfig.urpPassword = checkBox2.Checked ? RC2Util.Encrypt("hackerzhou", textBox2.Text) : "";
            XMLConfig.urpUsername = textBox1.Text;
            XMLConfig.isSavePassword = checkBox2.Checked;
            XMLConfig.useAutoRefresh = checkBox1.Checked;
            XMLConfig.autoRefreshInterval = (int)numericUpDown1.Value;
            XMLConfig.WriteConfig();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text = "";
            checkBox1.Checked = false;
            checkBox2.Checked = true;
            numericUpDown1.Value = 30;
            saveSettings();
        }

        public void StopAllThreads()
        {
            try
            {
                thread.Interrupt();
            }
            catch
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection c = listBox1.SelectedItems;
            ArrayList para = new ArrayList(c);
            filterSemester(para);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems[3].Text.StartsWith(comboBox1.Text))
                    {
                        item.Checked = true;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text))
            {
                button1.Visible = false;
                panel1.Visible = true;
                thread = new Thread(refresh);
                thread.Start((Object)new Object[] { textBox1.Text, textBox2.Text, true});
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            if(checkBox1.Checked){
                timer1.Interval = (int)numericUpDown1.Value * 60000;
                timer1.Start();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Interval = (int)numericUpDown1.Value * 60000;
            timer1.Start();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopyAllItemsToClipBoard(listView1, 1);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopySelectedItemsToClipBoard(listView1, 1);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopySelectedItemColumnToClipBoard(listView1, 3);
        }
    }
}
