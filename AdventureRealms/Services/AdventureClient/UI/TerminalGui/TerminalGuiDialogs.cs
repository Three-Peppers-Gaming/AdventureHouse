using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using AdventureRealms.Services.Shared.Models;

namespace AdventureRealms.Services.AdventureClient.UI.TerminalGui
{
    /// <summary>
    /// Handles dialog creation and management for Terminal.Gui Adventure Client
    /// </summary>
    public static class TerminalGuiDialogs
    {
        /// <summary>
        /// Show game selection dialog
        /// </summary>
        public static int ShowGameSelection(List<Game> availableGames)
        {
            try
            {
                var gameSelectionDialog = new Dialog("Select Game", 70, 15);
                gameSelectionDialog.ColorScheme = TerminalGuiColorSchemes.BlueScheme;
                
                // Create a simple list of game names only (not descriptions)
                var gameNames = availableGames.Select(g => g.Name).ToArray();
                
                var gameList = new ListView(gameNames)
                {
                    X = 1,
                    Y = 1,
                    Width = Dim.Fill() - 2,
                    Height = Dim.Fill() - 4,
                    ColorScheme = TerminalGuiColorSchemes.BlueScheme,
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
                        if (selectedIndex >= 0 && selectedIndex < availableGames.Count)
                        {
                            selectedGameId = availableGames[selectedIndex].Id;
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

        /// <summary>
        /// Show welcome message popup
        /// </summary>
        public static void ShowWelcomePopup(string welcomeMessage, string gameName)
        {
            if (string.IsNullOrEmpty(welcomeMessage))
            {
                return;
            }
            
            try
            {
                var welcomeDialog = new Dialog($"Welcome to {gameName}", 70, 15);
                welcomeDialog.ColorScheme = TerminalGuiColorSchemes.BlueScheme;
                
                var welcomeTextView = new TextView()
                {
                    X = 1,
                    Y = 1,
                    Width = Dim.Fill() - 2,
                    Height = Dim.Fill() - 4,
                    ReadOnly = true,
                    WordWrap = true,
                    ColorScheme = TerminalGuiColorSchemes.GameScheme,
                    Text = welcomeMessage
                };
                
                var okButton = new Button("OK")
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(welcomeTextView) + 1,
                    IsDefault = true
                };
                
                okButton.Clicked += () =>
                {
                    Application.RequestStop();
                };
                
                // Handle Enter and Escape keys
                welcomeDialog.KeyPress += (args) =>
                {
                    if (args.KeyEvent.Key == Key.Enter || args.KeyEvent.Key == Key.Esc)
                    {
                        Application.RequestStop();
                        args.Handled = true;
                    }
                };
                
                welcomeDialog.Add(welcomeTextView);
                welcomeDialog.Add(okButton);
                
                // Set initial focus to OK button
                okButton.SetFocus();
                
                Application.Run(welcomeDialog);
            }
            catch (Exception ex)
            {
                // If popup fails, fall back to simple message box
                MessageBox.Query("Welcome", welcomeMessage, "OK");
            }
        }

        /// <summary>
        /// Show help dialog
        /// </summary>
        public static void ShowHelp()
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
- Play ? New Game: Start a new adventure
- Play ? Switch Game: Change to different game
- Play ? Restart Current: Restart current game
- Help ? Game Help: Show this help
- Help ? Game Welcome: Show current game's welcome message
- Help ? About: About Adventure Realms";

            MessageBox.Query("Help", helpText, "OK");
        }

        /// <summary>
        /// Show about dialog
        /// </summary>
        public static void ShowAbout()
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
        }

        /// <summary>
        /// Show gamer tag input dialog
        /// </summary>
        public static string ShowGamerTagDialog(string currentGamerTag)
        {
            var gamerTagDialog = new Dialog("Enter Gamer Tag", 50, 8);
            gamerTagDialog.ColorScheme = TerminalGuiColorSchemes.BlueScheme;
            
            var label = new Label("Gamer Tag:")
            {
                X = 1,
                Y = 1
            };
            
            var tagField = new TextField(currentGamerTag)
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

            string result = currentGamerTag;
            
            okButton.Clicked += () =>
            {
                if (!string.IsNullOrWhiteSpace(tagField.Text.ToString()))
                {
                    result = tagField.Text.ToString().Trim();
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
            
            return result;
        }
    }
}