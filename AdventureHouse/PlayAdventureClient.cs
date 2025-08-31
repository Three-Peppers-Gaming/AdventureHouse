using AdventureHouse.Services;
using AdventureHouse.Services.Models;
using AdventureServer.Interfaces;
using AdventureServer.Services;
using AdventureHouse.Services.Data.AdventureData;
using AdventurHouse.Services;
using Spectre.Console;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AdventureHouse.Services.UI;
using AdventureHouse.Services.Input;
using AdventureHouse.Services.GameManagement;
using AdventureHouse.Services.Commands;

namespace AdventureHouse
{
    /// <summary>
    /// Main game client that orchestrates the adventure game flow
    /// Now uses a service-based architecture for better separation of concerns
    /// </summary>
    internal class PlayAdventureClient
    {
        private static GameMoveResult gmr = new(); // Initialize to avoid CS8618
        
        private static bool UseClassicMode = false;
        private static bool ScrollMode = false;
        
        // Services
        private static IDisplayService? _displayService;
        private static IInputService? _inputService;
        private static IGameStateService? _gameStateService;
        private static IConsoleCommandService? _consoleCommandService;
        private static AdventurHouse.Services.IPlayAdventure? _gameClient;
        private static IGameInstanceService? _gameInstanceService;

        /// <summary>
        /// Main entry point for playing the adventure game
        /// </summary>
        /// <param name="client">The game client interface</param>
        public static void PlayAdventure(AdventurHouse.Services.IPlayAdventure client)
        {
            string instanceID = string.Empty;
            string move = string.Empty;
            bool error = false;
            string errorMsg = string.Empty;

            // Store client reference
            _gameClient = client;

            // Initialize services
            InitializeServices();

            try
            {
                // Initialize with enhanced UI
                _displayService!.DisplayIntro(UseClassicMode);

                // Setup game with progress indicator
                _displayService!.ShowLoadingProgress(() =>
                {
                    gmr = _gameStateService!.InitializeGame(client, 1);
                }, UseClassicMode);

                instanceID = gmr.InstanceID;
                error = false;
            }
            catch (Exception e)
            {
                error = true;
                errorMsg = e.ToString();
                _displayService!.DisplayError(errorMsg, UseClassicMode);
                return;
            }

            // Main game loop
            while (move != "resign")
            {
                // Handle console commands BEFORE sending to game engine
                var normalizedMove = move.Trim().ToLower().TrimStart('/');
                
                var (wasProcessed, commandResult) = _consoleCommandService!.ProcessConsoleCommand(
                    normalizedMove,
                    _displayService!,
                    _inputService!,
                    _gameStateService!,
                    UseClassicMode,
                    ScrollMode);

                if (wasProcessed)
                {
                    // Handle the result of console command processing
                    switch (commandResult.Action)
                    {
                        case ConsoleCommandAction.Continue:
                            move = ""; // Clear the move so it doesn't get processed by game
                            continue; // Skip the rest of the loop
                            
                        case ConsoleCommandAction.ProcessAsGameCommand:
                            move = commandResult.GameCommand ?? "";
                            break; // Continue to process as game command
                            
                        case ConsoleCommandAction.ToggleMode:
                            if (commandResult.ToggleClassicMode)
                            {
                                UseClassicMode = !UseClassicMode;
                                var mode = UseClassicMode ? "Classic" : "Enhanced";
                                if (UseClassicMode)
                                {
                                    Console.WriteLine($"Switched to {mode} mode");
                                }
                                else
                                {
                                    AnsiConsole.Clear();
                                    AnsiConsole.MarkupLine($"[bold green]Switched to {mode} mode[/]");
                                }
                                move = commandResult.GameCommand ?? "look";
                            }
                            
                            if (commandResult.ToggleScrollMode)
                            {
                                ScrollMode = !ScrollMode;
                                move = "";
                                continue;
                            }
                            break; // Continue to process as game command
                    }
                }

                // Display game state
                _displayService!.DisplayGameState(gmr, !UseClassicMode, ScrollMode);

                // UPDATE MAP STATE based on current game state
                _gameStateService!.UpdateMapState(gmr);

                // Handle errors
                if (error)
                {
                    _displayService!.DisplayError(errorMsg, UseClassicMode);
                    error = false;
                }

                // Get user input with enhanced command buffer support
                move = _inputService!.GetUserInput(UseClassicMode);
                if (string.IsNullOrEmpty(move)) move = "";
                if (move.Length > UIConfiguration.MaxCommandLength) 
                    move = move.Substring(0, UIConfiguration.MaxCommandLength);

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
            _displayService!.DisplayFarewell(UseClassicMode);
        }

        /// <summary>
        /// Initialize all services used by the game client
        /// </summary>
        private static void InitializeServices()
        {
            // Create service instances
            var appVersionService = new AdventureServer.Services.AppVersionService();
            
            _displayService = new DisplayService(appVersionService);
            _inputService = new InputService();
            _gameStateService = new GameStateService();
            _consoleCommandService = new ConsoleCommandService();

            // Initialize command buffer
            _inputService.InitializeCommandBuffer();
        }
    }
}
