using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;

namespace GPATool.Util
{
    class UpdateUtil
    {
        private const double currentVersion = 1.1;
        public static void CheckUpdate()
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
    }
}
