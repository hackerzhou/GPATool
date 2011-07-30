using System;
using System.Net;
using System.Reflection;
using System.Text;

namespace GPATool.Util
{
    public class ScoreHTMLUtil
    {
        public static String TryGetHTMLString(String username, String password, String code
            , WebProxy proxy, bool isNormal)
        {
            String resultStr = null;
            int maxTry = 3;
            while (resultStr == null && maxTry-- > 0)
            {
                try
                {
                    if (isNormal)
                    {
                        resultStr = GetHTMLString(username, password, code, proxy);
                    }
                    else
                    {
                        Assembly asm = Assembly.Load("GPAToolPro");
                        Type util = asm.GetType("GPAToolPro.AdminFunctionLib");
                        Object result = util.InvokeMember("GetAdminScoreHTMLString", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod
                            , null, null, new object[] { username, password, code, proxy });
                        resultStr = (result == null && result is String) ? null : (String)result;
                    }
                }
                catch
                {
                    resultStr = null;
                }
            }
            return resultStr;
        }

        private static String GetHTMLString(String username, String password, String code, WebProxy proxy)
        {
            String result = "";
            CookieContainer Cc = new CookieContainer();
            HttpWebRequest req = HTTPUtil.GetHttpRequest("http://uis2.fudan.edu.cn:82/amserver/UI/Login?Login.Token2=" + password + "&Login.code=" + code + "&Login.Token1=" + username, "GET", Cc, 15000, 15000, proxy, true);
            using (WebResponse wr = req.GetResponse())
            {
            }
            req = HTTPUtil.GetHttpRequest("http://www.urp.fudan.edu.cn:84/epstar/app/fudan/ScoreManger/ScoreViewer/Student/Course.jsp", "GET", Cc, 15000, 15000, proxy, true);
            using (WebResponse wr = req.GetResponse())
            {
                result = HTTPUtil.GetStringFromStream(wr.GetResponseStream(),Encoding.GetEncoding("GB2312"));
            }
            req = HTTPUtil.GetHttpRequest("http://www.urp.fudan.edu.cn/logout.jsp", "GET", Cc, 15000, 15000, proxy, true);
            using (WebResponse wr = req.GetResponse())
            {
            }
            return result;
        }
    }
}
