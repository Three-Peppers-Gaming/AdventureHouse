using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; // ADD THIS - Missing using directive for StringBuilder
using AdventureHouse.Services.Models;
using AdventureHouse.Services.Data.AdventureData;

namespace AdventureHouse.Services.Models
{
    // Map level definitions for different areas of the house
    public enum MapLevel
    {
        GroundFloor = 0,    // Rooms 1-10
        UpperFloor = 1,     // Rooms 11-24  
        Attic = 2,          // Room 20
        MagicRealm = 3,     // Rooms 93-95
#if DEBUG
        DebugLevel = 88,    // Room 88 (debug only)
#endif
        Exit = 99           // Room 0 (victory)
    }

    // Represents a room position on the 2D map grid
    public class MapPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int RoomNumber { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public bool IsVisited { get; set; }
        public bool IsCurrentRoom { get; set; }
        public char DisplayChar { get; set; } = '.';
        public bool HasItemsVisible { get; set; }
        
        // ADD: Room connections for path drawing
        public int North { get; set; } = 99;  // 99 means no connection
        public int South { get; set; } = 99;
        public int East { get; set; } = 99;
        public int West { get; set; } = 99;
        public int Up { get; set; } = 99;
        public int Down { get; set; } = 99;
        
        public MapPosition(int x, int y, int roomNumber, string roomName)
        {
            X = x;
            Y = y;
            RoomNumber = roomNumber;
            RoomName = roomName;
            DisplayChar = GetRoomDisplayChar(roomName);
        }
        
        // USING ASCII ONLY - determine display character for room type
        private static char GetRoomDisplayChar(string roomName)
        {
            var gameConfig = new AdventureHouseConfiguration();
            return gameConfig.GetRoomDisplayChar(roomName);
        }
    }

    // Main map state management class
    public class MapState
    {
        private readonly Dictionary<MapLevel, Dictionary<int, MapPosition>> _levelMaps;
        private readonly HashSet<int> _visitedRooms;
        private int _currentRoom;
        private MapLevel _currentLevel;
        private readonly AdventureHouseConfiguration _gameConfig;
        
        public MapState()
        {
            _levelMaps = new Dictionary<MapLevel, Dictionary<int, MapPosition>>();
            _visitedRooms = new HashSet<int>();
            _currentRoom = 20; // Start in attic
            _gameConfig = new AdventureHouseConfiguration();
            
            InitializeMaps();
        }
        
        // Initialize all map levels with room positions
        private void InitializeMaps()
        {
            InitializeGroundFloor();
            InitializeUpperFloor();
            InitializeAttic();
            InitializeMagicRealm();
            
#if DEBUG
            InitializeDebugLevel();
#endif
        }
        
        // Ground floor layout (rooms 1-10, 24)
        private void InitializeGroundFloor()
        {
            var groundFloor = new Dictionary<int, MapPosition>
            {
                // Layout based on room connections from your data
                [1] = new MapPosition(5, 2, 1, "Main Entrance") { North = 10, South = 2 },
                [2] = new MapPosition(5, 4, 2, "Downstairs Hallway") { North = 1, South = 4, East = 3, Up = 11 },
                [3] = new MapPosition(7, 4, 3, "Guest Bathroom") { East = 5, West = 2 },
                [4] = new MapPosition(5, 6, 4, "Living Room") { North = 2, West = 5 },
                [5] = new MapPosition(3, 6, 5, "Family Room") { North = 6, East = 2 },
                [6] = new MapPosition(3, 4, 6, "Nook") { North = 7, South = 5, West = 24 },
                [7] = new MapPosition(3, 2, 7, "Kitchen") { North = 8, South = 6 },
                [8] = new MapPosition(3, 0, 8, "Utility Hall") { South = 7, East = 10, West = 9 },
                [9] = new MapPosition(1, 0, 9, "Garage") { East = 8 },
                [10] = new MapPosition(5, 0, 10, "Main Dining Room") { South = 1, West = 8 },
                [24] = new MapPosition(1, 4, 24, "Deck") { East = 6 }
            };
            
            _levelMaps[MapLevel.GroundFloor] = groundFloor;
        }
        
        // Upper floor layout (rooms 11-23, excluding 20 which is attic)
        private void InitializeUpperFloor()
        {
            var upperFloor = new Dictionary<int, MapPosition>
            {
                [11] = new MapPosition(5, 4, 11, "Upstairs Hallway") { South = 12, East = 16, West = 13, Down = 2 },
                [12] = new MapPosition(5, 6, 12, "Upstairs East Hallway") { North = 11, South = 15 },
                [13] = new MapPosition(3, 4, 13, "Upstairs North Hallway") { North = 18, South = 14, East = 11, West = 17 },
                [14] = new MapPosition(3, 6, 14, "Upstairs West Hallway") { North = 13, South = 23, West = 22 },
                [15] = new MapPosition(5, 8, 15, "Spare Room") { North = 12 },
                [16] = new MapPosition(7, 4, 16, "Utility Room") { West = 11 },
                [17] = new MapPosition(1, 4, 17, "Upstairs Bath") { East = 13 },
                [18] = new MapPosition(3, 2, 18, "Master Bedroom") { North = 21, South = 13, East = 19 },
                [19] = new MapPosition(5, 2, 19, "Master Bedroom Closet") { West = 18, Up = 20 },
                [21] = new MapPosition(3, 0, 21, "Master Bedroom Bath") { South = 18 },
                [22] = new MapPosition(1, 6, 22, "Children's Room") { East = 14 },
                [23] = new MapPosition(3, 8, 23, "Entertainment Room") { North = 14 }
            };
            
            _levelMaps[MapLevel.UpperFloor] = upperFloor;
        }
        
        // Attic (single room)
        private void InitializeAttic()
        {
            var attic = new Dictionary<int, MapPosition>
            {
                [20] = new MapPosition(4, 4, 20, "Attic") // Center of small map
            };
            
            _levelMaps[MapLevel.Attic] = attic;
        }
        
        // Magic realm (rooms 93-95)
        private void InitializeMagicRealm()
        {
            var magicRealm = new Dictionary<int, MapPosition>
            {
                [93] = new MapPosition(4, 2, 93, "Psychedelic Ladder"), // Top
                [94] = new MapPosition(4, 6, 94, "Memory Ladder"),      // Bottom
                [95] = new MapPosition(4, 4, 95, "Magic Mushroom")     // Center
            };
            
            _levelMaps[MapLevel.MagicRealm] = magicRealm;
        }
        
#if DEBUG
        // Debug level (room 88)
        private void InitializeDebugLevel()
        {
            var debugLevel = new Dictionary<int, MapPosition>
            {
                [88] = new MapPosition(4, 4, 88, "Debug Room") // Center of debug map
            };
            
            _levelMaps[MapLevel.DebugLevel] = debugLevel;
        }
#endif
        
        // FIXED: Determine which level a room belongs to - removed duplicate pattern
        private MapLevel GetLevelForRoom(int roomNumber)
        {
            return roomNumber switch
            {
                0 => MapLevel.Exit,
                20 => MapLevel.Attic,                      // Handle attic first (room 20)
                >= 1 and <= 10 or 24 => MapLevel.GroundFloor,  // Then ground floor
                >= 11 and <= 23 => MapLevel.UpperFloor,    // Upper floor (excluding 20)
                >= 93 and <= 95 => MapLevel.MagicRealm,
#if DEBUG
                88 => MapLevel.DebugLevel,
#endif
                _ => MapLevel.GroundFloor
            };
        }
        
        // Update player position and mark room as visited
        public void UpdatePlayerPosition(int roomNumber)
        {
            // Mark old room as not current
            if (_levelMaps.ContainsKey(_currentLevel) && _levelMaps[_currentLevel].ContainsKey(_currentRoom))
            {
                _levelMaps[_currentLevel][_currentRoom].IsCurrentRoom = false;
            }
            
            _currentRoom = roomNumber;
            _currentLevel = GetLevelForRoom(roomNumber);
            _visitedRooms.Add(roomNumber);
            
            // Mark new room as current and visited
            if (_levelMaps.ContainsKey(_currentLevel) && _levelMaps[_currentLevel].ContainsKey(_currentRoom))
            {
                _levelMaps[_currentLevel][_currentRoom].IsVisited = true;
                _levelMaps[_currentLevel][_currentRoom].IsCurrentRoom = true;
            }
        }
        
        // Update items visibility for a room
        public void UpdateRoomItems(int roomNumber, bool hasVisibleItems)
        {
            var level = GetLevelForRoom(roomNumber);
            if (_levelMaps.ContainsKey(level) && _levelMaps[level].ContainsKey(roomNumber))
            {
                _levelMaps[level][roomNumber].HasItemsVisible = hasVisibleItems;
            }
        }
        
        // Generate ASCII map for current level
        public string GenerateCurrentLevelMap()
        {
            return GenerateLevelMap(_currentLevel);
        }
        
        // Add a method to get the current room name
        public string GetCurrentRoomName()
        {
            if (_levelMaps.ContainsKey(_currentLevel) && _levelMaps[_currentLevel].ContainsKey(_currentRoom))
            {
                return _levelMaps[_currentLevel][_currentRoom].RoomName;
            }
            return UIConfiguration.UnknownRoomMessage;
        }

        // Generate ASCII map for specific level - USING ASCII ONLY with BOXED ROOMS
        public string GenerateLevelMap(MapLevel level)
        {
#if DEBUG
            // In release mode, don't show debug levels
            if (level == MapLevel.DebugLevel)
                return UIConfiguration.DebugNotAvailableMessage;
#endif

            if (!_levelMaps.ContainsKey(level))
                return UIConfiguration.NoMapAvailableMessage;
                
            var levelRooms = _levelMaps[level];
            if (!levelRooms.Any())
                return UIConfiguration.EmptyLevelMessage;
                
            // Calculate map bounds
            var minX = levelRooms.Values.Min(r => r.X);
            var maxX = levelRooms.Values.Max(r => r.X);
            var minY = levelRooms.Values.Min(r => r.Y);
            var maxY = levelRooms.Values.Max(r => r.Y);
            
            var roomsWidth = maxX - minX + 1;
            var roomsHeight = maxY - minY + 1;
            
            // Create larger map grid for boxed rooms (each room takes 4x3 characters)
            var mapWidth = roomsWidth * UIConfiguration.RoomBoxWidth - 1;   // 4 chars per room minus 1 for shared borders
            var mapHeight = roomsHeight * UIConfiguration.RoomBoxHeight - 1; // 3 chars per room minus 1 for shared borders
            
            // Ensure minimum size for single rooms
            if (mapWidth < UIConfiguration.MinMapWidth) mapWidth = UIConfiguration.MinMapWidth;
            if (mapHeight < UIConfiguration.MinMapHeight) mapHeight = UIConfiguration.MinMapHeight;
            
            var map = new char[mapHeight, mapWidth];
            
            // Fill with spaces
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    map[y, x] = UIConfiguration.SpaceCharacter;
                }
            }
            
            // FIRST: Draw path connections between visited rooms
            DrawPathConnections(map, levelRooms, minX, minY, mapWidth, mapHeight);
            
            // SECOND: Draw room boxes for visited rooms (this will overwrite paths at room locations)
            foreach (var room in levelRooms.Values.Where(r => r.IsVisited))
            {
                var roomStartX = (room.X - minX) * UIConfiguration.RoomBoxWidth;
                var roomStartY = (room.Y - minY) * UIConfiguration.RoomBoxHeight;
                
                // Ensure we don't go out of bounds
                if (roomStartX + 3 >= mapWidth || roomStartY + 2 >= mapHeight) continue;
                
                // Draw room box (4x3 characters)
                // Top border: +---+
                map[roomStartY, roomStartX] = '+';
                map[roomStartY, roomStartX + 1] = '-';
                map[roomStartY, roomStartX + 2] = '-';
                map[roomStartY, roomStartX + 3] = '+';
                
                // Middle row with room character and items
                map[roomStartY + 1, roomStartX] = '|';
                
                if (room.IsCurrentRoom)
                {
                    map[roomStartY + 1, roomStartX + 1] = UIConfiguration.PlayerCharacter; // Player position
                    map[roomStartY + 1, roomStartX + 2] = room.HasItemsVisible ? UIConfiguration.ItemsIndicator : UIConfiguration.SpaceCharacter;
                }
                else
                {
                    map[roomStartY + 1, roomStartX + 1] = room.DisplayChar; // Room character
                    map[roomStartY + 1, roomStartX + 2] = room.HasItemsVisible ? UIConfiguration.ItemsIndicator : UIConfiguration.SpaceCharacter; // Items indicator
                }
                
                map[roomStartY + 1, roomStartX + 3] = '|';
                
                // Bottom border: +---+
                map[roomStartY + 2, roomStartX] = '+';
                map[roomStartY + 2, roomStartX + 1] = '-';
                map[roomStartY + 2, roomStartX + 2] = '-';
                map[roomStartY + 2, roomStartX + 3] = '+';
            }
            
            // SIMPLIFIED: Just return the map content as string, no extra borders
            var result = new StringBuilder();
            
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    result.Append(map[y, x]);
                }
                if (y < mapHeight - 1) // Don't add newline after last row
                    result.AppendLine();
            }
            
            return result.ToString();
        }
        
        // NEW METHOD: Draw dotted path connections between rooms
        private void DrawPathConnections(char[,] map, Dictionary<int, MapPosition> levelRooms, int minX, int minY, int mapWidth, int mapHeight)
        {
            foreach (var room in levelRooms.Values.Where(r => r.IsVisited))
            {
                var roomCenterX = (room.X - minX) * UIConfiguration.RoomBoxWidth + 2; // Center of room box
                var roomCenterY = (room.Y - minY) * UIConfiguration.RoomBoxHeight + 1; // Center of room box
                
                // Draw connections to other visited rooms
                // EAST connection
                if (room.East != UIConfiguration.NoConnectionValue && levelRooms.ContainsKey(room.East) && levelRooms[room.East].IsVisited)
                {
                    var targetRoom = levelRooms[room.East];
                    var targetX = (targetRoom.X - minX) * UIConfiguration.RoomBoxWidth;
                    var pathY = roomCenterY;
                    
                    // Draw dotted line from room edge to target room edge
                    for (int x = roomCenterX + 2; x < targetX; x++)
                    {
                        if (x < mapWidth && pathY < mapHeight)
                        {
                            if (map[pathY, x] == UIConfiguration.SpaceCharacter) // Don't overwrite existing content
                                map[pathY, x] = (x % 2 == 0) ? '.' : UIConfiguration.SpaceCharacter; // Dotted line
                        }
                    }
                }
                
                // SOUTH connection
                if (room.South != UIConfiguration.NoConnectionValue && levelRooms.ContainsKey(room.South) && levelRooms[room.South].IsVisited)
                {
                    var targetRoom = levelRooms[room.South];
                    var targetY = (targetRoom.Y - minY) * UIConfiguration.RoomBoxHeight;
                    var pathX = roomCenterX;
                    
                    // Draw dotted line from room edge to target room edge
                    for (int y = roomCenterY + 2; y < targetY; y++)
                    {
                        if (pathX < mapWidth && y < mapHeight)
                        {
                            if (map[y, pathX] == UIConfiguration.SpaceCharacter) // Don't overwrite existing content
                                map[y, pathX] = (y % 2 == 0) ? ':' : UIConfiguration.SpaceCharacter; // Dotted vertical line
                        }
                    }
                }
                
                // UP/DOWN connections (show with special characters)
                if (room.Up != UIConfiguration.NoConnectionValue && levelRooms.ContainsKey(room.Up) && levelRooms[room.Up].IsVisited)
                {
                    // Show up connection with ^ symbol near room
                    if (roomCenterX + 1 < mapWidth && roomCenterY - 1 >= 0)
                    {
                        if (map[roomCenterY - 1, roomCenterX + 1] == UIConfiguration.SpaceCharacter)
                            map[roomCenterY - 1, roomCenterX + 1] = '^';
                    }
                }
                
                if (room.Down != UIConfiguration.NoConnectionValue && levelRooms.ContainsKey(room.Down) && levelRooms[room.Down].IsVisited)
                {
                    // Show down connection with v symbol near room
                    if (roomCenterX + 1 < mapWidth && roomCenterY + 3 < mapHeight)
                    {
                        if (map[roomCenterY + 3, roomCenterX + 1] == UIConfiguration.SpaceCharacter)
                            map[roomCenterY + 3, roomCenterX + 1] = 'v';
                    }
                }
            }
        }
        
        // Get display name for level with debug exclusion
        private static string GetLevelDisplayName(MapLevel level)
        {
            var gameConfig = new AdventureHouseConfiguration();
            return gameConfig.GetLevelDisplayName(level);
        }
        
        // Update the GetMapLegend method to include path connections
        public string GetMapLegend()
        {
            return _gameConfig.GetCompleteMapLegend();
        }
        
        // Get current level info
        public MapLevel CurrentLevel => _currentLevel;
        public int CurrentRoom => _currentRoom;
        public bool IsRoomVisited(int roomNumber) => _visitedRooms.Contains(roomNumber);
        public int VisitedRoomCount => _visitedRooms.Count;
    }
}