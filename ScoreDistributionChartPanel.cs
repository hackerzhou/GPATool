using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GPATool.Util;

namespace GPATool
{
    public enum ScoreDistributionChartPanelType
    {
        SCORE_DISTRIBUTION_COURSE,
        SCORE_DISTRIBUTION_SEMESTER,
        SCORE_DISTRIBUTION_TEACHER,
        NOT_SET
    }
    public partial class ScoreDistributionChartPanel : UserControl
    {
        private ScoreDistributionChartPanelType panelType = ScoreDistributionChartPanelType.NOT_SET;
        private Color savedDataPointColor;
        
        public ScoreDistributionChartPanel()
        {
            InitializeComponent();
        }

        public void SetPanelType(ScoreDistributionChartPanelType type)
        {
            if (this.panelType != type)
            {
                this.HidePanel();
                this.panelType = type;
                comboBox2.Items.Clear();
                if (type == ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_COURSE)
                {
                    comboBox2.Items.AddRange(new String[] { "饼状图", "柱状图" });
                    chart1.Series[0].ChartType = SeriesChartType.Pie;
                    listView2.Columns[0].Text = "成绩";
                    listView2.Columns[1].Text = "人数";
                    listView2.Columns[2].Text = "百分比";
                    chart1.ChartAreas[0].AxisY.Maximum = double.NaN;
                    chart1.ChartAreas[0].AxisY.MajorTickMark.Interval = 0;
                    chart1.ChartAreas[0].AxisY.Interval = 0;
                    chart1.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
                }
                else if (type == ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_SEMESTER
                    || type == ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_TEACHER)
                {
                    if (type == ScoreDistributionChartPanelType.SCORE_DISTRIBUTION_SEMESTER)
                    {
                        comboBox2.Items.AddRange(new String[] { "折线图", "柱状图" });
                        chart1.Series[0].ChartType = SeriesChartType.Line;
                        listView2.Columns[0].Text = "学期";
                    }
                    else
                    {
                        comboBox2.Items.AddRange(new String[] { "柱状图", "条状图" });
                        chart1.Series[0].ChartType = SeriesChartType.Column;
                        listView2.Columns[0].Text = "老师";
                    }
                    listView2.Columns[1].Text = "平均成绩";
                    listView2.Columns[2].Text = "Top 2 比例成绩";
                    chart1.ChartAreas[0].AxisY.Maximum = 4.0;
                    chart1.ChartAreas[0].AxisY.MajorTickMark.Interval = 0.5;
                    chart1.ChartAreas[0].AxisY.Interval = 0.5;
                    chart1.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
                }
                comboBox2.SelectedIndex = 0;
            }
        }

        public void RefreshData(String[][] listViewItemStr, DataPoint[] chartPoint)
        {
            chart1.Series[0].Points.Clear();
            listView2.Items.Clear();
            foreach (String[] listItem in listViewItemStr)
            {
                listView2.Items.Add(new ListViewItem(listItem));
            }
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            foreach (DataPoint dp in chartPoint)
            {
                chart1.Series[0].Points.Add(dp);
            }
            comboBox2_SelectedIndexChanged(null, null);
            this.ShowPanel();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool AnimateWindow(IntPtr hwnd, uint dwTime, uint dwFlags);
        public const Int32 AW_HOR_POSITIVE = 0x00000001;
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;
        public const Int32 AW_VER_POSITIVE = 0x00000004;
        public const Int32 AW_VER_NEGATIVE = 0x00000008;
        public const Int32 AW_CENTER = 0x00000010;
        public const Int32 AW_HIDE = 0x00010000;
        public const Int32 AW_ACTIVATE = 0x00020000;
        public const Int32 AW_SLIDE = 0x00040000;
        public const Int32 AW_BLEND = 0x00080000;
        private delegate void SetPanelOperation();
        public void ShowPanel()
        {
            if (this.InvokeRequired)
            {
                SetPanelOperation cb = new SetPanelOperation(ShowPanel);
                this.Invoke(cb);
            }
            else
            {
                AnimateWindow(Handle, 300, AW_VER_POSITIVE | AW_SLIDE);
                this.Show();
                this.chart1.Visible = true;
            }
        }

        public void HidePanel()
        {
            if (this.InvokeRequired)
            {
                SetPanelOperation cb = new SetPanelOperation(HidePanel);
                this.Invoke(cb);
            }
            else
            {
                AnimateWindow(Handle, 300, AW_VER_NEGATIVE | AW_HIDE | AW_SLIDE);
                this.Hide();
                this.chart1.Visible = false;
            }
        }

        private void ScoreDistributionChartPanel_Load(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewStripMenuHelper.CopyAllItemsToClipBoard(listView2);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0)
            {
                chart1.Series[0].Points[listView2.SelectedIndices[0]].BorderWidth = 0;
                listView2.SelectedIndices.Clear();
            }
            if ("饼状图".Equals(comboBox2.SelectedItem))
            {
                chart1.Series[0].ChartType = SeriesChartType.Pie;
                chart1.Series[0].IsVisibleInLegend = true;
            }
            else
            {
                chart1.Series[0].IsVisibleInLegend = false;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisX.ArrowStyle = AxisArrowStyle.Triangle;
                chart1.ChartAreas[0].AxisY.ArrowStyle = AxisArrowStyle.Triangle;
                if ("柱状图".Equals(comboBox2.SelectedItem))
                {
                    chart1.Series[0].ChartType = SeriesChartType.Column;
                    foreach (DataPoint dp in chart1.Series[0].Points)
                    {
                        dp.MarkerStyle = MarkerStyle.None;
                    }
                }
                else if ("折线图".Equals(comboBox2.SelectedItem))
                {
                    chart1.Series[0].ChartType = SeriesChartType.Line;
                    foreach (DataPoint dp in chart1.Series[0].Points)
                    {
                        dp.MarkerStyle = MarkerStyle.Circle;
                        dp.MarkerSize = 10;
                    }
                }
                else if ("条状图".Equals(comboBox2.SelectedItem))
                {
                    chart1.Series[0].ChartType = SeriesChartType.Bar;
                    foreach (DataPoint dp in chart1.Series[0].Points)
                    {
                        dp.MarkerStyle = MarkerStyle.None;
                    }
                }
            }
        }

        private void listView2_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                int index = e.ItemIndex;
                if (index >= 0 && index < chart1.Series[0].Points.Count)
                {
                    Color highlightColor = Color.Orange;
                    if ("柱状图".Equals(comboBox2.SelectedItem))
                    {
                        savedDataPointColor = chart1.Series[0].Points[index].Color;
                        chart1.Series[0].Points[index].Color = highlightColor;
                    }
                    else if ("饼状图".Equals(comboBox2.SelectedItem))
                    {
                        savedDataPointColor = chart1.Series[0].Points[index].BorderColor;
                        chart1.Series[0].Points[index].BorderColor = Color.OrangeRed;
                        chart1.Series[0].Points[index].BorderWidth = 3;
                        chart1.Series[0].Points[index].BorderDashStyle = ChartDashStyle.Solid;
                    }
                    else
                    {
                        savedDataPointColor = chart1.Series[0].Points[index].MarkerColor;
                        chart1.Series[0].Points[index].MarkerColor = highlightColor;
                    }
                }
            }
            else
            {
                if ("柱状图".Equals(comboBox2.SelectedItem))
                {
                    chart1.Series[0].Points[e.ItemIndex].Color = savedDataPointColor;
                }
                else if ("饼状图".Equals(comboBox2.SelectedItem))
                {
                    chart1.Series[0].Points[e.ItemIndex].BorderColor = savedDataPointColor;
                    chart1.Series[0].Points[e.ItemIndex].BorderWidth = 0;
                }
                else
                {
                    chart1.Series[0].Points[e.ItemIndex].MarkerColor = savedDataPointColor;
                }
            }
        }
    }
}
