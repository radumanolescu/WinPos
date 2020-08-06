namespace WinPos
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    class WindowPositions: ISaveWindows, IRestoreWindows
    {
        static void Main(string[] args)
        {
            String usageMessage = "Usage:\nWindowPositions (Save|Restore)";
            if (args.Length == 0)
            {
                System.Console.WriteLine(usageMessage);
                return;
            }
            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "Save":
                        ISaveWindows.SaveWindowSizePos();
                        break;
                    case "Restore":
                        IRestoreWindows.RestoreWindowSizePos();
                        break;
                    default:
                        System.Console.WriteLine(usageMessage);
                        break;
                }
            }
        }

    }
}
