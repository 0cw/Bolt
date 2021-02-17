using System;
using System.Drawing;
using System.Threading;
using Console = Colorful.Console;

namespace BoltCUI.Tools
{
    public class CheckerTools
    {
        public static void UpdateConsole()
        {
            var lastChecks = Program.TotalChecks;
            for (;;)
            {
                Program.Cpm = Program.TotalChecks - lastChecks;
                lastChecks = Program.TotalChecks;
                Console.Clear();
                Console.WriteLine("");
                Program.Ascii();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("    [", Color.White);
                Console.Write("HITS", Color.LimeGreen);
                Console.Write($"] {Program.Hits}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("FAILS", Color.Red);
                Console.Write($"] {Program.Fails}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("FREES", Color.OrangeRed);
                Console.Write($"] {Program.Frees}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("OTHERS", Color.DarkSalmon);
                Console.Write($"] {Program.Others}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("ERRORS", Color.DarkOrange);
                Console.Write($"] {Program.Errors}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("TOTAL CHECKS", Color.Aquamarine);
                Console.Write($"] {Program.TotalChecks}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("CPM", Color.RoyalBlue);
                Console.Write($"] {Program.Cpm * 60}\n\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("INFO", Color.Green);
                Console.Write("] Press \"S\" to go to the menu... NOTE: This does not stop the checker \n",
                    Color.White);

                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).Key == ConsoleKey.S)
                    {
                        Program.Menu0();
                        break;
                    }

                Thread.Sleep(1000);
            }
        }

        public static void UpdateTitle()
        {
            var lastChecks = Program.TotalChecks;
            for (;;)
            {
                Program.Cpm = Program.TotalChecks - lastChecks;
                lastChecks = Program.TotalChecks;

                Console.Title = string.Format(
                    "Bolt AIO | Checked: {0}/{1} | Hits: {2} | Frees: {3} | Others: {4}  | Bad: {5} | Errors: {6} | CPM: {7}",
                    Program.TotalChecks, Program.Combostotal, Program.Hits, Program.Frees, Program.Others,
                    Program.Fails, Program.Errors,
                    Program.Cpm * 60);
                Thread.Sleep(1000);

                if (Program.TotalChecks >= Program.Combostotal)
                {
                    Console.Title = "Bolt AIO | Hits: " + Program.Hits + " | Finished Checking...";
                    Console.Write("    [", Color.White);
                    Console.Write("Finished Checking!", Color.Purple);
                    Console.Write("]", Color.White);
                    Thread.Sleep(1000);
                    Program.Menu0();
                    break;
                }
            }
        }
    }
}