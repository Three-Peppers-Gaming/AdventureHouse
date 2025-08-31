using AdventureHouse.Services.Models;
using AdventureServer.Services;
using AdventureServer.Interfaces;
using Spectre.Console;
using System.Text;

namespace AdventureHouse.Services.UI
{
    /// <summary>
    /// Display service that handles all UI rendering logic
    /// </summary>
    public class DisplayService : IDisplayService
    {
        private readonly IAppVersionService _appVersionService;

        public DisplayService(IAppVersionService appVersionService)
        {
            _appVersionService = appVersionService;
        }

        public void DisplayIntro(bool useClassicMode)
        {
            if (useClassicMode)
            {
                DisplayIntroClassic();
            }
            else
            {
                DisplayIntroWithSpectre();
            }
        }

        private void DisplayIntroWithSpectre()
        {
            AnsiConsole.Clear();
            
            // Create a fancy title
            var title = new FigletText(UIConfiguration.WelcomeTitle)
                .LeftJustified()
                .Color(UIConfiguration.InfoColor);
            AnsiConsole.Write(title);

            // Version and copyright info in a panel
            var infoPanel = new Panel($"[bold green]Version:[/] {_appVersionService.Version}\n" +
                            $"[bold yellow]{UIConfiguration.CopyrightNotice}[/]\n\n" +
                            $"[cyan]{UIConfiguration.GameDescription}[/]")
                .Header("[bold yellow]Game Information[/]")
                .BorderColor(UIConfiguration.PrimaryBorderColor)
                .RoundedBorder();
    
            AnsiConsole.Write(infoPanel);

            // Instructions - USING ASCII ONLY (no Unicode arrows or bullets)
            AnsiConsole.MarkupLine($"\n[bold red]ATTENTION:[/] [yellow]{UIConfiguration.AttentionMessage}[/]");
            AnsiConsole.WriteLine();
            
            foreach (var instruction in UIConfiguration.KeyboardInstructions)
            {
                AnsiConsole.MarkupLine($"[dim]{instruction}[/]");
            }
            AnsiConsole.WriteLine();

            // NOW pause after ALL intro content is displayed
            PauseWithSkip(UIConfiguration.DefaultPauseMilliseconds, $"[dim]{UIConfiguration.ContinueOrWaitPrompt}[/]");
        }

        private void DisplayIntroClassic()
        {
            Console.Clear();
            Console.ForegroundColor = UIConfiguration.ClassicHeaderColor;
            Console.Write(UIConfiguration.WelcomeTitle.ToUpper());
            Console.ForegroundColor = UIConfiguration.ClassicPrimaryColor;
            Console.WriteLine($" - {_appVersionService.Version}");
            Console.WriteLine();
            
            // Highlight copyright in bright yellow
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(UIConfiguration.CopyrightNotice);
            Console.WriteLine();
            
            Console.ForegroundColor = UIConfiguration.ClassicWarningColor;
            Console.Write("ATTENTION:");
            Console.ForegroundColor = UIConfiguration.ClassicAccentColor;
            Console.WriteLine($" {UIConfiguration.AttentionMessage}");
            Console.WriteLine();
            Console.ForegroundColor = UIConfiguration.ClassicTextColor;
            
            foreach (var instruction in UIConfiguration.KeyboardInstructions)
            {
                Console.WriteLine(instruction);
            }
            Console.WriteLine();
            
            // Game description at the bottom in cyan for better wrapping
            Console.ForegroundColor = UIConfiguration.ClassicInfoColor;
            Console.WriteLine(UIConfiguration.GameDescription);
            Console.WriteLine();
            
            // NOW pause after ALL intro content is displayed
            PauseWithSkipClassic(UIConfiguration.DefaultPauseMilliseconds, UIConfiguration.ContinueOrWaitPrompt);
        }

        public void DisplayHelp(bool useClassicMode)
        {
            if (useClassicMode)
            {
                DisplayHelpClassic();
            }
            else
            {
                DisplayHelpWithSpectre();
            }
        }

        private void DisplayHelpWithSpectre()
        {
            AnsiConsole.Clear();
            
            var helpTable = new Table()
                .BorderColor(UIConfiguration.PrimaryBorderColor)
                .AddColumn("[bold cyan]Command[/]")
                .AddColumn("[bold cyan]Description[/]");

            foreach (var (command, description) in UIConfiguration.ConsoleCommands)
            {
                helpTable.AddRow($"[white]{command}[/]", description);
            }

            AnsiConsole.Write(new Panel(helpTable)
                .Header("[bold yellow]Console Commands[/]")
                .BorderColor(UIConfiguration.SecondaryBorderColor));

            // Command buffer help - USING ASCII ONLY (no Unicode arrows)
            var bufferHelp = new Panel(
                "[yellow]Enhanced Command Line:[/]\n" +
                string.Join("\n", UIConfiguration.EnhancedCommandLineFeatures.Select(f => $"* [cyan]{f.Replace("* ", "")}[/]")))
                .Header("[bold green]Keyboard Shortcuts[/]")
                .BorderColor(UIConfiguration.AccentBorderColor)
                .RoundedBorder();
            
            AnsiConsole.Write(bufferHelp);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]{UIConfiguration.ContinuePrompt}[/]");
            Console.ReadKey(true);
        }

        private void DisplayHelpClassic()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Console Commands:");
            Console.WriteLine();
            
            foreach (var (command, description) in UIConfiguration.ConsoleCommands)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{command.PadRight(8)} - ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(description);
            }
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Enhanced Command Line Features:");
            foreach (var feature in UIConfiguration.EnhancedCommandLineFeatures)
            {
                Console.WriteLine(feature);
            }
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(UIConfiguration.ContinuePrompt);
            Console.ReadKey(true);
        }

        public void DisplayGameState(GameMoveResult gameResult, bool useEnhanced, bool scrollMode)
        {
            if (!useEnhanced)
            {
                DisplayGameStateClassic(gameResult, scrollMode);
                return;
            }

            if (!scrollMode) AnsiConsole.Clear();

            // Room header with fancy styling - NO SANITIZATION NEEDED
            var roomHeader = new Panel($"[bold yellow]{gameResult.RoomName}[/]")
                .BorderColor(UIConfiguration.AccentBorderColor)
                .RoundedBorder();
            AnsiConsole.Write(roomHeader);

            // Room description with word wrapping - NO SANITIZATION NEEDED
            var descriptionPanel = new Panel(WrapTextForSpectre(gameResult.RoomMessage))
                .BorderColor(UIConfiguration.PrimaryBorderColor)
                .RoundedBorder();
            AnsiConsole.Write(descriptionPanel);

            // Items section - NO SANITIZATION NEEDED
            if (!string.IsNullOrEmpty(gameResult.ItemsMessage) && gameResult.ItemsMessage != "No Items")
            {
                AnsiConsole.MarkupLine($"[bold white]You See:[/] [cyan]{gameResult.ItemsMessage}[/]");
            }

            // Health status with color coding
            var healthColor = UIConfiguration.GetHealthColor(gameResult.HealthReport);
            
            AnsiConsole.MarkupLine($"[bold white]Health:[/] [{healthColor}]{gameResult.HealthReport}[/]");
            AnsiConsole.WriteLine();
        }

        private void DisplayGameStateClassic(GameMoveResult gameResult, bool scrollMode)
        {
            if (!scrollMode) Console.Clear();
            
            Console.ForegroundColor = UIConfiguration.ClassicAccentColor;
            Console.WriteLine($"Room: {gameResult.RoomName}");
            Console.WriteLine();
            
            Console.ForegroundColor = UIConfiguration.ClassicPrimaryColor;
            Console.WriteLine(WrapText(gameResult.RoomMessage));
            
            Console.ForegroundColor = UIConfiguration.ClassicSecondaryColor;
            Console.Write("You See: ");
            Console.ForegroundColor = UIConfiguration.ClassicInfoColor;
            Console.WriteLine(gameResult.ItemsMessage);
            
            // Health status with color coding
            Console.ForegroundColor = UIConfiguration.ClassicSecondaryColor;
            Console.Write("Health: ");
            Console.ForegroundColor = UIConfiguration.GetHealthColorClassic(gameResult.HealthReport);
            Console.WriteLine(gameResult.HealthReport);
            Console.WriteLine();
        }

        public void DisplayMap(MapState? mapState, bool useClassicMode)
        {
            if (mapState == null) return;
            
            if (useClassicMode)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(mapState.GenerateCurrentLevelMap());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(mapState.GetMapLegend());
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(UIConfiguration.ContinuePrompt);
                Console.ReadKey(true);
            }
            else
            {
                AnsiConsole.Clear();
                
                // FIXED: Use current room name instead of hardcoded "Adventure House Map"
                var currentRoomName = mapState.GetCurrentRoomName().TrimEnd('.', '!');
                
                AnsiConsole.Write(new Panel(mapState.GenerateCurrentLevelMap())
                    .Header($"[bold yellow]{currentRoomName}[/]")
                    .BorderColor(Color.Green));
                AnsiConsole.MarkupLine($"[dim]{mapState.GetMapLegend()}[/]");
                AnsiConsole.MarkupLine($"[dim]{UIConfiguration.ContinuePrompt}[/]");
                Console.ReadKey(true);
            }
        }

        public void ShowLoadingProgress(Action gameInitAction, bool useClassicMode)
        {
            if (useClassicMode)
            {
                Console.WriteLine(UIConfiguration.SetupGameMessage);
                gameInitAction();
                return;
            }

            // Use Spectre.Console's progress display
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask($"[green]{UIConfiguration.SetupGameMessage}[/]");
                    
                    task.Increment(25);
                    Thread.Sleep(UIConfiguration.LoadingProgressDelay);
                    
                    task.Increment(25);
                    gameInitAction();
                    
                    task.Increment(50);
                    Thread.Sleep(UIConfiguration.LoadingProgressDelay);
                });
        }

        public void DisplayDateTime(bool useClassicMode)
        {
            var timeText = UIConfiguration.GetDateTimeDisplay();
            if (useClassicMode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(timeText);
                Console.WriteLine(UIConfiguration.ContinuePrompt);
                Console.ReadKey(true);
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold green]{timeText}[/]");
                AnsiConsole.MarkupLine($"[dim]{UIConfiguration.ContinuePrompt}[/]");
                Console.ReadKey(true);
            }
        }

        public void DisplayCommandHistory(List<string> commandHistory, bool useClassicMode)
        {
            if (useClassicMode)
            {
                Console.WriteLine("Command History:");
                for (int i = Math.Max(0, commandHistory.Count - UIConfiguration.MaxHistoryDisplay); i < commandHistory.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {commandHistory[i]}");
                }
                Console.WriteLine(UIConfiguration.ContinuePrompt);
                Console.ReadKey(true);
            }
            else
            {
                var historyTable = new Table()
                    .BorderColor(Color.Blue)
                    .AddColumn("Command");
                
                for (int i = Math.Max(0, commandHistory.Count - UIConfiguration.MaxHistoryDisplay); i < commandHistory.Count; i++)
                {
                    historyTable.AddRow($"[cyan]{commandHistory[i]}[/]");
                }
                
                AnsiConsole.Write(new Panel(historyTable)
                    .Header("[bold yellow]Recent Command History[/]")
                    .BorderColor(Color.Blue));
                    
                AnsiConsole.MarkupLine($"[dim]{UIConfiguration.ContinuePrompt}[/]");
                Console.ReadKey(true);
            }
        }

        public void DisplayFarewell(bool useClassicMode)
        {
            if (useClassicMode)
            {
                Console.WriteLine(UIConfiguration.GameExitMessage);
            }
            else
            {
                AnsiConsole.Write(new Panel($"[bold green]{UIConfiguration.GameExitMessage}[/]")
                    .BorderColor(Color.Blue)
                    .RoundedBorder());
            }
        }

        public void DisplayError(string errorMessage, bool useClassicMode)
        {
            if (useClassicMode)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{UIConfiguration.ClientErrorMessage}:");
                Console.WriteLine(WrapText(errorMessage));
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold red]{UIConfiguration.ClientErrorMessage}:[/]");
                AnsiConsole.WriteException(new Exception(errorMessage));
            }
        }

        public void ClearDisplay(bool useClassicMode)
        {
            if (useClassicMode)
            {
                Console.Clear();
            }
            else
            {
                AnsiConsole.Clear();
            }
        }

        #region Helper Methods

        private static string WrapTextForSpectre(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            // Spectre.Console handles wrapping automatically, but we can clean up the text
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        private static string WrapText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            
            var maxWidth = (int)(Console.WindowWidth * UIConfiguration.TextWrapRatio);
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
                
                Thread.Sleep(UIConfiguration.ConsoleRefreshDelay);
            }
            
            // Clear the pause message lines more safely
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop - 2);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, UIConfiguration.SafeConsoleWidth)));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, UIConfiguration.SafeConsoleWidth)));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            catch
            {
                // If cursor positioning fails, just move on
                Console.WriteLine();
            }
        }

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
                
                Thread.Sleep(UIConfiguration.ConsoleRefreshDelay);
            }
            
            // Clear the pause message lines more safely
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop - 2);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, UIConfiguration.SafeConsoleWidth)));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - 1, UIConfiguration.SafeConsoleWidth)));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            catch
            {
                // If cursor positioning fails, just move on
                Console.WriteLine();
            }
        }

        #endregion
    }
}