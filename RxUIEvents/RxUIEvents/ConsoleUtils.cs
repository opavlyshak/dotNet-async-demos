using System.Runtime.InteropServices;

namespace RxUIEvents
{
    public class ConsoleUtils
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", CharSet = CharSet.Unicode)]
        public static extern bool AllocConsole();
    }
}