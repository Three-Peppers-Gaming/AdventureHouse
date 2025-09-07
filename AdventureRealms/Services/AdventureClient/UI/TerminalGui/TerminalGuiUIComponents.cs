using System;
using System.Linq;
using System.Text;
using Terminal.Gui;
using AdventureRealms.Services.Shared.Models;

namespace AdventureRealms.Services.AdventureClient.UI.TerminalGui
{
    /// <summary>
    /// Manages UI component creation and layout for Terminal.Gui Adventure Client
    /// </summary>
    public class TerminalGuiUIComponents
    {
        // UI Components
        public MenuBar MenuBar { get; private set; } = null!;
        public FrameView GameView { get; private set; } = null!;
        public FrameView MapView { get; private set; } = null!;
        public FrameView InputView { get; private set; } = null!;
        public FrameView LegendView { get; private set; } = null!;
        public FrameView ItemsView { get; private set; } = null!;
        public FrameView GameInfoView { get; private set; } = null!;
        public TextView GameTextView { get; private set; } = null!;
        public TextView MapTextView { get; private set; } = null!;
        public TextField InputField { get; private set; } = null!;
        public TextView LegendTextView { get; private set; } = null!;
        public TextView ItemsTextView { get; private set; } = null!;
        public TextView GameInfoTextView { get; private set; } = null!;

        /// <summary>
        /// Create the menu bar
        /// </summary>
        public void CreateMenuBar(
            Action startNewGameAction,
            Action restartGameAction,
            Action switchGameAction,
            Action showHelpAction,
            Action showGameWelcomeAction,
            Action showAboutAction,
            Action quitAction)
        {
            var playMenu = new MenuBarItem("_Play", new MenuItem[]
            {
                new MenuItem("_New Game", "Start a new adventure", startNewGameAction),
                new MenuItem("_Restart Current", "Restart current game", restartGameAction),
                null, // Separator
                new MenuItem("_Switch Game", "Switch to a different adventure", switchGameAction),
                null, // Separator
                new MenuItem("_Quit", "Exit Adventure Realms", quitAction)
            });
            
            var helpMenu = new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_Game Help", "Show game help (F1)", showHelpAction),
                new MenuItem("Game _Welcome", "Show current game's welcome message", showGameWelcomeAction),
                null, // Separator
                new MenuItem("_About", "About Adventure Realms", showAboutAction)
            });
            
            MenuBar = new MenuBar(new MenuBarItem[] { playMenu, helpMenu })
            {
                ColorScheme = TerminalGuiColorSchemes.MenuScheme
            };
        }

        /// <summary>
        /// Create all game view components
        /// </summary>
        public void CreateGameViews(TerminalGuiFocusManager? focusManager = null)
        {
            CalculateLayout(out var dimensions);
            
            CreateGameView(dimensions);
            CreateMapView(dimensions);
            CreateItemsView(dimensions);
            CreateGameInfoView(dimensions);
            CreateInputView(dimensions, focusManager);
            CreateLegendView(dimensions);
        }

        /// <summary>
        /// Set up focus manager after components are created
        /// </summary>
        public void SetupFocusManager(TerminalGuiFocusManager focusManager)
        {
            InputView.MouseClick += (args) =>
            {
                focusManager.OnInputViewClicked();
                args.Handled = true;
            };
            
            InputField.MouseClick += (args) =>
            {
                focusManager.OnInputViewClicked();
                args.Handled = true;
            };
            
            InputField.Enter += (sender) => focusManager.OnInputFieldEnter();
            InputField.Leave += (sender) => focusManager.OnInputFieldLeave();
        }

        /// <summary>
        /// Add all components to the application
        /// </summary>
        public void AddComponentsToApplication()
        {
            Application.Top.Add(MenuBar);
            Application.Top.Add(GameView);
            Application.Top.Add(MapView);
            Application.Top.Add(ItemsView);
            Application.Top.Add(InputView);
            Application.Top.Add(GameInfoView);
            Application.Top.Add(LegendView);
        }

        /// <summary>
        /// Show the welcome screen
        /// </summary>
        public void ShowWelcomeScreen()
        {
            var welcomeText = @"WELCOME TO ADVENTURE REALMS

Classic text adventure games - explore, solve puzzles, escape!

TO START: Use Play menu -> New Game
COMMANDS: n/s/e/w/u/d, look, get/drop item, inv, use item, help
GAMES: Adventure House, Space Station, Future Family

Type commands in green box below. Map appears on right when playing.

Copyright (c) Steven Sparks 2025.";

            GameTextView.Text = welcomeText;
            GameView.Title = "Welcome";
            
            MapTextView.Text = @"MAP AREA

Your map will
show here when
you start a game.

@ = You
+ = Items
Connections
shown between
visited rooms.";
            
            ItemsTextView.Text = "Items will appear here when you enter rooms.";
            ItemsView.Title = "Items";
            
            GameInfoTextView.Text = "No game selected";
        }

        /// <summary>
        /// Update game display with latest response
        /// </summary>
        public void UpdateGameDisplay(GamePlayResponse? response, string originalGameViewTitle)
        {
            if (response == null) return;
            
            GameView.Title = originalGameViewTitle;
            
            var gameText = new StringBuilder();
            
            // Prioritize command response over room description to avoid duplication
            if (!string.IsNullOrEmpty(response.CommandResponse))
            {
                gameText.AppendLine(response.CommandResponse);
                gameText.AppendLine();
            }
            else if (!string.IsNullOrEmpty(response.RoomDescription))
            {
                gameText.AppendLine(response.RoomDescription);
                gameText.AppendLine();
            }
            
            GameTextView.Text = gameText.ToString();
            GameTextView.MoveEnd();
        }

        /// <summary>
        /// Update items display
        /// </summary>
        public void UpdateItemsDisplay(GamePlayResponse? response)
        {
            if (response == null)
            {
                ItemsTextView.Text = "Nothing here.";
                return;
            }
            
            if (!string.IsNullOrEmpty(response.ItemsInRoom) && 
                response.ItemsInRoom != "No Items")
            {
                ItemsTextView.Text = response.ItemsInRoom;
                ItemsView.Title = "You see:";
            }
            else
            {
                ItemsTextView.Text = "Nothing here.";
                ItemsView.Title = "You see:";
            }
        }

        /// <summary>
        /// Update game info display
        /// </summary>
        public void UpdateGameInfo(GamePlayResponse? response, Game? currentGame, bool gameInProgress)
        {
            if (response == null || !gameInProgress)
            {
                GameInfoTextView.Text = "No game selected";
                return;
            }
            
            if (currentGame == null)
            {
                GameInfoTextView.Text = "Unknown game";
                return;
            }
            
            var gameInfo = $"Playing {currentGame.Name} and feeling {response.PlayerHealth}\nBy Steve Sparks 2019 to 2025!";
            GameInfoTextView.Text = gameInfo;
        }

        /// <summary>
        /// Update map display
        /// </summary>
        public void UpdateMapDisplay(PlayerMapData? mapData)
        {
            if (mapData == null)
            {
                MapTextView.Text = "No map data available.";
                return;
            }
            
            var mapText = CreateSimpleMap(mapData);
            MapTextView.Text = mapText;
        }

        private void CalculateLayout(out LayoutDimensions dimensions)
        {
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

            dimensions = new LayoutDimensions
            {
                TotalWidth = totalWidth,
                TotalHeight = totalHeight,
                LeftWidth = leftWidth,
                RightWidth = rightWidth,
                GameHeight = gameHeight,
                BottomHeight = bottomHeight,
                ItemsHeight = itemsHeight,
                GameInfoHeight = gameInfoHeight,
                InputHeight = inputHeight
            };
        }

        private void CreateGameView(LayoutDimensions dim)
        {
            GameView = new FrameView("Welcome")
            {
                X = 0,
                Y = 1, // Below menu bar
                Width = dim.LeftWidth,
                Height = dim.GameHeight,
                ColorScheme = TerminalGuiColorSchemes.BlueScheme
            };
            
            GameTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                ColorScheme = TerminalGuiColorSchemes.GameScheme,
                Text = "",
                CanFocus = false
            };
            
            GameView.Add(GameTextView);
        }

        private void CreateMapView(LayoutDimensions dim)
        {
            MapView = new FrameView("Map")
            {
                X = dim.LeftWidth,
                Y = 1, // Below menu bar
                Width = dim.RightWidth,
                Height = dim.GameHeight,
                ColorScheme = TerminalGuiColorSchemes.BlueScheme
            };
            
            MapTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = false,
                ColorScheme = TerminalGuiColorSchemes.MapScheme,
                CanFocus = false
            };
            
            MapView.Add(MapTextView);
        }

        private void CreateItemsView(LayoutDimensions dim)
        {
            ItemsView = new FrameView("You see:")
            {
                X = 0,
                Y = 1 + dim.GameHeight,
                Width = dim.LeftWidth,
                Height = dim.ItemsHeight,
                ColorScheme = TerminalGuiColorSchemes.BlueScheme
            };
            
            ItemsTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                ColorScheme = TerminalGuiColorSchemes.ItemsScheme,
                Text = "Nothing here.",
                CanFocus = false
            };
            
            ItemsView.Add(ItemsTextView);
        }

        private void CreateGameInfoView(LayoutDimensions dim)
        {
            GameInfoView = new FrameView("Game")
            {
                X = 0,
                Y = 1 + dim.GameHeight + dim.ItemsHeight,
                Width = dim.LeftWidth,
                Height = dim.GameInfoHeight,
                ColorScheme = TerminalGuiColorSchemes.BlueScheme
            };
            
            GameInfoTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                ColorScheme = TerminalGuiColorSchemes.GameInfoScheme,
                Text = "No game selected",
                CanFocus = false
            };
            
            GameInfoView.Add(GameInfoTextView);
        }

        private void CreateInputView(LayoutDimensions dim, TerminalGuiFocusManager? focusManager)
        {
            InputView = new FrameView("Command")
            {
                X = 0,
                Y = 1 + dim.GameHeight + dim.ItemsHeight + dim.GameInfoHeight,
                Width = dim.LeftWidth,
                Height = dim.InputHeight,
                ColorScheme = TerminalGuiColorSchemes.BlueScheme
            };
            
            InputField = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 2,
                Height = 1,
                ColorScheme = TerminalGuiColorSchemes.InputScheme,
                CanFocus = true
            };
            
            // Focus manager events will be set up later in SetupFocusManager
            InputView.Add(InputField);
        }

        private void CreateLegendView(LayoutDimensions dim)
        {
            LegendView = new FrameView("Keys")
            {
                X = dim.LeftWidth,
                Y = 1 + dim.GameHeight,
                Width = dim.RightWidth,
                Height = dim.BottomHeight,
                ColorScheme = TerminalGuiColorSchemes.BlueScheme
            };
            
            LegendTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = false,
                ColorScheme = TerminalGuiColorSchemes.MapScheme,
                Text = GetMapLegendText(),
                CanFocus = false
            };
            
            LegendView.Add(LegendTextView);
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
                foreach (var room in mapData.DiscoveredRooms.Take(10))
                {
                    var marker = room.IsCurrentLocation ? "@ " : "  ";
                    var items = room.HasItems ? " [+]" : "";
                    mapBuilder.AppendLine($"{marker}{room.Name}{items}");
                }
            }
            
            return mapBuilder.ToString();
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

        private struct LayoutDimensions
        {
            public int TotalWidth;
            public int TotalHeight;
            public int LeftWidth;
            public int RightWidth;
            public int GameHeight;
            public int BottomHeight;
            public int ItemsHeight;
            public int GameInfoHeight;
            public int InputHeight;
        }
    }
}