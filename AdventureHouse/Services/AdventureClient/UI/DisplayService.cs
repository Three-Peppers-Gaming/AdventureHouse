using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureClient.Models;
using AdventureHouse.Services.AdventureClient.AppVersion;
using Spectre.Console;
using System.Text;

namespace AdventureHouse.Services.AdventureClient.UI
{
    /// <summary>
    /// Display service that handles all UI rendering logic for the Adventure Client
    /// </summary>
    public class DisplayService : IDisplayService
    {
        private readonly IAppVersionService _appVersionService;
        private bool _useClassicMode = false;

        public DisplayService(IAppVersionService appVersionService)
        {
            _appVersionService = appVersionService;
        }

        public void DisplayIntro(bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                DisplayIntroClassic();
            }
            else
            {
                DisplayIntroWithSpectre();
            }
        }

        public int DisplayAdventureSelection(List<Game> availableGames, bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                return DisplayAdventureSelectionClassic(availableGames);
            }
            else
            {
                return DisplayAdventureSelectionWithSpectre(availableGames);
            }
        }

        public void DisplayHelp(bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.WriteLine("=== ADVENTURE HOUSE HELP ===");
                Console.WriteLine("Commands:");
                Console.WriteLine("  go <direction> - Move in a direction (north, south, east, west, up, down)");
                Console.WriteLine("  look          - Look around");
                Console.WriteLine("  get <item>    - Pick up an item");
                Console.WriteLine("  drop <item>   - Drop an item");
                Console.WriteLine("  inv           - Show inventory");
                Console.WriteLine("  help          - Show this help");
                Console.WriteLine("  quit          - Quit the game");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            else
            {
                var helpPanel = new Panel(
                    "[bold]Commands:[/]\n" +
                    "[green]go <direction>[/] - Move in a direction (north, south, east, west, up, down)\n" +
                    "[green]look[/]          - Look around\n" +
                    "[green]get <item>[/]    - Pick up an item\n" +
                    "[green]drop <item>[/]   - Drop an item\n" +
                    "[green]inv[/]           - Show inventory\n" +
                    "[green]help[/]          - Show this help\n" +
                    "[green]quit[/]          - Quit the game")
                    .Header("Adventure House Help")
                    .BorderColor(Color.Blue);
                
                AnsiConsole.Write(helpPanel);
                AnsiConsole.MarkupLine("\n[dim]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }

        public void DisplayGameState(GameMoveResult gameResult, bool useEnhanced, bool scrollMode)
        {
            if (_useClassicMode)
            {
                Console.WriteLine("\n" + "=".PadRight(70, '='));
                Console.WriteLine($"Room: {gameResult.RoomName}");
                Console.WriteLine($"Health: {gameResult.HealthReport}");
                Console.WriteLine("=".PadRight(70, '='));
                Console.WriteLine(gameResult.RoomMessage);
                
                if (!string.IsNullOrEmpty(gameResult.ItemsMessage))
                {
                    Console.WriteLine($"\nItems here: {gameResult.ItemsMessage}");
                }
            }
            else
            {
                if (!scrollMode)
                {
                    AnsiConsole.Clear();
                }

                // Enhanced display with panels
                var layout = new Layout("Root")
                    .SplitRows(
                        new Layout("Header").Size(3),
                        new Layout("Content").Size(20),
                        new Layout("Items").Size(3));

                // Header panel
                var headerPanel = new Panel($"[bold]{gameResult.RoomName}[/] | Health: [green]{gameResult.HealthReport}[/]")
                    .Header("Adventure Status")
                    .BorderColor(Color.Blue);
                layout["Header"].Update(headerPanel);

                // Content panel
                var contentPanel = new Panel(gameResult.RoomMessage)
                    .Header("Location")
                    .BorderColor(Color.Green);
                layout["Content"].Update(contentPanel);

                // Items panel
                var itemsText = string.IsNullOrEmpty(gameResult.ItemsMessage) ? "[dim]No items here[/]" : gameResult.ItemsMessage;
                var itemsPanel = new Panel(itemsText)
                    .Header("Items")
                    .BorderColor(Color.Yellow);
                layout["Items"].Update(itemsPanel);

                AnsiConsole.Write(layout);
            }
        }

        public void DisplayMap(MapState? mapState, bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            DisplayMessage("Map display functionality would be implemented here based on mapState");
        }

        public void ShowLoadingProgress(Action gameInitAction, bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.WriteLine("Loading game...");
                gameInitAction();
                Console.WriteLine("Game loaded!");
            }
            else
            {
                AnsiConsole.Status()
                    .Start("Loading game...", ctx =>
                    {
                        gameInitAction();
                        ctx.Status("Game loaded!");
                        Thread.Sleep(500);
                    });
            }
        }

        public void DisplayDateTime(bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            var currentTime = DateTime.Now;
            
            if (useClassicMode)
            {
                Console.WriteLine($"Current time: {currentTime:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            else
            {
                var timePanel = new Panel($"[bold]{currentTime:yyyy-MM-dd HH:mm:ss}[/]")
                    .Header("Current Time")
                    .BorderColor(Color.Blue);
                AnsiConsole.Write(timePanel);
                AnsiConsole.MarkupLine("\n[dim]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }

        public void DisplayCommandHistory(List<string> commandHistory, bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.WriteLine("\nCommand History:");
                for (int i = Math.Max(0, commandHistory.Count - 10); i < commandHistory.Count; i++)
                {
                    Console.WriteLine($"{i + 1:D2}: {commandHistory[i]}");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[bold]Command History:[/]");
                var recentHistory = commandHistory.TakeLast(10);
                foreach (var cmd in recentHistory)
                {
                    AnsiConsole.MarkupLine($"  [dim]>[/] {cmd.EscapeMarkup()}");
                }
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public void DisplayFarewell(bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.WriteLine("\nThank you for playing Adventure House!");
                Console.WriteLine("Press any key to exit...");
            }
            else
            {
                AnsiConsole.MarkupLine("\n[bold green]Thank you for playing Adventure House![/]");
                AnsiConsole.MarkupLine("[dim]Press any key to exit...[/]");
            }
            Console.ReadKey(true);
        }

        public void DisplayError(string errorMessage, bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.WriteLine($"ERROR: {errorMessage}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]ERROR: {errorMessage.EscapeMarkup()}[/]");
            }
        }

        public void ClearDisplay(bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.Clear();
            }
            else
            {
                AnsiConsole.Clear();
            }
        }

        public void DisplayMessage(string message)
        {
            if (_useClassicMode)
            {
                Console.WriteLine(message);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]{message.EscapeMarkup()}[/]");
            }
        }

        public void DisplayConsoleOutput(string output)
        {
            if (_useClassicMode)
            {
                Console.WriteLine(output);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]{output.EscapeMarkup()}[/]");
                AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }

        private void DisplayIntroWithSpectre()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Adventure House").Centered().Color(Color.Green));
            AnsiConsole.MarkupLine("[bold yellow]Welcome to the Adventure![/]");
            AnsiConsole.WriteLine();
        }

        private void DisplayIntroClassic()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("Adventure House".PadLeft(42));
            Console.WriteLine("Welcome to the Adventure!".PadLeft(47));
            Console.WriteLine("=".PadRight(70, '='));
        }

        private int DisplayAdventureSelectionWithSpectre(List<Game> availableGames)
        {
            var gameChoices = availableGames.Select(g => $"{g.Name} v{g.Ver} - {g.Desc}").ToArray();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Choose your adventure:[/]")
                    .AddChoices(gameChoices));
            
            var selectedIndex = Array.IndexOf(gameChoices, selection);
            return availableGames[selectedIndex].Id;
        }

        private int DisplayAdventureSelectionClassic(List<Game> availableGames)
        {
            Console.WriteLine("\nAvailable Adventures:");
            for (int i = 0; i < availableGames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableGames[i].Name} v{availableGames[i].Ver}");
                Console.WriteLine($"   {availableGames[i].Desc}");
            }
            Console.Write("\nSelect adventure (1-" + availableGames.Count + "): ");
            
            if (int.TryParse(Console.ReadLine(), out int selection) && 
                selection >= 1 && selection <= availableGames.Count)
            {
                return availableGames[selection - 1].Id;
            }
            return availableGames[0].Id; // Default to first game
        }
    }
}