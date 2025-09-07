using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureRealms.Services.Shared.Models
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
        /// Current player position and map discovery data for client rendering.
        /// Only contains what the client needs to know - not full game state.
        /// </summary>
        public PlayerMapData? MapData { get; set; }
        
        /// <summary>
        /// Command history for client display
        /// </summary>
        public List<string>? CommandHistory { get; set; }
        
        /// <summary>
        /// Available games list (for game selection)
        /// </summary>
        public List<Game>? AvailableGames { get; set; }
    }

    /// <summary>
    /// Request model for PlayGame endpoint
    /// Contains everything needed for any game interaction
    /// </summary>
    public class GamePlayRequest
    {
        /// <summary>
        /// Session identifier - empty string for new game
        /// </summary>
        public string SessionId { get; set; } = string.Empty;
        
        /// <summary>
        /// Game ID to start (only used when SessionId is empty for new game)
        /// </summary>
        public int GameId { get; set; }
        
        /// <summary>
        /// Player command/action (empty for new game)
        /// </summary>
        public string Command { get; set; } = string.Empty;
        
        /// <summary>
        /// Client preferences (optional)
        /// </summary>
        public bool UseClassicMode { get; set; } = false;
        public bool UseScrollMode { get; set; } = false;
    }

    /// <summary>
    /// Response model for PlayGame endpoint
    /// Contains everything client needs to render current game state
    /// </summary>
    public class GamePlayResponse
    {
        /// <summary>
        /// Session identifier for subsequent requests
        /// "-1" indicates session ended/error
        /// </summary>
        public string SessionId { get; set; } = string.Empty;
        
        /// <summary>
        /// Current game state information
        /// </summary>
        public string GameName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string RoomDescription { get; set; } = string.Empty;
        public string ItemsInRoom { get; set; } = string.Empty;
        public string PlayerHealth { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        
        /// <summary>
        /// Response to player's command
        /// </summary>
        public string CommandResponse { get; set; } = string.Empty;
        
        /// <summary>
        /// Welcome message for new game sessions (only sent on game start)
        /// </summary>
        public string WelcomeMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// Map data for client rendering (only discovered areas)
        /// </summary>
        public PlayerMapData? MapData { get; set; }
        
        /// <summary>
        /// Game status flags
        /// </summary>
        public bool GameCompleted { get; set; } = false;
        public bool PlayerDead { get; set; } = false;
        public bool InvalidCommand { get; set; } = false;
        
        /// <summary>
        /// Available games (only sent on session start)
        /// </summary>
        public List<Game>? AvailableGames { get; set; }
    }

    /// <summary>
    /// Represents only the map information needed by the client to render the player's current state.
    /// This is a deliberate subset of the full game state - clients should not have access to the complete map.
    /// </summary>
    public class PlayerMapData
    {
        /// <summary>
        /// Current room number where the player is located
        /// </summary>
        public int CurrentRoom { get; set; }
        
        /// <summary>
        /// Current level/floor identifier for map organization
        /// </summary>
        public string CurrentLevel { get; set; } = string.Empty;
        
        /// <summary>
        /// Display name for the current level (e.g., "Ground Floor", "Attic")
        /// </summary>
        public string CurrentLevelDisplayName { get; set; } = string.Empty;
        
        /// <summary>
        /// List of all rooms the player has discovered/visited.
        /// Only visited rooms should be rendered by the client.
        /// </summary>
        public List<DiscoveredRoom> DiscoveredRooms { get; set; } = new();
        
        /// <summary>
        /// Total count of rooms the player has visited (for statistics)
        /// </summary>
        public int VisitedRoomCount { get; set; }
        
        /// <summary>
        /// Game-specific configuration data needed for map rendering
        /// (room symbols, level organization, etc.)
        /// </summary>
        public MapRenderingConfig RenderingConfig { get; set; } = new();
    }

    /// <summary>
    /// Represents a room that the player has discovered and can be rendered on the map.
    /// Contains only the information needed for client-side map rendering.
    /// </summary>
    public class DiscoveredRoom
    {
        /// <summary>
        /// Unique room identifier
        /// </summary>
        public int RoomNumber { get; set; }
        
        /// <summary>
        /// Room display name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Level/floor this room belongs to
        /// </summary>
        public string Level { get; set; } = string.Empty;
        
        /// <summary>
        /// Suggested display position for this room (x, y coordinates)
        /// </summary>
        public MapPosition Position { get; set; } = new();
        
        /// <summary>
        /// Character/symbol to display for this room type
        /// </summary>
        public char DisplayChar { get; set; } = '.';
        
        /// <summary>
        /// Whether this room currently contains items (for + indicator)
        /// </summary>
        public bool HasItems { get; set; }
        
        /// <summary>
        /// Whether this is the player's current location
        /// </summary>
        public bool IsCurrentLocation { get; set; }
        
        /// <summary>
        /// Discovered connections to other rooms (only show connected rooms that have been visited)
        /// </summary>
        public List<RoomConnection> Connections { get; set; } = new();
    }

    /// <summary>
    /// Represents a connection between two discovered rooms
    /// </summary>
    public class RoomConnection
    {
        /// <summary>
        /// Direction of the connection (north, south, east, west, up, down)
        /// </summary>
        public string Direction { get; set; } = string.Empty;
        
        /// <summary>
        /// Target room number for this connection
        /// </summary>
        public int TargetRoom { get; set; }
        
        /// <summary>
        /// Whether the target room has been discovered/visited
        /// (only show connections to visited rooms)
        /// </summary>
        public bool TargetRoomDiscovered { get; set; }
    }

    /// <summary>
    /// Map position coordinates for room placement
    /// </summary>
    public class MapPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>
    /// Configuration data needed by clients to render maps consistently
    /// </summary>
    public class MapRenderingConfig
    {
        /// <summary>
        /// Game name for display
        /// </summary>
        public string GameName { get; set; } = string.Empty;
        
        /// <summary>
        /// Mapping of room types to display characters
        /// </summary>
        public Dictionary<string, char> RoomTypeChars { get; set; } = new();
        
        /// <summary>
        /// Mapping of level names to display names
        /// </summary>
        public Dictionary<string, string> LevelDisplayNames { get; set; } = new();
        
        /// <summary>
        /// Default character for unknown room types
        /// </summary>
        public char DefaultRoomChar { get; set; } = '.';
        
        /// <summary>
        /// Symbol for player's current location
        /// </summary>
        public char PlayerChar { get; set; } = '@';
        
        /// <summary>
        /// Symbol for rooms with items
        /// </summary>
        public char ItemsChar { get; set; } = '+';
    }
}