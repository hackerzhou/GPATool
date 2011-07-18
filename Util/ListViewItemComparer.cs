using System;
using System.Collections;
using System.Windows.Forms;
using GPATool.Bean;

namespace GPATool.Util
{
    class ListViewItemComparer : IComparer
    {
        private int col;
        private ColumnHeader header;
        public ListViewItemComparer(int column, ColumnHeader h)
        {
            col = column;
            header = h;
            String tag = (String)header.Tag;
            if (tag.EndsWith("Desc"))
            {
                header.Tag = tag.Replace("Desc", "");
            }
            else
            {
                header.Tag = tag + "Desc";
            }
        }
        public int Compare(object x, object y)
        {
            String tag = (String)header.Tag;
            String xValue = ((ListViewItem)x).SubItems[col].Text;
            String yValue = ((ListViewItem)y).SubItems[col].Text;
            if (tag.Equals("double"))
            {
                double d1 = double.Parse(xValue);
                double d2 = double.Parse(yValue);
                return d1.CompareTo(d2);
            }
            else if (tag.Equals("doubleDesc"))
            {
                double d1 = double.Parse(xValue);
                double d2 = double.Parse(yValue);
                return d2.CompareTo(d1);
            }
            else if (tag.Equals("StringDesc"))
            {
                return String.Compare(yValue, xValue);
            }
            else if (tag.Equals("String"))
            {
                return String.Compare(xValue, yValue);
            }
            else if (tag.Equals("Score"))
            {
                return Lesson.getScoreValue(xValue).CompareTo(Lesson.getScoreValue(yValue));
            }
            else
            {
                return Lesson.getScoreValue(yValue).CompareTo(Lesson.getScoreValue(xValue));
            }
        }
    }
}
