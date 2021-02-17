using System;
using System.IO;
using System.Threading;

namespace Bolt_AIO
{
    internal class Export
    {
        private static readonly object resultLock = new object();
        public static string now = DateTime.Now.ToString("dd-MM-yy HH-mm-ss");

        public static void Initialize()
        {
            Directory.CreateDirectory($@"Results\{now}");
        }

        public static void InitializeBlacklist()
        {
            if (!File.Exists("blacklist.txt")) File.Create(@"blacklist.txt");
        }

        public static void AsResult(string fileName, string content)
        {
            var resultLock = Export.resultLock;
            var flag = false;
            try
            {
                Monitor.Enter(resultLock, ref flag);
                File.AppendAllText(@"Results\" + $@"{now}\" + fileName + ".txt", content + Environment.NewLine);
            }
            finally
            {
                if (flag)
                    Monitor.Exit(resultLock);
            }
        }
    }
}