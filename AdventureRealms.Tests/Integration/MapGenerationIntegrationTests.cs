using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.Shared.FortuneService;
using AdventureRealms.Services.Shared.CommandProcessing;
using AdventureRealms.Services.AdventureClient.UI;
using AdventureRealms.Services.AdventureClient.Models;
using AdventureRealms.Services.AdventureClient.AppVersion;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace AdventureRealms.Tests.Integration
{
    /// <summary>
    /// Integration tests for end-to-end map generation workflow
    /// Tests the complete flow from server map data generation to client display
    /// </summary>
    public class MapGenerationIntegrationTests
    {
        private readonly IPlayAdventure _server;
        private readonly IDisplayService _displayService;

        public MapGenerationIntegrationTests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            _server = new AdventureFrameworkService(cache, fortune, commandProcessor);
            
            var appVersionService = new AppVersionService();
            _displayService = new DisplayService(appVersionService);
        }

        [Fact]
        public void EndToEndMapGeneration_ShouldWorkForAllGames()
        {
            // Arrange
            var gameIds = new[] { 1, 2, 3 }; // All available games

            foreach (var gameId in gameIds)
            {
                // Act - Start a game session
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = gameId, Command = "" };
                var gameResponse = _server.PlayGame(newGameRequest);

                // Assert - Basic response validation
                Assert.NotNull(gameResponse);
                Assert.NotEqual("-1", gameResponse.SessionId);
                Assert.NotNull(gameResponse.MapData);

                // Validate map data structure
                ValidateMapDataStructure(gameResponse.MapData, $"Game {gameId}");

                // Test map display generation
                var mapText = GenerateConsoleMapText(gameResponse.MapData);
                Assert.NotNull(mapText);
                Assert.False(string.IsNullOrEmpty(mapText));

                // Validate map text contains expected elements
                ValidateMapText(mapText, gameResponse.MapData, $"Game {gameId}");
            }
        }

        [Fact]
        public void MapGeneration_ShouldUpdateWithPlayerMovement()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            var mapDataHistory = new List<PlayerMapData> { gameStart.MapData };
            var mapTextHistory = new List<string> { GenerateConsoleMapText(gameStart.MapData) };

            // Act - Execute a series of movement commands
            var movementCommands = new[] { "d", "w", "s", "n", "e", "u" };

            foreach (var command in movementCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                // Break if session ended
                if (response.SessionId == "-1") break;

                mapDataHistory.Add(response.MapData);
                mapTextHistory.Add(GenerateConsoleMapText(response.MapData));
            }

            // Assert - Validate progression
            Assert.True(mapDataHistory.Count > 1, "Should have captured multiple map states");

            for (int i = 1; i < mapDataHistory.Count; i++)
            {
                var previous = mapDataHistory[i - 1];
                var current = mapDataHistory[i];

                // Visited room count should never decrease
                Assert.True(current.VisitedRoomCount >= previous.VisitedRoomCount,
                    $"Visited room count should not decrease: {previous.VisitedRoomCount} -> {current.VisitedRoomCount}");

                // Should always have exactly one current location
                Assert.Single(current.DiscoveredRooms.Where(r => r.IsCurrentLocation));

                // Map text should be updated
                Assert.NotEqual(mapTextHistory[i - 1], mapTextHistory[i]);
            }
        }

        [Fact]
        public void MapGeneration_ShouldHandleTerminalGuiRendering()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameResponse = _server.PlayGame(newGameRequest);
            
            // Convert to MapModel for Terminal.Gui rendering
            var mapModel = CreateMapModelFromPlayerMapData(gameResponse.MapData);

            // Act - Test Terminal.Gui rendering
            var renderer = new TerminalGuiRenderer();
            var mapBounds = new Terminal.Gui.Rect(0, 0, 80, 24);
            var statusBounds = new Terminal.Gui.Rect(0, 0, 80, 3);

            var mapView = renderer.CreateMapView(mapModel, mapBounds);
            var statusView = renderer.CreateStatusView(mapModel, "Great", statusBounds);
            var legendView = renderer.CreateLegendView(mapModel, statusBounds);
            var mapString = renderer.RenderMapToString(mapModel, 80, 40);

            // Assert
            Assert.NotNull(mapView);
            Assert.NotNull(statusView);
            Assert.NotNull(legendView);
            Assert.NotNull(mapString);
            Assert.False(string.IsNullOrEmpty(mapString));

            // Validate bounds are set correctly
            Assert.Equal(mapBounds, mapView.Bounds);
            Assert.Equal(statusBounds, statusView.Bounds);
        }

        [Fact]
        public void MapGeneration_ShouldSupportMultipleConcurrentSessions()
        {
            // Arrange
            var sessionCount = 5;
            var sessions = new Dictionary<string, PlayerMapData>();

            // Act - Create multiple sessions
            for (int i = 0; i < sessionCount; i++)
            {
                var gameId = (i % 3) + 1; // Rotate through games
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = gameId, Command = "" };
                var gameResponse = _server.PlayGame(newGameRequest);

                Assert.NotEqual("-1", gameResponse.SessionId);
                sessions[gameResponse.SessionId] = gameResponse.MapData;
            }

            // Execute some commands in each session
            foreach (var kvp in sessions)
            {
                var sessionId = kvp.Key;
                var initialMapData = kvp.Value;
                var moveRequest = new GamePlayRequest { SessionId = sessionId, Command = "d" };
                var moveResponse = _server.PlayGame(moveRequest);

                // Assert - Each session should maintain independent map state
                Assert.NotNull(moveResponse.MapData);
                ValidateMapDataStructure(moveResponse.MapData, sessionId);

                // Sessions should have unique current rooms (or at least independent state)
                Assert.True(moveResponse.MapData.VisitedRoomCount >= initialMapData.VisitedRoomCount);
            }

            // All sessions should be unique
            Assert.Equal(sessionCount, sessions.Keys.Distinct().Count());
        }

        [Fact]
        public void MapGeneration_ShouldHandleInvalidCommands()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            var initialMapData = gameStart.MapData;

            // Act - Execute invalid commands
            var invalidCommands = new[] { "teleport", "fly", "swim", "dance", "sing" };

            foreach (var command in invalidCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                // Assert - Map data should remain stable
                Assert.NotNull(response.MapData);
                Assert.Equal(initialMapData.CurrentRoom, response.MapData.CurrentRoom);
                Assert.Equal(initialMapData.VisitedRoomCount, response.MapData.VisitedRoomCount);

                // Should still be able to generate map text
                var mapText = GenerateConsoleMapText(response.MapData);
                Assert.NotNull(mapText);
                Assert.False(string.IsNullOrEmpty(mapText));
            }
        }

        [Fact]
        public void MapGeneration_ShouldMaintainConsistencyAcrossGameRestart()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Move around a bit
            var exploreRequest = new GamePlayRequest { SessionId = sessionId, Command = "d" };
            var exploreResponse = _server.PlayGame(exploreRequest);

            // Act - Restart the game
            var restartRequest = new GamePlayRequest { SessionId = sessionId, Command = "newgame" };
            var restartResponse = _server.PlayGame(restartRequest);

            // Assert
            Assert.NotNull(restartResponse.MapData);
            
            // After restart, should be back to initial state
            Assert.Equal(gameStart.MapData.CurrentRoom, restartResponse.MapData.CurrentRoom);
            
            // Map structure should be valid
            ValidateMapDataStructure(restartResponse.MapData, "Restarted game");
            
            // Should be able to generate map text
            var mapText = GenerateConsoleMapText(restartResponse.MapData);
            Assert.NotNull(mapText);
            Assert.False(string.IsNullOrEmpty(mapText));
        }

        [Fact]
        public void MapGeneration_ShouldWorkWithItemInteractions()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Interact with items and check map updates
            var commands = new[] { "get bugle", "pet kitten", "inv", "drop bugle", "get bugle" };

            foreach (var command in commands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                // Assert - Map should remain consistent through item interactions
                Assert.NotNull(response.MapData);
                ValidateMapDataStructure(response.MapData, $"After command: {command}");

                // Current room should update item status appropriately
                var currentRoom = response.MapData.DiscoveredRooms.FirstOrDefault(r => r.IsCurrentLocation);
                Assert.NotNull(currentRoom);

                // Should be able to generate map text
                var mapText = GenerateConsoleMapText(response.MapData);
                Assert.NotNull(mapText);
                Assert.True(mapText.Contains(currentRoom.Name));
            }
        }

        #region Helper Methods

        private void ValidateMapDataStructure(PlayerMapData mapData, string context)
        {
            Assert.NotNull(mapData);
            Assert.True(mapData.VisitedRoomCount > 0, $"{context}: Should have visited rooms");
            Assert.NotEmpty(mapData.DiscoveredRooms);
            Assert.NotNull(mapData.RenderingConfig);
            Assert.False(string.IsNullOrEmpty(mapData.RenderingConfig.GameName), $"{context}: Game name should be set");
            
            // Should have exactly one current location
            var currentRooms = mapData.DiscoveredRooms.Where(r => r.IsCurrentLocation).ToList();
            Assert.Single(currentRooms);
            
            // Current room should match the current room number
            var currentRoom = currentRooms.First();
            Assert.Equal(mapData.CurrentRoom, currentRoom.RoomNumber);
            
            // All rooms should have valid data
            foreach (var room in mapData.DiscoveredRooms)
            {
                Assert.True(room.RoomNumber > 0, $"{context}: Room {room.RoomNumber} should have positive number");
                Assert.False(string.IsNullOrEmpty(room.Name), $"{context}: Room {room.RoomNumber} should have name");
                Assert.NotNull(room.Position);
                Assert.NotNull(room.Connections);
            }
        }

        private void ValidateMapText(string mapText, PlayerMapData mapData, string context)
        {
            Assert.True(mapText.Contains(mapData.RenderingConfig.GameName));
            Assert.True(mapText.Contains("Current Location:"));
            Assert.True(mapText.Contains("Rooms Visited:"));
            Assert.True(mapText.Contains("Discovered Rooms:"));
            
            // Should contain current room name
            var currentRoom = mapData.DiscoveredRooms.FirstOrDefault(r => r.IsCurrentLocation);
            if (currentRoom != null)
            {
                Assert.True(mapText.Contains(currentRoom.Name));
                Assert.True(mapText.Contains("@"));
            }
        }

        private MapModel CreateMapModelFromPlayerMapData(PlayerMapData playerMapData)
        {
            // This is a simplified conversion for testing purposes
            // In a real implementation, you'd need the full room data from the server
            var rooms = new List<AdventureRealms.Services.AdventureServer.Models.Room>();
            
            foreach (var discoveredRoom in playerMapData.DiscoveredRooms)
            {
                var room = new AdventureRealms.Services.AdventureServer.Models.Room
                {
                    Number = discoveredRoom.RoomNumber,
                    Name = discoveredRoom.Name,
                    // For testing, set some default connections
                    N = 99, S = 99, E = 99, W = 99, U = 99, D = 99
                };
                rooms.Add(room);
            }

            // Create a basic game configuration for testing
            var gameConfig = new AdventureRealms.Services.Data.AdventureData.AdventureHouseConfiguration();
            
            return new MapModel(gameConfig, rooms, playerMapData.CurrentRoom);
        }

        private string GenerateConsoleMapText(PlayerMapData mapData)
        {
            if (mapData == null || mapData.DiscoveredRooms.Count == 0)
            {
                return "No map data available.";
            }

            var mapText = new StringBuilder();
            mapText.AppendLine($"Map for {mapData.RenderingConfig.GameName} - {mapData.CurrentLevelDisplayName}");
            mapText.AppendLine($"Current Location: {mapData.DiscoveredRooms.FirstOrDefault(r => r.IsCurrentLocation)?.Name ?? "Unknown"}");
            mapText.AppendLine($"Rooms Visited: {mapData.VisitedRoomCount}");
            mapText.AppendLine();
            
            // Simple list of discovered rooms
            mapText.AppendLine("Discovered Rooms:");
            foreach (var room in mapData.DiscoveredRooms.OrderBy(r => r.RoomNumber))
            {
                var marker = room.IsCurrentLocation ? "@ " : "  ";
                var items = room.HasItems ? " [+]" : "";
                mapText.AppendLine($"{marker}{room.Name} ({room.RoomNumber}){items}");
            }
            
            return mapText.ToString();
        }

        #endregion
    }
}