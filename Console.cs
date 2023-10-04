using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cobbs_Engine
{
    internal static partial class Program
    {
        internal enum ConsoleState : byte
        {
            Open = 5,
            Closed = 0,
        }

        internal static class Console
        {
            [DllImport("kernel32.dll")]
            internal static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern int SetWindowText(IntPtr hWnd, string text);

            internal static readonly IntPtr Window = GetConsoleWindow();

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
            internal static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        }
    }
}
