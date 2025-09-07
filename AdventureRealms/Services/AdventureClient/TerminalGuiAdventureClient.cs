using System;
using System.Linq;
using Terminal.Gui;
using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureClient.Models;
using AdventureRealms.Services.AdventureClient.UI;
using AdventureRealms.Services.AdventureClient.AppVersion;
using AdventureRealms.Services.AdventureClient.UI.TerminalGui;

namespace AdventureRealms.Services.AdventureClient
{
    /// <summary>
    /// Terminal.Gui-based Adventure Client - main entry point
    /// Now using separated components for better maintainability
    /// </summary>
    public class TerminalGuiAdventureClient : IAdventureClient
    {
        private readonly IPlayAdventure _server;
        private readonly IDisplayService _displayService;
        private readonly TerminalGuiRenderer _renderer;
        
        // Component managers
        private TerminalGuiGameState _gameState = null!;
        private TerminalGuiUIComponents _uiComponents = null!;
        private TerminalGuiFocusManager _focusManager = null!;
        private TerminalGuiEventHandlers _eventHandlers = null!;

        public TerminalGuiAdventureClient(IPlayAdventure server)
        {
            _server = server;
            var appVersionService = new AppVersionService();
            _displayService = new DisplayService(appVersionService);
            _renderer = new TerminalGuiRenderer();
        }

        public void StartAdventure(IPlayAdventure adventureServer)
        {
            try
            {
                Application.Init();
                
                // Initialize components
                InitializeComponents();
                
                // Load available games
                _gameState.AvailableGames = _server.GameList();
                
                if (_gameState.AvailableGames == null || _gameState.AvailableGames.Count == 0)
                {
                    MessageBox.ErrorQuery("Error", "No games available from server", "OK");
                    return;
                }
                
                // Create and run the main UI
                CreateMainUI();
                
                // Start the focus monitoring system
                _focusManager.StartFocusMonitor();
                
                Application.Run();
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.ErrorQuery("Error", $"Failed to start Terminal.Gui client: {ex.Message}", "OK");
                }
                catch
                {
                    Console.WriteLine($"Terminal.Gui Error: {ex.Message}");
                }
                throw; // Re-throw so Program.cs can fall back to console mode
            }
            finally
            {
                try
                {
                    _focusManager?.StopFocusMonitor();
                    Application.Shutdown();
                }
                catch
                {
                    // Ignore shutdown errors
                }
            }
        }

        /// <summary>
        /// Initialize all component managers
        /// </summary>
        private void InitializeComponents()
        {
            _gameState = new TerminalGuiGameState();
            _uiComponents = new TerminalGuiUIComponents();
        }

        /// <summary>
        /// Create the main UI
        /// </summary>
        private void CreateMainUI()
        {
            // Create menu bar first
            CreateMenuBar();
            
            // Create UI components without focus manager
            _uiComponents.CreateGameViews();
            
            // Now create focus manager with actual components
            _focusManager = new TerminalGuiFocusManager(_uiComponents.InputField, _uiComponents.InputView);
            
            // Set up focus manager events
            _uiComponents.SetupFocusManager(_focusManager);
            
            // Create event handlers
            _eventHandlers = new TerminalGuiEventHandlers(_server, _gameState, _uiComponents, _focusManager);
            
            // Set up event handlers
            _eventHandlers.SetupInputFieldKeyPress();
            _eventHandlers.SetupGameTextViewKeyPress();
            
            // Show welcome screen initially
            _uiComponents.ShowWelcomeScreen();
            
            // Add components to application
            _uiComponents.AddComponentsToApplication();
            
            // Set up initial focus
            Application.Top.LayoutSubviews();
            _focusManager.SetInputFieldShouldHaveFocus(true);
            Application.Refresh();
        }

        /// <summary>
        /// Create the menu bar
        /// </summary>
        private void CreateMenuBar()
        {
            _uiComponents.CreateMenuBar(
                startNewGameAction: StartNewGame,
                restartGameAction: RestartGame,
                switchGameAction: SwitchGame,
                showHelpAction: ShowHelp,
                showGameWelcomeAction: ShowGameWelcome,
                showAboutAction: ShowAbout,
                quitAction: () => Application.RequestStop(),
                toggleScrollModeAction: ToggleScrollMode,
                clearTextAction: ClearGameText
            );
        }

        #region Menu Actions

        /// <summary>
        /// Start a new game
        /// </summary>
        private void StartNewGame()
        {
            try
            {
                var selectedGameId = TerminalGuiDialogs.ShowGameSelection(_gameState.AvailableGames);
                if (selectedGameId != -1)
                {
                    _gameState.CurrentGameId = selectedGameId;
                    StartGame(_gameState.CurrentGameId);
                    _gameState.GameInProgress = true;
                    
                    // Show welcome message popup after successful game start
                    if (!string.IsNullOrEmpty(_gameState.CurrentGameWelcomeMessage))
                    {
                        var currentGame = _gameState.GetCurrentGame();
                        TerminalGuiDialogs.ShowWelcomePopup(_gameState.CurrentGameWelcomeMessage, currentGame?.Name ?? "Adventure Game");
                    }
                    
                    UpdateAllDisplays();
                    
                    // Restore focus after game start
                    _focusManager.SetInputFieldShouldHaveFocus(true);
                    Application.Top.LayoutSubviews();
                    Application.Refresh();
                    
                    Application.MainLoop.Invoke(() => {
                        _focusManager.EnsureInputFieldHasFocus();
                    });
                }
                else
                {
                    _uiComponents.GameTextView.Text = "Game selection cancelled. Use Play -> New Game to try again.";
                    _focusManager.SetInputFieldShouldHaveFocus(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to start new game: {ex.Message}", "OK");
                _focusManager.SetInputFieldShouldHaveFocus(true);
            }
        }

        /// <summary>
        /// Start a specific game
        /// </summary>
        private void StartGame(int gameId)
        {
            try
            {
                var request = new GamePlayRequest
                {
                    SessionId = "",
                    GameId = gameId,
                    Command = ""
                };
                
                var response = _server.PlayGame(request);
                
                if (response == null)
                {
                    throw new InvalidOperationException("Server returned null response");
                }
                
                if (string.IsNullOrEmpty(response.SessionId) || response.SessionId == "-1")
                {
                    throw new InvalidOperationException("Failed to start game session");
                }
                
                _gameState.UpdateFromGameResponse(response);
                
                // Clear previous game text when starting new game
                _uiComponents.GameTextView.Text = "";
                
                // Initialize items display
                _uiComponents.ItemsTextView.Text = "Nothing here.";
                _uiComponents.ItemsView.Title = "You see:";
                
                // Create map model from game data
                if (response.MapData != null)
                {
                    _gameState.CurrentMapModel = CreateMapModelFromPlayerMapData(response.MapData);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to start game {gameId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Restart the current game
        /// </summary>
        private void RestartGame()
        {
            if (_gameState.CurrentGameId == 0)
            {
                MessageBox.Query("No Game", "Please start a game first using Play → New Game", "OK");
                _focusManager.SetInputFieldShouldHaveFocus(true);
                return;
            }
            
            StartGame(_gameState.CurrentGameId);
            _gameState.GameInProgress = true;
            
            // Show welcome message when restarting
            if (!string.IsNullOrEmpty(_gameState.CurrentGameWelcomeMessage))
            {
                var currentGame = _gameState.GetCurrentGame();
                TerminalGuiDialogs.ShowWelcomePopup(_gameState.CurrentGameWelcomeMessage, currentGame?.Name ?? "Adventure Game");
            }
            
            UpdateAllDisplays();
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Switch to a different game
        /// </summary>
        private void SwitchGame()
        {
            if (!_gameState.GameInProgress)
            {
                StartNewGame();
                return;
            }
            
            var newGameId = TerminalGuiDialogs.ShowGameSelection(_gameState.AvailableGames);
            if (newGameId != -1 && newGameId != _gameState.CurrentGameId)
            {
                _gameState.CurrentGameId = newGameId;
                StartGame(_gameState.CurrentGameId);
                _gameState.GameInProgress = true;
                
                // Show welcome message for the new game
                if (!string.IsNullOrEmpty(_gameState.CurrentGameWelcomeMessage))
                {
                    var currentGame = _gameState.GetCurrentGame();
                    TerminalGuiDialogs.ShowWelcomePopup(_gameState.CurrentGameWelcomeMessage, currentGame?.Name ?? "Adventure Game");
                }
                
                UpdateAllDisplays();
            }
            
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Show help dialog
        /// </summary>
        private void ShowHelp()
        {
            TerminalGuiDialogs.ShowHelp();
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Show current game's welcome message
        /// </summary>
        private void ShowGameWelcome()
        {
            if (!_gameState.GameInProgress || string.IsNullOrEmpty(_gameState.CurrentGameWelcomeMessage))
            {
                MessageBox.Query("No Game", "Please start a game first to view its welcome message", "OK");
                _focusManager.SetInputFieldShouldHaveFocus(true);
                return;
            }
            
            var currentGame = _gameState.GetCurrentGame();
            TerminalGuiDialogs.ShowWelcomePopup(_gameState.CurrentGameWelcomeMessage, currentGame?.Name ?? "Adventure Game");
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Show about dialog
        /// </summary>
        private void ShowAbout()
        {
            TerminalGuiDialogs.ShowAbout();
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Toggle scroll mode for game text display
        /// </summary>
        private void ToggleScrollMode()
        {
            _gameState.ScrollMode = !_gameState.ScrollMode;
            var modeText = _gameState.ScrollMode ? "Scroll Mode Enabled" : "Clear Mode Enabled";
            MessageBox.Query("Display Mode", modeText, "OK");
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Clear the game text display
        /// </summary>
        private void ClearGameText()
        {
            _uiComponents.ClearGameDisplay();
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        #endregion

        /// <summary>
        /// Update all display components
        /// </summary>
        private void UpdateAllDisplays()
        {
            _uiComponents.UpdateGameDisplay(_gameState.LastGameResponse, _gameState.OriginalGameViewTitle, _gameState.ScrollMode);
            _uiComponents.UpdateItemsDisplay(_gameState.LastGameResponse);
            _uiComponents.UpdateGameInfo(_gameState.LastGameResponse, _gameState.GetCurrentGame(), _gameState.GameInProgress);
            _uiComponents.UpdateMapDisplay(_gameState.LastGameResponse?.MapData);
        }

        /// <summary>
        /// Create map model from player map data (simplified for now)
        /// </summary>
        private MapModel? CreateMapModelFromPlayerMapData(PlayerMapData playerMapData)
        {
            // Simplified - we're using simple text display now
            return null; // Not needed for simple text map
        }
    }
}