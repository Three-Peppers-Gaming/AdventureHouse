using System;
using Terminal.Gui;
using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.Shared.Models;

namespace AdventureRealms.Services.AdventureClient.UI.TerminalGui
{
    /// <summary>
    /// Handles input and events for Terminal.Gui Adventure Client
    /// </summary>
    public class TerminalGuiEventHandlers
    {
        private readonly IPlayAdventure _server;
        private readonly TerminalGuiGameState _gameState;
        private readonly TerminalGuiUIComponents _uiComponents;
        private readonly TerminalGuiFocusManager _focusManager;

        public TerminalGuiEventHandlers(
            IPlayAdventure server,
            TerminalGuiGameState gameState,
            TerminalGuiUIComponents uiComponents,
            TerminalGuiFocusManager focusManager)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _uiComponents = uiComponents ?? throw new ArgumentNullException(nameof(uiComponents));
            _focusManager = focusManager ?? throw new ArgumentNullException(nameof(focusManager));
        }

        /// <summary>
        /// Set up input field key press handler
        /// </summary>
        public void SetupInputFieldKeyPress()
        {
            _uiComponents.InputField.KeyPress += OnInputKeyPress;
        }

        /// <summary>
        /// Set up game text view key press handler to return focus to input
        /// </summary>
        public void SetupGameTextViewKeyPress()
        {
            _uiComponents.GameTextView.KeyPress += (args) =>
            {
                // ANY key press in game text should return focus to command input
                _focusManager.SetInputFieldShouldHaveFocus(true);
                args.Handled = true;
            };
        }

        /// <summary>
        /// Handle input field key press events
        /// </summary>
        private void OnInputKeyPress(View.KeyEventEventArgs args)
        {
            // Handle Tab key - just ignore it
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
                _uiComponents.InputField.Text.Length >= 50)
            {
                args.Handled = true;
                return;
            }
            
            if (args.KeyEvent.Key == Key.Enter)
            {
                HandleEnterKey();
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.CursorUp)
            {
                HandleUpArrow();
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.CursorDown)
            {
                HandleDownArrow();
                args.Handled = true;
            }
        }

        /// <summary>
        /// Handle Enter key press
        /// </summary>
        private void HandleEnterKey()
        {
            var command = _uiComponents.InputField.Text.ToString();
            if (!string.IsNullOrWhiteSpace(command))
            {
                if (!_gameState.GameInProgress)
                {
                    MessageBox.Query("No Game", "Please start a game first using Play ? New Game", "OK");
                    _uiComponents.InputField.Text = "";
                    _focusManager.SetInputFieldShouldHaveFocus(true);
                    return;
                }
                
                ProcessCommand(command);
                _gameState.AddCommandToHistory(command);
                _uiComponents.InputField.Text = "";
            }
            
            _focusManager.SetInputFieldShouldHaveFocus(true);
        }

        /// <summary>
        /// Handle up arrow key (command history - previous)
        /// </summary>
        private void HandleUpArrow()
        {
            var previousCommand = _gameState.GetPreviousCommand();
            if (previousCommand != null)
            {
                _uiComponents.InputField.Text = previousCommand;
            }
        }

        /// <summary>
        /// Handle down arrow key (command history - next)
        /// </summary>
        private void HandleDownArrow()
        {
            var nextCommand = _gameState.GetNextCommand();
            if (nextCommand != null)
            {
                _uiComponents.InputField.Text = nextCommand;
            }
        }

        /// <summary>
        /// Process game command
        /// </summary>
        private void ProcessCommand(string command)
        {
            if (!_gameState.IsSessionValid())
            {
                _uiComponents.GameTextView.Text = "Game session ended. Please restart or start a new game using Play menu.";
                _gameState.GameInProgress = false;
                return;
            }
            
            var request = new GamePlayRequest
            {
                SessionId = _gameState.CurrentSessionId,
                Command = command
            };
            
            try
            {
                var response = _server.PlayGame(request);
                _gameState.UpdateFromGameResponse(response);
                
                if (response.SessionId == "-1")
                {
                    _uiComponents.GameTextView.Text = "*** GAME SESSION ENDED ***";
                    _gameState.CurrentSessionId = "-1";
                    _gameState.GameInProgress = false;
                }
                else
                {
                    UpdateAllDisplays();
                }
                
                if (response.PlayerDead)
                {
                    MessageBox.Query("Game Over", "You have died! Game ended.", "OK");
                    _gameState.GameInProgress = false;
                    _focusManager.SetInputFieldShouldHaveFocus(true);
                }
                else if (response.GameCompleted)
                {
                    MessageBox.Query("Congratulations!", "You have completed the game!", "OK");
                    _gameState.GameInProgress = false;
                    _focusManager.SetInputFieldShouldHaveFocus(true);
                }
                
                _focusManager.SetInputFieldShouldHaveFocus(true);
            }
            catch (Exception ex)
            {
                _uiComponents.GameTextView.Text = $"*** ERROR: {ex.Message} ***";
                _focusManager.SetInputFieldShouldHaveFocus(true);
            }
        }

        /// <summary>
        /// Update all display components
        /// </summary>
        private void UpdateAllDisplays()
        {
            _uiComponents.UpdateGameDisplay(_gameState.LastGameResponse, _gameState.OriginalGameViewTitle);
            _uiComponents.UpdateItemsDisplay(_gameState.LastGameResponse);
            _uiComponents.UpdateGameInfo(_gameState.LastGameResponse, _gameState.GetCurrentGame(), _gameState.GameInProgress);
            _uiComponents.UpdateMapDisplay(_gameState.LastGameResponse?.MapData);
        }
    }
}