using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Colorful;

namespace Bolt_AIO
{
    public class Variables
    {
        public static string Version = "1.1";

        public static string Permission = "y";

        public static string Type = "";

        public static string Folder = "";

        public static int Threads = 250;

        public static int TimeOut = 5000;

        public static int Alive = 0;

        public static int Dead = 0;

        public static List<string> Proxies = new List<string>();

        public static Queue<string> ProxiesQueue = new Queue<string>();

        private static readonly object locker = new object();

        private static readonly object Locked = new object();

        public string FilePath { get; set; }

        public void AppendToFile(string textToAppend)
        {
            var obj = locker;
            lock (obj)
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (var streamWriter = new StreamWriter(fileStream, Encoding.Unicode))
                    {
                        streamWriter.WriteLine(textToAppend);
                    }
                }
            }
        }

        public static void PrintWithPrefix(string prefix, string message, string prefixColour)
        {
            var locked = Locked;
            lock (locked)
            {
                Console.Write("    [", Color.White);
                Console.Write(prefix, Color.Purple);
                Console.WriteLine("] " + message, Color.White);
            }
        }

        public static void PrintWithoutPrefix(string message)
        {
            var locked = Locked;
            lock (locked)
            {
                Console.WriteLine(message, Color.White);
            }
        }
    }
}