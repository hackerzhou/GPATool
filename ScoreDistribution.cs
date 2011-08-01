using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GPATool.Bean;
using GPATool.Util;
using System.Threading;

namespace GPATool
{
    public partial class ScoreDistribution : UserControl
    {
        private List<ScoreDistributionItem> courseList = null;
        private Thread tSDThread;
        private Thread sSDThread;
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
            scoreDistributionChartPanel1.Visible = false;
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
            listView1.Height = 449;
            Object para = (e == null) ? getQueryParameter(sender.ToString()) : getQueryParameter();
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
            disableView();
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

        delegate void LoadCourseListViewDelegate(List<ScoreDistributionItem> para);
        private void loadCourseListViewHandler(List<ScoreDistributionItem> para)
        {
            courseList = para;
            listView1.Items.Clear();
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
                SetViewDelegate cb = new SetViewDelegate(disableView);
                this.Invoke(cb);
            }
            else
            {
                scoreDistributionChartPanel1.HidePanel();
                button1.Visible = listView1.Enabled = groupBox1.Enabled = false;
                panel1.Show();
                pictureBox1.Refresh();
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
                panel1.Hide();
                button1.Visible = groupBox1.Enabled = listView1.Enabled = true;
            }
        }

        private void busyAnalysticView()
        {
            if (this.InvokeRequired)
            {
                SetViewDelegate cb = new SetViewDelegate(busyAnalysticView);
                this.Invoke(cb);
            }
            else
            {
                button1.Enabled = listView1.Enabled = false;
                pictureBox2.Visible = true;
                pictureBox2.Refresh();
                scoreDistributionChartPanel1.HidePanel();
            }
        }

        private void unBusyAnalysticView()
        {
            if (this.InvokeRequired)
            {
                SetViewDelegate cb = new SetViewDelegate(unBusyAnalysticView);
                this.Invoke(cb);
            }
            else
            {
                button1.Enabled = listView1.Enabled = true;
                pictureBox2.Visible = false;
                scoreDistributionChartPanel1.ShowPanel();
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
            String[][] listViewItems = new String[args.Scores.Count][];
            DataPoint[] dps = new DataPoint[args.Scores.Count];
            for (int i = 0;i<args.Scores.Count;i++)
            {
                ScoreItem si = args.Scores[i];
                String percentage = ((double)si.StudentCount / args.StudentCount).ToString("P");
                listViewItems[i] = new String[] { si.DisplayValue, si.StudentCount.ToString(), percentage };
                dps[i] = new DataPoint(0, si.StudentCount);
                dps[i] .Label = si.DisplayValue;
                dps[i] .AxisLabel = si.DisplayValue;
                dps[i] .LabelBorderWidth = 100;
                dps[i] .ToolTip = si.DisplayValue + " " + si.StudentCount + "人";
            }
            scoreDistributionChartPanel1.SetPanelType(ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_COURSE);
            scoreDistributionChartPanel1.RefreshData(listViewItems,dps);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopyAllItemsToClipBoard(listView1);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopySelectedItemsToClipBoard(listView1);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                ScoreDistributionItem i = courseList[listView1.SelectedIndices[0]];
                if (i != null && !string.IsNullOrEmpty(i.Teacher) && !string.IsNullOrEmpty(i.LessonName))
                {
                    sSDThread = new Thread(new ParameterizedThreadStart(queryAnalyticsSD));
                    sSDThread.Start(new object[] { i, "Semester" });
                }
                else
                {
                    MessageBox.Show("该课程信息不完整，无法进行继续查询！", "无法查询");
                }
            }
        }

        private void queryAnalyticsSD(Object o)
        {
            ScoreDistributionItem args = (ScoreDistributionItem)((Object[])o)[0];
            String analyticsType= (String)((Object[])o)[1];
            bool isSemesterAnalytics = "Semester".Equals(analyticsType);
            busyAnalysticView();
            List<ScoreDistributionItem> list = null;
            try
            {
                if (isSemesterAnalytics)
                {
                    list = ScoreDistributionHelper.QueryTeacherSDChangeBySemester(args);
                }
                else
                {
                    list = ScoreDistributionHelper.QueryLessonSDChangeByTeacher(args);
                }
            }
            catch (Exception e)
            {
                unBusyAnalysticView();
                MessageBox.Show("数据查询出错！\n详细信息：" + e.Message);
                return;
            }
            String[][] listViewItems = new String[list.Count][];
            DataPoint[] dps = new DataPoint[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                ScoreDistributionItem sdi = list[i];
                dps[i] = new DataPoint(0, sdi.AverageScore);
                dps[i].AxisLabel = " ";
                listViewItems[i] = new String[] { isSemesterAnalytics ? sdi.Semester : sdi.Teacher, sdi.AverageScore.ToString("0.00"), sdi.Remark };
                dps[i].ToolTip = (isSemesterAnalytics ? "学期：" + sdi.Semester : "教师：" + sdi.Teacher) + "\n平均成绩：" + sdi.AverageScore.ToString("0.00") + " (" + Lesson.GetScoreDetailString(sdi.AverageScore) + ")\n比例最大的成绩：" + sdi.Remark;
                dps[i].MarkerStyle = MarkerStyle.Circle;
                dps[i].MarkerSize = 10;
            }
            this.setSDChartPanelHandler(isSemesterAnalytics ? ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_SEMESTER : ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_TEACHER
                , listViewItems, dps);
            unBusyAnalysticView();
        }

        delegate void SetSDChartPanelDelegate(ScoreDistributionChartPanelType t, String[][] s, DataPoint[] d);
        private void setSDChartPanelHandler(ScoreDistributionChartPanelType t, String[][] s, DataPoint[] d)
        {
            if (this.InvokeRequired)
            {
                SetSDChartPanelDelegate cb = new SetSDChartPanelDelegate(setSDChartPanelHandler);
                this.Invoke(cb, t, s, d);
            }
            else
            {
                scoreDistributionChartPanel1.SetPanelType(t);
                scoreDistributionChartPanel1.RefreshData(s, d);
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                ScoreDistributionItem i = courseList[listView1.SelectedIndices[0]];
                if (i != null && !string.IsNullOrEmpty(i.Teacher) && !string.IsNullOrEmpty(i.LessonName))
                {
                    tSDThread = new Thread(new ParameterizedThreadStart(queryAnalyticsSD));
                    tSDThread.Start(new object[] { i, "Teacher" });
                }
                else
                {
                    MessageBox.Show("该课程信息不完整，无法进行继续查询！", "无法查询");
                }
            }
        }

        public void StopAllThreads()
        {
            try
            {
                if (tSDThread != null && tSDThread.ThreadState != ThreadState.Stopped)
                {
                    tSDThread.Interrupt();
                }
            }
            catch
            {
            }
            try
            {
                if (sSDThread != null && sSDThread.ThreadState != ThreadState.Stopped)
                {
                    sSDThread.Interrupt();
                }
            }
            catch
            {
            }
        }
    }
}
