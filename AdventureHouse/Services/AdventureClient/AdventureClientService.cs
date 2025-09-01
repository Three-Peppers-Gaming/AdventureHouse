using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.AdventureClient.UI;
using AdventureHouse.Services.AdventureClient.Input;
using AdventureHouse.Services.AdventureClient.AppVersion;

namespace AdventureHouse.Services.AdventureClient
{
    /// <summary>
    /// The main Adventure Client service that handles all user interaction
    /// This is completely separate from the Adventure Server which handles game logic
    /// </summary>
    public class AdventureClientService : IAdventureClient
    {
        private readonly IDisplayService _displayService;
        private readonly IInputService _inputService;
        private bool _useClassicMode = false;
        private bool _useScrollMode = false;
        private string _currentInstanceId = string.Empty;

        public AdventureClientService()
        {
            // Initialize client services
            var appVersionService = new AppVersionService();
            _displayService = new DisplayService(appVersionService);
            _inputService = new InputService();
            _inputService.InitializeCommandBuffer();
        }

        public void StartAdventure(IPlayAdventure adventureServer)
        {
            string move = string.Empty;

            try
            {
                // Display intro and get available games
                _displayService.DisplayIntro(_useClassicMode);
                var availableGames = adventureServer.FrameWork_GetGames();
                int selectedGameId = _displayService.DisplayAdventureSelection(availableGames, _useClassicMode);

                // Start game session with the Adventure Server
                var gameResult = adventureServer.FrameWork_StartGameSession(selectedGameId, _useClassicMode, _useScrollMode);
                _currentInstanceId = gameResult.InstanceID;

                // Display initial console output if any
                if (!string.IsNullOrEmpty(gameResult.ConsoleOutput))
                {
                    _displayService.DisplayConsoleOutput(gameResult.ConsoleOutput);
                }

                // Main game loop - send moves to server and display results
                while (move != "resign" && gameResult.InstanceID != "-1")
                {
                    // Display current game state
                    _displayService.DisplayGameState(gameResult, !_useClassicMode, _useScrollMode);

                    // Get user input
                    move = _inputService.GetUserInput(_useClassicMode);
                    if (string.IsNullOrEmpty(move)) move = "";

                    // Send move to Adventure Server and get result
                    var gameMove = new GameMove
                    {
                        InstanceID = _currentInstanceId,
                        Move = move,
                        UseClassicMode = _useClassicMode,
                        UseScrollMode = _useScrollMode
                    };

                    gameResult = adventureServer.FrameWork_GameMove(gameMove);

                    // Handle any client-side actions requested by the server
                    HandleClientActions(gameResult);
                }

                // Display farewell
                _displayService.DisplayFarewell(_useClassicMode);
            }
            catch (Exception e)
            {
                _displayService.DisplayError($"An error occurred: {e.Message}", _useClassicMode);
            }
        }

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

            // Handle console output
            if (!string.IsNullOrEmpty(result.ConsoleOutput))
            {
                _displayService.DisplayConsoleOutput(result.ConsoleOutput);
            }

            // Handle map display
            if (result.MapData != null)
            {
                _displayService.DisplayMap(result.MapData as AdventureClient.Models.MapState, _useClassicMode);
            }

            // Handle command history
            if (result.CommandHistory != null)
            {
                _displayService.DisplayCommandHistory(result.CommandHistory, _useClassicMode);
            }
        }
    }
}