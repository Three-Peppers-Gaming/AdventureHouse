using AdventureHouse.Services.UI;
using AdventureHouse.Services.Input;
using AdventureHouse.Services.GameManagement;

namespace AdventureHouse.Services.Commands
{
    /// <summary>
    /// Console command service that processes console-specific commands
    /// </summary>
    public class ConsoleCommandService : IConsoleCommandService
    {
        public (bool wasProcessed, ConsoleCommandResult result) ProcessConsoleCommand(
            string normalizedCommand,
            IDisplayService displayService,
            IInputService inputService,
            IGameStateService gameStateService,
            bool useClassicMode,
            bool scrollMode)
        {
            // STRIP FORWARD SLASHES and normalize commands
            var command = normalizedCommand.Trim().ToLower().TrimStart('/');
            
            switch (command)
            {
                case "chelp":
                case "help":  // Now both chelp and /help work
                case "chlp": // Common typo
                    displayService.DisplayHelp(useClassicMode);
                    return (true, ConsoleCommandResult.Continue());
                    
                case "map":  // Now both map and /map work
                    displayService.DisplayMap(gameStateService.MapState, useClassicMode);
                    return (true, ConsoleCommandResult.Continue());
                    
                case "time":
                    displayService.DisplayDateTime(useClassicMode);
                    return (true, ConsoleCommandResult.Continue());

                case "intro":
                    displayService.DisplayIntro(useClassicMode);
                    return (true, ConsoleCommandResult.Continue());

                case "classic":
                    var mode = useClassicMode ? "Enhanced" : "Classic";
                    if (!useClassicMode) // About to switch TO classic
                    {
                        displayService.ClearDisplay(false); // Clear with current mode
                        Console.WriteLine($"Switched to Classic mode");
                    }
                    else // About to switch TO enhanced
                    {
                        Console.Clear();
                        displayService.ClearDisplay(true);
                        displayService.ClearDisplay(false);
                        // The message will be handled by the caller after mode switch
                    }
                    return (true, ConsoleCommandResult.ToggleClassic());

                case "clear":
                case "cls":
                    displayService.ClearDisplay(useClassicMode);
                    return (true, ConsoleCommandResult.ClearAndLook());

                case "scroll":
                    var scrollStatus = scrollMode ? "disabled" : "enabled";
                    if (useClassicMode)
                    {
                        Console.WriteLine($"Scrolling {scrollStatus}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        displayService.ClearDisplay(false);
                        // Use AnsiConsole for enhanced mode message
                        Spectre.Console.AnsiConsole.MarkupLine($"[yellow]Scrolling {scrollStatus}[/]");
                        Spectre.Console.AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                        Console.ReadKey(true);
                    }
                    return (true, ConsoleCommandResult.ToggleScroll());
                    
                case "history":
                    // Show command history - NO SANITIZATION NEEDED
                    displayService.DisplayCommandHistory(inputService.CommandHistory, useClassicMode);
                    return (true, ConsoleCommandResult.Continue());

                default:
                    // Command not processed by console command service
                    return (false, ConsoleCommandResult.Continue());
            }
        }
    }
}