using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bolt_AIO;
using BoltCUI.Tools;
using Console = Colorful.Console;

namespace BoltCUI
{
    internal class Program
    {
        private static readonly List<Action> PickedModules = new List<Action>();
        public static int Fails = 0;
        public static int Hits = 0;
        public static int Frees = 0;
        public static int Errors = 0;
        public static int TotalChecks = 0;
        public static int Others = 0;
        public static int Cpm;
        public static string ProxyType1 = "";
        public static string lorc = "";
        private static readonly OpenFileDialog Ofd = new OpenFileDialog();
        public static List<string> Proxies = new List<string>();
        public static List<string> Combos = new List<string>();
        public static int Proxytotal;
        public static int Combostotal;
        public static int Threads;
        public static int Stop = 0;
        public static int Proxyindex = 0;
        public static int Combosindex = 0;
        public static bool CheckerRunning;

        public static string RequestUriString { get; set; }

        public static void addModule(Action actions)
        {
            PickedModules.Add(actions);
            prefix("Added module successfully", "\n");
            Thread.Sleep(5);
        }

        public static void prefix(string prefix, string description)
        {
            Console.Write("    [", Color.White);
            Console.Write(prefix, Color.Purple);
            Console.Write("] " + description, Color.White);
        }

        private static void Main(string[] args)
        {
            CheckerRunning = false;
            
            Menu0();
        }

        public static void Menu0()
        {
            DiscordRPC1.Initialize();
            Export.Initialize();
            menu1:
            System.Console.Clear();
            Console.Title =
                "                                                                                                       [>] BoltAIO | CUI Version 1.0 | Quanotics#3931 [<]";
            Console.WriteLine("");
            Ascii();
            Console.WriteLine("");
            System.Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("");
            prefix("1", "Checker\n");
            prefix("2", "Tools\n");
            prefix("3", "Settings\n");
            prefix("4", "Information\n");
            Console.WriteLine("");
            Console.WriteLine("");
            prefix(">", "");
            var userinput = Console.ReadLine();
            switch (userinput)
            {
                case "1":
                {
                    Checker();
                    break;
                }
                case "2":
                {
                    Tools0();
                    break;
                }
                case "3":
                {
                    SettingsTools.Settings0();
                    break;
                }
                case "4":
                {
                    SettingsTools.Information0();
                    break;
                }
                default:
                    prefix("Invalid Option", "");
                    Thread.Sleep(300);
                    goto menu1;
                    break;
            }
        }

        public static void Tools0()
        {
            tools1:
            DiscordRPC1.Initialize();
            Export.Initialize();
            System.Console.Clear();
            Console.Title =
                "                                                                                                       [>] BoltAIO | Tools | Quanotics#3931 [<]";
            Console.WriteLine("");
            Ascii();
            Console.WriteLine("");
            System.Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("");
            prefix("1", "Proxy Scraper & Checker\n");
            prefix("2", "Combo Editor\n");
            prefix("3", "Combo Leecher\n");
            prefix("4", "Parser\n");
            //prefix("5", "Vuln Scanner\n");
            //prefix("6", "Token Checker\n");
            prefix("5", "Nitro Sniper\n");
            prefix("X", "Go Back\n");
            Console.WriteLine("");
            Console.WriteLine("");
            prefix(">", "");
            var userinput = Console.ReadLine().ToUpper();
            switch (userinput)
            {
                case "1":
                {
                    //ProxyTools.Check1();
                    break;
                }
                case "2":
                {
                    comboeditor0();
                    break;
                }
                case "3":
                {
                    ComboLeecher.ComboLeecherGUI();
                    break;
                }
                case "4":
                {
                    Parser.Parser0();
                    break;
                }
                case "5":
                {
                    Sniper.Sniper0();
                    break;
                }
                case "X":
                {
                    System.Console.Clear();
                    Menu0();
                    break;
                }
                default:
                    prefix("Invalid Option", "");
                    Thread.Sleep(300);
                    goto tools1;
                    break;
            }
        }

        public static void comboeditor0()
        {
            comboeditor:
            Console.Title =
                "                                                                                                       [>] BoltAIO | Combo Editor | Quanotics#3931 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            prefix("1", "Capture Remover\n");
            prefix("2", "Mail:Pass Edit\n");
            prefix("3", "Dupe Remover\n");
            prefix("4", "Domain Sorter\n");
            prefix("X", "Go Back\n");
            prefix(">", "");
            var Read = System.Console.ReadLine().ToUpper();
            switch (Read)
            {
                case "1":
                {
                    ComboTool.CaptureRemover();
                    break;
                }
                case "2":
                {
                    ComboTool.MailPassEdit();
                    break;
                }
                case "3":
                {
                    ComboTool.DupeRemover();
                    break;
                }
                case "4":
                {
                    ComboTool.sorter();
                    break;
                }
                case "X":
                {
                    System.Console.Clear();
                    Tools0();
                    break;
                }
                default:
                    prefix("Invalid Option", "");
                    Thread.Sleep(300);
                    goto comboeditor;
                    break;
            }
        }

        public static void Ascii()
        {
            Console.WriteLine("                                ██████╗  ██████╗ ██╗  ████████╗    ██╗   ██╗██████╗ ",
                Color.Purple);
            Console.WriteLine("                                ██╔══██╗██╔═══██╗██║  ╚══██╔══╝    ██║   ██║╚════██╗",
                Color.Purple);
            Console.WriteLine("                                ██████╔╝██║   ██║██║     ██║       ██║   ██║ █████╔╝",
                Color.Purple);
            Console.WriteLine("                                ██╔══██╗██║   ██║██║     ██║       ╚██╗ ██╔╝██╔═══╝ ",
                Color.Purple);
            Console.WriteLine("                                ██████╔╝╚██████╔╝███████╗██║        ╚████╔╝ ███████╗",
                Color.Purple);
            Console.WriteLine("                                ╚═════╝  ╚═════╝ ╚══════╝╚═╝         ╚═══╝  ╚══════╝",
                Color.Purple);
        }


        private static void Checker()
        {
            if (CheckerRunning)
            {
                Task.Factory.StartNew(delegate { CheckerTools.UpdateConsole(); });
            }
            else
            {
                startSelection:
                Console.Title =
                    "                                                                                                      [>] BoltAIO | Checker | Quanotics#3931 [<]";
                Console.Clear();
                Thread.Sleep(50);
                Console.WriteLine("");
                Ascii();
                Console.WriteLine("");
                Console.WriteLine("");
                prefix("MODULES", "Some Are Broken\n\n");
                ModulesList.modules();
                Console.WriteLine("");
                Console.WriteLine("");
                prefix("B", "Start Checker\n");
                prefix("P", "Add All Modules\n");
                prefix("X", "Go Back\n");
                Console.WriteLine("");
                prefix("Select Modules", "\n");
                prefix(">", "");
                var moduleInput = Console.ReadLine().ToUpper();
                switch (moduleInput)
                {
                    case "X":
                        Menu0();
                        break;
                    case "1":
                        addModule(Abvmod.Check);
                        goto startSelection;
                    case "2":
                        addModule(Adflymod.Check);
                        goto startSelection;
                    case "3":
                        addModule(Ahamod.Check);
                        goto startSelection;
                    case "4":
                        addModule(Albertonsmod.Check);
                        goto startSelection;
                    case "5":
                        addModule(Aliexpressmod.Check);
                        goto startSelection;
                    case "6":
                        addModule(Apowersoftmod.Check);
                        goto startSelection;
                    case "7":
                        addModule(Applemod.Check);
                        goto startSelection;
                    case "8":
                        addModule(Aviramod.Check);
                        goto startSelection;
                    case "9":
                        addModule(Azuremod.Check);
                        goto startSelection;
                    case "10":
                        addModule(Bagelboymod.Check);
                        goto startSelection;
                    case "11":
                        addModule(Beegcommod.Check);
                        goto startSelection;
                    case "12":
                        addModule(BFWmod.Check);
                        goto startSelection;
                    case "13":
                        addModule(Bitdefendermod.Check);
                        goto startSelection;
                    case "14":
                        addModule(Bitesquadmod.Check);
                        goto startSelection;
                    case "15":
                        addModule(Bitlaunchmod.Check);
                        goto startSelection;
                    case "16":
                        addModule(Blimmod.Check);
                        goto startSelection;
                    case "17":
                        addModule(CBSmod.Check);
                        goto startSelection;
                    case "18":
                        addModule(chaturbatemod.Check);
                        goto startSelection;
                    case "19":
                        addModule(cheggmod.Check);
                        goto startSelection;
                    case "20":
                        addModule(Clarovideomod.Check);
                        goto startSelection;
                    case "21":
                        addModule(Coinbasemod.Check);
                        goto startSelection;
                    case "22":
                        addModule(coinsphmod.Check);
                        goto startSelection;
                    case "23":
                        addModule(ColdStoneCreamerymod.Check);
                        goto startSelection;
                    case "24":
                        addModule(Crunchyrollmod.Check);
                        goto startSelection;
                    case "25":
                        addModule(DCuniversemod.Check);
                        goto startSelection;
                    case "26":
                        addModule(Disneymod.Check);
                        goto startSelection;
                    case "27":
                        addModule(Dominosmod.Check);
                        goto startSelection;
                    case "28":
                        addModule(DoorDashmod.Check);
                        goto startSelection;
                    case "29":
                        addModule(Duolingomod.Check);
                        goto startSelection;
                    case "30":
                        addModule(Easyjetmod.Check);
                        goto startSelection;
                    case "31":
                        addModule(Ebayvaildmod.Check);
                        goto startSelection;
                    case "32":
                        addModule(Elasticmailmod.Check);
                        goto startSelection;
                    case "33":
                        addModule(Facebookmod.Check);
                        goto startSelection;
                    case "34":
                        addModule(filminmod.Check);
                        goto startSelection;
                    case "35":
                        addModule(Fitbitmod.Check);
                        goto startSelection;
                    case "36":
                        addModule(Flightclubmod.Check);
                        goto startSelection;
                    case "37":
                        addModule(Foapmod.Check);
                        goto startSelection;
                    case "38":
                        addModule(Forever21mod.Check);
                        goto startSelection;
                    case "39":
                        addModule(Foxmod.Check);
                        goto startSelection;
                    case "40":
                        addModule(FWRDmod.Check);
                        goto startSelection;
                    case "41":
                        addModule(Gameflymod.Check);
                        goto startSelection;
                    case "42":
                        addModule(GetUpsidemod.Check);
                        goto startSelection;
                    case "43":
                        addModule(Gfuelmod.Check);
                        goto startSelection;
                    case "44":
                        addModule(Godaddymod.Check);
                        goto startSelection;
                    case "45":
                        addModule(GooseVPNmod.Check);
                        goto startSelection;
                    case "46":
                        addModule(Guccimod.Check);
                        goto startSelection;
                    case "47":
                        addModule(HeadspaceUKmod.Check);
                        goto startSelection;
                    case "48":
                        addModule(HMAmod.Check);
                        goto startSelection;
                    case "49":
                        addModule(HolaVPNmod.Check);
                        goto startSelection;
                    case "50":
                        addModule(Hotspotshieldmod.Check);
                        goto startSelection;
                    case "51":
                        addModule(Hulumod.Check);
                        goto startSelection;
                    case "52":
                        addModule(IbVPNmod.Check);
                        goto startSelection;
                    case "53":
                        addModule(Instagrammod.Check);
                        goto startSelection;
                    case "54":
                        addModule(IpVanishmod.Check);
                        goto startSelection;
                    case "55":
                        addModule(KFCMod.Check);
                        goto startSelection;
                    case "56":
                        addModule(latercommod.Check);
                        goto startSelection;
                    case "57":
                        addModule(Leageoflegendsmod.Check);
                        goto startSelection;
                    case "58":
                        addModule(Luminatimod.Check);
                        goto startSelection;
                    case "59":
                        addModule(McAfeemod.Check);
                        goto startSelection;
                    case "60":
                        addModule(Minecraftmod.Check);
                        goto startSelection;
                    case "61":
                        addModule(Minglemod.Check);
                        goto startSelection;
                    case "62":
                        addModule(MyCanal.Check);
                        goto startSelection;
                    case "63":
                        addModule(Napstermod.Check);
                        goto startSelection;
                    case "64":
                        addModule(NBAmod.Check);
                        goto startSelection;
                    case "65":
                        addModule(Netflixmod.Check);
                        goto startSelection;
                    case "66":
                        addModule(NordVPNmod.Check);
                        goto startSelection;
                    case "67":
                        addModule(Onlyfansmod.Check);
                        goto startSelection;
                    case "68":
                        addModule(Originmod.Check);
                        goto startSelection;
                    case "69":
                        addModule(Outbackstakehousemod.Check);
                        goto startSelection;
                    case "70":
                        addModule(patreonmod.Check);
                        goto startSelection;
                    case "71":
                        addModule(Pizzahutmod.Check);
                        goto startSelection;
                    case "72":
                        addModule(Plextvmod.Check);
                        goto startSelection;
                    case "73":
                        addModule(Pornhubmod.Check);
                        goto startSelection;
                    case "74":
                        addModule(Postmatesfleet.Check);
                        goto startSelection;
                    case "75":
                        addModule(Razermod.Check);
                        goto startSelection;
                    case "76":
                        addModule(Robinhoodmod.Check);
                        goto startSelection;
                    case "77":
                        addModule(Scribdmod.Check);
                        goto startSelection;
                    case "78":
                        addModule(Shopifymod.Check);
                        goto startSelection;
                    case "79":
                        addModule(SliceLifemod.Check);
                        goto startSelection;
                    case "80":
                        addModule(SlingTVmod.Check);
                        goto startSelection;
                    case "81":
                        addModule(Smartproxymod.Check);
                        goto startSelection;
                    case "82":
                        addModule(SonyVegasmod.Check);
                        goto startSelection;
                    case "83":
                        addModule(Tunnelbearmod.Check);
                        goto startSelection;
                    case "84":
                        addModule(Twitchmod.Check);
                        goto startSelection;
                    case "85":
                        addModule(UFCmod.Check);
                        goto startSelection;
                    case "86":
                        addModule(Uplaymod.Check);
                        goto startSelection;
                    case "87":
                        addModule(Venmomod.Check);
                        goto startSelection;
                    case "88":
                        addModule(Vypervpnmod.Check);
                        goto startSelection;
                    case "89":
                        addModule(Wishmod.Check);
                        goto startSelection;
                    case "90":
                        addModule(WWEmod.Check);
                        goto startSelection;
                    case "91":
                        addModule(Xcamsmod.Check);
                        goto startSelection;
                    case "92":
                        addModule(XVPNmod.Check);
                        goto startSelection;
                    case "93":
                        addModule(Yahoomod.Check);
                        goto startSelection;
                    case "94":
                        addModule(Zenmatemod.Check);
                        goto startSelection;
                    case "95":
                        addModule(Zorrovpnmod.Check);
                        goto startSelection;
                    case "P":
                        addModule(Abvmod.Check);
                        addModule(Adflymod.Check);
                        addModule(Ahamod.Check);
                        addModule(Albertonsmod.Check);
                        addModule(Aliexpressmod.Check);
                        addModule(Apowersoftmod.Check);
                        addModule(Applemod.Check);
                        addModule(Aviramod.Check);
                        addModule(Azuremod.Check);
                        addModule(Bagelboymod.Check);
                        addModule(Beegcommod.Check);
                        addModule(BFWmod.Check);
                        addModule(Bitdefendermod.Check);
                        addModule(Bitesquadmod.Check);
                        addModule(Bitlaunchmod.Check);
                        addModule(Blimmod.Check);
                        addModule(CBSmod.Check);
                        addModule(chaturbatemod.Check);
                        addModule(cheggmod.Check);
                        addModule(Clarovideomod.Check);
                        addModule(Coinbasemod.Check);
                        addModule(coinsphmod.Check);
                        addModule(Crunchyrollmod.Check);
                        addModule(DCuniversemod.Check);
                        addModule(Disneymod.Check);
                        addModule(Dominosmod.Check);
                        addModule(DoorDashmod.Check);
                        addModule(Duolingomod.Check);
                        addModule(Easyjetmod.Check);
                        addModule(Ebayvaildmod.Check);
                        addModule(Elasticmailmod.Check);
                        addModule(Facebookmod.Check);
                        addModule(filminmod.Check);
                        addModule(Fitbitmod.Check);
                        addModule(Flightclubmod.Check);
                        addModule(Foapmod.Check);
                        addModule(Forever21mod.Check);
                        addModule(Foxmod.Check);
                        addModule(FWRDmod.Check);
                        addModule(Gameflymod.Check);
                        addModule(GetUpsidemod.Check);
                        addModule(Gfuelmod.Check);
                        addModule(Godaddymod.Check);
                        addModule(GooseVPNmod.Check);
                        addModule(Guccimod.Check);
                        addModule(HeadspaceUKmod.Check);
                        addModule(HMAmod.Check);
                        addModule(HolaVPNmod.Check);
                        addModule(Hotspotshieldmod.Check);
                        addModule(Hulumod.Check);
                        addModule(IbVPNmod.Check);
                        addModule(Instagrammod.Check);
                        addModule(IpVanishmod.Check);
                        addModule(KFCMod.Check);
                        addModule(latercommod.Check);
                        addModule(Leageoflegendsmod.Check);
                        addModule(Luminatimod.Check);
                        addModule(McAfeemod.Check);
                        addModule(Minecraftmod.Check);
                        addModule(Minglemod.Check);
                        addModule(MyCanal.Check);
                        addModule(Napstermod.Check);
                        addModule(NBAmod.Check);
                        addModule(Netflixmod.Check);
                        addModule(NordVPNmod.Check);
                        addModule(Onlyfansmod.Check);
                        addModule(Originmod.Check);
                        addModule(Outbackstakehousemod.Check);
                        addModule(patreonmod.Check);
                        addModule(Pizzahutmod.Check);
                        addModule(Plextvmod.Check);
                        addModule(Pornhubmod.Check);
                        addModule(Postmatesfleet.Check);
                        addModule(Razermod.Check);
                        addModule(Robinhoodmod.Check);
                        addModule(Scribdmod.Check);
                        addModule(Shopifymod.Check);
                        addModule(SliceLifemod.Check);
                        addModule(SlingTVmod.Check);
                        addModule(Smartproxymod.Check);
                        addModule(SonyVegasmod.Check);
                        addModule(Tunnelbearmod.Check);
                        addModule(Twitchmod.Check);
                        addModule(UFCmod.Check);
                        addModule(Uplaymod.Check);
                        addModule(Venmomod.Check);
                        addModule(Vypervpnmod.Check);
                        addModule(Wishmod.Check);
                        addModule(WWEmod.Check);
                        addModule(Zorrovpnmod.Check);
                        addModule(Xcamsmod.Check);
                        addModule(XVPNmod.Check);
                        addModule(Yahoomod.Check);
                        addModule(Zenmatemod.Check);
                        goto startSelection;
                    case "B":
                    {
                        if (PickedModules.Count == 0)
                        {
                            prefix("Error", "You need to select at least 1 module");
                            Thread.Sleep(1000);
                            goto startSelection;
                            return;
                        }
                        Console.Clear();
                        System.Console.WriteLine();
                        Ascii();
                        System.Console.WriteLine();
                        System.Console.WriteLine();
                        prefix("Select Combolist", "");
                        ComboLoad();
                        prefix("Select Proxylist", "");
                        ProxyLoad();
                        for (;;)
                        {
                            prefix("Select Proxy Type", "\n");
                            prefix("1", "HTTP\n");
                            prefix("2", "SOCKS4\n");
                            prefix("3", "SOCKS5\n");
                            prefix(">", "");
                            var Read = System.Console.ReadLine();
                            switch (Read)
                            {
                                case "1":
                                    ProxyType1 = "HTTP";
                                    break;

                                case "2":
                                    ProxyType1 = "SOCKS4";
                                    break;

                                case "3":
                                    ProxyType1 = "SOCKS5";
                                    break;
                            }

                            if (ProxyType1 != "HTTP" && ProxyType1 != "SOCKS4" && ProxyType1 != "SOCKS5") continue;
                            break;
                        }

                        for (;;)
                        {
                            Console.WriteLine();
                            prefix("Select Display Type", "\n");
                            prefix("1", "CUI\n");
                            prefix("2", "LOG\n");
                            prefix(">", "");
                            var Read = System.Console.ReadLine();
                            switch (Read)
                            {
                                case "1":
                                    lorc = "CUI";
                                    break;

                                case "2":
                                    lorc = "LOG";
                                    break;
                            }

                            if (lorc != "CUI" && lorc != "LOG") continue;
                            break;
                        }

                        selectThreads:
                        Console.WriteLine();
                        prefix("How many threads do you want to use", "\n");
                        prefix(">", "");
                        try
                        {
                            Threads = Convert.ToInt32(System.Console.ReadLine());
                        }
                        catch
                        {
                            prefix("Error! Input a number", "");
                            Console.Write("    [", Color.White);
                            Console.Write("Error! Input a number", Color.Red);
                            Console.Write("]\n", Color.White);
                            goto selectThreads;
                        }

                        CheckerRunning = true;
                        var totalmodules = PickedModules.Count().ToString();
                        Console.Write("    [Selected ", Color.White);
                        Console.Write(totalmodules, Color.Purple);
                        Console.Write(" Modules]\n", Color.White);

                        Task.Factory.StartNew(delegate { CheckerTools.UpdateTitle(); });

                        switch (lorc)
                        {
                            case "CUI":
                                Task.Factory.StartNew(delegate { CheckerTools.UpdateConsole(); });
                                break;
                            default:
                                Console.Clear();
                                Console.WriteLine("");
                                Ascii();
                                Console.WriteLine("");
                                Console.WriteLine("");
                                break;
                        }

                        var num = 0;
                        while (num <= Threads)
                            foreach (var checkFunction in PickedModules.Distinct())
                            {
                                new Thread(new ThreadStart(checkFunction)).Start();
                                num = num + 1;
                            }

                        break;
                    }
                    default:
                        prefix("Invalid Option", "");
                        Thread.Sleep(300);
                        goto startSelection;
                }
            }
        }

        public static void ComboLoad()
        {
            string fileName;
            var t = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Combo List";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                Combos = new List<string>(File.ReadAllLines(fileName));

                Combostotal = Combos.Count * PickedModules.Count();
                Console.Write("Selected ", Color.White);
                Console.Write(Combostotal, Color.Purple);
                Console.Write(" Combos\n", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public static void ProxyLoad()
        {
            string fileName;
            var x = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Proxy List";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                Proxies = new List<string>(File.ReadAllLines(fileName));
                Proxytotal = Proxies.Count();
                Console.Write("Selected ", Color.White);
                Console.Write(Proxytotal, Color.Purple);
                Console.Write(" Proxies\n\n", Color.White);
            });
            x.SetApartmentState(ApartmentState.STA);
            x.Start();
            x.Join();
        }
    }
}