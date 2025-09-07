using System.Collections.Generic;
using System.Linq;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureClient.Models;

namespace AdventureRealms.Services.AdventureClient.UI.TerminalGui
{
    /// <summary>
    /// Manages game state for the Terminal.Gui Adventure Client
    /// </summary>
    public class TerminalGuiGameState
    {
        // Game State
        public string CurrentSessionId { get; set; } = "";
        public int CurrentGameId { get; set; } = 0; // 0 = no game selected
        public List<Game> AvailableGames { get; set; } = new();
        public MapModel? CurrentMapModel { get; set; }
        public GamePlayResponse? LastGameResponse { get; set; }
        public List<string> CommandHistory { get; set; } = new();
        public int HistoryIndex { get; set; } = -1;
        public bool GameInProgress { get; set; } = false;
        public string PlayerGamerTag { get; set; } = "Player"; // Default gamer tag
        
        // UI State
        public string CurrentRoomName { get; set; } = "";
        public string OriginalGameViewTitle { get; set; } = "Welcome";
        public string CurrentGameWelcomeMessage { get; set; } = "";
        public bool ScrollMode { get; set; } = true; // Default to scroll mode enabled

        /// <summary>
        /// Reset the game state for a new game
        /// </summary>
        public void ResetForNewGame()
        {
            CurrentSessionId = "";
            CurrentGameId = 0;
            CurrentMapModel = null;
            LastGameResponse = null;
            GameInProgress = false;
            CurrentRoomName = "";
            OriginalGameViewTitle = "Welcome";
            CurrentGameWelcomeMessage = "";
        }

        /// <summary>
        /// Update state after receiving a game response
        /// </summary>
        public void UpdateFromGameResponse(GamePlayResponse response)
        {
            LastGameResponse = response;
            if (response != null)
            {
                CurrentSessionId = response.SessionId;
                CurrentRoomName = response.RoomName;
                OriginalGameViewTitle = response.RoomName;
                
                // Update player gamer tag from server response
                if (!string.IsNullOrEmpty(response.PlayerName))
                {
                    PlayerGamerTag = response.PlayerName;
                }
                
                // Store welcome message from server response
                if (!string.IsNullOrEmpty(response.WelcomeMessage))
                {
                    CurrentGameWelcomeMessage = response.WelcomeMessage;
                }
            }
        }

        /// <summary>
        /// Add command to history
        /// </summary>
        public void AddCommandToHistory(string command)
        {
            CommandHistory.Add(command);
            HistoryIndex = CommandHistory.Count;
        }

        /// <summary>
        /// Get previous command from history
        /// </summary>
        public string? GetPreviousCommand()
        {
            if (HistoryIndex > 0)
            {
                HistoryIndex--;
                return CommandHistory[HistoryIndex];
            }
            return null;
        }

        /// <summary>
        /// Get next command from history
        /// </summary>
        public string? GetNextCommand()
        {
            if (HistoryIndex < CommandHistory.Count - 1)
            {
                HistoryIndex++;
                return CommandHistory[HistoryIndex];
            }
            else if (HistoryIndex == CommandHistory.Count - 1)
            {
                HistoryIndex = CommandHistory.Count;
                return ""; // Empty string for clearing input
            }
            return null;
        }

        /// <summary>
        /// Get current game information
        /// </summary>
        public Game? GetCurrentGame()
        {
            return AvailableGames.FirstOrDefault(g => g.Id == CurrentGameId);
        }

        /// <summary>
        /// Check if session is valid
        /// </summary>
        public bool IsSessionValid()
        {
            return !string.IsNullOrEmpty(CurrentSessionId) && CurrentSessionId != "-1";
        }
    }
}