using System.Text;
using System.Windows.Forms;

namespace GPATool.Util
{
    class ListViewStripMenuHelper
    {
        public static void CopyAllItemsToClipBoard(ListView listView, int colStart = 0)
        {
            StringBuilder buf = new StringBuilder();
            foreach (ListViewItem i in listView.Items)
            {
                for (int j = colStart; j < i.SubItems.Count; j++)
                {
                    buf.Append(listView.Columns[j].Text + "：" + i.SubItems[j].Text + ";");
                }
                buf.AppendLine();
            }
            Clipboard.SetDataObject(buf.ToString());
        }

        public static void CopySelectedItemsToClipBoard(ListView listView, int colStart = 0)
        {
            StringBuilder buf = new StringBuilder();
            foreach (ListViewItem i in listView.SelectedItems)
            {
                for (int j = 0; j < i.SubItems.Count; j++)
                {
                    buf.Append(listView.Columns[j].Text + "：" + i.SubItems[j].Text + ";");
                }
                buf.AppendLine();
            }
            Clipboard.SetDataObject(buf.ToString());
        }
        public static void CopySelectedItemColumnToClipBoard(ListView listView, int columnIndex)
        {
            Clipboard.SetDataObject(listView.SelectedItems[0].SubItems[columnIndex].Text);
        }
    }
}
