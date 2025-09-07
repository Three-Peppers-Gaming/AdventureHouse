using System;
using System.Linq;
using System.Text;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.AdventureClient.UI;
using AdventureRealms.Services.AdventureClient.Input;
using AdventureRealms.Services.AdventureClient.AppVersion;

namespace AdventureRealms.Services.AdventureClient
{
    /// <summary>
    /// The main Adventure Client service that handles all user interaction
    /// This is completely separate from the Adventure Server which handles game logic
    /// </summary>
    public class AdventureClientService : IAdventureClient
    {
        private readonly IDisplayService _displayService;
        private readonly IInputService _inputService;
        private readonly IAppVersionService _appVersionService;
        private bool _useClassicMode = false;
        private bool _useScrollMode = false;
        private string _currentInstanceId = string.Empty;

        public AdventureClientService()
        {
            // Initialize client services
            _appVersionService = new AppVersionService();
            _displayService = new DisplayService(_appVersionService);
            _inputService = new InputService();
        }

        /// <summary>
        /// Start the adventure game using console interface.
        /// Provides a simple text-based interface for playing adventure games.
        /// </summary>
        public void StartAdventure(IPlayAdventure adventureServer)
        {
            try
            {
                _displayService.DisplayIntro(_useClassicMode);
                
                // Get available games using new clean API
                var games = adventureServer.GameList();
                int selectedGameId = GetGameSelection(games);
                if (selectedGameId == -1) return;

                // Start game session using new clean API
                var newGameRequest = new AdventureRealms.Services.Shared.Models.GamePlayRequest 
                { 
                    SessionId = "", 
                    GameId = selectedGameId, 
                    Command = "",
                    UseClassicMode = _useClassicMode,
                    UseScrollMode = _useScrollMode
                };
                var gameResult = adventureServer.PlayGame(newGameRequest);
                
                if (gameResult == null || gameResult.SessionId == "-1")
                {
                    _displayService.DisplayMessage("Failed to start game session.");
                    return;
                }

                _currentInstanceId = gameResult.SessionId;
                
                // Display initial game state
                DisplayGamePlayResponse(gameResult);

                // Main game loop
                string input;
                do
                {
                    input = _inputService.GetUserInput(_useClassicMode);
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    // Handle resignation
                    if (input.ToLower().Trim() == "resign" || input.ToLower().Trim() == "/resign")
                    {
                        _displayService.DisplayMessage("Game Resigned\n\nThank you for playing Adventure Realms!");
                        break;
                    }

                    // Handle console commands locally - don't send to server
                    if (ProcessConsoleCommands(input)) continue;

                    // Handle map command locally  
                    if (input.ToLower().Trim() == "map" || input.ToLower().Trim() == "/map")
                    {
                        if (gameResult != null && gameResult.MapData != null)
                        {
                            var mapText = GenerateConsoleMapText(gameResult.MapData);
                            _displayService.DisplayMap(mapText, _useClassicMode);
                        }
                        continue;
                    }

                    // Send command using new clean API
                    var commandRequest = new AdventureRealms.Services.Shared.Models.GamePlayRequest
                    {
                        SessionId = _currentInstanceId,
                        Command = input,
                        UseClassicMode = _useClassicMode,
                        UseScrollMode = _useScrollMode
                    };

                    var result = adventureServer.PlayGame(commandRequest);
                    DisplayGamePlayResponse(result);

                    if (result.SessionId == "-1" || result.GameCompleted || result.PlayerDead) break;

                } while (input.ToLower() != "quit" && input.ToLower() != "resign");

                _displayService.DisplayMessage("Thanks for playing Adventure Realms!");
            }
            catch (Exception ex)
            {
                _displayService.DisplayMessage($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Get game selection from user
        /// </summary>
        private int GetGameSelection(List<Game> games)
        {
            if (games == null || games.Count == 0)
            {
                _displayService.DisplayMessage("No games available.");
                return -1;
            }

            return _displayService.DisplayAdventureSelection(games, _useClassicMode);
        }

        /// <summary>
        /// Display the results from the new GamePlayResponse
        /// </summary>
        private void DisplayGamePlayResponse(AdventureRealms.Services.Shared.Models.GamePlayResponse response)
        {
            // Show welcome message popup if this is a new game session
            if (!string.IsNullOrEmpty(response.WelcomeMessage))
            {
                _displayService.DisplayWelcomeMessage(response.WelcomeMessage, response.GameName, _useClassicMode);
            }

            // For new game sessions, add client instructions locally
            var commandResponse = response.CommandResponse;
            if (string.IsNullOrEmpty(commandResponse) && !string.IsNullOrEmpty(response.SessionId) && response.SessionId != "-1")
            {
                // This appears to be a new game session - add client instructions
                commandResponse = _useClassicMode 
                    ? "Type 'help' for game commands or '/help' for console commands."
                    : "Welcome to Adventure Realms! Type 'help' for game commands or '/help' for console commands.";
            }

            // Convert to legacy GameMoveResult format for existing display logic
            // Combine room description and command response, but avoid double spacing
            var roomMessage = response.RoomDescription?.Trim() ?? "";
            if (!string.IsNullOrEmpty(commandResponse))
            {
                roomMessage = string.IsNullOrEmpty(roomMessage) 
                    ? commandResponse 
                    : roomMessage + "\n\n" + commandResponse;
            }

            var legacyResult = new GameMoveResult
            {
                InstanceID = response.SessionId,
                RoomName = response.RoomName,
                RoomMessage = roomMessage,
                ItemsMessage = response.ItemsInRoom,
                HealthReport = response.PlayerHealth,
                PlayerName = response.PlayerName,
                MapData = response.MapData
            };

            DisplayGameResult(legacyResult);
        }

        /// <summary>
        /// Display the game result to the user
        /// </summary>
        private void DisplayGameResult(GameMoveResult result)
        {
            if (result == null) return;

            // Handle special client actions first
            HandleClientActions(result);

            // Display the main game state
            _displayService.DisplayGameState(result, !_useClassicMode, _useScrollMode);
        }

        /// <summary>
        /// Process console commands that don't need to go to the server
        /// </summary>
        private bool ProcessConsoleCommands(string input)
        {
            var cmd = input.ToLower().Trim();

            switch (cmd)
            {
                case "/help":
                case "chelp":
                    _displayService.DisplayHelp(_useClassicMode);
                    return true;

                case "clear":
                case "/clear":
                    _displayService.ClearDisplay(_useClassicMode);
                    return true;

                case "time":
                case "/time":
                    _displayService.DisplayDateTime(_useClassicMode);
                    return true;

                case "classic":
                case "/classic":
                    _useClassicMode = !_useClassicMode;
                    var mode = _useClassicMode ? "Classic" : "Enhanced";
                    _displayService.DisplayMessage($"Switched to {mode} mode");
                    return true;

                default:
                    return false; // Let server handle this command including "help"
            }
        }

        /// <summary>
        /// Generate simple text representation of map data for console display
        /// </summary>
        private string GenerateConsoleMapText(PlayerMapData mapData)
        {
            if (mapData == null || mapData.DiscoveredRooms.Count == 0)
            {
                return "No map data available.";
            }

            var mapText = new StringBuilder();
            mapText.AppendLine($"Map for {mapData.RenderingConfig.GameName} - {mapData.CurrentLevelDisplayName}");
            mapText.AppendLine($"Current Location: {mapData.DiscoveredRooms.FirstOrDefault(r => r.IsCurrentLocation)?.Name ?? "Unknown"}");
            mapText.AppendLine($"Rooms Visited: {mapData.VisitedRoomCount}");
            mapText.AppendLine();
            
            // Simple list of discovered rooms
            mapText.AppendLine("Discovered Rooms:");
            foreach (var room in mapData.DiscoveredRooms.OrderBy(r => r.RoomNumber))
            {
                var marker = room.IsCurrentLocation ? "@ " : "  ";
                var items = room.HasItems ? " [+]" : "";
                mapText.AppendLine($"{marker}{room.Name} ({room.RoomNumber}){items}");
            }
            
            return mapText.ToString();
        }

        /// <summary>
        /// Handle special client actions from the server
        /// </summary>
        private void HandleClientActions(GameMoveResult result)
        {
            // Handle mode toggles
            if (result.ToggleClassicMode)
            {
                _useClassicMode = !_useClassicMode;
                var mode = _useClassicMode ? "Classic" : "Enhanced";
                _displayService.DisplayMessage($"Switched to {mode} mode");
            }

            if (result.ToggleScrollMode)
            {
                _useScrollMode = !_useScrollMode;
                var status = _useScrollMode ? "enabled" : "disabled";
                _displayService.DisplayMessage($"Scrolling {status}");
            }

            // Handle display clearing
            if (result.ClearDisplay)
            {
                _displayService.ClearDisplay(_useClassicMode);
            }

            // Handle command history
            if (result.CommandHistory != null)
            {
                _displayService.DisplayCommandHistory(result.CommandHistory, _useClassicMode);
            }
        }
    }
}