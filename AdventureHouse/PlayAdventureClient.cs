using AdventureHouse.Services;
using AdventureHouse.Services.Models;
using AdventureServer.Interfaces;
using AdventureServer.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Streaming.Adaptive;

namespace AdventureHouse
{
    internal class PlayAdventureClient
    {

        private static IAppVersionService appVersionService = new AppVersionService();
        private static GameMoveResult gmr;
        const string DevBy = "Developed by";
        readonly static string SteveSparks = "Steve Sparks";
        readonly static string RepoName = "GitHub";
        readonly static string RepoURL = "https://github.com/Three-Peppers-Gaming";
        readonly static string WelcomeTitle = "Adventure House";
        const string UNDERLINE = "\x1B[4m";
        const string RESET = "\x1B[0m";
        private static Boolean Monotone = false;
        private static Boolean Scroll = false;
        private static ConsoleColor Monocolor = ConsoleColor.White;

        private static void SetColor(ConsoleColor consoleWordColor)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = consoleWordColor;
        }

        private static void Write(ConsoleColor c, string text = "")
        {
            if (!Monotone) SetColor(c);
            else SetColor(Monocolor);
            Console.Write(text);
        }

        private static void WriteLn(ConsoleColor c, string text = "")
        {
            Write(c, text);
            Console.WriteLine();
        }

        // # of characters until the next space, EOL or end of string;
        // This is used to see if the next place we can break a line in within 10% of the console max width;
        public static int PeekToNexBreak(int currentposition, string s)
        {
            int l = s.Length;
            int p = currentposition;
            string cstr = string.Empty;
            int c = 2;

            if (currentposition == s.Length) return 0;
            if ((currentposition + 1) == s.Length) return 1;

            p = p + 2;

            while (p < l-1) 
            {
                p++; 
                cstr = s[p].ToString();

                if (cstr == "\r") { return c; }
                if (cstr == " ") { return c; }

                c++;

            }

            return c;
        }


        public static string Wrap(string text)
        {

            string outstr = string.Empty;
            string cstr = string.Empty;

            int l = text.Length;
            int linelength = 0;
            int maxwidth = (int)(Console.WindowWidth * .9);
            int pad = (int)(Console.WindowWidth - maxwidth);

            for (int i = 0; i < l; i++)
            {
                cstr = text[i].ToString();


                if ((cstr == "\r") || (cstr == "\n"))
                {

                  linelength = 0;

                }

                // We have space, should we wrap the line?
                if (cstr == " ")
                {
                    int peek = PeekToNexBreak(i, text); // find # charts out the next oppertunity to break the line. 
                    int temp = (linelength + peek);

                    if (((linelength + peek) > maxwidth))
                    {
                        outstr = outstr + "\r\n"; // ignore the space and add crlf
                        cstr = "";
                        linelength = 0;
                    }

                    if ((cstr == " ") && (linelength == 0)) cstr= "";

                }

                outstr = outstr + cstr;
                linelength++;

            }

            return outstr;
        }


        public static string _oldWrap(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            
            var approxLineCount = text.Length / Console.WindowWidth ;
            var lines = new StringBuilder(text.Length + (approxLineCount * 4));


            for (var i = 0; i < text.Length;)
            {
                var lineLimit = Math.Min(Console.WindowWidth, text.Length - i);
                
                var line = text.Substring(i, lineLimit);

                var isLastLine = lineLimit + i == text.Length;

                if (isLastLine)
                {
                    i = i + lineLimit;
                    lines.Append(line);
                }
                else
                {
                    var lastSpace = line.LastIndexOf(" ", StringComparison.Ordinal);
                    lines.AppendLine(line.Substring(0, lastSpace));

                    //omit extra space
                    i = i + lastSpace + 1;
                }
            }
            return lines.ToString();
        }

        private static void DisplayHelp()
        {
            WriteLn(ConsoleColor.White);
            Write(ConsoleColor.DarkCyan, WelcomeTitle.ToUpper());
            WriteLn(ConsoleColor.DarkGreen, $" - {appVersionService.Version}");
            WriteLn(ConsoleColor.Green);
            WriteLn(ConsoleColor.White,"Console Commands:");
            WriteLn(ConsoleColor.Green,Wrap("Commands to help manage the game play experience."));
            WriteLn(ConsoleColor.Red);
            Write(ConsoleColor.White, "Clear  - "); WriteLn(ConsoleColor.Yellow, "Clear the screen and scroll buffer");
            Write(ConsoleColor.White, "Color  - "); WriteLn(ConsoleColor.Yellow, "Restore text to use color formatting after using a color command.");
            Write(ConsoleColor.White, "Intro  - "); WriteLn(ConsoleColor.Yellow, "Display Game Information.");
            Write(ConsoleColor.White, "White  - "); WriteLn(ConsoleColor.Yellow, "Set text color to white and remove color formatting.");
            Write(ConsoleColor.Green, "Green  - "); WriteLn(ConsoleColor.Yellow, "Set text color to green and remove color formatting.");
            Write(ConsoleColor.Cyan,  "Cyan   - "); WriteLn(ConsoleColor.Yellow, "Set text color to cyan and remove color formatting.");
            Write(ConsoleColor.White, "Scroll - "); WriteLn(ConsoleColor.Yellow, "Toggle Scrolling of the adventuire text verse clearing screen between moves.");
            Write(ConsoleColor.White, "Time   - "); WriteLn(ConsoleColor.Yellow, "Display System date and time.");
            Write(ConsoleColor.White, "Resign - "); WriteLn(ConsoleColor.Yellow, "Exit Game.");
            WriteLn(ConsoleColor.Green);
        }


        private static void DisplayIntro()
        {
            WriteLn(ConsoleColor.White);
            Write(ConsoleColor.DarkCyan,WelcomeTitle.ToUpper());
            WriteLn(ConsoleColor.DarkGreen, $" - {appVersionService.Version}");
            WriteLn(ConsoleColor.White);
            Write(ConsoleColor.White,"Developed By: "); WriteLn(ConsoleColor.Red,SteveSparks);
            WriteLn(ConsoleColor.White);
            Write(ConsoleColor.White,"Find out more on ");
            Write(ConsoleColor.Green,RepoName);
            Write(ConsoleColor.White," at ");
            WriteLn(ConsoleColor.Cyan,RepoURL);
            WriteLn(ConsoleColor.White);
            Write(ConsoleColor.DarkRed, "ATTENTION:");
            WriteLn(ConsoleColor.Yellow, " To exit type \"resign\", For other commands type \"chelp\"");
            WriteLn(ConsoleColor.Green);
        }


        public static void PlayAdventure(AdventureFrameworkService _client)
        {
            string instanceID = string.Empty;
            string move = string.Empty;
            bool error = false;
            string errormsg = string.Empty; 

            try
            {
                DisplayIntro();
               
                // default to game 1 until I we have selection system
                // Gets the first game and sets up the game 
                gmr = _client.FrameWork_NewGame(1);
                instanceID = gmr.InstanceID;
                error = false;

            }
            catch (Exception e)
            {

                errormsg = e.ToString();
                WriteLn(ConsoleColor.Red,Wrap(errormsg));
                WriteLn(ConsoleColor.White);
                return;
            }

            while (move != "resign")
            {

                switch (move)
                {
                    case "chelp":
                        DisplayHelp();
                        move = "";
                        break;
                    case "time":
                        WriteLn(ConsoleColor.Green);
                        Write(ConsoleColor.White, $"Date and Time - ");
                        WriteLn(ConsoleColor.Green, $"{DateAndTime.Now.ToString()}");
                        WriteLn(ConsoleColor.Green);
                       move = "";
                        break;
                    case "dev":
                    case "developer":
                    case "intro":
                        DisplayIntro();
                        move = "";
                        break;
                    case "mono":
                        if (Monotone == true) Monotone = false;
                        else Monotone = true;
                        Console.Clear();
                        move = "look";
                        break;
                    case "white":
                        Monotone = true;
                        Monocolor = ConsoleColor.White; 
                        Console.Clear();
                        move = "look";
                        break;
                    case "green":
                        Monotone = true;
                        Monocolor = ConsoleColor.Green;
                        Console.Clear();
                        move = "look";
                        break;
                    case "cyan":
                        Monotone = true;
                        Monocolor = ConsoleColor.Cyan;
                        Console.Clear();
                        move = "look";
                        break;
                    case "color":
                        Monotone = false;
                        Console.Clear();
                        move = "look";
                        break;
                    case "clear":
                    case "cls":
                        Console.Clear();
                        move = "look";
                        break;
                    case "scroll":
                        if (Scroll == true)
                        {
                            Scroll = false;
                            Console.Clear();
                            move = "look";
                        }
                        else
                        {
                            Scroll = true;
                            move = "";
                        }
                        move = "";
                        break;
                    default:

                        if (Scroll == true) { WriteLn(ConsoleColor.White); Write(ConsoleColor.White, "Room: "); }
                        Write(ConsoleColor.Yellow,gmr.RoomName);
                        WriteLn(ConsoleColor.Yellow);
                        WriteLn(ConsoleColor.Yellow);
                        string msg = Wrap(gmr.RoomMessage);
                        WriteLn(ConsoleColor.Green,Wrap(gmr.RoomMessage));
                        Write(ConsoleColor.White,"You See: ");
                        Write(ConsoleColor.Cyan,Wrap(gmr.ItemsMessage));
                        WriteLn(ConsoleColor.White," ");
                        WriteLn(ConsoleColor.White, " ");

                        Write(ConsoleColor.White,"Next Action?"); 
                        Write(ConsoleColor.Green," >"); 


                        if (error == true)
                        {
                            WriteLn(ConsoleColor.Red);
                            WriteLn(ConsoleColor.White,"Client Error:");
                            WriteLn(ConsoleColor.Red,Wrap(errormsg));
                        
                        }

                        move = Console.ReadLine();
                        
                        if (move == null) move = "";
                        if (move.Length > 100) move = move.Substring(0,100);  
                        
                        if (!Scroll) Console.Clear();
                        try
                        {
                            
                            gmr = _client.FrameWork_GameMove(new GameMove() { InstanceID = instanceID, Move = move });

                        }
                        catch (Exception e)
                        {
                            error = true;
                            errormsg = e.ToString();
                            WriteLn(ConsoleColor.Red, Wrap(errormsg));
                        }

                        break;

                }

            }

            return;
        }


    }
}
