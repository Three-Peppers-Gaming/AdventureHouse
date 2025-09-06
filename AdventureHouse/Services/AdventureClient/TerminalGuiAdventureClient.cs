using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureClient.Models;
using AdventureHouse.Services.AdventureClient.UI;
using AdventureHouse.Services.AdventureClient.AppVersion;

namespace AdventureHouse.Services.AdventureClient
{
    /// <summary>
    /// Terminal.Gui-based Adventure Client with welcome screen and menu-driven game selection:
    /// - Welcome screen on startup with instructions
    /// - Game selection via Play menu (not automatic)
    /// - Game text on left, map on right
    /// - Input below game text, map legend below input  
    /// - Blue borders, black background, white game text, green input
    /// - Game switching and terminal window features
    /// </summary>
    public class TerminalGuiAdventureClient : IAdventureClient
    {
        private readonly IPlayAdventure _server;
        private readonly IDisplayService _displayService;
        private readonly TerminalGuiRenderer _renderer;
        
        // UI Components
        private Window _mainWindow = null!;
        private FrameView _gameView = null!;
        private FrameView _mapView = null!;
        private FrameView _inputView = null!;
        private FrameView _legendView = null!;
        private FrameView _itemsView = null!;  // New items display box
        private FrameView _gameInfoView = null!;  // New game info box
        private MenuBar _menuBar = null!;
        private TextView _gameTextView = null!;
        private TextView _mapTextView = null!;
        private TextField _inputField = null!;
        private TextView _legendTextView = null!;
        private TextView _itemsTextView = null!;  // New items text view
        private TextView _gameInfoTextView = null!;  // New game info text view
        
        // Game State
        private string _currentSessionId = "";
        private int _currentGameId = 0; // 0 = no game selected
        private List<Game> _availableGames = null!;
        private MapModel? _currentMapModel;
        private GamePlayResponse? _lastGameResponse;
        private List<string> _commandHistory = new();
        private int _historyIndex = -1;
        private bool _gameInProgress = false;
        private string _playerGamerTag = "Player"; // Default gamer tag
        
        // Add this field to track current room
        private string _currentRoomName = "";
        
        // Add field to track the original title when focus changes
        private string _originalGameViewTitle = "Welcome";
        
        // Color Schemes with proper menu colors and readability
        private static readonly ColorScheme BlueScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),     // Green borders
            Focus = new Terminal.Gui.Attribute(Color.Red, Color.Black),        // Red when selected
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),  // Green hotkeys (P, V, H, G)
            HotFocus = new Terminal.Gui.Attribute(Color.Red, Color.Black),     // Red hotkeys when selected
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        private static readonly ColorScheme MenuScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),     // White menu text (lay, elp, ame)
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),      // White when selected
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),  // Green hotkeys (P, V, H, G)
            HotFocus = new Terminal.Gui.Attribute(Color.Red, Color.Black),     // Red hotkeys when selected
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        private static readonly ColorScheme InputScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        private static readonly ColorScheme GameScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        private static readonly ColorScheme MapScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Magenta, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Magenta, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        // Green text on black background for game info box
        private static readonly ColorScheme GameInfoScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        // Light blue text for player commands - changed to Cyan
        private static readonly ColorScheme PlayerCommandScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        // Location text color scheme - Bright Yellow
        private static readonly ColorScheme LocationScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
        // Response text color scheme - Red
        private static readonly ColorScheme ResponseScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Red, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
        
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
                
                // Load available games
                _availableGames = _server.GameList();
                
                if (_availableGames == null || _availableGames.Count == 0)
                {
                    MessageBox.ErrorQuery("Error", "No games available from server", "OK");
                    return;
                }
                
                // Create and run the main UI with welcome screen (no immediate game selection)
                CreateMainUI();
                
                // Start the focus monitoring system
                StartFocusMonitor();
                
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
                    // If MessageBox fails, just print to console
                    Console.WriteLine($"Terminal.Gui Error: {ex.Message}");
                }
                throw; // Re-throw so Program.cs can fall back to console mode
            }
            finally
            {
                try
                {
                    StopFocusMonitor();
                    Application.Shutdown();
                }
                catch
                {
                    // Ignore shutdown errors
                }
            }
        }
        
        private void CreateMainUI()
        {
            CreateMenuBar();
            CreateGameViews();
            
            // Show welcome screen initially (no game running)
            ShowWelcomeScreen();
            
            // Add directly to Application.Top without outer window
            Application.Top.Add(_menuBar);
            Application.Top.Add(_gameView);
            Application.Top.Add(_mapView);
            Application.Top.Add(_itemsView);
            Application.Top.Add(_inputView);
            Application.Top.Add(_gameInfoView);
            Application.Top.Add(_legendView);
            
            // Force layout calculation first
            Application.Top.LayoutSubviews();
            
            // AGGRESSIVE FOCUS SETTING - try multiple approaches
            _inputFieldShouldHaveFocus = true;
            
            // Approach 1: Direct focus
            _inputField.SetFocus();
            
            // Approach 2: Through parent
            _inputView.SetFocus();
            _inputField.SetFocus();
            
            // Approach 3: Make sure it's the most focused
            Application.Top.SetFocus();
            _inputField.SetFocus();
            
            // Approach 4: Use our robust method
            EnsureInputFieldHasFocus();
            
            Application.Refresh();
        }
        
        private void CreateMenuBar()
        {
            var playMenu = new MenuBarItem("_Play", new MenuItem[]
            {
                new MenuItem("_New Game", "Start a new adventure", () => StartNewGame()),
                new MenuItem("_Restart Current", "Restart current game", () => RestartGame()),
                null, // Separator
                new MenuItem("_Switch Game", "Switch to a different adventure", () => SwitchGame()),
                null, // Separator
                new MenuItem("_Quit", "Exit Adventure Realms", () => Application.RequestStop())
            });
            
            var helpMenu = new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_Game Help", "Show game help (F1)", () => ShowHelp()),
                new MenuItem("_About", "About Adventure Realms", () => ShowAbout())
            });
            
            _menuBar = new MenuBar(new MenuBarItem[] { playMenu, helpMenu })
            {
                ColorScheme = MenuScheme  // Use MenuScheme for proper colors
            };
        }
        
        private void CreateGameViews()
        {
            // Calculate layout dimensions with no outer window
            var totalWidth = Math.Max(Application.Driver?.Cols ?? 120, 80);
            var totalHeight = Math.Max(Application.Driver?.Rows ?? 30, 20) - 1; // Only menu bar
            var leftWidth = (totalWidth * 60) / 100; // 60% for game area
            var rightWidth = totalWidth - leftWidth; // 40% for map area
            var gameHeight = (totalHeight * 65) / 100; // 65% for game text
            var bottomHeight = totalHeight - gameHeight; // 35% for items, input and legend
            var itemsHeight = 4; // Fixed height for items box
            var gameInfoHeight = 3; // Fixed height for game info box
            var inputHeight = bottomHeight - itemsHeight - gameInfoHeight; // Remaining space for input
            
            // Ensure minimum sizes and proper bounds
            gameHeight = Math.Max(gameHeight, 8);
            inputHeight = Math.Max(inputHeight, 3);
            itemsHeight = Math.Max(itemsHeight, 3);
            gameInfoHeight = Math.Max(gameInfoHeight, 2);
            leftWidth = Math.Max(leftWidth, 40);
            rightWidth = Math.Max(rightWidth, 25);
            
            // Game text view (top-left) - title will be dynamic room name
            _gameView = new FrameView("Welcome")
            {
                X = 0,
                Y = 1, // Below menu bar
                Width = leftWidth,
                Height = gameHeight,
                ColorScheme = BlueScheme
            };
            
            _gameTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                ColorScheme = GameScheme,
                Text = "",
                CanFocus = false  // DISABLE FOCUS to prevent interference
            };
            
            // Add key handler to return to input field when user presses Tab or Enter
            _gameTextView.KeyPress += (args) =>
            {
                // ANY key press in game text should return focus to command input
                // This ensures users can always get back to typing commands
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
                args.Handled = true;
            };
            
            _gameView.Add(_gameTextView);
            
            // Map view (top-right)
            _mapView = new FrameView("Map")
            {
                X = leftWidth,
                Y = 1, // Below menu bar
                Width = rightWidth,
                Height = gameHeight,
                ColorScheme = BlueScheme
            };
            
            _mapTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = false,
                ColorScheme = MapScheme,
                CanFocus = false  // Disable focus for map area
            };
            
            _mapView.Add(_mapTextView);
            
            // Items view (middle-left) - "You see:" instead of "Items Here"
            _itemsView = new FrameView("You see:")
            {
                X = 0,
                Y = 1 + gameHeight,
                Width = leftWidth,
                Height = itemsHeight,
                ColorScheme = BlueScheme
            };
            
            _itemsTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                ColorScheme = new ColorScheme
                {
                    Normal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
                    Focus = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
                    HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
                    HotFocus = new Terminal.Gui.Attribute(Color.White, Color.Black),
                    Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
                },
                Text = "Nothing here.",
                CanFocus = false  // Disable focus for items view
            };
            
            _itemsView.Add(_itemsTextView);
            
            // Game info view (below items) - green text on black background
            _gameInfoView = new FrameView("Game")
            {
                X = 0,
                Y = 1 + gameHeight + itemsHeight,
                Width = leftWidth,
                Height = gameInfoHeight,
                ColorScheme = BlueScheme
            };
            
            _gameInfoTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                ColorScheme = GameInfoScheme,
                Text = "No game selected",
                CanFocus = false  // Disable focus for game info view
            };
            
            _gameInfoView.Add(_gameInfoTextView);
            
            // Input view (bottom-left) - single line with max 50 characters
            _inputView = new FrameView("Command")
            {
                X = 0,
                Y = 1 + gameHeight + itemsHeight + gameInfoHeight,
                Width = leftWidth,
                Height = inputHeight,
                ColorScheme = BlueScheme
            };
            
            // Add click handler to the frame itself to set focus when clicked
            _inputView.MouseClick += (args) =>
            {
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
                args.Handled = true;
            };
            
            _inputField = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 2,
                Height = 1,
                ColorScheme = InputScheme,
                CanFocus = true  // Explicitly enable focus
            };
            
            // Add mouse click handler to ensure focus when clicked
            _inputField.MouseClick += (args) =>
            {
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
                args.Handled = true;
            };
            
            // Add focus event handlers for the input field
            _inputField.Enter += (sender) =>
            {
                if (_isSettingFocusProgrammatically) return;
                
                // When command input gets focus, restore the original frame title
                _inputView.Title = "Command";
                _inputFieldShouldHaveFocus = true;
            };
            
            _inputField.Leave += (sender) =>
            {
                if (_isSettingFocusProgrammatically) return;
                
                // When leaving command input, update the frame title to show focus instruction
                _inputView.Title = "Command <= Click here to set focus";
                _inputFieldShouldHaveFocus = false;
            };
            
            _inputField.KeyPress += OnInputKeyPress;
            _inputView.Add(_inputField);
            
            // Legend view (right side) - two column format
            _legendView = new FrameView("Keys")
            {
                X = leftWidth,
                Y = 1 + gameHeight,
                Width = rightWidth,
                Height = bottomHeight,
                ColorScheme = BlueScheme
            };
            
            _legendTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = false, // Don't wrap for column formatting
                ColorScheme = MapScheme,
                Text = GetMapLegendText(),
                CanFocus = false  // Disable focus for Keys legend area
            };
            
            _legendView.Add(_legendTextView);
            
            // Set initial focus to input field using robust management
            _inputFieldShouldHaveFocus = true;
            EnsureInputFieldHasFocus();
        }
        
        private void ShowWelcomeScreen()
        {
            var welcomeText = @"WELCOME TO ADVENTURE REALMS

Classic text adventure games - explore, solve puzzles, escape!

TO START: Use Play menu -> New Game
COMMANDS: n/s/e/w/u/d, look, get/drop item, inv, use item, help
GAMES: Adventure House, Space Station, Future Family

Type commands in green box below. Map appears on right when playing.

Copyright (c) Steven Sparks 2025.";

            _gameTextView.Text = welcomeText;
            
            // Set both the view title and original title tracking
            _gameView.Title = "Welcome";
            _originalGameViewTitle = "Welcome";
            
            _mapTextView.Text = @"MAP AREA

Your map will
show here when
you start a game.

@ = You
+ = Items
Connections
shown between
visited rooms.";
            
            // Clear items display on welcome screen
            _itemsTextView.Text = "Items will appear here when you enter rooms.";
            _itemsView.Title = "Items";
            
            // Show no game selected message
            _gameInfoTextView.Text = "No game selected";
            
            _gameInProgress = false;
        }
        
        private void StartNewGame()
        {
            try
            {
                var selectedGameId = ShowGameSelection();
                if (selectedGameId != -1)
                {
                    _currentGameId = selectedGameId;
                    StartGame(_currentGameId);
                    _gameInProgress = true;
                    UpdateGameDisplay();
                    UpdateMapDisplay();
                    UpdateGameInfo();
                    
                    // AGGRESSIVE FOCUS RECOVERY AFTER GAME START
                    _inputFieldShouldHaveFocus = true;
                    Application.Top.LayoutSubviews();
                    
                    // Try multiple times with delays
                    EnsureInputFieldHasFocus();
                    Application.Refresh();
                    
                    // Set up a delayed focus attempt
                    Application.MainLoop.Invoke(() => {
                        _inputField.SetFocus();
                        EnsureInputFieldHasFocus();
                    });
                }
                else
                {
                    // User cancelled - show message in game area and return focus
                    _gameTextView.Text = "Game selection cancelled. Use Play -> New Game to try again.";
                    _inputFieldShouldHaveFocus = true;
                    EnsureInputFieldHasFocus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to start new game: {ex.Message}", "OK");
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
            }
        }
        
        private int ShowGameSelection()
        {
            try
            {
                var gameSelectionDialog = new Dialog("Select Game", 70, 15);
                gameSelectionDialog.ColorScheme = BlueScheme;
                
                // Create a simple list of game names only (not descriptions)
                var gameNames = _availableGames.Select(g => g.Name).ToArray();
                
                var gameList = new ListView(gameNames)
                {
                    X = 1,
                    Y = 1,
                    Width = Dim.Fill() - 2,
                    Height = Dim.Fill() - 4,
                    ColorScheme = BlueScheme,
                    SelectedItem = 0,
                    CanFocus = true
                };
                
                var selectButton = new Button("Select")
                {
                    X = Pos.Center() - 8,
                    Y = Pos.Bottom(gameList) + 1,
                    IsDefault = true
                };
                
                var cancelButton = new Button("Cancel")
                {
                    X = Pos.Center() + 2,
                    Y = Pos.Bottom(gameList) + 1
                };
                
                int selectedGameId = -1;
                
                selectButton.Clicked += () =>
                {
                    try
                    {
                        var selectedIndex = gameList.SelectedItem;
                        if (selectedIndex >= 0 && selectedIndex < _availableGames.Count)
                        {
                            selectedGameId = _availableGames[selectedIndex].Id;
                            Application.RequestStop();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Error", $"Selection error: {ex.Message}", "OK");
                    }
                };
                
                cancelButton.Clicked += () =>
                {
                    selectedGameId = -1;
                    Application.RequestStop();
                };
                
                // Handle Enter key on list
                gameList.KeyPress += (args) =>
                {
                    if (args.KeyEvent.Key == Key.Enter)
                    {
                        selectButton.OnClicked();
                        args.Handled = true;
                    }
                };
                
                gameSelectionDialog.Add(gameList);
                gameSelectionDialog.Add(selectButton);
                gameSelectionDialog.Add(cancelButton);
                
                // Set initial focus to the list
                gameList.SetFocus();
                
                Application.Run(gameSelectionDialog);
                
                return selectedGameId;
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to show game selection: {ex.Message}", "OK");
                return 1; // Default to first game
            }
        }
        
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
                
                _lastGameResponse = _server.PlayGame(request);
                
                if (_lastGameResponse == null)
                {
                    throw new InvalidOperationException("Server returned null response");
                }
                
                _currentSessionId = _lastGameResponse.SessionId;
                
                if (string.IsNullOrEmpty(_currentSessionId) || _currentSessionId == "-1")
                {
                    throw new InvalidOperationException("Failed to start game session");
                }
                
                // Update player gamer tag from server response
                if (!string.IsNullOrEmpty(_lastGameResponse.PlayerName))
                {
                    _playerGamerTag = _lastGameResponse.PlayerName;
                }
                
                // Clear previous game text when starting new game
                _gameTextView.Text = "";
                
                // Reset room tracking for new game
                _currentRoomName = "";
                
                // Initialize items display
                _itemsTextView.Text = "Nothing here.";
                _itemsView.Title = "You see:";
                
                // Create map model from game data
                if (_lastGameResponse.MapData != null)
                {
                    // Convert PlayerMapData to MapModel for rendering
                    _currentMapModel = CreateMapModelFromPlayerMapData(_lastGameResponse.MapData);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to start game {gameId}: {ex.Message}", ex);
            }
        }
        
        private void OnInputKeyPress(View.KeyEventEventArgs args)
        {
            // Remove Tab navigation to game text since it can't be focused anymore
            // Handle Tab like any other key - just ignore it
            if (args.KeyEvent.Key == Key.Tab)
            {
                args.Handled = true;
                return;
            }
            
            // Limit input to 50 characters
            if (args.KeyEvent.Key != Key.Enter && 
                args.KeyEvent.Key != Key.Backspace && 
                args.KeyEvent.Key != Key.Delete &&
                args.KeyEvent.Key != Key.CursorUp &&
                args.KeyEvent.Key != Key.CursorDown &&
                args.KeyEvent.Key != Key.CursorLeft &&
                args.KeyEvent.Key != Key.CursorRight &&
                _inputField.Text.Length >= 50)
            {
                args.Handled = true;
                return;
            }
            
            if (args.KeyEvent.Key == Key.Enter)
            {
                var command = _inputField.Text.ToString();
                if (!string.IsNullOrWhiteSpace(command))
                {
                    if (!_gameInProgress)
                    {
                        MessageBox.Query("No Game", "Please start a game first using Play → New Game", "OK");
                        _inputField.Text = "";
                        _inputFieldShouldHaveFocus = true;
                        EnsureInputFieldHasFocus();
                        return;
                    }
                    
                    ProcessCommand(command);
                    _commandHistory.Add(command);
                    _historyIndex = _commandHistory.Count;
                    _inputField.Text = "";
                }
                // Keep focus on input field after processing command
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.CursorUp)
            {
                // Command history - previous
                if (_historyIndex > 0)
                {
                    _historyIndex--;
                    _inputField.Text = _commandHistory[_historyIndex];
                }
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.CursorDown)
            {
                // Command history - next
                if (_historyIndex < _commandHistory.Count - 1)
                {
                    _historyIndex++;
                    _inputField.Text = _commandHistory[_historyIndex];
                }
                else if (_historyIndex == _commandHistory.Count - 1)
                {
                    _historyIndex = _commandHistory.Count;
                    _inputField.Text = "";
                }
                args.Handled = true;
            }
        }
        
        private void ProcessCommand(String command)
        {
            if (string.IsNullOrEmpty(_currentSessionId) || _currentSessionId == "-1")
            {
                _gameTextView.Text = "Game session ended. Please restart or start a new game using Play menu.";
                _gameInProgress = false;
                return;
            }
            
            var request = new GamePlayRequest
            {
                SessionId = _currentSessionId,
                Command = command
            };
            
            try
            {
                // Send command to server without displaying it first
                _lastGameResponse = _server.PlayGame(request);
                
                if (_lastGameResponse.SessionId == "-1")
                {
                    _gameTextView.Text = "*** GAME SESSION ENDED ***";
                    _currentSessionId = "-1";
                    _gameInProgress = false;
                }
                else
                {
                    UpdateGameDisplay();
                    UpdateMapDisplay();
                    UpdateGameInfo();
                }
                
                if (_lastGameResponse.PlayerDead)
                {
                    MessageBox.Query("Game Over", "You have died! Game ended.", "OK");
                    _gameInProgress = false;
                    _inputFieldShouldHaveFocus = true;
                    EnsureInputFieldHasFocus();
                }
                else if (_lastGameResponse.GameCompleted)
                {
                    MessageBox.Query("Congratulations!", "You have completed the game!", "OK");
                    _gameInProgress = false;
                    _inputFieldShouldHaveFocus = true;
                    EnsureInputFieldHasFocus();
                }
                
                // Always use robust focus management
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
            }
            catch (Exception ex)
            {
                _gameTextView.Text = $"*** ERROR: {ex.Message} ***";
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
            }
        }
        
        private string FormatCommandAsChat(string command)
        {
            // This method is no longer used since we don't display player commands
            // Left for backward compatibility but returns empty string
            return "";
        }
        
        private void UpdateGameDisplay()
        {
            if (_lastGameResponse == null) return;
            
            // Check if this is a room change
            bool isRoomChange = !string.IsNullOrEmpty(_currentRoomName) && 
                               _currentRoomName != _lastGameResponse.RoomName;
            
            // Update current room tracking
            _currentRoomName = _lastGameResponse.RoomName;
            
            // Store the clean room name as the original title (without any focus messages)
            _originalGameViewTitle = _lastGameResponse.RoomName;
            
            // Update game view title - restore clean title first
            _gameView.Title = _originalGameViewTitle;
            
            var gameText = new StringBuilder();
            
            // Prioritize command response over room description to avoid duplication
            // Command response often contains room description plus additional info
            if (!string.IsNullOrEmpty(_lastGameResponse.CommandResponse))
            {
                gameText.AppendLine(_lastGameResponse.CommandResponse);
                gameText.AppendLine();
            }
            // Only show room description if there's no command response
            else if (!string.IsNullOrEmpty(_lastGameResponse.RoomDescription))
            {
                gameText.AppendLine(_lastGameResponse.RoomDescription);
                gameText.AppendLine();
            }
    
            // Update items in dedicated items box (not in main game text)
            UpdateItemsDisplay();
            
            // Always clear the game window and show fresh content (no accumulation)
            _gameTextView.Text = gameText.ToString();
            
            // Auto-scroll to bottom to show latest content
            _gameTextView.MoveEnd();
            
            // Use robust focus management instead of simple SetFocus
            if (_gameInProgress)
            {
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
            }
        }
        
        private void UpdateItemsDisplay()
        {
            if (_lastGameResponse == null)
            {
                _itemsTextView.Text = "Nothing here.";
                return;
            }
            
            // Update items display in dedicated box
            if (!string.IsNullOrEmpty(_lastGameResponse.ItemsInRoom) && 
                _lastGameResponse.ItemsInRoom != "No Items")
            {
                _itemsTextView.Text = _lastGameResponse.ItemsInRoom;
                _itemsView.Title = "You see:";
            }
            else
            {
                _itemsTextView.Text = "Nothing here.";
                _itemsView.Title = "You see:";
            }
        }
        
        private void UpdateGameInfo()
        {
            if (_lastGameResponse == null || !_gameInProgress)
            {
                _gameInfoTextView.Text = "No game selected";
                return;
            }
            
            // Get game details
            var currentGame = _availableGames.FirstOrDefault(g => g.Id == _currentGameId);
            if (currentGame == null)
            {
                _gameInfoTextView.Text = "Unknown game";
                return;
            }
            
            // Format: Line 1: Playing [Game Name] and feeling [health], Line 2: By [Author Name]
            var gameInfo = $"Playing {currentGame.Name} and feeling {_lastGameResponse.PlayerHealth}\nBy Steve Sparks 2019 to 2025!";
            _gameInfoTextView.Text = gameInfo;
        }
        
        private void UpdateMapDisplay()
        {
            if (_lastGameResponse?.MapData == null)
            {
                _mapTextView.Text = "No map data available.";
                return;
            }
            
            // Create simple ASCII map from discovered rooms
            var mapText = CreateSimpleMap(_lastGameResponse.MapData);
            _mapTextView.Text = mapText;
        }
        
        private string CreateSimpleMap(PlayerMapData mapData)
        {
            if (mapData.DiscoveredRooms == null || mapData.DiscoveredRooms.Count == 0)
            {
                return "No rooms discovered yet.";
            }
            
            var mapBuilder = new StringBuilder();
            mapBuilder.AppendLine($"Level: {mapData.CurrentLevelDisplayName}");
            mapBuilder.AppendLine();
            
            // Group rooms by level for better display
            var currentLevelRooms = mapData.DiscoveredRooms
                .Where(r => r.Level == mapData.CurrentLevel)
                .OrderBy(r => r.Position.Y)
                .ThenBy(r => r.Position.X)
                .ToList();
            
            if (currentLevelRooms.Any())
            {
                foreach (var room in currentLevelRooms)
                {
                    var marker = room.IsCurrentLocation ? "@ " : "  ";
                    var items = room.HasItems ? " [+]" : "";
                    mapBuilder.AppendLine($"{marker}{room.Name}{items}");
                }
            }
            else
            {
                mapBuilder.AppendLine("Current level rooms:");
                foreach (var room in mapData.DiscoveredRooms.Take(10)) // Show up to 10 rooms
                {
                    var marker = room.IsCurrentLocation ? "@ " : "  ";
                    var items = room.HasItems ? " [+]" : "";
                    mapBuilder.AppendLine($"{marker}{room.Name}{items}");
                }
            }
            
            return mapBuilder.ToString();
        }
        
        private MapModel CreateMapModelFromPlayerMapData(PlayerMapData playerMapData)
        {
            // Simplified - we're using simple text display now
            return null; // Not needed for simple text map
        }
        
        private string GetMapLegendText()
        {
            return @"@ = You       n/s/e/w = Move
+ = Items     get/drop = Items
. = H-path    inv = Inventory
: = V-path    look = Examine
^ = Up        help = Game help
v = Down      Enter = Send

Click Command box to type
Focus is auto-managed
Ready to play!";
        }
        
        private void SwitchGame()
        {
            if (!_gameInProgress)
            {
                StartNewGame();
                return;
            }
            
            var newGameId = ShowGameSelection();
            if (newGameId != -1 && newGameId != _currentGameId)
            {
                _currentGameId = newGameId;
                StartGame(_currentGameId);
                _gameInProgress = true;
                UpdateGameDisplay();
                UpdateMapDisplay();
                UpdateGameInfo();
            }
            // Return focus to input after game selection
            _inputFieldShouldHaveFocus = true;
            EnsureInputFieldHasFocus();
        }
        
        private void RestartGame()
        {
            if (_currentGameId == 0)
            {
                MessageBox.Query("No Game", "Please start a game first using Play → New Game", "OK");
                _inputFieldShouldHaveFocus = true;
                EnsureInputFieldHasFocus();
                return;
            }
            
            StartGame(_currentGameId);
            _gameInProgress = true;
            UpdateGameDisplay();
            UpdateMapDisplay();
            UpdateGameInfo();
            _inputFieldShouldHaveFocus = true;
            EnsureInputFieldHasFocus();
        }
        
        private void RefreshDisplay()
        {
            if (_gameInProgress)
            {
                UpdateGameDisplay();
                UpdateMapDisplay();
                UpdateGameInfo();
            }
            else
            {
                ShowWelcomeScreen();
            }
            Application.Refresh();
            _inputField.SetFocus();
        }
        
        private void ShowHelp()
        {
            var helpText = @"Adventure Realms

GAME COMMANDS:
- Movement: n, s, e, w, u, d (or full names)
- Actions: look, get <item>, drop <item>, use <item>
- Info: inv (inventory), help (game help)
- Combat: attack <monster> 
- Other: eat <item>, read <item>, pet <animal>

INTERFACE:
- Type commands in the green input field (max 50 chars)
- Game text appears in the left panel
- Map shows on the right panel
- Legend shows keyboard shortcuts

MENU OPTIONS:
- Play → New Game: Start a new adventure
- Play → Switch Game: Change to different game
- Play → Restart Current: Restart current game
- Help → Game Help: Show this help
- Help → About: About Adventure Realms";

            MessageBox.Query("Help", helpText, "OK");
            _inputFieldShouldHaveFocus = true;
            EnsureInputFieldHasFocus();
        }
        
        private void ShowAbout()
        {
            var aboutText = @"Adventure Realms v1.0

A collection of classic text adventure games.

Features:
- Multiple adventure games
- Real-time ASCII map display  
- Terminal.Gui interface
- Classic console fallback

Commands: n/s/e/w/u/d, look, get/drop, inv, use, help

Games:
- Adventure House: Escape the mysterious house
- Space Station: Survive the abandoned station  
- Future Family: Escape the sky apartment

Copyright (c) Steven Sparks 2025.";

            MessageBox.Query("About Adventure Realms", aboutText, "OK");
            _inputFieldShouldHaveFocus = true;
            EnsureInputFieldHasFocus();
        }

        private void SetPlayerGamerTag()
        {
            // Simple dialog to set gamer tag
            var gamerTagDialog = new Dialog("Enter Gamer Tag", 50, 8);
            gamerTagDialog.ColorScheme = BlueScheme;
            
            var label = new Label("Gamer Tag:")
            {
                X = 1,
                Y = 1
            };
            
            var tagField = new TextField(_playerGamerTag)
            {
                X = 1,
                Y = 2,
                Width = Dim.Fill() - 2,
                Height = 1
            };
            
            var okButton = new Button("OK")
            {
                X = Pos.Center() - 5,
                Y = 4,
                IsDefault = true
            };
            
            var cancelButton = new Button("Cancel")
            {
                X = Pos.Center() + 2,
                Y = 4
            };
            
            okButton.Clicked += () =>
            {
                if (!string.IsNullOrWhiteSpace(tagField.Text.ToString()))
                {
                    _playerGamerTag = tagField.Text.ToString().Trim();
                }
                Application.RequestStop();
            };
            
            cancelButton.Clicked += () =>
            {
                Application.RequestStop();
            };
            
            gamerTagDialog.Add(label);
            gamerTagDialog.Add(tagField);
            gamerTagDialog.Add(okButton);
            gamerTagDialog.Add(cancelButton);
            
            Application.Run(gamerTagDialog);
        }

        // Add more robust focus tracking fields
        private bool _inputFieldShouldHaveFocus = true;
        private bool _isSettingFocusProgrammatically = false;
        private System.Timers.Timer? _focusMonitorTimer;

        // Add a robust focus management method
        private void EnsureInputFieldHasFocus()
        {
            if (!_inputFieldShouldHaveFocus) return;
            
            try
            {
                _isSettingFocusProgrammatically = true;
                
                // First make sure the input field is focusable
                if (!_inputField.CanFocus)
                {
                    _inputField.CanFocus = true;
                }
                
                // Set focus multiple ways to ensure it works
                if (_inputField.HasFocus == false)
                {
                    _inputField.SetFocus();
                    
                    // Also try setting it as the focused control on the parent
                    if (_inputView != null)
                    {
                        _inputView.SetFocus();
                        _inputField.SetFocus();
                    }
                }
                
                // Ensure title is correct
                if (_inputView.Title.ToString() != "Command")
                {
                    _inputView.Title = "Command";
                }
            }
            finally
            {
                _isSettingFocusProgrammatically = false;
            }
        }
        
        // Add active focus monitoring system
        private void StartFocusMonitor()
        {
            _focusMonitorTimer = new System.Timers.Timer(100); // Check every 100ms
            _focusMonitorTimer.Elapsed += (sender, e) =>
            {
                // Run focus check on main thread
                Application.MainLoop.Invoke(() =>
                {
                    if (_inputFieldShouldHaveFocus && !_inputField.HasFocus && !_isSettingFocusProgrammatically)
                    {
                        // Focus was lost, try to recover it
                        EnsureInputFieldHasFocus();
                    }
                });
            };
            _focusMonitorTimer.Start();
        }
        
        private void StopFocusMonitor()
        {
            _focusMonitorTimer?.Stop();
            _focusMonitorTimer?.Dispose();
            _focusMonitorTimer = null;
        }
    }
}