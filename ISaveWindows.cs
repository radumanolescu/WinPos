namespace WinPos
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class WindowInfo
    {
        public WindowInfo(string name, int x, int y, int w, int h)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public string name;
        public int x;
        public int y;
        public int w;
        public int h;
    }

    interface ISaveWindows : IWinPos
    {
        // From https://www.pinvoke.net/default.aspx/user32.getwindowplacement
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        const UInt32 SW_HIDE = 0;
        const UInt32 SW_SHOWNORMAL = 1;
        const UInt32 SW_NORMAL = 1;
        const UInt32 SW_SHOWMINIMIZED = 2;
        const UInt32 SW_SHOWMAXIMIZED = 3;
        const UInt32 SW_MAXIMIZE = 3;
        const UInt32 SW_SHOWNOACTIVATE = 4;
        const UInt32 SW_SHOW = 5;
        const UInt32 SW_MINIMIZE = 6;
        const UInt32 SW_SHOWMINNOACTIVE = 7;
        const UInt32 SW_SHOWNA = 8;
        const UInt32 SW_RESTORE = 9;

        static WINDOWPLACEMENT GetPlacement(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement;
        }

        static WINDOWPLACEMENT GetWindowPlacement(string windowTitle)
        {
            IntPtr target_hwnd = GetWindowHandle(windowTitle);
            return GetPlacement(target_hwnd);
        }

        static void SaveWindowSizePos()
        {
            string[] desktopWindowsCaptions = GetDesktopWindowsCaptions();
            // System.IO.File.WriteAllLines(@"C:\tmp\WindowCaptions.txt", desktopWindowsCaptions);
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(WindowPositionsFileName))
            {
                foreach (string windowCaption in desktopWindowsCaptions)
                {
                    if (InScope(windowCaption))
                    {
                        // positionAndSize(line);
                        ISaveWindows.WINDOWPLACEMENT placement = GetWindowPlacement(windowCaption);
                        int x = placement.rcNormalPosition.X;
                        int y = placement.rcNormalPosition.Y;
                        int w = placement.rcNormalPosition.Width;
                        int h = placement.rcNormalPosition.Height;
                        String s = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", windowCaption, x, y, w-x, h-y);
                        file.WriteLine(s);
                        //Console.WriteLine(windowCaption + ": " + placement.rcNormalPosition); //
                    }
                    else
                    {
                        Console.WriteLine(windowCaption);
                    }
                }
            }

        }

    }
}
