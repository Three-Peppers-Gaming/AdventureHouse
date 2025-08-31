using AdventureHouse.Services;
using AdventureHouse.Services.Models;
using AdventureServer.Interfaces;
using AdventureServer.Services;
using Spectre.Console;
using System.Text;

namespace AdventureHouse
{
    internal class PlayAdventureClient
    {
        private static readonly IAppVersionService appVersionService = new AppVersionService();
        private static GameMoveResult gmr = new(); // Initialize to avoid CS8618
        private static readonly string SteveSparks = "Steve Sparks";
        private static readonly string RepoURL = "https://github.com/Three-Peppers-Gaming";
        private static readonly string WelcomeTitle = "Adventure House";
        
        private static bool UseClassicMode = false;
        private static bool ScrollMode = false;
        private static readonly List<string> CommandHistory = new();

        // Simple command buffer implementation using built-in Console features
        private static void InitializeCommandBuffer()
        {
            // Initialize command history with common commands
            var commonCommands = new[]
            {
                // Console commands
                "chelp", "clear", "classic", "intro", "scroll", "time", "resign",
                
                // Game commands - basic
                "help", "look", "get", "drop", "use", "eat", "read", "wave", "throw",
                "inv", "inventory", "pet", "shoo", "points", "health", "quit", "newgame",
                
                // Directions
                "north", "south", "east", "west", "up", "down", "n", "s", "e", "w", "u", "d",
                "go north", "go south", "go east", "go west", "go up", "go down",
            };
            
            // Pre-populate command history
            foreach (var cmd in commonCommands)
            {
                CommandHistory.Add(cmd);
            }
        }

        private static void DisplayIntroWithSpectre()
        {
            AnsiConsole.Clear();
            
            // Create a fancy title
            var title = new FigletText(WelcomeTitle)
                .LeftJustified()
                .Color(Color.Cyan1);
            AnsiConsole.Write(title);

            // Version and developer info in a panel
            var infoPanel = new Panel($"[bold green]Version:[/] {appVersionService.Version}\n" +
                            $"[bold green]Developed by:[/] [red]{SteveSparks}[/]\n" +
                            $"[bold green]GitHub:[/] [link={RepoURL}]{RepoURL}[/]")
                .Header("[bold yellow]Game Information[/]")
                .BorderColor(Color.Green)
                .RoundedBorder();
    
            AnsiConsole.Write(infoPanel);

            // Instructions - USING ASCII ONLY (no Unicode arrows or bullets)
            AnsiConsole.MarkupLine("\n[bold red]ATTENTION:[/] [yellow]To exit type \"resign\", For console help type \"chelp\", For game help type \"help\"[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[dim]* Use Up/Down arrows for command history.[/]");
            AnsiConsole.MarkupLine("[dim]* Use Left/Right arrows to edit[/]");
            AnsiConsole.MarkupLine("[dim]* ESC to clear line[/]");
            AnsiConsole.WriteLine();

            // NOW pause after ALL intro content is displayed
            PauseWithSkip(5000, "[dim]Press Enter to continue or wait 5 seconds...[/]");
        }

        private static void DisplayIntroClassic()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(WelcomeTitle.ToUpper());
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($" - {appVersionService.Version}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Developed By: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(SteveSparks);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Find out more on GitHub at ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(RepoURL);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("ATTENTION:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" To exit type \"resign\", For console help type \"chelp\", For game help type \"help\"");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            // USING ASCII ONLY - no Unicode bullets or arrows
            Console.WriteLine("* Use Up/Down arrows for command history * Use Left/Right arrows to edit * ESC to clear line");
            Console.WriteLine();
            
            // NOW pause after ALL intro content is displayed
            PauseWithSkipClassic(5000, "Press Enter to continue or wait 5 seconds...");
        }

        private static void DisplayHelpWithSpectre()
        {
            AnsiConsole.Clear();
            
            var helpTable = new Table()
                .BorderColor(Color.Green)
                .AddColumn("[bold cyan]Command[/]")
                .AddColumn("[bold cyan]Description[/]");

            helpTable.AddRow("[white]chelp[/]", "Display this console commands help");
            helpTable.AddRow("[white]help[/]", "Display in-game adventure help");
            helpTable.AddRow("[white]clear[/]", "Clear the screen and scroll buffer");
            helpTable.AddRow("[white]classic[/]", "Toggle classic console mode");
            helpTable.AddRow("[white]intro[/]", "Display game information");
            helpTable.AddRow("[white]scroll[/]", "Toggle scrolling mode");
            helpTable.AddRow("[white]time[/]", "Display system date and time");
            helpTable.AddRow("[white]history[/]", "Show recent command history");
            helpTable.AddRow("[white]resign[/]", "Exit game");

            AnsiConsole.Write(new Panel(helpTable)
                .Header("[bold yellow]Console Commands[/]")
                .BorderColor(Color.Blue));

            // Command buffer help - USING ASCII ONLY (no Unicode arrows)
            var bufferHelp = new Panel(
                "[yellow]Enhanced Command Line:[/]\n" +
                "* [cyan]Up/Down[/] arrows: Navigate command history\n" +
                "* [cyan]Left/Right[/] arrows: Edit current command\n" +
                "* [cyan]ESC[/]: Clear current line\n" +
                "* [cyan]Enter[/]: Execute command")
                .Header("[bold green]Keyboard Shortcuts[/]")
                .BorderColor(Color.Yellow)
                .RoundedBorder();
            
            AnsiConsole.Write(bufferHelp);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        private static void DisplayGameState(GameMoveResult gameResult, bool useEnhanced = true)
        {
            if (!useEnhanced || UseClassicMode)
            {
                DisplayGameStateClassic(gameResult);
                return;
            }

            if (!ScrollMode) AnsiConsole.Clear();

            // Room header with fancy styling - NO SANITIZATION NEEDED
            var roomHeader = new Panel($"[bold yellow]{gameResult.RoomName}[/]")
                .BorderColor(Color.Yellow)
                .RoundedBorder();
            AnsiConsole.Write(roomHeader);

            // Room description with word wrapping - NO SANITIZATION NEEDED
            var descriptionPanel = new Panel(WrapTextForSpectre(gameResult.RoomMessage))
                .BorderColor(Color.Green)
                .RoundedBorder();
            AnsiConsole.Write(descriptionPanel);

            // Items section - NO SANITIZATION NEEDED
            if (!string.IsNullOrEmpty(gameResult.ItemsMessage) && gameResult.ItemsMessage != "No Items")
            {
                AnsiConsole.MarkupLine($"[bold white]You See:[/] [cyan]{gameResult.ItemsMessage}[/]");
            }

            // Health status with color coding
            var healthColor = gameResult.HealthReport.ToLower() switch
            {
                "great" => "green",
                "okay" => "yellow",
                "bad" => "orange1",
                "horrible" => "red",
                "dead" => "darkred",
                _ => "white"
            };
            
            AnsiConsole.MarkupLine($"[bold white]Health:[/] [{healthColor}]{gameResult.HealthReport}[/]");
            AnsiConsole.WriteLine();
        }

        private static void DisplayGameStateClassic(GameMoveResult gameResult)
        {
            if (!ScrollMode) Console.Clear();
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Room: {gameResult.RoomName}");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(WrapText(gameResult.RoomMessage));
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("You See: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(gameResult.ItemsMessage);
            Console.WriteLine();
        }

        private static string WrapTextForSpectre(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            // Spectre.Console handles wrapping automatically, but we can clean up the text
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        private static string WrapText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            
            var maxWidth = (int)(Console.WindowWidth * 0.9);
            var lines = new StringBuilder();
            
            foreach (var paragraph in text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(paragraph))
                {
                    lines.AppendLine();
                    continue;
                }
                
                var words = paragraph.Split(' ');
                var currentLine = new StringBuilder();
                
                foreach (var word in words)
                {
                    if (currentLine.Length + word.Length + 1 > maxWidth)
                    {
                        lines.AppendLine(currentLine.ToString());
                        currentLine.Clear();
                    }
                    
                    if (currentLine.Length > 0) currentLine.Append(' ');
                    currentLine.Append(word);
                }
                
                if (currentLine.Length > 0)
                    lines.AppendLine(currentLine.ToString());
            }
            
            return lines.ToString();
        }

        private static string GetUserInputWithHistory()
        {
            string input = string.Empty;
            int historyIndex = CommandHistory.Count;
            List<ConsoleKeyInfo> currentLine = new();
            int cursorPosition = 0;

            Console.Write("Next Action? > ");
            int startPosition = Console.CursorLeft;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        input = new string(currentLine.Select(k => k.KeyChar).ToArray());
                        
                        // Add to history if not empty and not duplicate of last command
                        if (!string.IsNullOrWhiteSpace(input) && 
                            (CommandHistory.Count == 0 || CommandHistory.Last() != input))
                        {
                            CommandHistory.Add(input);
                        }
                        return input;

                    case ConsoleKey.UpArrow:
                        if (historyIndex > 0)
                        {
                            historyIndex--;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (historyIndex < CommandHistory.Count - 1)
                        {
                            historyIndex++;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        else if (historyIndex == CommandHistory.Count - 1)
                        {
                            historyIndex++;
                            currentLine.Clear();
                            cursorPosition = 0;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorPosition < currentLine.Count)
                        {
                            cursorPosition++;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.Home:
                        cursorPosition = 0;
                        Console.SetCursorPosition(startPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.End:
                        cursorPosition = currentLine.Count;
                        Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.Escape:
                        currentLine.Clear();
                        cursorPosition = 0;
                        historyIndex = CommandHistory.Count;
                        RefreshLine(currentLine, startPosition, cursorPosition);
                        break;

                    case ConsoleKey.Backspace:
                        if (cursorPosition > 0)
                        {
                            currentLine.RemoveAt(cursorPosition - 1);
                            cursorPosition--;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.Delete:
                        if (cursorPosition < currentLine.Count)
                        {
                            currentLine.RemoveAt(cursorPosition);
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    default:
                        // Only accept printable ASCII characters (32-126)
                        if (!char.IsControl(keyInfo.KeyChar) && keyInfo.KeyChar >= 32 && keyInfo.KeyChar <= 126)
                        {
                            currentLine.Insert(cursorPosition, keyInfo);
                            cursorPosition++;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;
                }
            }
        }

        // Simpler version for enhanced mode
        private static string GetUserInputWithHistorySimple()
        {
            string input = string.Empty;
            int historyIndex = CommandHistory.Count;
            List<ConsoleKeyInfo> currentLine = new();
            int cursorPosition = 0;

            int startPosition = Console.CursorLeft;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        input = new string(currentLine.Select(k => k.KeyChar).ToArray());
                        
                        // Add to history if not empty and not duplicate of last command
                        if (!string.IsNullOrWhiteSpace(input) && 
                            (CommandHistory.Count == 0 || CommandHistory.Last() != input))
                        {
                            CommandHistory.Add(input);
                        }
                        return input;

                    case ConsoleKey.UpArrow:
                        if (historyIndex > 0)
                        {
                            historyIndex--;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (historyIndex < CommandHistory.Count - 1)
                        {
                            historyIndex++;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        else if (historyIndex == CommandHistory.Count - 1)
                        {
                            historyIndex++;
                            currentLine.Clear();
                            cursorPosition = 0;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorPosition < currentLine.Count)
                        {
                            cursorPosition++;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.Home:
                        cursorPosition = 0;
                        Console.SetCursorPosition(startPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.End:
                        cursorPosition = currentLine.Count;
                        Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.Escape:
                        currentLine.Clear();
                        cursorPosition = 0;
                        historyIndex = CommandHistory.Count;
                        RefreshLine(currentLine, startPosition, cursorPosition);
                        break;

                    case ConsoleKey.Backspace:
                        if (cursorPosition > 0)
                        {
                            currentLine.RemoveAt(cursorPosition - 1);
                            cursorPosition--;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.Delete:
                        if (cursorPosition < currentLine.Count)
                        {
                            currentLine.RemoveAt(cursorPosition);
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    default:
                        // Only accept printable ASCII characters (32-126)
                        if (!char.IsControl(keyInfo.KeyChar) && keyInfo.KeyChar >= 32 && keyInfo.KeyChar <= 126)
                        {
                            currentLine.Insert(cursorPosition, keyInfo);
                            cursorPosition++;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;
                }
            }
        }

        private static void LoadFromHistory(int historyIndex, ref List<ConsoleKeyInfo> currentLine, ref int cursorPosition, int startPosition)
        {
            if (historyIndex >= 0 && historyIndex < CommandHistory.Count)
            {
                currentLine.Clear();
                string historyCommand = CommandHistory[historyIndex];
                foreach (char c in historyCommand)
                {
                    currentLine.Add(new ConsoleKeyInfo(c, (ConsoleKey)c, false, false, false));
                }
                cursorPosition = currentLine.Count;
                RefreshLine(currentLine, startPosition, cursorPosition);
            }
        }

        private static void RefreshLine(List<ConsoleKeyInfo> currentLine, int startPosition, int cursorPosition)
        {
            try
            {
                // Clear the line
                Console.SetCursorPosition(startPosition, Console.CursorTop);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - startPosition - 1, 120)));
                Console.SetCursorPosition(startPosition, Console.CursorTop);
                
                // Write current line
                foreach (var keyInfo in currentLine)
                {
                    Console.Write(keyInfo.KeyChar);
                }
                
                // Set cursor position
                Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
            }
            catch
            {
                // If console operations fail, just continue
            }
        }

        private static string GetUserInput()
        {
            if (UseClassicMode)
            {
                Console.ForegroundColor = ConsoleColor.White;
                
                // Use enhanced input with command buffer
                var input = GetUserInputWithHistory();
                return input;
            }
            else
            {
                // For enhanced mode, use Spectre.Console prompt but then use our command buffer
                AnsiConsole.MarkupLine("[bold white]Next Action?[/]");
                Console.Write("> ");
                
                // Get input using our custom command buffer
                var input = GetUserInputWithHistorySimple();
                return input;
            }
        }

        private static void ShowLoadingProgress(Action gameInitAction)
        {
            if (UseClassicMode)
            {
                Console.WriteLine("Setting up your adventure...");
                gameInitAction();
                return;
            }

            // Use Spectre.Console's progress display
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("[green]Setting up your adventure...[/]");
                    
                    task.Increment(25);
                    Thread.Sleep(100); // Brief pause for visual effect
                    
                    task.Increment(25);
                    gameInitAction();
                    
                    task.Increment(50);
                    Thread.Sleep(100);
                });
        }

        public static void PlayAdventure(AdventureFrameworkService client)
        {
            string instanceID = string.Empty;
            string move = string.Empty;
            bool error = false;
            string errorMsg = string.Empty;

            // Initialize command buffer
            InitializeCommandBuffer();

            try
            {
                // Initialize with enhanced UI
                if (!UseClassicMode)
                {
                    AnsiConsole.Write(new Rule("[bold green]Initializing Adventure House[/]"));
                    DisplayIntroWithSpectre();
                }
                else
                {
                    DisplayIntroClassic();
                }

                // Setup game with progress indicator
                ShowLoadingProgress(() =>
                {
                    gmr = client.FrameWork_NewGame(1);
                });

                instanceID = gmr.InstanceID;
                error = false;
            }
            catch (Exception e)
            {
                errorMsg = e.ToString();
                if (UseClassicMode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(WrapText(errorMsg));
                }
                else
                {
                    AnsiConsole.WriteException(e);
                }
                return;
            }

            while (move != "resign")
            {
                // Handle console commands BEFORE sending to game engine
                switch (move.Trim().ToLower())
                {
                    case "chelp":
                    case "chlp": // Common typo
                        if (UseClassicMode) DisplayHelpClassic();
                        else DisplayHelpWithSpectre();
                        move = ""; // Clear the move so it doesn't get processed by game
                        continue; // Skip the rest of the loop
                        
                    case "time":
                        var timeText = $"Date and Time: {DateTime.Now:F}";
                        if (UseClassicMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(timeText);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"[bold green]{timeText}[/]");
                            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                            Console.ReadKey(true);
                        }
                        move = "";
                        continue;

                    case "intro":
                        if (UseClassicMode) DisplayIntroClassic();
                        else DisplayIntroWithSpectre();
                        move = "";
                        continue;

                    case "classic":
                        UseClassicMode = !UseClassicMode;
                        var mode = UseClassicMode ? "Classic" : "Enhanced";
                        if (UseClassicMode)
                        {
                            Console.Clear();
                            Console.WriteLine($"Switched to {mode} mode");
                        }
                        else
                        {
                            AnsiConsole.Clear();
                            AnsiConsole.MarkupLine($"[bold green]Switched to {mode} mode[/]");
                        }
                        move = "look";
                        break; // Continue to process as game command

                    case "clear":
                    case "cls":
                        if (UseClassicMode) Console.Clear();
                        else AnsiConsole.Clear();
                        move = "look";
                        break; // Continue to process as game command

                    case "scroll":
                        ScrollMode = !ScrollMode;
                        var scrollStatus = ScrollMode ? "enabled" : "disabled";
                        if (UseClassicMode)
                        {
                            Console.WriteLine($"Scrolling {scrollStatus}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"[yellow]Scrolling {scrollStatus}[/]");
                            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                            Console.ReadKey(true);
                        }
                        move = "";
                        continue;
                        
                    case "history":
                        // Show command history - NO SANITIZATION NEEDED
                        if (UseClassicMode)
                        {
                            Console.WriteLine("Command History:");
                            for (int i = Math.Max(0, CommandHistory.Count - 10); i < CommandHistory.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}: {CommandHistory[i]}");
                            }
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        else
                        {
                            var historyTable = new Table()
                                .BorderColor(Color.Blue)
                                .AddColumn("Command");
                            
                            for (int i = Math.Max(0, CommandHistory.Count - 10); i < CommandHistory.Count; i++)
                            {
                                historyTable.AddRow($"[cyan]{CommandHistory[i]}[/]");
                            }
                            
                            AnsiConsole.Write(new Panel(historyTable)
                                .Header("[bold yellow]Recent Command History[/]")
                                .BorderColor(Color.Blue));
                                
                            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                            Console.ReadKey(true);
                        }
                        move = "";
                        continue;
                }

                // Display game state
                DisplayGameState(gmr, !UseClassicMode);

                // Handle errors
                if (error)
                {
                    if (UseClassicMode)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Client Error:");
                        Console.WriteLine(WrapText(errorMsg));
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[bold red]Client Error:[/]");
                        AnsiConsole.WriteException(new Exception(errorMsg));
                    }
                    error = false;
                }

                // Get user input with enhanced command buffer support
                move = GetUserInput();
                if (string.IsNullOrEmpty(move)) move = "";
                if (move.Length > 100) move = move.Substring(0, 100);

                // Process move through game engine (if it's not a console command)
                if (!string.IsNullOrEmpty(move))
                {
                    try
                    {
                        gmr = client.FrameWork_GameMove(new GameMove() { InstanceID = instanceID, Move = move });
                    }
                    catch (Exception e)
                    {
                        error = true;
                        errorMsg = e.ToString();
                    }
                }
            }

            // Farewell message
            if (UseClassicMode)
            {
                Console.WriteLine("Thanks for playing Adventure House!");
            }
            else
            {
                AnsiConsole.Write(new Panel("[bold green]Thanks for playing Adventure House![/]")
                    .BorderColor(Color.Blue)
                    .RoundedBorder());
            }
        }

        private static void DisplayHelpClassic()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Console Commands:");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Chelp   - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Display this console commands help");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Help    - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Display in-game adventure help");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Clear   - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Clear the screen and scroll buffer");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Classic - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Toggle between classic and enhanced display modes");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Intro   - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Display Game Information");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Scroll  - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Toggle scrolling mode");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Time    - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Display System date and time");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("History - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Show recent command history");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Resign  - ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Exit Game");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Enhanced Command Line Features:");
            // USING ASCII ONLY - no Unicode bullets or arrows
            Console.WriteLine("* Up/Down arrows: Navigate command history");
            Console.WriteLine("* Left/Right arrows: Edit current command");
            Console.WriteLine("* Home/End: Jump to start/end of line");
            Console.WriteLine("* ESC: Clear current line");
            Console.WriteLine("* Backspace/Delete: Edit text");
            Console.WriteLine("* Enter: Execute command");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        // Enhanced pause with skip functionality for Spectre.Console mode
        private static void PauseWithSkip(int milliseconds, string message)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine(message);
            
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromMilliseconds(milliseconds);
            
            while (DateTime.Now - startTime < timeout)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        break; // Skip the remaining wait time
                    }
                    // Consume any other keys to prevent beeping
                }
                
                Thread.Sleep(50); // Small sleep to prevent excessive CPU usage
            }
            
            // Clear the pause message lines more safely
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop - 2);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, 120))); // Safe width limit
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, 120))); // Safe width limit
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            catch
            {
                // If cursor positioning fails, just move on
                Console.WriteLine();
            }
        }

        // Classic pause with skip functionality for Classic mode
        private static void PauseWithSkipClassic(int milliseconds, string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
            
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromMilliseconds(milliseconds);
            
            while (DateTime.Now - startTime < timeout)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        break; // Skip the remaining wait time
                    }
                    // Consume any other keys to prevent beeping
                }
                
                Thread.Sleep(50); // Small sleep to prevent excessive CPU usage
            }
            
            // Clear the pause message lines more safely
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop - 2);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, 120))); // Safe width limit
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, 120))); // Safe width limit
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            catch
            {
                // If cursor positioning fails, just move on
                Console.WriteLine();
            }
        }
    }
}
