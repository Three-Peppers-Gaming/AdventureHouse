using System;
using System.Collections.Generic;
using System.Linq;
using AdventureRealms.Services.Data.AdventureData;
using AdventureRealms.Services.AdventureServer.Models;

namespace AdventureRealms.Services.AdventureClient.Models
{
    /// <summary>
    /// Data model for map rendering that holds room and path data sent by the server.
    /// This model is renderer-agnostic and can be used by different UI frameworks.
    /// </summary>
    public class MapModel
    {
        /// <summary>
        /// Collection of all rooms indexed by room number
        /// </summary>
        public Dictionary<int, MapRoomData> Rooms { get; private set; }

        /// <summary>
        /// Set of room numbers that have been visited by the player
        /// </summary>
        public HashSet<int> VisitedRooms { get; private set; }

        /// <summary>
        /// Current player location (room number)
        /// </summary>
        public int CurrentRoom { get; private set; }

        /// <summary>
        /// Current map level the player is on
        /// </summary>
        public MapLevel CurrentLevel { get; private set; }

        /// <summary>
        /// Game configuration for display settings
        /// </summary>
        public IGameConfiguration GameConfig { get; private set; }

        /// <summary>
        /// Rooms grouped by level for efficient level-based rendering
        /// </summary>
        public Dictionary<MapLevel, List<MapRoomData>> RoomsByLevel { get; private set; }

        public MapModel(IGameConfiguration gameConfig, List<Room> rooms, int startingRoom = 20)
        {
            GameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            Rooms = new Dictionary<int, MapRoomData>();
            VisitedRooms = new HashSet<int>();
            RoomsByLevel = new Dictionary<MapLevel, List<MapRoomData>>();
            CurrentRoom = startingRoom;
            CurrentLevel = gameConfig.GetLevelForRoom(startingRoom);

            InitializeFromRooms(rooms);
        }

        /// <summary>
        /// Initialize the map model from server room data
        /// </summary>
        private void InitializeFromRooms(List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                var position = GameConfig.GetRoomPosition(room.Number);
                var level = GameConfig.GetLevelForRoom(room.Number);
                
                var roomData = new MapRoomData
                {
                    RoomNumber = room.Number,
                    RoomName = room.Name ?? $"Room {room.Number}",
                    X = position.X,
                    Y = position.Y,
                    Level = level,
                    DisplayChar = GameConfig.GetRoomDisplayChar(room.Name ?? ""),
                    IsVisited = false,
                    IsCurrentRoom = room.Number == CurrentRoom,
                    HasVisibleItems = false,
                    // Connection data for path rendering
                    North = room.N,
                    South = room.S,
                    East = room.E,
                    West = room.W,
                    Up = room.U,
                    Down = room.D
                };

                Rooms[room.Number] = roomData;

                // Group by level for efficient rendering
                if (!RoomsByLevel.ContainsKey(level))
                    RoomsByLevel[level] = new List<MapRoomData>();
                RoomsByLevel[level].Add(roomData);
            }
        }

        /// <summary>
        /// Update player position and mark room as visited
        /// </summary>
        public void UpdatePlayerPosition(int roomNumber)
        {
            // Mark old room as not current
            if (Rooms.ContainsKey(CurrentRoom))
            {
                Rooms[CurrentRoom].IsCurrentRoom = false;
            }

            CurrentRoom = roomNumber;
            CurrentLevel = GameConfig.GetLevelForRoom(roomNumber);
            VisitedRooms.Add(roomNumber);

            // Mark new room as current and visited
            if (Rooms.ContainsKey(CurrentRoom))
            {
                Rooms[CurrentRoom].IsVisited = true;
                Rooms[CurrentRoom].IsCurrentRoom = true;
            }
        }

        /// <summary>
        /// Update items visibility for a room
        /// </summary>
        public void UpdateRoomItems(int roomNumber, bool hasVisibleItems)
        {
            if (Rooms.ContainsKey(roomNumber))
            {
                Rooms[roomNumber].HasVisibleItems = hasVisibleItems;
            }
        }

        /// <summary>
        /// Get all visited rooms for the current level
        /// </summary>
        public List<MapRoomData> GetVisitedRoomsForCurrentLevel()
        {
            if (!RoomsByLevel.ContainsKey(CurrentLevel))
                return new List<MapRoomData>();

            return RoomsByLevel[CurrentLevel]
                .Where(room => VisitedRooms.Contains(room.RoomNumber))
                .ToList();
        }

        /// <summary>
        /// Get all visited rooms for a specific level
        /// </summary>
        public List<MapRoomData> GetVisitedRoomsForLevel(MapLevel level)
        {
            if (!RoomsByLevel.ContainsKey(level))
                return new List<MapRoomData>();

            return RoomsByLevel[level]
                .Where(room => VisitedRooms.Contains(room.RoomNumber))
                .ToList();
        }

        /// <summary>
        /// Get the current room name
        /// </summary>
        public string GetCurrentRoomName()
        {
            return Rooms.ContainsKey(CurrentRoom) 
                ? Rooms[CurrentRoom].RoomName 
                : UIConfiguration.UnknownRoomMessage;
        }

        /// <summary>
        /// Get map bounds for a specific level (for rendering calculations)
        /// </summary>
        public (int MinX, int MaxX, int MinY, int MaxY) GetLevelBounds(MapLevel level)
        {
            var levelRooms = GetVisitedRoomsForLevel(level);
            if (!levelRooms.Any())
                return (0, 0, 0, 0);

            return (
                levelRooms.Min(r => r.X),
                levelRooms.Max(r => r.X),
                levelRooms.Min(r => r.Y),
                levelRooms.Max(r => r.Y)
            );
        }

        /// <summary>
        /// Get connection data for path rendering between two rooms
        /// </summary>
        public List<MapConnection> GetConnectionsForRoom(int roomNumber)
        {
            if (!Rooms.ContainsKey(roomNumber))
                return new List<MapConnection>();

            var room = Rooms[roomNumber];
            var connections = new List<MapConnection>();

            // Only show connections to visited rooms
            if (room.North != GameConfig.StartingRoom && VisitedRooms.Contains(room.North))
                connections.Add(new MapConnection { FromRoom = roomNumber, ToRoom = room.North, Direction = "north" });

            if (room.South != GameConfig.StartingRoom && VisitedRooms.Contains(room.South))
                connections.Add(new MapConnection { FromRoom = roomNumber, ToRoom = room.South, Direction = "south" });

            if (room.East != GameConfig.StartingRoom && VisitedRooms.Contains(room.East))
                connections.Add(new MapConnection { FromRoom = roomNumber, ToRoom = room.East, Direction = "east" });

            if (room.West != GameConfig.StartingRoom && VisitedRooms.Contains(room.West))
                connections.Add(new MapConnection { FromRoom = roomNumber, ToRoom = room.West, Direction = "west" });

            if (room.Up != GameConfig.StartingRoom && VisitedRooms.Contains(room.Up))
                connections.Add(new MapConnection { FromRoom = roomNumber, ToRoom = room.Up, Direction = "up" });

            if (room.Down != GameConfig.StartingRoom && VisitedRooms.Contains(room.Down))
                connections.Add(new MapConnection { FromRoom = roomNumber, ToRoom = room.Down, Direction = "down" });

            return connections;
        }
    }

    /// <summary>
    /// Represents a single room in the map with all necessary rendering data
    /// </summary>
    public class MapRoomData
    {
        public int RoomNumber { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public MapLevel Level { get; set; }
        public char DisplayChar { get; set; } = '.';
        public bool IsVisited { get; set; }
        public bool IsCurrentRoom { get; set; }
        public bool HasVisibleItems { get; set; }

        // Connection data for path rendering
        public int North { get; set; } = 99;
        public int South { get; set; } = 99;
        public int East { get; set; } = 99;
        public int West { get; set; } = 99;
        public int Up { get; set; } = 99;
        public int Down { get; set; } = 99;
    }

    /// <summary>
    /// Represents a connection between two rooms for path rendering
    /// </summary>
    public class MapConnection
    {
        public int FromRoom { get; set; }
        public int ToRoom { get; set; }
        public string Direction { get; set; } = string.Empty;
    }
}