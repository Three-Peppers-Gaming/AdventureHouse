using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureClient.Models;
using AdventureHouse.Services.AdventureClient.AppVersion;
using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventureHouse.Services.AdventureClient.UI
{
    /// <summary>
    /// Display service that handles all UI rendering logic for the Adventure Client.
    /// Now supports both console modes (classic/Spectre.Console) and Terminal.Gui mode.
    /// </summary>
    public class DisplayService : IDisplayService
    {
        private readonly IAppVersionService _appVersionService;
        private readonly TerminalGuiRenderer _terminalGuiRenderer;
        private bool _useClassicMode = false;

        public DisplayService(IAppVersionService appVersionService)
        {
            _appVersionService = appVersionService;
            _terminalGuiRenderer = new TerminalGuiRenderer();
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
                Console.WriteLine("=== ADVENTURE HELP ===");
                Console.WriteLine("Available Commands:");
                Console.WriteLine("  Game Commands:");
                Console.WriteLine("    go <direction>  - Move (north, south, east, west, up, down)");
                Console.WriteLine("    n, s, e, w, u, d - Short directions");
                Console.WriteLine("    look            - Look around current room");
                Console.WriteLine("    get <item>      - Pick up an item");
                Console.WriteLine("    drop <item>     - Drop an item from inventory");
                Console.WriteLine("    inv             - Show your inventory");
                Console.WriteLine("    use <item>      - Use an item");
                Console.WriteLine("    eat <item>      - Eat food items");
                Console.WriteLine("    read <item>     - Read text items");
                Console.WriteLine("    pet <creature>  - Pet friendly creatures");
                Console.WriteLine("    attack <monster> with <weapon> - Combat");
                Console.WriteLine("    help            - Show game-specific help");
                Console.WriteLine("    quit            - Quit the game");
                Console.WriteLine("");
                Console.WriteLine("  Console Commands (client-side):");
                Console.WriteLine("    /help           - Show this help");
                Console.WriteLine("    /map            - Show current map");
                Console.WriteLine("    /time           - Show current time");
                Console.WriteLine("    /clear          - Clear screen");
                Console.WriteLine("    /classic        - Toggle classic/enhanced mode");
                Console.WriteLine("    resign          - Exit game");
                Console.WriteLine("");
                Console.WriteLine("Note: Type 'help' for game-specific story and hints!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            else
            {
                var helpPanel = new Panel(
                    "[bold]Game Commands:[/]\n" +
                    "[green]go <direction>[/] - Move (north, south, east, west, up, down)\n" +
                    "[green]n, s, e, w, u, d[/] - Short directions\n" +
                    "[green]look[/]           - Look around current room\n" +
                    "[green]get <item>[/]     - Pick up an item\n" +
                    "[green]drop <item>[/]    - Drop an item from inventory\n" +
                    "[green]inv[/]            - Show your inventory\n" +
                    "[green]use <item>[/]     - Use an item\n" +
                    "[green]eat <item>[/]     - Eat food items\n" +
                    "[green]read <item>[/]    - Read text items\n" +
                    "[green]pet <creature>[/] - Pet friendly creatures\n" +
                    "[green]attack <monster> with <weapon>[/] - Combat\n" +
                    "[green]help[/]           - Show game-specific help\n" +
                    "[green]quit[/]           - Quit the game\n\n" +
                    "[bold]Console Commands:[/]\n" +
                    "[yellow]/help[/]          - Show this help\n" +
                    "[yellow]/map[/]           - Show current map\n" +
                    "[yellow]/time[/]          - Show current time\n" +
                    "[yellow]/clear[/]         - Clear screen\n" +
                    "[yellow]/classic[/]       - Toggle classic/enhanced mode\n" +
                    "[yellow]resign[/]         - Exit game\n\n" +
                    "[dim]Note: Type 'help' for game-specific story and hints![/]")
                    .Header("Adventure Commands")
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
                // Compact classic display - status, message, items, then prompt
                Console.Clear(); // Clear screen for clean display
                
                // Status line
                Console.WriteLine($"Room: {gameResult.RoomName} | Health: {gameResult.HealthReport}");
                Console.WriteLine("=".PadRight(70, '='));
                
                // Room message (trim excessive whitespace)
                var message = gameResult.RoomMessage?.Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }
                
                // Items (if any)
                if (!string.IsNullOrEmpty(gameResult.ItemsMessage) && 
                    gameResult.ItemsMessage != "No Items" && 
                    !gameResult.ItemsMessage.Contains("nothing"))
                {
                    Console.WriteLine($"\nItems here: {gameResult.ItemsMessage}");
                }
                
                Console.WriteLine(); // Single blank line before prompt
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

        public void DisplayMap(string mapText, bool useClassicMode)
        {
            _useClassicMode = useClassicMode;
            
            if (useClassicMode)
            {
                Console.WriteLine(mapText);
                // No "Press any key to continue..." for maps in classic mode
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]{mapText.EscapeMarkup()}[/]");
                // No "Press any key to continue..." for maps in enhanced mode
            }
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
                // Check if it's markdown content (starts with # or contains table format)
                if (IsMarkdownContent(output))
                {
                    DisplayMarkdownContent(output);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]{output.EscapeMarkup()}[/]");
                }
                AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }

        private bool IsMarkdownContent(string content)
        {
            return content.StartsWith("#") || content.Contains("|---|") || content.Contains("**");
        }

        private void DisplayMarkdownContent(string markdown)
        {
            try
            {
                if (markdown.Contains("| Current Level Map | Legend & Guide |"))
                {
                    DisplayMapMarkdown(markdown);
                }
                else
                {
                    DisplayGeneralMarkdown(markdown);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error rendering markdown: {ex.Message.EscapeMarkup()}[/]");
                AnsiConsole.MarkupLine($"[yellow]{markdown.EscapeMarkup()}[/]");
            }
        }

        private void DisplayMapMarkdown(string markdown)
        {
            var lines = markdown.Split('\n', StringSplitOptions.None);
            var mapLines = new List<string>();
            var legendLines = new List<string>();
            bool inTable = false;
            bool headerPassed = false;

            // Parse header information
            string gameTitle = "";
            string currentLocation = "";
            string currentLevel = "";
            string roomsVisited = "";

            // Debug: Let's see what we're getting
            var debugInfo = new StringBuilder();
            debugInfo.AppendLine($"Total markdown lines: {lines.Length}");
            
            foreach (var line in lines.Take(10)) // First 10 lines for debug
            {
                debugInfo.AppendLine($"Line: '{line}'");
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("# "))
                {
                    gameTitle = line.Substring(2).Trim();
                }
                else if (line.StartsWith("**Current Location:**"))
                {
                    currentLocation = line.Replace("**Current Location:**", "").Trim();
                }
                else if (line.StartsWith("**Current Level:**"))
                {
                    currentLevel = line.Replace("**Current Level:**", "").Trim();
                }
                else if (line.StartsWith("**Rooms Visited:**"))
                {
                    roomsVisited = line.Replace("**Rooms Visited:**", "").Trim();
                }
                else if (line.Contains("| Current Level Map | Legend & Guide |"))
                {
                    inTable = true;
                    debugInfo.AppendLine("Found table header");
                    continue;
                }
                else if (line.Contains("|---|"))
                {
                    headerPassed = true;
                    debugInfo.AppendLine("Passed table separator");
                    continue;
                }
                else if (inTable && headerPassed && line.StartsWith("|") && line.EndsWith("|"))
                {
                    debugInfo.AppendLine($"Processing table row: '{line}'");
                    var parts = line.Split('|');
                    if (parts.Length >= 3) // Should be: "", mapContent, legendContent, ""
                    {
                        // Get map content (index 1) and legend content (index 2)
                        var mapPart = parts[1].Trim();
                        var legendPart = parts[2].Trim();
                        
                        debugInfo.AppendLine($"Map part: '{mapPart}', Legend part: '{legendPart}'");
                        
                        // Clean up the map content - remove backticks but preserve the actual map
                        if (mapPart.StartsWith("`") && mapPart.EndsWith("`"))
                        {
                            mapPart = mapPart.Substring(1, mapPart.Length - 2);
                        }
                        
                        mapLines.Add(mapPart);
                        legendLines.Add(legendPart);
                    }
                }
                else if (inTable && (line.Trim() == "" || line.StartsWith("---")))
                {
                    debugInfo.AppendLine("End of table detected");
                    break; // End of table
                }
            }

            // Display the formatted map
            AnsiConsole.Clear();

            // Header
            var rule = new Rule($"[bold blue]{gameTitle}[/]").RuleStyle("grey");
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();

            // Status information
            var statusTable = new Table().Border(TableBorder.None);
            statusTable.AddColumn("").Width(30);
            statusTable.AddColumn("").Width(50);
            statusTable.AddRow($"[bold]Location:[/] {currentLocation}", $"[bold]Level:[/] {currentLevel}");
            statusTable.AddRow($"[bold]Rooms Visited:[/] {roomsVisited}", "");
            AnsiConsole.Write(statusTable);
            AnsiConsole.WriteLine();

            // Main layout with map and legend side by side
            var layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Map").Ratio(2),  // Give map more space
                    new Layout("Legend").Ratio(1));

            // Map panel - preserve the ASCII art exactly as generated
            var mapContent = new StringBuilder();
            foreach (var mapLine in mapLines)
            {
                // Don't trim or modify the map content - preserve spacing
                if (!string.IsNullOrEmpty(mapLine) || mapLines.IndexOf(mapLine) < mapLines.Count - 1)
                {
                    mapContent.AppendLine(mapLine);
                }
            }

            // If no map content, show debug info
            if (mapContent.Length == 0)
            {
                mapContent.AppendLine("DEBUG: No map content found");
                mapContent.AppendLine($"Total table rows parsed: {mapLines.Count}");
                mapContent.AppendLine("Debug info:");
                mapContent.Append(debugInfo.ToString());
                
                // Also show first few lines of original markdown
                mapContent.AppendLine("\nFirst 10 lines of markdown:");
                for (int i = 0; i < Math.Min(10, lines.Length); i++)
                {
                    mapContent.AppendLine($"{i}: '{lines[i]}'");
                }
            }

            var mapPanel = new Panel(mapContent.ToString())
                .Header("[bold green]Current Level Map[/]")
                .BorderColor(Color.Green);
            layout["Map"].Update(mapPanel);

            // Legend panel
            var legendContent = new StringBuilder();
            foreach (var legendLine in legendLines)
            {
                if (!string.IsNullOrEmpty(legendLine.Trim()))
                {
                    legendContent.AppendLine(legendLine);
                }
            }

            // If no legend content, use a default
            if (legendContent.Length == 0)
            {
                legendContent.AppendLine("Map Legend:");
                legendContent.AppendLine("@ = Your location");
                legendContent.AppendLine("+ = Items here");
                legendContent.AppendLine(". = Connections");
            }

            var legendPanel = new Panel(legendContent.ToString())
                .Header("[bold yellow]Legend & Guide[/]")
                .BorderColor(Color.Yellow);
            layout["Legend"].Update(legendPanel);

            AnsiConsole.Write(layout);

            // Footer with symbols
            AnsiConsole.WriteLine();
            var symbolsPanel = new Panel(
                "[bold]Map Symbols:[/]\n" +
                "[cyan]@[/] = Your current location\n" +
                "[green]+[/] = Items available in room\n" +
                "[dim].[/] = Horizontal connections\n" +
                "[dim]:[/] = Vertical connections\n" +
                "[yellow]^[/] = Stairs/ladder going up\n" +
                "[yellow]v[/] = Stairs/ladder going down\n\n" +
                "[dim]Only visited rooms and connections are shown.[/]")
                .Header("[bold]Quick Reference[/]")
                .BorderColor(Color.Blue);
            AnsiConsole.Write(symbolsPanel);
        }

        private void DisplayGeneralMarkdown(string markdown)
        {
            var lines = markdown.Split('\n', StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (line.StartsWith("# "))
                {
                    // Main header
                    var title = line.Substring(2).Trim();
                    var rule = new Rule($"[bold blue]{title}[/]").RuleStyle("grey");
                    AnsiConsole.Write(rule);
                }
                else if (line.StartsWith("## "))
                {
                    // Sub header
                    var subtitle = line.Substring(3).Trim();
                    AnsiConsole.MarkupLine($"\n[bold yellow]{subtitle}[/]");
                }
                else if (line.StartsWith("| ") && line.Contains(" | "))
                {
                    // Table - collect all table lines and render as Spectre table
                    var tableLines = new List<string> { line };
                    continue; // We'll handle tables separately
                }
                else if (line.StartsWith("**") && line.EndsWith("**"))
                {
                    // Bold text
                    var text = line.Replace("**", "");
                    AnsiConsole.MarkupLine($"[bold]{text.EscapeMarkup()}[/]");
                }
                else if (line.StartsWith("- "))
                {
                    // Bullet point
                    var text = line.Substring(2).Trim();
                    AnsiConsole.MarkupLine($"  [dim]•[/] {text.EscapeMarkup()}");
                }
                else if (line.StartsWith("`") && line.EndsWith("`"))
                {
                    // Code
                    var code = line.Replace("`", "");
                    AnsiConsole.MarkupLine($"[grey]{code.EscapeMarkup()}[/]");
                }
                else if (line.Trim() == "---")
                {
                    // Horizontal rule
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(new Rule().RuleStyle("grey"));
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    // Regular text
                    var processed = ProcessInlineMarkdown(line);
                    AnsiConsole.MarkupLine(processed);
                }
                else
                {
                    // Empty line
                    AnsiConsole.WriteLine();
                }
            }
        }

        private string ProcessInlineMarkdown(string text)
        {
            // Handle inline code
            text = Regex.Replace(text, @"`([^`]+)`", "[grey]$1[/]");

            // Handle bold text
            text = Regex.Replace(text, @"\*\*([^*]+)\*\*", "[bold]$1[/]");

            // Escape any remaining markup
            text = text.Replace("[", "[[").Replace("]", "]]");

            // Unescape our processed markup
            text = text.Replace("[[grey]]", "[grey]").Replace("[[/grey]]", "[/]").Replace("[[bold]]", "[bold]").Replace("[[/bold]]", "[/]");

            return text;
        }

        private void DisplayIntroWithSpectre()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Adventure Realms").Centered().Color(Color.Green));
            AnsiConsole.MarkupLine("[bold yellow]Welcome to the Adventure![/]");
            AnsiConsole.WriteLine();
        }

        private void DisplayIntroClassic()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("Adventure Realms".PadLeft(42));
            Console.WriteLine("Welcome to the Adventure!".PadLeft(47));
            Console.WriteLine("=".PadRight(70, '='));
        }

        private int DisplayAdventureSelectionWithSpectre(List<Game> availableGames)
        {
            // First display a nice table with all game information
            var gamesTable = new Table();
            gamesTable.AddColumn("[bold]Game Name[/]").Width(30);
            gamesTable.AddColumn("[bold]Version[/]").Width(10);
            gamesTable.AddColumn("[bold]Description[/]").Width(60);

            foreach (var game in availableGames)
            {
                var description = game.Desc;
                // Wrap descriptions for better display
                if (description.Length > 80)
                {
                    // Simple word wrapping
                    var words = description.Split(' ');
                    var lines = new List<string>();
                    var currentLine = "";
                    
                    foreach (var word in words)
                    {
                        if ((currentLine + " " + word).Length > 80)
                        {
                            if (!string.IsNullOrEmpty(currentLine))
                                lines.Add(currentLine);
                            currentLine = word;
                        }
                        else
                        {
                            currentLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                        }
                    }
                    if (!string.IsNullOrEmpty(currentLine))
                        lines.Add(currentLine);
                    
                    description = string.Join("\n", lines);
                }
                
                gamesTable.AddRow(
                    $"[green]{game.Name.EscapeMarkup()}[/]",
                    $"[cyan]{game.Ver.EscapeMarkup()}[/]",
                    $"[yellow]{description.EscapeMarkup()}[/]"
                );
            }

            var panel = new Panel(gamesTable)
                .Header("[bold blue]Available Adventures[/]")
                .BorderColor(Color.Blue);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            // Then use a simple selector with just game names
            var gameNames = availableGames.Select(g => g.Name).ToArray();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Choose your adventure:[/]")
                    .AddChoices(gameNames));
            
            var selectedGame = availableGames.First(g => g.Name == selection);
            return selectedGame.Id;
        }

        private int DisplayAdventureSelectionClassic(List<Game> availableGames)
        {
            Console.WriteLine("\nAvailable Adventures:");
            Console.WriteLine("=".PadRight(100, '='));
            
            // Display table header
            Console.WriteLine($"{"#",-3} {"Game Name",-35} {"Version",-10} {"Description"}");
            Console.WriteLine("-".PadRight(100, '-'));
            
            // Display games in table format
            for (int i = 0; i < availableGames.Count; i++)
            {
                var description = availableGames[i].Desc;
                // Wrap long descriptions
                if (description.Length > 50)
                {
                    description = description.Substring(0, 47) + "...";
                }
                Console.WriteLine($"{i + 1,-3} {availableGames[i].Name,-35} {availableGames[i].Ver,-10} {description}");
            }
            
            Console.WriteLine("=".PadRight(100, '='));
            Console.Write("\nSelect adventure (1-" + availableGames.Count + "): ");

            if (int.TryParse(Console.ReadLine(), out int selection) &&
                selection >= 1 && selection <= availableGames.Count)
            {
                return availableGames[selection - 1].Id;
            }
            return availableGames[0].Id; // Default to first game
        }

        /// <summary>
        /// Create a Terminal.Gui map view from MapModel data.
        /// This bridges the gap between the existing DisplayService and Terminal.Gui rendering.
        /// </summary>
        public Terminal.Gui.FrameView CreateTerminalGuiMapView(MapModel mapModel, Terminal.Gui.Rect bounds)
        {
            return _terminalGuiRenderer.CreateMapView(mapModel, bounds);
        }

        /// <summary>
        /// Create a Terminal.Gui status view from game data.
        /// </summary>
        public Terminal.Gui.FrameView CreateTerminalGuiStatusView(MapModel mapModel, string healthStatus, Terminal.Gui.Rect bounds)
        {
            return _terminalGuiRenderer.CreateStatusView(mapModel, healthStatus, bounds);
        }

        /// <summary>
        /// Create a Terminal.Gui legend view.
        /// </summary>
        public Terminal.Gui.FrameView CreateTerminalGuiLegendView(MapModel mapModel, Terminal.Gui.Rect bounds)
        {
            return _terminalGuiRenderer.CreateLegendView(mapModel, bounds);
        }

        /// <summary>
        /// Update an existing Terminal.Gui map view with new data.
        /// </summary>
        public void UpdateTerminalGuiMapView(Terminal.Gui.FrameView mapView, MapModel mapModel)
        {
            _terminalGuiRenderer.UpdateMapView(mapView, mapModel);
        }

        /// <summary>
        /// Update an existing Terminal.Gui status view with new data.
        /// </summary>
        public void UpdateTerminalGuiStatusView(Terminal.Gui.FrameView statusView, MapModel mapModel, string healthStatus)
        {
            _terminalGuiRenderer.UpdateStatusView(statusView, mapModel, healthStatus);
        }

        /// <summary>
        /// Render map to string for Terminal.Gui TextView display.
        /// </summary>
        public string RenderMapToString(MapModel mapModel, int width = 80, int height = 40)
        {
            return _terminalGuiRenderer.RenderMapToString(mapModel, width, height);
        }
    }
}
