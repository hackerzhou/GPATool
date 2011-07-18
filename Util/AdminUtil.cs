using System;
using System.Management;
using System.Reflection;

namespace GPATool.Util
{
    public class AdminUtil
    {
        private static bool hasChecked = false;
        private static bool isAdmin = false;
        
        public static bool IsAdmin(String hardwareKey)
        {
            if (!hasChecked)
            {
                isAdmin = CheckAdmin(hardwareKey);
                hasChecked = true;
            }
            return isAdmin;
        }

        private static bool CheckAdmin(String hardwareKey)
        {
            bool isAdmin = false;
            Type checker = null;
            try
            {
                Assembly asm = Assembly.Load("GPAToolPro");
                checker = asm.GetType("GPAToolPro.AdminAuthUtil");
                if (checker != null)
                {
                    object result = null;
                    result = checker.InvokeMember("IsAdmin", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { hardwareKey });
                    if (result != null && result is bool)
                    {
                        isAdmin = (bool)result;
                    }
                }
            }
            catch
            {
            }
            return isAdmin;
        }

        public static String GetHardwareId()
        {
            string result = string.Empty;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_PhysicalMedia");
            foreach (ManagementObject mo in mos.Get())
            {
                result = mo["SerialNumber"].ToString().Trim();
                break;
            }
            return result;
        }
    }
}
