
namespace WinPos
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    interface IRestoreWindows: IWinPos
    {
        // Define the SetWindowPos API function.
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd,
            IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        // Define the SetWindowPosFlags enumeration.
        [Flags()]
        enum SetWindowPosFlags : uint
        {
            SynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        static string[] GetSavedSizes()
        {
            string[] lines = System.IO.File.ReadAllLines(WindowPositionsFileName);
            return lines;
        }

        /// <summary>
        /// Parse the saved window information and populate a dictionary
        /// </summary>
        /// <param name="lines">Lines of text describing in-scope application size and position</param>
        /// <returns>dictionary of application positions and sizes, by name</returns>
        static Dictionary<string, WindowInfo> GetWindowInfo(string[] lines)
        {
            var winMap = new Dictionary<string, WindowInfo>();
            foreach(string line in lines)
            {
                string[] words = line.Split('\t');
                string programName = RecognizedProgram(words[0]);
                if (!programName.Equals(""))
                {
                    int x = int.Parse(words[1]);
                    int y = int.Parse(words[2]);
                    int w = int.Parse(words[3]);
                    int h = int.Parse(words[4]);
                    winMap[programName] = new WindowInfo(programName, x, y, w, h);
                    string s = String.Format("{0}:x={1},y={2},w={3},h={4}", programName,x,y,w,h);
                    Console.WriteLine(s);
                }
            }
            return winMap;
        }

        // Size and position an application.
        static void SetPositionAndSize(string windowTitle, Dictionary<string, WindowInfo> winInfo)
        {
            IntPtr target_hwnd = GetWindowHandle(windowTitle);
            if (target_hwnd == IntPtr.Zero)
            {
                Console.WriteLine(String.Format("CannotFindHandle '{0}'", windowTitle));
                return;
            }
            string programName = RecognizedProgram(windowTitle);
            if (programName == "")
            {
                Console.WriteLine(String.Format("CannotFindProgram '{0}'", windowTitle));
                return;
            }
            if (!winInfo.ContainsKey(programName))
            {
                Console.WriteLine(String.Format("CannotFindInfo '{0}'", programName));
                return;
            }
            WindowInfo wi = winInfo[programName];

            // Set the window's size and position.
            SetWindowPos(target_hwnd, IntPtr.Zero, wi.x, wi.y, wi.w, wi.h, 0);
        }

        static void RestoreWindowSizePos()
        {
            string[] savedSizes = GetSavedSizes();
            Dictionary<string, WindowInfo> windowInfo = GetWindowInfo(savedSizes);
            string[] desktopWindowsCaptions = GetDesktopWindowsCaptions();
            foreach (string windowCaption in desktopWindowsCaptions)
            {
                SetPositionAndSize(windowCaption, windowInfo);
            }
        }

    }
}
