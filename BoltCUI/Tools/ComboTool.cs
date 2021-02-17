using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Bolt_AIO;
using Console = Colorful.Console;

namespace BoltCUI.Tools
{
    public class ComboTool
    {
        public static List<string> count = new List<string>();

        public static void CaptureRemover()
        {
            Console.Title =
                "                                                                                                 [>] BoltAIO | Combo Editor -> MailPassEdit | Quanotics#3931 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            Console.Write("    [", Color.White);
            Console.Write("Select", Color.White);
            Console.Write(" Combos", Color.Purple);
            Console.Write("]\n", Color.White);
            var type1 = DateTime.Now.ToString("dd-MM-yy HH-mm-ss");

            string fileName = null;
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

                Program.Combos = new List<string>(File.ReadAllLines(fileName));
                Program.Combostotal = Program.Combos.Count();
                Console.Write("    [", Color.White);
                Console.Write("Selected ", Color.White);
                Console.Write(Program.Combostotal, Color.Purple);
                Console.Write(" Combos", Color.White);
                Console.Write("]\n", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            var hello = File.ReadAllLines(fileName);

            var read = File.ReadAllLines(fileName);
            using (var rite = new StreamWriter("./Results/" + "Capture Remover " + type1 + ".txt"))
            {
                foreach (var text in read)
                    if (text.Contains("@"))
                        if (text.Contains(":"))
                        {
                            var helloa = text;
                            {
                                var email = helloa.Split(' ')[0];
                                Console.WriteLine("    ------------------------------------");
                                Console.WriteLine("    " + email, Color.White);
                                Console.WriteLine("    ------------------------------------");
                                rite.WriteLine(email);
                            }
                        }
            }

            Thread.Sleep(250);
            Console.Write("    [", Color.White);
            Console.Write("Done Editing", Color.Purple);
            Console.Write("]\n", Color.White);
            Thread.Sleep(500);
            Program.comboeditor0();
        }


        public static void DupeRemover()
        {
            Console.Title =
                "                                                                                                 [>] BoltAIO | Combo Editor -> DupeRemover | Quanotics#3931 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            Console.Write("    [", Color.White);
            Console.Write("Select Combos", Color.Purple);
            Console.Write("]\n", Color.White);
            var type1 = DateTime.Now.ToString("dd-MM-yy HH-mm-ss");

            string fileName = null;
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

                Program.Combos = new List<string>(File.ReadAllLines(fileName));
                Program.Combostotal = Program.Combos.Count();
                Console.Write("    [", Color.White);
                Console.Write("Selected ", Color.White);
                Console.Write(Program.Combostotal, Color.Purple);
                Console.Write(" Combos", Color.White);
                Console.Write("]\n", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            Console.Write("    [", Color.White);
            Console.Write("Removing Dupes...", Color.Purple);
            Console.Write("]\n", Color.White);

            var lines = File.ReadAllLines(fileName);
            File.WriteAllLines(fileName, lines.Distinct().ToArray());

            Console.Write("    [", Color.White);
            Console.Write("Done Removing Dupes...", Color.Purple);
            Console.Write("]\n", Color.White);
            Thread.Sleep(1000);
            Program.comboeditor0();
        }

        public static void MailPassEdit()
        {
            Console.Title =
                "                                                                                                 [>] BoltAIO | Combo Editor -> MailPassEdit | Quanotics#3931 [<]";
            System.Console.Clear();
            System.Console.WriteLine();
            Program.Ascii();
            System.Console.WriteLine();
            System.Console.WriteLine();
            Console.Write("    [", Color.White);
            Console.Write("Select Combos", Color.Purple);
            Console.Write("]\n", Color.White);


            string fileName1 = null;
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
                    fileName1 = openFileDialog.FileName;
                } while (!File.Exists(fileName1));

                Program.Combos = new List<string>(File.ReadAllLines(fileName1));
                Program.Combostotal = Program.Combos.Count();
                Console.Write("    [", Color.White);
                Console.Write("Selected ", Color.White);
                Console.Write(Program.Combostotal, Color.Purple);
                Console.Write(" Combos", Color.White);
                Console.Write("]\n", Color.White);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            var readAllLines = File.ReadAllLines(fileName1);

            Console.Write("    [", Color.White);
            Console.Write("Cleaning Combos...", Color.Purple);
            Console.Write("]\n", Color.White);

            using (var writer1 = new StreamWriter(fileName1))
            {
                foreach (var s in readAllLines)
                    if (s.Contains(":") && s.Contains("@"))
                        writer1.WriteLine(s);
            }

            var linesa = File.ReadAllLines(fileName1).Where(arg => !string.IsNullOrWhiteSpace(arg));
            File.WriteAllLines(fileName1, linesa);

            var readText = File.ReadAllLines(fileName1);
            File.WriteAllText(fileName1, string.Empty);
            using (var writer = new StreamWriter(fileName1))
            {
                foreach (var s in readText)
                {
                    var email = s.Split(':')[0];
                    var pass = s.Split(':')[1];

                    switch (pass.Equals(""))
                    {
                        case false:
                        {
                            if (!email.Equals(""))
                                if (!s.Contains("//https"))
                                    if (!s.Contains("DELETED"))
                                        if (!s.Contains("::"))
                                            if (!s.Contains(":::"))
                                                if (!s.Contains("::::"))
                                                    if (!pass.Contains("@gmail.com"))
                                                        if (!pass.Contains("@yahoo.com"))
                                                            if (!pass.Contains("@outlook.com"))
                                                                if (!pass.Contains("@hotmail.com"))
                                                                    writer.WriteLine(s);
                            break;
                        }
                    }
                }
            }

            Thread.Sleep(500);
            var type1 = DateTime.Now.ToString("dd-MM-yy HH-mm-ss");
            Console.Write("    [", Color.White);
            Console.Write("Editing Combos...", Color.Purple);
            Console.Write("]\n", Color.White);
            using (var writer = new StreamWriter("./Results/" + "MailPassEdited " + type1 + ".txt"))
            {
                var a = 0;

                var array = File.ReadAllLines(fileName1);
                foreach (var text in readAllLines)
                {
                    var mail = text.Split(':')[0];
                    var pass = text.Split(':')[1];
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1));
                    writer.WriteLine(mail + ":" + char.ToLower(pass[0]) + pass.Substring(1));
                    writer.WriteLine(mail + ":" + pass + "12");
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1) + "!");
                    writer.WriteLine(mail + ":" + pass + "123");
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1) + "123");
                    writer.WriteLine(mail + ":" + pass + "00");
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1) + "?");
                    writer.WriteLine(mail + ":" + pass + "*");
                    writer.WriteLine(mail + ":" + pass + "$");
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1) + "@");
                    writer.WriteLine(mail + ":" + pass + "1!");
                    writer.WriteLine(mail + ":" + pass + "1234");
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1) + "1234");
                    writer.WriteLine(mail + ":" + pass + "12345");
                    writer.WriteLine(mail + ":" + char.ToUpper(pass[0]) + pass.Substring(1) + "12345");
                    writer.WriteLine(mail + ":" + pass + "?");
                    writer.WriteLine(mail + ":" + pass + "!@");
                    writer.WriteLine(mail + ":" + pass + "!@#");
                    writer.WriteLine(mail + ":" + pass + "*$@");
                    writer.WriteLine(mail + ":" + pass + "1");
                    a = a + 21;
                    System.Console.Title = $"Bolt AIO | Total Combos Edited: {a.ToString()}";
                }
            }

            Console.Write("    [", Color.White);
            Console.Write("Removing Dupes...", Color.Purple);
            Console.Write("]\n", Color.White);

            var lines1 = File.ReadAllLines("./Results/" + "MailPassEdited " + type1 + ".txt");
            File.WriteAllLines("./Results/" + "MailPassEdited " + type1 + ".txt", lines1.Distinct().ToArray());


            System.Console.Title = $"Bolt AIO | Total Combos Edited: {count} ";

            var lines = File.ReadAllLines("./Results/" + "MailPassEdited " + type1 + ".txt");

            Console.Write("    [", Color.White);
            Console.Write("Randomizing Lines...", Color.Purple);
            Console.Write("]\n", Color.White);
            var rnd = new Random();
            lines = lines.OrderBy(line => rnd.Next()).ToArray();
            File.WriteAllLines("./Results/" + "MailPassEdited " + type1 + ".txt", lines);

            Thread.Sleep(250);
            Console.Write("    [", Color.White);
            Console.Write("Redirecting to Combo editor...", Color.Purple);
            Console.Write("]\n", Color.White);
            Thread.Sleep(500);
            Program.comboeditor0();
        }

        public static void sorter()
        {
            var gmailc = 0;
            var yahooc = 0;
            var hotmailc = 0;
            var aolc = 0;
            var homailukc = 0;
            var hotmailfrc = 0;
            var yahoofrc = 0;
            var wanadoofrc = 0;
            var orangefrc = 0;
            var comcastc = 0;
            var yahoocouk = 0;
            var yahoocombr = 0;
            var yahoocoin = 0;
            var livecom = 0;
            var icloud = 0;
            var freefr = 0;
            var gmxde = 0;
            var webde = 0;
            var yandexru = 0;
            var ymail = 0;
            var outlookc = 0;
            var mailruc = 0;
            var googlemail = 0;
            var livecouk = 0;
            var verizonnet = 0;
            var protonmailc = 0;
            var gmxnet = 0;
            Console.Clear();
            Program.Ascii();
            string fileName = null;
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
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();


            Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine("Sorting ComboList..", Color.White);
            var array = File.ReadAllLines(fileName);
            foreach (var text in array)
                if (text.Contains("@gmail.com"))
                    gmailc++;
            foreach (var text in array)
                if (text.Contains("@yahoo.com"))
                    yahooc++;
            foreach (var text in array)
                if (text.Contains("@hotmail.com"))
                    hotmailc++;
            foreach (var text in array)
                if (text.Contains("@aol.com"))
                    aolc++;
            foreach (var text in array)
                if (text.Contains("hotmail.co.uk"))
                    homailukc++;
            foreach (var text in array)
                if (text.Contains("@hotmail.fr"))
                    hotmailfrc++;
            foreach (var text in array)
                if (text.Contains("@yahoo.fr"))
                    yahoofrc++;
            foreach (var text in array)
                if (text.Contains("@wanadoo.fr"))
                    wanadoofrc++;
            foreach (var text in array)
                if (text.Contains("@orange.fr"))
                    orangefrc++;
            foreach (var text in array)
                if (text.Contains("@comcast.net"))
                    comcastc++;
            foreach (var text in array)
                if (text.Contains("@yahoo.co.uk"))
                    yahoocouk++;
            foreach (var text in array)
                if (text.Contains("@yahoo.com.br"))
                    yahoocombr++;
            foreach (var text in array)
                if (text.Contains("@yahoo.co.in"))
                    yahoocoin++;
            foreach (var text in array)
                if (text.Contains("@live.com"))
                    livecom++;
            foreach (var text in array)
                if (text.Contains("@icloud.com"))
                    icloud++;
            foreach (var text in array)
                if (text.Contains("@free.fr"))
                    freefr++;
            foreach (var text in array)
                if (text.Contains("@gmx.de"))
                    gmxde++;
            foreach (var text in array)
                if (text.Contains("@web.de"))
                    webde++;
            foreach (var text in array)
                if (text.Contains("@yandex.ru"))
                    yandexru++;
            foreach (var text in array)
                if (text.Contains("@ymail.com"))
                    ymail++;
            foreach (var text in array)
                if (text.Contains("@outlook.com"))
                    outlookc++;
            foreach (var text in array)
                if (text.Contains("@mail.ru"))
                    mailruc++;
            foreach (var text in array)
                if (text.Contains("@googlemail.com"))
                    googlemail++;
            foreach (var text in array)
                if (text.Contains("@live.co.uk"))
                    livecouk++;
            foreach (var text in array)
                if (text.Contains("@verizon.net"))
                    verizonnet++;
            foreach (var text in array)
                if (text.Contains("@protonmail.com"))
                    protonmailc++;
            foreach (var text in array)
                if (text.Contains("@gmx.net"))
                    gmxnet++;

            var count1 = File.ReadAllLines(fileName).Length;
            var other1 = gmailc + yahooc + hotmailc + gmxnet + aolc + homailukc + hotmailfrc + yahoofrc + orangefrc +
                         wanadoofrc + orangefrc + comcastc + yahoocouk + yahoocombr + yahoocoin + livecom + icloud +
                         freefr + gmxde + webde + yandexru + ymail + outlookc + mailruc + googlemail + livecouk +
                         verizonnet + protonmailc;
            var other = count1 - other1;
            Console.Clear();
            Program.Ascii();
            Console.WriteLine("    Total Number Of Lines = " + count1, Color.White);
            Console.WriteLine("    ----------------------------------------------------------");
            Console.WriteLine("    Gmail.com = " + gmailc, Color.White);
            Console.WriteLine("    Yahoo.com = " + yahooc, Color.White);
            Console.WriteLine("    Hotmail.com = " + hotmailc, Color.White);
            Console.WriteLine("    Aol.com = " + aolc, Color.White);
            Console.WriteLine("    Homtmail.co.uk = " + homailukc, Color.White);
            Console.WriteLine("    Hotmail.fr = " + hotmailfrc, Color.White);
            Console.WriteLine("    Yahoo.fr = " + yahoofrc, Color.White);
            Console.WriteLine("    Wandoo.fr = " + wanadoofrc, Color.White);
            Console.WriteLine("    Orange.fr = " + orangefrc, Color.White);
            Console.WriteLine("    Comcast.net = " + comcastc, Color.White);
            Console.WriteLine("    Gmx.net = " + gmxnet, Color.White);
            Console.WriteLine("    Yahoo.co.uk = " + yahoocouk, Color.White);
            Console.WriteLine("    Yahoo.co.br = " + yahoocombr, Color.White);
            Console.WriteLine("    Yahoo.co.in = " + yahoocoin, Color.White);
            Console.WriteLine("    Live.com = " + livecom, Color.White);
            Console.WriteLine("    Icloud.com = " + icloud, Color.White);
            Console.WriteLine("    free.fr = " + freefr, Color.White);
            Console.WriteLine("    gmx.de = " + gmxde, Color.White);
            Console.WriteLine("    web.de = " + webde, Color.White);
            Console.WriteLine("    yandex.ru = " + yandexru, Color.White);
            Console.WriteLine("    ymail.com = " + ymail, Color.White);
            Console.WriteLine("    outlook.com = " + outlookc, Color.White);
            Console.WriteLine("    mail.ru = " + mailruc, Color.White);
            Console.WriteLine("    googlemail.com = " + googlemail, Color.White);
            Console.WriteLine("    live.co.uk = " + livecouk, Color.White);
            Console.WriteLine("    verizon.net = " + verizonnet, Color.White);
            Console.WriteLine("    protonmail.com = " + protonmailc, Color.White);
            Console.WriteLine("    Others = " + other, Color.White);
            Console.WriteLine("    ----------------------------------------------------------");

            Console.WriteLine("    Saving..", Color.White);
            Directory.CreateDirectory("./DomainResults");
            var array1 = File.ReadAllLines(fileName);
            using (var gmail = new StreamWriter("./DomainResults/@gmail.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@gmail.com") || text.Contains("@Gmail.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@yahoo.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@yahoo.com") || text.Contains("@Yahoo.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@hotmail.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@hotmail.com") || text.Contains("@Hotmail.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@aol.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@aol.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@hotmail.co.uk.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@hotmail.co.uk"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@gmx.net.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@gmx.net"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@hotmail.fr.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@hotmail.fr"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@yahoo.fr.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@yahoo.fr"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@wanadoo.fr.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@wanadoo.fr"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@orange.fr.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@orange.fr"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@comcast.net.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@comcast.net"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@yahoo.co.uk.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@yahoo.co.uk"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@yahoo.com.br.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@yahoo.com.br"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@yahoo.co.in.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@yahoo.co.in"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@live.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@live.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@icloud.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@icloud.com.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@free.fr.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@free.fr"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@gmx.de.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@gmx.de"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@web.de.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@web.de"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@yandex.ru.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@yandex.ru"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@ymail.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@ymail.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@outlook.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@outlook.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@mail.ru.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@mail.ru"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@googlemail.com.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@googlemail.com"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@live.co.uk.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@live.co.uk"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/@verizon.net.txt"))
            {
                foreach (var text in array)
                    if (text.Contains("@verizon.net"))
                        gmail.WriteLine(text);
            }

            using (var gmail = new StreamWriter("./DomainResults/other domain.txt"))
            {
                foreach (var text in array)
                    if (!text.Contains("@gmail.com"))
                        if (!text.Contains("@yahoo.com"))
                            if (!text.Contains("@hotmail.com"))
                                if (!text.Contains("@gmx.net"))
                                    if (!text.Contains("@aol.com"))
                                        if (!text.Contains("@hotmail.co.uk"))
                                            if (!text.Contains("@hotmail.fr"))
                                                if (!text.Contains("@yahoo.fr"))
                                                    if (!text.Contains("@wandoo.fr"))
                                                        if (!text.Contains("@orange.fr"))
                                                            if (!text.Contains("@comcast.net"))
                                                                if (!text.Contains("@yahoo.co.uk"))
                                                                    if (!text.Contains("@yahoo.co.br"))
                                                                        if (!text.Contains("@yahoo.co.in"))
                                                                            if (!text.Contains("@live.com"))
                                                                                if (!text.Contains("@icloud.com"))
                                                                                    if (!text.Contains("@free.fr"))
                                                                                        if (!text.Contains("@gmx.de"))
                                                                                            if (!text.Contains(
                                                                                                "@web.de"))
                                                                                                if (!text.Contains(
                                                                                                    "@yandex.ru"))
                                                                                                    if (!text.Contains(
                                                                                                        "@ymail.com"))
                                                                                                        if (!text
                                                                                                            .Contains(
                                                                                                                "@outlook.com")
                                                                                                        )
                                                                                                            if (!text
                                                                                                                .Contains(
                                                                                                                    "@mail.ru")
                                                                                                            )
                                                                                                                if (!
                                                                                                                    text
                                                                                                                        .Contains(
                                                                                                                            "@googlemail.com")
                                                                                                                )
                                                                                                                    if (
                                                                                                                        !text
                                                                                                                            .Contains(
                                                                                                                                "@live.co.uk")
                                                                                                                    )
                                                                                                                        if
                                                                                                                        (!
                                                                                                                            text
                                                                                                                                .Contains(
                                                                                                                                    "@verizon.net")
                                                                                                                        )
                                                                                                                            if
                                                                                                                            (!
                                                                                                                                text
                                                                                                                                    .Contains(
                                                                                                                                        "@protonmail.com")
                                                                                                                            )
                                                                                                                                gmail
                                                                                                                                    .WriteLine(
                                                                                                                                        text);
            }

            Console.WriteLine();

            Console.Write("    [", Color.White);
            Console.Write("Files Saved In '", Color.White);
            Console.Write("DomainResults", Color.Purple);
            Console.Write("' files Saved In", Color.White);
            Console.Write("]\n", Color.White);
            Thread.Sleep(3500);
            Console.Clear();
            Program.comboeditor0();
        }
    }
}