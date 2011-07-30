using System;
using System.IO;
using System.Net;
using System.Text;

namespace GPATool.Util
{
    public class HTTPUtil
    {
        public static HttpWebRequest GetHttpRequest(String url, String method, CookieContainer cc, int timeout, int readTimeout, WebProxy proxy, bool keepalive, bool allowAutoRedirect = false)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(Uri.EscapeUriString(url));
            if (proxy != null)
            {
                req.Proxy = proxy;
                if (proxy.Credentials != null)
                {
                    req.UseDefaultCredentials = true;
                }
            }
            else
            {
                req.Proxy = null;
            }
            req.AllowAutoRedirect = allowAutoRedirect;
            req.Timeout = timeout;
            req.ReadWriteTimeout = readTimeout;
            req.KeepAlive = keepalive;
            req.CookieContainer = cc;
            return req;
        }

        public static String GetStringFromStream(Stream s,Encoding enc)
        {
            StreamReader streamReader = new StreamReader(s, enc);
            string content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }

        public static void WriteStreamToFile(Stream s, String path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            byte[] buf = new byte[10240];
            int readCount = 0;
            while ((readCount = s.Read(buf, 0, buf.Length)) > 0)
            {
                fs.Write(buf, 0, readCount);
            }
            fs.Flush();
            fs.Close();
        }

        public static bool SaveUrlContentToFile(String url, String path)
        {
            try
            {
                HttpWebRequest req = GetHttpRequest(url, "GET", null, 60000, 60000, XMLConfig.GetProxy(), true);
                using (WebResponse wr = req.GetResponse())
                {
                    WriteStreamToFile(wr.GetResponseStream(), path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static String GetFileNameFromUrl(String url)
        {
            return url.LastIndexOf('/') > 0 && url.LastIndexOf('/') + 1 < url.Length ? url.Substring(url.LastIndexOf('/') + 1) : url;
        }
    }
}
