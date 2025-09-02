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
        private MenuBar _menuBar = null!;
        private TextView _gameTextView = null!;
        private TextView _mapTextView = null!;
        private TextField _inputField = null!;
        private TextView _legendTextView = null!;
        private TextView _itemsTextView = null!;  // New items text view
        // Removed StatusBar - using menus instead
        
        // Game State
        private string _currentSessionId = "";
        private int _currentGameId = 0; // 0 = no game selected
        private List<Game> _availableGames = null!;
        private MapModel? _currentMapModel;
        private GamePlayResponse? _lastGameResponse;
        private List<string> _commandHistory = new();
        private int _historyIndex = -1;
        private bool _gameInProgress = false;
        
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
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),  // White on black
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),   // White on black
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
            Application.Top.Add(_legendView);
        }
        
        private void CreateMenuBar()
        {
            var playMenu = new MenuBarItem("_Play", new MenuItem[]
            {
                new MenuItem("_New Game", "Start a new adventure", () => StartNewGame()),
                new MenuItem("_Restart Current", "Restart current game", () => RestartGame()),
                null, // Separator
                new MenuItem("_Switch Game", "Switch to a different adventure", () => SwitchGame())
            });
            
            var viewMenu = new MenuBarItem("_View", new MenuItem[]
            {
                new MenuItem("_Console Terminal", "Open console terminal window", () => OpenTerminalWindow()),
                new MenuItem("_Refresh", "Refresh display", () => RefreshDisplay()),
                null,
                new MenuItem("_Help", "Show help", () => ShowHelp())
            });
            
            var helpMenu = new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_Game Help", "Show game help (F1)", () => ShowHelp()),
                new MenuItem("_About", "About Adventure Realms", () => ShowAbout())
            });
            
            var gameMenu = new MenuBarItem("_Game", new MenuItem[]
            {
                new MenuItem("_Quit", "Exit Adventure Realms", () => Application.RequestStop())
            });
            
            _menuBar = new MenuBar(new MenuBarItem[] { playMenu, viewMenu, helpMenu, gameMenu })
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
            var inputHeight = bottomHeight - itemsHeight; // Remaining space for input
            
            // Ensure minimum sizes and proper bounds
            gameHeight = Math.Max(gameHeight, 8);
            inputHeight = Math.Max(inputHeight, 3);
            itemsHeight = Math.Max(itemsHeight, 3);
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
                Text = ""
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
                ColorScheme = MapScheme
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
                Text = "Nothing here."
            };
            
            _itemsView.Add(_itemsTextView);
            
            // Input view (bottom-left)
            _inputView = new FrameView("Command")
            {
                X = 0,
                Y = 1 + gameHeight + itemsHeight,
                Width = leftWidth,
                Height = inputHeight,
                ColorScheme = BlueScheme
            };
            
            _inputField = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 2,
                Height = 1,
                ColorScheme = InputScheme
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
                Text = GetMapLegendText()
            };
            
            _legendView.Add(_legendTextView);
            
            // Set initial focus to input field
            _inputField.SetFocus();
        }
        
        private void ShowWelcomeScreen()
        {
            var welcomeText = @"WELCOME TO ADVENTURE REALMS

Classic text adventure games - explore, solve puzzles, escape!

TO START: Use Play menu -> New Game
COMMANDS: n/s/e/w/u/d, look, get/drop item, inv, use item, help
GAMES: Adventure House, Space Station, Future Family

Type commands in green box below. Map appears on right when playing.";

            _gameTextView.Text = welcomeText;
            _gameView.Title = "Welcome"; // Set title for welcome screen
            
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
                }
                else
                {
                    // User cancelled - show message in game area
                    _gameTextView.Text = "Game selection cancelled. Use Play -> New Game to try again.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to start new game: {ex.Message}", "OK");
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
                
                // Clear previous game text when starting new game
                _gameTextView.Text = "";
                
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
            if (args.KeyEvent.Key == Key.Enter)
            {
                var command = _inputField.Text.ToString();
                if (!string.IsNullOrWhiteSpace(command))
                {
                    if (!_gameInProgress)
                    {
                        MessageBox.Query("No Game", "Please start a game first using Play → New Game", "OK");
                        _inputField.Text = "";
                        return;
                    }
                    
                    ProcessCommand(command);
                    _commandHistory.Add(command);
                    _historyIndex = _commandHistory.Count;
                    _inputField.Text = "";
                }
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
        
        private void ProcessCommand(string command)
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
                // Add command to history display
                var existingText = _gameTextView.Text.ToString();
                _gameTextView.Text = existingText + $"\n> {command}\n";
                _gameTextView.MoveEnd();
                
                _lastGameResponse = _server.PlayGame(request);
                
                if (_lastGameResponse.SessionId == "-1")
                {
                    _gameTextView.Text += "\n*** GAME SESSION ENDED ***\n";
                    _currentSessionId = "-1";
                    _gameInProgress = false;
                }
                else
                {
                    UpdateGameDisplay();
                    UpdateMapDisplay();
                }
                
                if (_lastGameResponse.PlayerDead)
                {
                    MessageBox.Query("Game Over", "You have died! Game ended.", "OK");
                    _gameInProgress = false;
                }
                else if (_lastGameResponse.GameCompleted)
                {
                    MessageBox.Query("Congratulations!", "You have completed the game!", "OK");
                    _gameInProgress = false;
                }
            }
            catch (Exception ex)
            {
                _gameTextView.Text += $"\n*** ERROR: {ex.Message} ***\n";
                _gameTextView.MoveEnd();
            }
        }
        
        private void UpdateGameDisplay()
        {
            if (_lastGameResponse == null) return;
            
            // Update game view title to show current room name
            _gameView.Title = _lastGameResponse.RoomName;
            
            var gameText = new StringBuilder();
            
            // Add health status (no room name since it's in title)
            gameText.AppendLine($"Health: {_lastGameResponse.PlayerHealth}");
            gameText.AppendLine();
            
            // Add room description
            if (!string.IsNullOrEmpty(_lastGameResponse.RoomDescription))
            {
                gameText.AppendLine(_lastGameResponse.RoomDescription);
                gameText.AppendLine();
            }
            
            // Add command response without header - just the response
            if (!string.IsNullOrEmpty(_lastGameResponse.CommandResponse))
            {
                gameText.AppendLine(_lastGameResponse.CommandResponse);
                gameText.AppendLine();
            }
            
            // Update items in dedicated items box (not in main game text)
            UpdateItemsDisplay();
            
            // Always append to existing text for scrolling history
            var existingText = _gameTextView.Text.ToString();
            if (!string.IsNullOrEmpty(existingText) && _gameInProgress)
            {
                _gameTextView.Text = existingText + "\n" + gameText.ToString();
            }
            else
            {
                _gameTextView.Text = gameText.ToString();
            }
            
            // Auto-scroll to bottom to show latest content
            _gameTextView.MoveEnd();
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
        
        private void UpdateMapDisplay()
        {
            if (_lastGameResponse?.MapData == null)
            {
                _mapTextView.Text = "No map data available.";
                return;
            }
            
            // Create simple ASCII map from discovered rooms
            var mapText = CreateSimpleAsciiMap(_lastGameResponse.MapData);
            _mapTextView.Text = mapText;
        }
        
        private string CreateSimpleAsciiMap(PlayerMapData mapData)
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
v = Down      Enter = Send";
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
            }
        }
        
        private void RestartGame()
        {
            if (_currentGameId == 0)
            {
                MessageBox.Query("No Game", "Please start a game first using Play → New Game", "OK");
                return;
            }
            
            StartGame(_currentGameId);
            _gameInProgress = true;
            UpdateGameDisplay();
            UpdateMapDisplay();
        }
        
        private void RefreshDisplay()
        {
            if (_gameInProgress)
            {
                UpdateGameDisplay();
                UpdateMapDisplay();
            }
            else
            {
                ShowWelcomeScreen();
            }
            Application.Refresh();
        }
        
        private void ShowHelp()
        {
            var helpText = @"Adventure Realms - Terminal.Gui Interface

GAME COMMANDS:
- Movement: n, s, e, w, u, d (or full names)
- Actions: look, get <item>, drop <item>, use <item>
- Info: inv (inventory), help (game help)
- Combat: attack <monster> with <weapon>
- Other: eat <item>, read <item>, pet <animal>

INTERFACE:
- Type commands in the green input field
- Game text appears in the left panel
- Map shows on the right panel
- Legend shows keyboard shortcuts

MENU OPTIONS:
- Play → New Game: Start a new adventure
- Play → Switch Game: Change to different game
- Play → Restart Current: Restart current game
- View → Console Terminal: Classic terminal mode
- View → Help: Show this help";

            MessageBox.Query("Help", helpText, "OK");
        }
        
        private void ShowAbout()
        {
            var aboutText = @"Adventure Realms v1.0

A collection of classic text adventure games built with .NET 9.

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

Built by the Adventure House team using modern .NET technologies.";

            MessageBox.Query("About Adventure Realms", aboutText, "OK");
        }
        
        private void OpenTerminalWindow()
        {
            // Create a terminal window that opens the classic console client
            var terminalDialog = new Dialog("Console Terminal", 100, 30);
            terminalDialog.ColorScheme = BlueScheme;
            
            var terminalText = new TextView()
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 2,
                Height = Dim.Fill() - 4,
                ReadOnly = true,
                ColorScheme = BlueScheme,
                Text = "Console Terminal Mode\n\n" +
                      "This would open a classic console interface within this window.\n" +
                      "In a full implementation, this could run the classic AdventureClientService\n" +
                      "or provide a console-like interface for advanced commands."
            };
            
            var closeButton = new Button("Close")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(terminalText) + 1
            };
            
            closeButton.Clicked += () => Application.RequestStop();
            
            terminalDialog.Add(terminalText);
            terminalDialog.Add(closeButton);
            
            Application.Run(terminalDialog);
        }
    }
}