using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Bolt_AIO;
using Leaf.xNet;

namespace BoltCUI.Tools
{
    public class ComboLeecher
    {
        private static readonly TextBox Links = new TextBox();
        public static List<string> keywords = new List<string>();
        public static List<string> sites = new List<string>();
        public static List<string> engines = new List<string>();
        public static int KeywordIndex;
        public static int sitesIndex;
        public static int EnginesIndex;

        public static void ComboLeecherGUI()
        {
            Console.Title =
                "                                                                                                 [>] BoltAIO | Combo Leecher | Quanotics#3931 [<]";
            Console.Clear();
            Console.WriteLine();
            Program.Ascii();
            Console.WriteLine();
            Console.WriteLine();
            Colorful.Console.Write("\n    [", Color.White);
            Colorful.Console.Write("Select ", Color.White);
            Colorful.Console.Write("keywords", Color.Purple);
            Colorful.Console.Write("]", Color.White);
            loadKeywords();
            var num = 0;
            while (num <= 5)
            {
                new Thread(SetGetLinks).Start();
                num = num + 1;
            }
        }

        public static void loadKeywords()
        {
            string fileName = null;
            var t = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Keywords";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                keywords = new List<string>(File.ReadAllLines(fileName));
                Colorful.Console.Write("\n    [", Color.White);
                Colorful.Console.Write("Selected ", Color.White);
                Colorful.Console.Write(keywords.Count.ToString(), Color.Purple);
                Colorful.Console.Write(" Keywords", Color.White);
                Colorful.Console.Write("]", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public static void SetGetLinks()
        {
            sites.Add("api.throwbin.io/v1/paste");
            sites.Add("pastebin.com");
            sites.Add("notes.io");
            sites.Add("bitbin.it");
            sites.Add("pastelink.net");
            sites.Add("justpaste.it");
            sites.Add("controlc.com");
            sites.Add("anonfiles.com");
            sites.Add("pasted.co");
            sites.Add("ivpaste.com");
            sites.Add("paste.org.ru");
            sites.Add("codepad.org");
            sites.Add("dumpz.org");
            sites.Add("everfall.compaste");
            sites.Add("lpaste.net");
            sites.Add("sebsauvage.netpaste");
            sites.Add("heypasteit.com");
            sites.Add("pastebin.centos.org");
            sites.Add("p.hgc.host");
            sites.Add("paste.ofcode.org");
            sites.Add("0bin.net");
            sites.Add("vpaste.net");
            sites.Add("apaste.info");
            sites.Add("slexy.org");
            sites.Add("www.paste4btc.com");
            sites.Add("paste.ee");
            sites.Add("pasteall.org");
            sites.Add("paste.bitlair.nl");
            sites.Add("paste.rohitab.com");
            sites.Add("posu.org");
            sites.Add("kpaste.net");

            engines.Add("https://www.bing.com/search?num=100&q=");
            engines.Add("https://search.yahoo.com/search?q=");
            engines.Add("https://www.yandex.com/search/?text=");
            engines.Add("https://www.google.com/search?num=100&q=");
            engines.Add("https://duckduckgo.com/?num=100&q=");
            engines.Add("https://www.ask.com/web?q=");
            engines.Add("https://www.wow.com/search?q=");
            engines.Add("https://search.aol.com/aol/search?q=");
            engines.Add("https://nova.rambler.ru/search?query=");

            for (;;)
                try
                {
                    if (EnginesIndex == engines.Count) break;
                    var engine0 = engines[EnginesIndex];
                    if (sitesIndex == sites.Count)
                    {
                        Interlocked.Increment(ref EnginesIndex);
                        sitesIndex = 0;
                        Colorful.Console.Write("    INFO ", Color.ForestGreen);
                        Colorful.Console.Write(" | ", Color.LightGreen);
                        Colorful.Console.Write("Now Leeching Search Engine ", Color.White);
                        Colorful.Console.Write(engine0 + "\n", Color.LightGreen);
                    }

                    try
                    {
                        var site0 = sites[sitesIndex];
                        if (KeywordIndex == keywords.Count)
                        {
                            Interlocked.Increment(ref sitesIndex);
                            KeywordIndex = 0;
                            Colorful.Console.Write("    INFO ", Color.ForestGreen);
                            Colorful.Console.Write(" | ", Color.LightGreen);
                            Colorful.Console.Write("Now Leeching Site ", Color.White);
                            Colorful.Console.Write(site0 + "\n", Color.LightGreen);
                        }

                        try
                        {
                            var keyword0 = keywords[KeywordIndex];
                            if (KeywordIndex == keywords.Count)
                            {
                                Interlocked.Increment(ref sitesIndex);
                                Colorful.Console.Write("    INFO", Color.ForestGreen);
                                Colorful.Console.Write(" | ", Color.LightGreen);
                                Colorful.Console.Write("Now Leeching Site ", Color.White);
                                Colorful.Console.Write(site0 + "\n", Color.LightGreen);
                            }

                            GrabLinks(keyword0, site0, engine0);
                        }
                        catch (Exception e)
                        {
                            Colorful.Console.Write("    ERROR", Color.Red);
                            Colorful.Console.Write(" | ", Color.IndianRed);
                            Colorful.Console.Write("I think we chillin' doe.\n", Color.White);
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        Colorful.Console.Write("    ERROR", Color.Red);
                        Colorful.Console.Write(" | ", Color.IndianRed);
                        Colorful.Console.Write("I think we chillin' doe.\n", Color.White);
                        throw;
                    }
                }
                catch (Exception e)
                {
                    Colorful.Console.Write("    ERROR", Color.Red);
                    Colorful.Console.Write(" | ", Color.IndianRed);
                    Colorful.Console.Write("I think we chillin' doe.\n", Color.White);
                }

            Colorful.Console.Write("\n\n    INFO", Color.ForestGreen);
            Colorful.Console.Write(" | ", Color.LightGreen);
            Colorful.Console.Write("Done Scraping", Color.Purple);
            Thread.Sleep(1500);
            Program.Menu0();
        }

        public static void GrabLinks(string keyword, string site, string engine)
        {
            Links.MaxLength = int.MaxValue;
            try
            {
                Interlocked.Increment(ref KeywordIndex);
                using (var req = new HttpRequest())
                {
                    Colorful.Console.Write("    INFO", Color.ForestGreen);
                    Colorful.Console.Write(" | ", Color.LightGreen);
                    Colorful.Console.Write("Finding Links With Keyword ", Color.White);
                    Colorful.Console.Write(keyword + "\n", Color.LightGreen);
                    MatchCollection regex;
                    req.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
                    var response = req.Get(string.Concat(new[]
                    {
                        engine,
                        keyword,
                        "+site:",
                        site
                    })).ToString();
                    regex = new Regex("(https:\\/\\/" + site + "\\/\\w+)").Matches(response);
                    if (regex.Count != 0)
                    {
                        var arr = (from m in regex.OfType<Match>()
                            select m.Value).ToArray();
                        getResults(arr, req);
                    }
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.Write("    ERROR", Color.Red);
                Colorful.Console.Write(" | ", Color.IndianRed);
                Colorful.Console.Write("I think we chillin' doe.\n", Color.White);
            }
        }

        public static void getResults(string[] links, HttpRequest req)
        {
            try
            {
                if (req == null)
                    using (var req1 = new HttpRequest())
                    {
                        Links.Text = string.Join(Environment.NewLine, links.Distinct());
                        req1.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
                        foreach (var line in Links.Lines)
                        {
                            Colorful.Console.Write("    LEECHING", Color.RoyalBlue);
                            Colorful.Console.Write(" | ", Color.LightBlue);
                            Colorful.Console.Write("Scraping Lines from ", Color.White);
                            Colorful.Console.Write(line + "\n", Color.LightBlue);

                            var response = req.Get(line).ToString().Replace("|", ":");
                            if (!response.Contains(":")) response = response.Replace(" ", ":");

                            if (line.Contains("anonfiles.com"))
                            {
                                var DLink = Parse(req1.Get(line).ToString(), " href=\"https://",
                                    "\">                    <img");
                                var res = "https://" + DLink;
                                ReadResult(req1.Get(res).ToString());
                            }
                            else
                            {
                                ReadResult(req1.Get(line).ToString());
                            }
                        }

                        goto finish;
                    }

                Links.Text = string.Join(Environment.NewLine, links.Distinct());
                foreach (var line2 in Links.Lines)
                {
                    var _req = new HttpRequest();
                    if (line2.Contains("anonfiles.com"))
                    {
                        Colorful.Console.Write("    LEECHING", Color.RoyalBlue);
                        Colorful.Console.Write(" | ", Color.LightBlue);
                        Colorful.Console.Write("Scraping Lines from ", Color.White);
                        Colorful.Console.Write(line2 + "\n", Color.LightBlue);

                        var DLink = Parse(_req.Get(line2).ToString(), " href=\"https://",
                            "\">                    <img");
                        var res = "https://" + DLink;
                        ReadResult(_req.Get(res).ToString());
                    }
                    else
                    {
                        Colorful.Console.Write("    LEECHING", Color.RoyalBlue);
                        Colorful.Console.Write(" | ", Color.LightBlue);
                        Colorful.Console.Write("Scraping Lines from ", Color.White);
                        Colorful.Console.Write(line2 + "\n", Color.LightBlue);
                        ReadResult(req.Get(line2).ToString());
                    }
                }

                finish: ;
            }
            catch (Exception e)
            {
                Colorful.Console.Write("    ERROR", Color.Red);
                Colorful.Console.Write(" | ", Color.IndianRed);
                Colorful.Console.Write("I think we chillin' doe.\n", Color.White);
                throw;
            }
        }

        public static void ReadResult(string response)
        {
            try
            {
                var enumerator =
                    new Regex("([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}):([a-zA-Z0-9_\\-\\.]+)")
                        .Matches(response).GetEnumerator();
                {
                    while (enumerator.MoveNext())
                    {
                        var obj = enumerator.Current;
                        var i = (Match) obj;
                        Export.AsResult("/Scraped_combos", i.Value);
                        var lines = File.ReadAllLines(@"Results\" + $@"{Export.now}\" + "/Scraped_combos" + ".txt");
                        File.WriteAllLines(@"Results\" + $@"{Export.now}\" + "/Scraped_combos" + ".txt",
                            lines.Distinct().ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Colorful.Console.Write("    ERROR", Color.Red);
                Colorful.Console.Write(" | ", Color.IndianRed);
                Colorful.Console.Write("I think we chillin' doe.\n", Color.White);
                throw;
            }
        }

        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] {left}, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }
    }
}