using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventureHouse.Services.Models;
using AdventureHouse.Services.Data.AdventureData;

namespace AdventureHouse.Services.Models
{
    // Map level definitions for different areas - these can be generic across games
    public enum MapLevel
    {
        GroundFloor = 0,
        UpperFloor = 1,
        Attic = 2,
        MagicRealm = 3,
#if DEBUG
        DebugLevel = 88,
#endif
        Exit = 99
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
        
        // Room connections for path drawing
        public int North { get; set; } = 99;
        public int South { get; set; } = 99;
        public int East { get; set; } = 99;
        public int West { get; set; } = 99;
        public int Up { get; set; } = 99;
        public int Down { get; set; } = 99;
        
        public MapPosition(int x, int y, int roomNumber, string roomName, IGameConfiguration gameConfig)
        {
            X = x;
            Y = y;
            RoomNumber = roomNumber;
            RoomName = roomName;
            DisplayChar = gameConfig.GetRoomDisplayChar(roomName);
        }
    }

    // Generic map state management class
    public class MapState
    {
        private readonly Dictionary<MapLevel, Dictionary<int, MapPosition>> _levelMaps;
        private readonly HashSet<int> _visitedRooms;
        private int _currentRoom;
        private MapLevel _currentLevel;
        private readonly IGameConfiguration _gameConfig;
        
        public MapState(IGameConfiguration gameConfig, List<Room> rooms, int startingRoom = 20)
        {
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _levelMaps = new Dictionary<MapLevel, Dictionary<int, MapPosition>>();
            _visitedRooms = new HashSet<int>();
            _currentRoom = startingRoom;
            
            InitializeMapsFromRooms(rooms);
            _currentLevel = _gameConfig.GetLevelForRoom(_currentRoom);
        }
        
        // Initialize all map levels from room data
        private void InitializeMapsFromRooms(List<Room> rooms)
        {
            // Group rooms by level
            var roomsByLevel = rooms.GroupBy(room => _gameConfig.GetLevelForRoom(room.Number))
                                   .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var kvp in roomsByLevel)
            {
                var level = kvp.Key;
                var levelRooms = kvp.Value;
                var levelMap = new Dictionary<int, MapPosition>();
                
                foreach (var room in levelRooms)
                {
                    var position = _gameConfig.GetRoomPosition(room.Number);
                    var x = position.X;
                    var y = position.Y;
                    var mapPos = new MapPosition(x, y, room.Number, room.Name, _gameConfig)
                    {
                        North = room.N,
                        South = room.S,
                        East = room.E,
                        West = room.W,
                        Up = room.U,
                        Down = room.D
                    };
                    
                    levelMap[room.Number] = mapPos;
                }
                
                _levelMaps[level] = levelMap;
            }
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
            _currentLevel = _gameConfig.GetLevelForRoom(roomNumber);
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
            var level = _gameConfig.GetLevelForRoom(roomNumber);
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
        
        // Get the current room name
        public string GetCurrentRoomName()
        {
            if (_levelMaps.ContainsKey(_currentLevel) && _levelMaps[_currentLevel].ContainsKey(_currentRoom))
            {
                return _levelMaps[_currentLevel][_currentRoom].RoomName;
            }
            return UIConfiguration.UnknownRoomMessage;
        }

        // Generate ASCII map for specific level
        public string GenerateLevelMap(MapLevel level)
        {
#if DEBUG
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
            
            // Create larger map grid for boxed rooms
            var mapWidth = roomsWidth * UIConfiguration.RoomBoxWidth - 1;
            var mapHeight = roomsHeight * UIConfiguration.RoomBoxHeight - 1;
            
            // Ensure minimum size
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
            
            // Draw path connections between visited rooms
            DrawPathConnections(map, levelRooms, minX, minY, mapWidth, mapHeight);
            
            // Draw room boxes for visited rooms
            foreach (var room in levelRooms.Values.Where(r => r.IsVisited))
            {
                DrawRoomBox(map, room, minX, minY, mapWidth, mapHeight);
            }
            
            // Convert to string
            var result = new StringBuilder();
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    result.Append(map[y, x]);
                }
                if (y < mapHeight - 1)
                    result.AppendLine();
            }
            
            return result.ToString();
        }
        
        // Draw a room box on the map
        private void DrawRoomBox(char[,] map, MapPosition room, int minX, int minY, int mapWidth, int mapHeight)
        {
            var roomStartX = (room.X - minX) * UIConfiguration.RoomBoxWidth;
            var roomStartY = (room.Y - minY) * UIConfiguration.RoomBoxHeight;
            
            // Ensure we don't go out of bounds
            if (roomStartX + 3 >= mapWidth || roomStartY + 2 >= mapHeight) return;
            
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
                map[roomStartY + 1, roomStartX + 1] = UIConfiguration.PlayerCharacter;
                map[roomStartY + 1, roomStartX + 2] = room.HasItemsVisible ? UIConfiguration.ItemsIndicator : UIConfiguration.SpaceCharacter;
            }
            else
            {
                map[roomStartY + 1, roomStartX + 1] = room.DisplayChar;
                map[roomStartY + 1, roomStartX + 2] = room.HasItemsVisible ? UIConfiguration.ItemsIndicator : UIConfiguration.SpaceCharacter;
            }
            
            map[roomStartY + 1, roomStartX + 3] = '|';
            
            // Bottom border: +---+
            map[roomStartY + 2, roomStartX] = '+';
            map[roomStartY + 2, roomStartX + 1] = '-';
            map[roomStartY + 2, roomStartX + 2] = '-';
            map[roomStartY + 2, roomStartX + 3] = '+';
        }
        
        // Draw dotted path connections between rooms
        private void DrawPathConnections(char[,] map, Dictionary<int, MapPosition> levelRooms, int minX, int minY, int mapWidth, int mapHeight)
        {
            foreach (var room in levelRooms.Values.Where(r => r.IsVisited))
            {
                var roomCenterX = (room.X - minX) * UIConfiguration.RoomBoxWidth + 2;
                var roomCenterY = (room.Y - minY) * UIConfiguration.RoomBoxHeight + 1;
                
                // Draw connections to other visited rooms
                DrawConnection(map, room, levelRooms, room.East, minX, minY, mapWidth, mapHeight, roomCenterX, roomCenterY, "east");
                DrawConnection(map, room, levelRooms, room.South, minX, minY, mapWidth, mapHeight, roomCenterX, roomCenterY, "south");
                DrawConnection(map, room, levelRooms, room.Up, minX, minY, mapWidth, mapHeight, roomCenterX, roomCenterY, "up");
                DrawConnection(map, room, levelRooms, room.Down, minX, minY, mapWidth, mapHeight, roomCenterX, roomCenterY, "down");
            }
        }
        
        // Draw a specific connection
        private void DrawConnection(char[,] map, MapPosition room, Dictionary<int, MapPosition> levelRooms,
            int targetRoomNumber, int minX, int minY, int mapWidth, int mapHeight,
            int roomCenterX, int roomCenterY, string direction)
        {
            if (targetRoomNumber == _gameConfig.NoConnectionValue || !levelRooms.ContainsKey(targetRoomNumber) || !levelRooms[targetRoomNumber].IsVisited)
                return;
            
            var targetRoom = levelRooms[targetRoomNumber];
            
            switch (direction)
            {
                case "east":
                    var targetX = (targetRoom.X - minX) * UIConfiguration.RoomBoxWidth;
                    for (int x = roomCenterX + 2; x < targetX; x++)
                    {
                        if (x < mapWidth && roomCenterY < mapHeight && map[roomCenterY, x] == UIConfiguration.SpaceCharacter)
                            map[roomCenterY, x] = (x % 2 == 0) ? '.' : UIConfiguration.SpaceCharacter;
                    }
                    break;
                    
                case "south":
                    var targetY = (targetRoom.Y - minY) * UIConfiguration.RoomBoxHeight;
                    for (int y = roomCenterY + 2; y < targetY; y++)
                    {
                        if (roomCenterX < mapWidth && y < mapHeight && map[y, roomCenterX] == UIConfiguration.SpaceCharacter)
                            map[y, roomCenterX] = (y % 2 == 0) ? ':' : UIConfiguration.SpaceCharacter;
                    }
                    break;
                    
                case "up":
                    if (roomCenterX + 1 < mapWidth && roomCenterY - 1 >= 0 && map[roomCenterY - 1, roomCenterX + 1] == UIConfiguration.SpaceCharacter)
                        map[roomCenterY - 1, roomCenterX + 1] = '^';
                    break;
                    
                case "down":
                    if (roomCenterX + 1 < mapWidth && roomCenterY + 3 < mapHeight && map[roomCenterY + 3, roomCenterX + 1] == UIConfiguration.SpaceCharacter)
                        map[roomCenterY + 3, roomCenterX + 1] = 'v';
                    break;
            }
        }
        
        // Get map legend
        public string GetMapLegend()
        {
            return _gameConfig.GetCompleteMapLegend();
        }
        
        // Properties
        public MapLevel CurrentLevel => _currentLevel;
        public int CurrentRoom => _currentRoom;
        public bool IsRoomVisited(int roomNumber) => _visitedRooms.Contains(roomNumber);
        public int VisitedRoomCount => _visitedRooms.Count;
    }
}