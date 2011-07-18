using System;
using System.IO;
using System.Net;
using System.Text;

namespace GPATool.Util
{
    public class HTTPUtil
    {
        public static HttpWebRequest getHttpRequest(String url, String method, CookieContainer cc, int timeout, int readTimeout, WebProxy proxy, bool keepalive, bool allowAutoRedirect = false)
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
        public static String getStringFromStream(Stream s,Encoding enc)
        {
            StreamReader streamReader = new StreamReader(s, enc);
            string content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }
    }
}
