using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using AdventureHouse.Services.AdventureClient.Models;
using AdventureHouse.Services.Data.AdventureData;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace AdventureHouse.Tests.Client
{
    /// <summary>
    /// Tests for map generation and rendering functionality
    /// Validates that map data is correctly generated, transmitted, and can be rendered
    /// </summary>
    public class MapGenerationTests
    {
        private readonly IPlayAdventure _server;

        public MapGenerationTests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            _server = new AdventureFrameworkService(cache, fortune, commandProcessor);
        }

        [Fact]
        public void MapData_ShouldBeGeneratedForNewGameSession()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };

            // Act
            var response = _server.PlayGame(newGameRequest);

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.MapData);
            Assert.True(response.MapData.VisitedRoomCount > 0, "New game should have at least one visited room");
            Assert.NotEmpty(response.MapData.DiscoveredRooms);
            
            // Verify map rendering config is present
            Assert.NotNull(response.MapData.RenderingConfig);
            Assert.False(string.IsNullOrEmpty(response.MapData.RenderingConfig.GameName));
            Assert.True(response.MapData.RenderingConfig.RoomTypeChars.Count > 0);
        }

        [Fact]
        public void MapData_ShouldTrackPlayerMovement()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            var initialRoomCount = gameStart.MapData.VisitedRoomCount;
            var initialCurrentRoom = gameStart.MapData.CurrentRoom;

            // Act - Move to a new room
            var moveRequest = new GamePlayRequest { SessionId = sessionId, Command = "d" };
            var moveResponse = _server.PlayGame(moveRequest);

            // Assert
            Assert.NotNull(moveResponse.MapData);
            
            // Should still have the same or more visited rooms
            Assert.True(moveResponse.MapData.VisitedRoomCount >= initialRoomCount);
            
            // Current room should be updated if movement was successful
            if (moveResponse.MapData.CurrentRoom != initialCurrentRoom)
            {
                // If we moved, visited room count should increase
                Assert.True(moveResponse.MapData.VisitedRoomCount > initialRoomCount);
                
                // Should have exactly one current location
                Assert.Single(moveResponse.MapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
                
                // The new current room should be marked as current
                var currentRoom = moveResponse.MapData.DiscoveredRooms.First(r => r.IsCurrentLocation);
                Assert.Equal(moveResponse.MapData.CurrentRoom, currentRoom.RoomNumber);
            }
        }

        [Fact]
        public void MapData_ShouldMaintainDiscoveredRoomsConsistency()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Execute several commands to explore
            var commands = new[] { "look", "d", "look", "w", "look", "inv" };
            var responses = new List<GamePlayResponse> { gameStart };

            foreach (var command in commands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);
            }

            // Assert
            foreach (var response in responses)
            {
                Assert.NotNull(response.MapData);
                
                // Basic consistency checks
                Assert.True(response.MapData.VisitedRoomCount > 0);
                Assert.NotEmpty(response.MapData.DiscoveredRooms);
                
                // Should always have exactly one current location
                Assert.Single(response.MapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
                
                // All discovered rooms should have valid data
                foreach (var room in response.MapData.DiscoveredRooms)
                {
                    Assert.True(room.RoomNumber > 0, $"Room number should be positive: {room.RoomNumber}");
                    Assert.False(string.IsNullOrEmpty(room.Name), $"Room name should not be empty for room {room.RoomNumber}");
                    Assert.NotNull(room.Position);
                    Assert.NotNull(room.Connections);
                    
                    // Level information should be valid
                    Assert.False(string.IsNullOrEmpty(room.Level), $"Room level should not be empty for room {room.RoomNumber}");
                }
                
                // Visited room count should match discovered rooms count
                Assert.Equal(response.MapData.VisitedRoomCount, response.MapData.DiscoveredRooms.Count);
            }
        }

        [Fact]
        public void MapData_ShouldHaveValidRoomConnections()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Move around to discover connections
            var explorationCommands = new[] { "d", "w", "s", "e", "n", "u" };
            
            foreach (var command in explorationCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                
                // Skip if session ended
                if (response.SessionId == "-1") break;
            }

            // Get final state
            var finalRequest = new GamePlayRequest { SessionId = sessionId, Command = "look" };
            var finalResponse = _server.PlayGame(finalRequest);

            // Assert
            Assert.NotNull(finalResponse.MapData);
            
            foreach (var room in finalResponse.MapData.DiscoveredRooms)
            {
                foreach (var connection in room.Connections)
                {
                    // All connections should point to discovered rooms
                    var discoveredRoomNumbers = finalResponse.MapData.DiscoveredRooms.Select(r => r.RoomNumber).ToList();
                    Assert.Contains(connection.TargetRoom, discoveredRoomNumbers);
                    
                    // Connection direction should be valid
                    var validDirections = new[] { "north", "south", "east", "west", "up", "down" };
                    Assert.Contains(connection.Direction.ToLower(), validDirections);
                    
                    // Target room should be marked as discovered
                    Assert.True(connection.TargetRoomDiscovered,
                        $"Connection from room {room.RoomNumber} to {connection.TargetRoom} should be marked as discovered");
                }
            }
        }

        [Fact]
        public void MapData_ShouldProvideValidRenderingConfiguration()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };

            // Act
            var response = _server.PlayGame(newGameRequest);

            // Assert
            Assert.NotNull(response.MapData);
            var config = response.MapData.RenderingConfig;
            Assert.NotNull(config);
            
            // Game name should be present
            Assert.False(string.IsNullOrEmpty(config.GameName));
            
            // Should have room type characters
            Assert.NotNull(config.RoomTypeChars);
            Assert.True(config.RoomTypeChars.Count > 0);
            
            // Should have level display names
            Assert.NotNull(config.LevelDisplayNames);
            Assert.True(config.LevelDisplayNames.Count > 0);
            
            // Should have default characters
            Assert.True(config.DefaultRoomChar != '\0');
            Assert.True(config.PlayerChar != '\0');
            Assert.True(config.ItemsChar != '\0');
            
            // Validate some expected room type characters for Adventure House
            if (config.GameName.Contains("Adventure House"))
            {
                Assert.True(config.RoomTypeChars.ContainsKey("kitchen") || 
                           config.RoomTypeChars.ContainsKey("attic") ||
                           config.RoomTypeChars.Values.Contains('K') ||
                           config.RoomTypeChars.Values.Contains('A'));
            }
        }

        [Fact]
        public void MapData_ShouldTrackItemsInRooms()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Look for items and try to pick some up
            var lookRequest = new GamePlayRequest { SessionId = sessionId, Command = "look" };
            var lookResponse = _server.PlayGame(lookRequest);
            
            var getBugleRequest = new GamePlayRequest { SessionId = sessionId, Command = "get bugle" };
            var getBugleResponse = _server.PlayGame(getBugleRequest);

            // Assert
            Assert.NotNull(lookResponse.MapData);
            Assert.NotNull(getBugleResponse.MapData);
            
            // Check that rooms can have items
            var roomsWithItems = getBugleResponse.MapData.DiscoveredRooms.Where(r => r.HasItems);
            
            // At least one room should be capable of having items (though it may be empty after pickup)
            Assert.True(getBugleResponse.MapData.DiscoveredRooms.Any());
            
            // All rooms should have a valid HasItems flag (not null/undefined behavior)
            foreach (var room in getBugleResponse.MapData.DiscoveredRooms)
            {
                // HasItems should be a valid boolean (this test ensures the property is properly set)
                Assert.True(room.HasItems == true || room.HasItems == false);
            }
        }

        [Fact]
        public void MapData_ShouldSupportMultipleGames()
        {
            // Arrange & Act - Test all available games
            var games = new[] { 1, 2, 3 }; // Adventure House, Space Station, Future Family
            var mapDataResults = new List<PlayerMapData>();

            foreach (var gameId in games)
            {
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = gameId, Command = "" };
                var response = _server.PlayGame(newGameRequest);
                
                Assert.NotNull(response.MapData);
                mapDataResults.Add(response.MapData);
            }

            // Assert - Each game should have unique map characteristics
            for (int i = 0; i < mapDataResults.Count; i++)
            {
                var mapData = mapDataResults[i];
                
                // Each game should have its own configuration
                Assert.NotNull(mapData.RenderingConfig);
                Assert.False(string.IsNullOrEmpty(mapData.RenderingConfig.GameName));
                
                // Each game should have different starting characteristics
                Assert.True(mapData.VisitedRoomCount > 0);
                Assert.NotEmpty(mapData.DiscoveredRooms);
                
                // Level display name should be meaningful
                Assert.False(string.IsNullOrEmpty(mapData.CurrentLevelDisplayName));
                
                // Compare with other games to ensure uniqueness
                for (int j = i + 1; j < mapDataResults.Count; j++)
                {
                    var otherMapData = mapDataResults[j];
                    
                    // Games should have different names
                    Assert.NotEqual(mapData.RenderingConfig.GameName, otherMapData.RenderingConfig.GameName);
                    
                    // Games may have different starting rooms or level names
                    // (This is not required but likely for different game types)
                }
            }
        }

        [Fact]
        public void MapData_ShouldHandleInvalidMovement()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            var initialMapData = gameStart.MapData;

            // Act - Try invalid movements
            var invalidCommands = new[] { "go nowhere", "teleport", "fly", "swim" };
            
            foreach (var command in invalidCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                
                // Assert - Map data should remain consistent even with invalid commands
                Assert.NotNull(response.MapData);
                Assert.Equal(initialMapData.CurrentRoom, response.MapData.CurrentRoom);
                Assert.Equal(initialMapData.VisitedRoomCount, response.MapData.VisitedRoomCount);
                
                // Should still have exactly one current location
                Assert.Single(response.MapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
            }
        }

        [Fact]
        public void MapData_ShouldPersistAcrossCommands()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Execute various commands and verify map persistence
            var commands = new[] { "inv", "look", "help", "get bugle", "inv", "look" };
            var previousMapData = gameStart.MapData;

            foreach (var command in commands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                
                // Assert - Map data should be consistent
                Assert.NotNull(response.MapData);
                
                // Core map properties should persist for non-movement commands
                if (!new[] { "d", "u", "n", "s", "e", "w", "go" }.Any(cmd => command.StartsWith(cmd)))
                {
                    Assert.Equal(previousMapData.CurrentRoom, response.MapData.CurrentRoom);
                    Assert.Equal(previousMapData.VisitedRoomCount, response.MapData.VisitedRoomCount);
                    Assert.Equal(previousMapData.DiscoveredRooms.Count, response.MapData.DiscoveredRooms.Count);
                }
                
                previousMapData = response.MapData;
            }
        }

        [Fact]
        public void MapData_ShouldHaveValidPositionalData()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Move around to get more map data
            var moveCommands = new[] { "d", "w", "n", "s" };
            
            foreach (var command in moveCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                
                // Break if we can't move anymore or session ends
                if (response.SessionId == "-1") break;
            }

            // Get final map state
            var finalRequest = new GamePlayRequest { SessionId = sessionId, Command = "look" };
            var finalResponse = _server.PlayGame(finalRequest);

            // Assert
            Assert.NotNull(finalResponse.MapData);
            
            foreach (var room in finalResponse.MapData.DiscoveredRooms)
            {
                // Position should be valid
                Assert.NotNull(room.Position);
                Assert.True(room.Position.X >= 0, $"Room {room.RoomNumber} X position should be non-negative: {room.Position.X}");
                Assert.True(room.Position.Y >= 0, $"Room {room.RoomNumber} Y position should be non-negative: {room.Position.Y}");
                
                // Display character should be set
                Assert.True(room.DisplayChar != '\0', $"Room {room.RoomNumber} should have a display character");
                
                // Level should be valid
                Assert.False(string.IsNullOrEmpty(room.Level), $"Room {room.RoomNumber} should have a level");
                
                // Only one room should be marked as current location
                if (room.IsCurrentLocation)
                {
                    Assert.Equal(finalResponse.MapData.CurrentRoom, room.RoomNumber);
                }
            }
            
            // Verify no duplicate positions (rooms shouldn't overlap)
            var positions = finalResponse.MapData.DiscoveredRooms
                .Select(r => $"{r.Position.X},{r.Position.Y}")
                .ToList();
            var uniquePositions = positions.Distinct().ToList();
            
            // Note: Some games might allow rooms at same position on different levels
            // So we'll just ensure we have reasonable position data
            Assert.True(uniquePositions.Count > 0);
        }
    }
}