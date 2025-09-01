using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureHouse.Services.Shared.Models
{
    public class GameMoveResult
    {
        public string InstanceID { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string RoomMessage { get; set; } = string.Empty;
        public string ItemsMessage { get; set; } = string.Empty;
        public string HealthReport { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        
        /// <summary>
        /// Special console output that should be displayed to the user
        /// (for console commands like /help, /map, etc.)
        /// </summary>
        public string ConsoleOutput { get; set; } = string.Empty;
        
        /// <summary>
        /// Indicates if the client should toggle classic mode
        /// </summary>
        public bool ToggleClassicMode { get; set; } = false;
        
        /// <summary>
        /// Indicates if the client should toggle scroll mode
        /// </summary>
        public bool ToggleScrollMode { get; set; } = false;
        
        /// <summary>
        /// Indicates if the client should clear the display
        /// </summary>
        public bool ClearDisplay { get; set; } = false;
        
        /// <summary>
        /// Map data for client display (when map command is used)
        /// </summary>
        public object? MapData { get; set; }
        
        /// <summary>
        /// Command history for client display
        /// </summary>
        public List<string>? CommandHistory { get; set; }
        
        /// <summary>
        /// Available games list (for game selection)
        /// </summary>
        public List<Game>? AvailableGames { get; set; }
    }
}