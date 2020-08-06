namespace WinPos
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Linq;

    /// &lt;summary>
    /// EnumDesktopWindows Demo - shows the caption of all desktop windows.
    /// Authors: Svetlin Nakov, Martin Kulov 
    /// Bulgarian Association of Software Developers - http://www.devbg.org/en/
    /// &lt;/summary>
    interface IWinPos
    {
        const int MAXTITLE = 255;

        const string WindowPositionsFileName = @"C:\sw\WinPos\WindowPositions.txt";
        const string InScopeProgramsFileName = @"C:\sw\WinPos\ProgramsInScope.txt";

        static ArrayList mTitlesList;

        delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
         ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool _EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
         ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern int _GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        static HashSet<string> programNames = GetInScopePrograms();

        static HashSet<string> GetInScopePrograms()
        {
            string[] lines = System.IO.File.ReadAllLines(InScopeProgramsFileName);
            return lines.ToHashSet();
        }

        static bool InScope(string windowCaption)
        {
            bool isInScope = false;
            foreach (string progName in programNames)
            {
                if (windowCaption.Contains(progName))
                {
                    isInScope = true;
                }
            }
            return isInScope;
        }

        static string RecognizedProgram(string windowCaption)
        {
            string programName = "";
            foreach (string progName in programNames)
            {
                if (windowCaption.Contains(progName))
                {
                    programName = progName;
                }
            }
            return programName;
        }

        // From http://csharphelper.com/blog/2016/12/set-another-applications-size-and-position-in-c/
        // Define the FindWindow API function.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        static IntPtr GetWindowHandle(string windowTitle)
        {
            // Get the target window's handle.
            IntPtr target_hwnd = FindWindowByCaption(IntPtr.Zero, windowTitle);
            if (target_hwnd == IntPtr.Zero)
            {
                Console.WriteLine("Could not find a window with the title \"" + windowTitle + "\"");
            }
            return target_hwnd;
        }

        /// <summary>
        /// Returns the caption of a windows by given HWND identifier.
        /// </summary>
        static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(MAXTITLE);
            int titleLength = _GetWindowText(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }

        static bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            string title = GetWindowText(hWnd);
            mTitlesList.Add(title);
            return true;
        }

        /// <summary>
        /// Returns the caption of all desktop windows.
        /// </summary>
        static string[] GetDesktopWindowsCaptions()
        {
            mTitlesList = new ArrayList();
            EnumDelegate enumfunc = new EnumDelegate(EnumWindowsProc);
            IntPtr hDesktop = IntPtr.Zero; // current desktop
            bool success = _EnumDesktopWindows(hDesktop, enumfunc, IntPtr.Zero);

            if (success)
            {
                // Copy the result to string array
                string[] titles = new string[mTitlesList.Count];
                mTitlesList.CopyTo(titles);
                return titles;
            }
            else
            {
                // Get the last Win32 error code
                int errorCode = Marshal.GetLastWin32Error();

                string errorMessage = String.Format(
                "EnumDesktopWindows failed with code {0}.", errorCode);
                throw new Exception(errorMessage);
            }
        }


    }

}
