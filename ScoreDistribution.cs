using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GPATool.Bean;
using GPATool.Util;

namespace GPATool
{
    public partial class ScoreDistribution : UserControl
    {
        private List<ScoreDistributionItem> courseList = null;
        public ScoreDistribution()
        {
            InitializeComponent();
        }

        private void ScoreDistribution_Load(object sender, EventArgs e)
        {
            comboBox1.DataBindings.Add("Enabled", checkBox1, "Checked");
            textBox1.DataBindings.Add("Enabled", checkBox2, "Checked");
            textBox2.DataBindings.Add("Enabled", checkBox3, "Checked");
            textBox3.DataBindings.Add("Enabled", checkBox4, "Checked");
            numericUpDown1.DataBindings.Add("Enabled", checkBox6, "Checked");
            numericUpDown2.DataBindings.Add("Enabled", checkBox5, "Checked");
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.AutoResizeColumn(6, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.BeginInvoke(new LoadData(LoadDataHandler));
        }

        private delegate void LoadData();
        private void LoadDataHandler()
        {
            if (File.Exists("data.s3db"))
            {
                this.comboBox1.Items.AddRange(ScoreDistributionHelper.LoadAllSemesters().ToArray());
                if (this.comboBox1.Items.Count > 0)
                {
                    this.comboBox1.SelectedIndex = 0;
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader ch = listView1.Columns[e.Column];
            String orderBy = null;
            if (((String)ch.Tag).Equals("semester"))
            {
                ch.Tag = "semesterDesc";
                orderBy = "order by s.name desc";
            }
            else if (((String)ch.Tag).Equals("semesterDesc"))
            {
                ch.Tag = "semester";
                orderBy = "order by s.name";
            }
            else if (((String)ch.Tag).Equals("lessonCode"))
            {
                ch.Tag = "lessonCodeDesc";
                orderBy = "order by l.lessonCode desc";
            }
            else if (((String)ch.Tag).Equals("lessonCodeDesc"))
            {
                ch.Tag = "lessonCode";
                orderBy = "order by l.lessonCode";
            }
            else if (((String)ch.Tag).Equals("lessonName"))
            {
                ch.Tag = "lessonNameDesc";
                orderBy = "order by l.lessonName desc";
            }
            else if (((String)ch.Tag).Equals("lessonNameDesc"))
            {
                ch.Tag = "lessonName";
                orderBy = "order by l.lessonName";
            }
            else if (((String)ch.Tag).Equals("teacher"))
            {
                ch.Tag = "teacherDesc";
                orderBy = "order by t.name desc";
            }
            else if (((String)ch.Tag).Equals("teacherDesc"))
            {
                ch.Tag = "teacher";
                orderBy = "order by t.name";
            }
            else if (((String)ch.Tag).Equals("credit"))
            {
                ch.Tag = "creditDesc";
                orderBy = "order by l.creditPoint desc";
            }
            else if (((String)ch.Tag).Equals("creditDesc"))
            {
                ch.Tag = "credit";
                orderBy = "order by l.creditPoint";
            }
            else if (((String)ch.Tag).Equals("stuCount"))
            {
                ch.Tag = "stuCountDesc";
                orderBy = "order by c.totalStudentNumber desc";
            }
            else if (((String)ch.Tag).Equals("stuCountDesc"))
            {
                ch.Tag = "stuCount";
                orderBy = "order by c.totalStudentNumber";
            }
            button1_Click(orderBy, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists("data.s3db"))
            {
                MessageBox.Show("找不到给分数据库文件data.s3db！", "数据文件丢失");
                return;
            }
            listView1.Height = 430;
            Object para = (e == null) ? getQueryParameter(sender.ToString()) : getQueryParameter();
            disableView();
            this.BeginInvoke(new QueryCourseDelegate(queryCourse), para);
        }

        private Object getQueryParameter(String orderBy = null)
        {
            Object[] para = new Object[8];
            if (checkBox1.Checked && comboBox1.SelectedItem != null && !string.IsNullOrEmpty(comboBox1.SelectedItem.ToString()))
            {
                para[0] = comboBox1.SelectedItem.ToString();
            }
            else
            {
                para[0] = null;
            }
            para[1] = (checkBox2.Checked && textBox1.Text.Length > 1) ? textBox1.Text : null;
            para[2] = (checkBox4.Checked && textBox3.Text.Length > 1) ? textBox3.Text : null;
            para[3] = (checkBox3.Checked && textBox2.Text.Length > 1) ? textBox2.Text : null;
            para[4] = (checkBox5.Checked) ? (int)numericUpDown2.Value : -1;
            para[5] = (checkBox6.Checked) ? (int)numericUpDown1.Value : -1;
            para[6] = checkBox7.Checked;
            para[7] = orderBy;
            for (int i = 0; i < para.Length; i++)
            {
                if (para[i] is String)
                {
                    String s = (String)para[i];
                    s = s.Replace("'", "");
                    para[i] = s;
                }
            }
            return para;
        }
        private delegate void QueryCourseDelegate(Object args);

        private void queryCourse(Object args)
        {
            Object[] o = (Object[])args;
            String semester = (String)o[0];
            String lessonCodeContains = (String)o[1];
            String lessonNameContains = (String)o[2];
            String teacherNameContains = (String)o[3];
            int ignoreLessThan = (int)o[4];
            int limit = (int)o[5];
            bool ignoreImcompleteInfo = (bool)o[6];
            String orderBy = (String)o[7];
            List<ScoreDistributionItem> list = null;
            try
            {
                list = ScoreDistributionHelper.Query(semester, lessonCodeContains, lessonNameContains
                    , teacherNameContains, ignoreLessThan, limit, ignoreImcompleteInfo, orderBy);
                loadCourseListViewHandler(list);
            }
            catch(Exception e)
            {
                MessageBox.Show("请检查数据文件data.s3db是否被破坏或删除！\n详细信息：" + e.Message, "数据查询出错");
            }
            finally
            {
                enableView();
            }
        }

        private void loadCourseListViewHandler(List<ScoreDistributionItem> para)
        {
            courseList = para;
            listView1.Items.Clear();
            listView2.Items.Clear();
            foreach (ScoreDistributionItem s in para)
            {
                listView1.Items.Add(new ListViewItem(new String[] { s.Id.ToString(), s.Semester, s.LessonCode, s.LessonName, s.Teacher, s.Credits.ToString(), s.StudentCount.ToString() }));
            }
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumn(4, ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.AutoResizeColumn(6, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        delegate void SetViewDelegate();

        private void disableView()
        {
            if (this.InvokeRequired)
            {
                SetViewDelegate cb = new SetViewDelegate(enableView);
                this.Invoke(cb);
            }
            else
            {
                panel1.Visible = true;
                panel2.Visible = listView1.Enabled = groupBox1.Enabled = button1.Visible = false;
            }
        }

        private void enableView()
        {
            if (this.InvokeRequired)
            {
                SetViewDelegate cb = new SetViewDelegate(enableView);
                this.Invoke(cb);
            }
            else
            {
                panel1.Visible = false;
                listView1.Enabled = groupBox1.Enabled = button1.Visible = true;
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && e.ItemIndex != -1 && courseList.Count > e.ItemIndex)
            {
                listView1.Height = 200;
                this.BeginInvoke(new QueryDistributionDelegate(queryDistribution), courseList[e.ItemIndex]);
            }
        }
        private delegate void QueryDistributionDelegate(ScoreDistributionItem args);
        private void queryDistribution(ScoreDistributionItem args)
        {
            if (args.Scores == null || args.Scores.Count == 0)
            {
                try
                {
                    ScoreDistributionHelper.QueryScoreDistribution(args);
                }
                catch (Exception e)
                {
                    MessageBox.Show("数据查询出错！\n详细信息：" + e.Message);
                }
            }
            listView2.Items.Clear();
            chart1.Series["Series1"].Points.Clear();
            foreach (ScoreItem si in args.Scores)
            {
                String percentage = ((double)si.StudentCount / args.StudentCount).ToString("P");
                listView2.Items.Add(new ListViewItem(new String[] { si.DisplayValue, si.StudentCount.ToString(), percentage }));
                DataPoint p = new DataPoint(0, si.StudentCount);
                p.Label = si.DisplayValue;
                p.LabelBorderWidth = 100;
                p.ToolTip = si.DisplayValue + " " + si.StudentCount + "人";
                chart1.Series["Series1"].Points.Add(p);
            }
            panel2.Visible = true;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopyAllItemsToClipBoard(listView2);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopyAllItemsToClipBoard(listView1);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopySelectedItemsToClipBoard(listView1);
        }
    }
}
