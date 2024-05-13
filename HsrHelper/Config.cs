using System.Runtime.InteropServices;
using System.Text;

namespace HsrHelper
{
    internal static class Config
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string name, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private const string path = "./config.ini";

        public static void WriteINI(string name, string key, string value)
        {
            WritePrivateProfileString(name, key, value, path);
        }

        public static string ReadINI(string name, string key)
        {
            StringBuilder sb = new StringBuilder(255);
            int ini = GetPrivateProfileString(name, key, "", sb, 255, path);
            return sb.ToString();
        }

    }
}