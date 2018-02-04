using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Joiner
{
    class Helper
    {
        static Random r = new Random();
        public static string RandomString(int Length)
        {
            string chars = "qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
            
            string ret = chars[r.Next(10, chars.Length)].ToString();
            for (int i = 1; i < Length; i++)
                ret += chars[r.Next(chars.Length)];
            return ret;
        }
        public static void Join()
        {
            try
            {
                var obf = new Process();
                obf.StartInfo.CreateNoWindow = true;
                obf.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                obf.StartInfo.FileName = Application.StartupPath + "\\NSIS\\makensis.exe";
                obf.StartInfo.UseShellExecute = false;
                obf.StartInfo.RedirectStandardOutput = true;
                obf.StartInfo.Arguments = "source.txt";
                obf.Start();
                obf.BeginOutputReadLine();
                obf.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
