using QLTK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTK.Functions
{
    internal class Utils
    {
        public static DialogResult notification(string text, MessageBoxIcon icon = MessageBoxIcon.Asterisk)
            => MessageBox.Show(text, "Thông báo!", MessageBoxButtons.OK, icon);
        public static DialogResult Question(string text, MessageBoxButtons btn = MessageBoxButtons.YesNo)
            => MessageBox.Show(text, "Thông báo!", btn, MessageBoxIcon.Question);

        public static bool ExistedWindow(Account account, out IntPtr hWnd)
        {
            hWnd = IntPtr.Zero;
            if (account.process == null || account.process.HasExited)
                return false;

            hWnd = account.process.MainWindowHandle;
            return hWnd != IntPtr.Zero;
        }

        #region winAPI
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);
        [DllImport("gdi32.dll")]
        public static extern int AddFontResource(string lpszFilename);
        [DllImport("gdi32.dll")]
        public static extern int CreateScalableFontResource(uint fdwHidden, string
        lpszFontRes, string lpszFontFile, string lpszCurrentPath);
        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        #endregion
    }
}
