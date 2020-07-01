using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ugh
{
    #nullable enable
    public partial class Cli
    {
        private GitHub _ghc;
        private OSPlatform _os;
        private readonly string? _pwd;
        
        private OSPlatform getOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            else
            {
                return OSPlatform.Linux;
                // and by this we mean POSIX
            }
        }
        
        public Cli(GitHub ghc)
        {
            this._ghc = ghc;
            this._os = getOSPlatform();
            this._pwd = System.IO.Directory.GetCurrentDirectory();
        }

        public async Task Run(string[] args)
        {
            await ParseArguments(args);
        }
        
        private async Task ParseArguments(string[] args)
        {
            if (args.Length < 1)
            {
                await SendHelp();
            }
            else
            {
                var arg2 = (args.ElementAtOrDefault(1) ?? String.Empty);
                switch (args[0])
                {
                    case "repo":
                        switch (arg2)
                        {
                            case "upload":
                                throw new NotImplementedException();
                            case "":
                                await SendRepo(_pwd);
                                return;
                            default:
                                await SendRepo(arg2.Split('/'));
                                return;
                        }
                    case "user":
                        switch (arg2)
                        {
                            case "login":
                                await LogIn();
                                return;
                            case "logout":
                                await LogOut();
                                return;
                            case "":
                                await SendUser();
                                return;
                            default:
                                await SendUser(arg2);
                                return;
                        }
                    case "keys":
                        switch (arg2)
                        {
                            case "":
                                await SendHelp();
                                return;
                            default:
                                if (args.ElementAtOrDefault(2) != null)
                                {
                                    await SendKeys(arg2, args[2]);
                                }
                                else
                                {
                                    await SendKeys(arg2);
                                }
                                return;
                        }
                    default:
                        SendHelp();
                        return;
                }
            }
        }

        private void Display(string rawHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml($"<body>{rawHtml}</body>");
            
            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body"); 
            var childNodes = htmlBody.ChildNodes;

            foreach (var node in childNodes)
            {
                if (node.NodeType != HtmlNodeType.Element) continue;
                var txt = node.InnerText.Trim();
                if (node.OuterHtml.StartsWith("<h1>"))
                {
                    Console.WriteLine($"\x1b[1m{txt}\x1b[0m");
                }
                else if (node.OuterHtml.StartsWith("<li>"))
                {
                    Console.WriteLine($"  {txt}");
                }
                else if (node.OuterHtml.StartsWith("<e>"))
                {
                    Console.WriteLine($"\x1b[31m{txt}\x1b[0m");
                }
                else if (node.OuterHtml.StartsWith("<warn>"))
                {
                    Console.WriteLine($"\x1b[1;30;1;43m{txt}\x1b[0m");
                }
                else if (node.OuterHtml.StartsWith("<i>")) {
                    Console.WriteLine($"\x1b[3m{txt}\x1b[0m");
                }
                else
                {
                    Console.WriteLine(txt);
                }
            }
        }
    }
#nullable disable
}