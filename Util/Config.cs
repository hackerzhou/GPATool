using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;

namespace GPATool.Util
{
    class Config
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(
           string lpAppName,
           string lpKeyName,
           string lpDefault,
           StringBuilder lpReturnedString,
           uint nSize,
           string lpFileName);

        private const String configFile = "Config.ini";
        public static void CheckConfigFile()
        {
            if (!File.Exists(configFile))
            {
                StreamWriter writer = new StreamWriter(configFile, false, Encoding.GetEncoding("GBK"));
                writer.WriteLine("[Login]");
                writer.WriteLine("Username=");
                writer.WriteLine("Password=");
                writer.WriteLine("IsSavePassword=true");
                writer.WriteLine("[AutoRefresh]");
                writer.WriteLine("UseAutoRefresh=false");
                writer.WriteLine("AutoRefreshInterval=30");
                writer.WriteLine("[Proxy]");
                writer.WriteLine("UseProxy=");
                writer.WriteLine("Host=");
                writer.WriteLine("Port=");
                writer.WriteLine("UseAuth=");
                writer.WriteLine("Username=");
                writer.WriteLine("Password=");
                writer.WriteLine("[Admin]");
                writer.WriteLine("HardwareKey=");
                writer.WriteLine("HardwareId="+Admin.AdminAuth.getHardwareId());
                writer.Flush();
                writer.Close();
            }
        }
        public static String getUsername()
        {
            return ReadConfig("Login", "Username", "");
        }
        public static String getPassword()
        {
            return RC2Util.Decrypt("hackerzhou", ReadConfig("Login", "Password", ""));
        }
        public static String getHardwareKey()
        {
            return ReadConfig("Admin", "HardwareKey", "");
        }
        public static bool getIsSavePassword()
        {
            bool result = false;
            bool.TryParse(ReadConfig("Login", "IsSavePassword", "true"), out result);
            return result;
        }
        public static WebProxy getConfigedProxy()
        {
            if (!getUseProxy())
            {
                return null;
            }
            WebProxy proxy = new WebProxy(getProxyHost(), getProxyPort());
            if (getUseAuth())
            {
                proxy.Credentials = new NetworkCredential(ReadConfig("Proxy", "Username", ""), ReadConfig("Proxy", "Password", ""));
            }
            return proxy;
        }
        public static bool getUseProxy()
        {
            bool result = false;
            bool.TryParse(ReadConfig("Proxy", "UseProxy", "false"), out result);
            return result;
        }
        public static bool getUseAutoRefresh()
        {
            bool result = false;
            bool.TryParse(ReadConfig("AutoRefresh", "UseAutoRefresh", "false"), out result);
            return result;
        }
        public static int getAutoRefreshInterval()
        {
            int result = 30;
            int.TryParse(ReadConfig("AutoRefresh", "AutoRefreshInterval", "30"), out result);
            return result;
        }
        public static String getProxyHost()
        {
            return ReadConfig("Proxy", "Host", "");
        }
        public static bool getUseAuth()
        {
            bool result = false;
            bool.TryParse(ReadConfig("Proxy", "UseAuth", "false"), out result);
            return result;
        }
        public static String getProxyUsername()
        {
            return ReadConfig("Proxy", "Username", "");
        }
        public static String getProxyPassword()
        {
            return ReadConfig("Proxy", "Password", "");
        }
        public static int getProxyPort()
        {
            int port = 8080;
            try
            {
                port=int.Parse(ReadConfig("Proxy", "Port", "808"));
                if (port <= 0 || port >= 65535)
                {
                    port = 8080;
                }
            }
            catch
            {
            }
            return port;
        }
        public static void updateStyleIndex(int styleIndex)
        {
            WriteConfig("Style", "Index", styleIndex.ToString());
        }
        public static int getStyleIndex()
        {
            int index = 4;
            if (!int.TryParse(ReadConfig("Style", "Index", "4"), out index))
            {
                index = 4;
            }
            return index;
        }
        public static void updateLogin(String username,String password,bool isSavePassword)
        {
            WriteConfig("Login", "Username", username);
            WriteConfig("Login", "Password", RC2Util.Encrypt("hackerzhou",password));
            WriteConfig("Login", "IsSavePassword", isSavePassword.ToString());
        }
        public static void updateProxy(bool useProxy, String host, int port, bool useAuth, String username, String password)
        {
            WriteConfig("Proxy", "UseProxy", useProxy.ToString());
            WriteConfig("Proxy", "Host", host);
            WriteConfig("Proxy", "Port", port.ToString());
            WriteConfig("Proxy", "UseAuth", useAuth.ToString());
            WriteConfig("Proxy", "Username", username);
            WriteConfig("Proxy", "Password", password);
        }
        public static void updateAutoRefresh(bool useAutoRefresh, int autoRefreshInterval)
        {
            WriteConfig("AutoRefresh", "UseAutoRefresh", useAutoRefresh.ToString());
            WriteConfig("AutoRefresh", "AutoRefreshInterval", autoRefreshInterval.ToString());
        }

        private static string ReadConfig(string Section, string Key, string NoText)
        {
            CheckConfigFile();
            if (File.Exists(configFile))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, new FileInfo(configFile).FullName);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        private static bool WriteConfig(string Section, string Key, string Value)
        {
            CheckConfigFile();
            if (File.Exists(configFile))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, new FileInfo(configFile).FullName);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
