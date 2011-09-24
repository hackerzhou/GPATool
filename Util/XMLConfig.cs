using System;
using System.Net;
using System.Xml;

namespace GPATool.Util
{
    class XMLConfig
    {
        public static String urpUsername;
        public static String urpPassword;
        public static bool isSavePassword = true;
        public static bool useProxy;
        public static String proxyHost;
        public static int proxyPort = 8080;
        public static bool proxyUseAuth;
        public static String proxyUsername;
        public static String proxyPassword;
        public static String adminHardwareKey;
        public static String adminHardwareId;
        public static int styleIndex = 4;
        public static bool useAutoRefresh;
        public static int autoRefreshInterval = 30;
        public static bool isAdmin;
        public static String adminUsername;
        public static String adminPassword;
        public static bool dirty = true;

        public static void WriteConfig()
        {
            try
            {
                XmlDocument doc = GetDocument();
                doc.Save("Config.xml");
            }
            catch
            {
            }
        }

        public static void LoadConfig()
        {
            if (dirty)
            {
                try
                {
                    adminHardwareId = AdminUtil.GetHardwareId();
                    XmlDocument doc = new XmlDocument();
                    doc.Load("Config.xml");
                    urpUsername = doc.SelectSingleNode("//Config/ScoreQuery/@urpUsername").InnerText;
                    urpPassword = RC2Util.Decrypt("hackerzhou", doc.SelectSingleNode("//Config/ScoreQuery/@urpPassword").InnerText);
                    isSavePassword = bool.Parse(doc.SelectSingleNode("//Config/ScoreQuery/@isSavePassword").InnerText);
                    useAutoRefresh = bool.Parse(doc.SelectSingleNode("//Config/ScoreQuery/@useAutoRefresh").InnerText);
                    useProxy = bool.Parse(doc.SelectSingleNode("//Config/Proxy/@useProxy").InnerText);
                    proxyHost = doc.SelectSingleNode("//Config/Proxy/@proxyHost").InnerText;
                    proxyPort = int.Parse(doc.SelectSingleNode("//Config/Proxy/@proxyPort").InnerText);
                    proxyUseAuth = bool.Parse(doc.SelectSingleNode("//Config/Proxy/@proxyUseAuth").InnerText);
                    proxyUsername = doc.SelectSingleNode("//Config/Proxy/@proxyUsername").InnerText;
                    proxyPassword = doc.SelectSingleNode("//Config/Proxy/@proxyPassword").InnerText;
                    adminHardwareKey = doc.SelectSingleNode("//Config/Admin/@adminHardwareKey").InnerText;
                    adminUsername = doc.SelectSingleNode("//Config/Admin/@adminUsername").InnerText;
                    adminPassword = doc.SelectSingleNode("//Config/Admin/@adminPassword").InnerText;
                    styleIndex = int.Parse(doc.SelectSingleNode("//Config/Style/@styleIndex").InnerText);
                    isAdmin = AdminUtil.IsAdmin(adminHardwareKey);
                }
                catch
                {
                    WriteConfig();
                }
                dirty = false;
            }
        }

        public static XmlDocument GetDocument()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Config");
            doc.AppendChild(root);
            XmlElement scoreQuery = doc.CreateElement("ScoreQuery");
            scoreQuery.SetAttribute("urpUsername", urpUsername);
            scoreQuery.SetAttribute("urpPassword", urpPassword);
            scoreQuery.SetAttribute("isSavePassword", isSavePassword.ToString());
            scoreQuery.SetAttribute("useAutoRefresh", useAutoRefresh.ToString());
            scoreQuery.SetAttribute("autoRefreshInterval", autoRefreshInterval.ToString());
            root.AppendChild(scoreQuery);
            XmlElement proxy = doc.CreateElement("Proxy");
            proxy.SetAttribute("useProxy", useProxy.ToString());
            proxy.SetAttribute("proxyHost", proxyHost);
            proxy.SetAttribute("proxyPort", proxyPort.ToString());
            proxy.SetAttribute("proxyUseAuth", proxyUseAuth.ToString());
            proxy.SetAttribute("proxyUsername", proxyUsername);
            proxy.SetAttribute("proxyPassword", proxyPassword);
            root.AppendChild(proxy);
            XmlElement admin = doc.CreateElement("Admin");
            admin.SetAttribute("adminHardwareKey", adminHardwareKey);
            admin.SetAttribute("adminHardwareId", adminHardwareId);
            admin.SetAttribute("adminUsername", adminUsername);
            admin.SetAttribute("adminPassword", adminPassword);
            root.AppendChild(admin);
            XmlElement style = doc.CreateElement("Style");
            style.SetAttribute("styleIndex", styleIndex.ToString());
            root.AppendChild(style);
            return doc;
        }

        public static WebProxy GetProxy()
        {
            WebProxy proxy = XMLConfig.useProxy ? new WebProxy(XMLConfig.proxyHost, XMLConfig.proxyPort) : null;
            if (proxyUseAuth)
            {
                proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
            }
            return proxy;
        }
    }
}
