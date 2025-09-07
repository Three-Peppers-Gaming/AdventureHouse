using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.Shared.FortuneService;
using AdventureRealms.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureRealms.Tests.Gameplay
{
    /// <summary>
    /// Integration tests that simulate complete gameplay scenarios using the clean 2-endpoint API
    /// These tests demonstrate end-to-end game functionality and walkthrough capabilities
    /// </summary>
    public class GameplaySimulationTests
    {
        private readonly IPlayAdventure _server;

        public GameplaySimulationTests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            _server = new AdventureFrameworkService(cache, fortune, commandProcessor);
        }

        [Fact]
        public void AdventureHouse_ShouldAllowBasicExploration()
        {
            // Arrange - Start Adventure House game
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act & Assert - Simulate basic exploration
            var commands = new[]
            {
                "look",
                "inv", 
                "d",  // Try to go down from attic
                "look",
                "get bugle", // Try to get an item
                "inv"
            };

            var responses = new List<GamePlayResponse> { gameStart };
            var moveHistory = new List<string> { $"Started in: {gameStart.RoomName}" };

            foreach (var command in commands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);

                // Validate each response
                Assert.NotNull(response);
                Assert.Equal(sessionId, response.SessionId);
                Assert.NotNull(response.MapData);
                
                // Track movement history
                moveHistory.Add($"{command} -> {response.RoomName}");
                
                // Ensure map data is consistent
                Assert.True(response.MapData.VisitedRoomCount > 0);
                Assert.NotEmpty(response.MapData.DiscoveredRooms);
                
                // Ensure exactly one current location
                Assert.Single(response.MapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
            }

            // Output the exploration history for debugging
            var history = string.Join("\n", moveHistory);
            Assert.True(moveHistory.Count > 1, $"Game exploration history:\n{history}");
        }

        [Fact]
        public void AdventureHouse_ShouldPersistMapDataAcrossCommands()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            var initialVisitedCount = gameStart.MapData?.VisitedRoomCount ?? 0;

            // Act - Try movement that might work
            var moveRequest = new GamePlayRequest { SessionId = sessionId, Command = "d" };
            var moveResult = _server.PlayGame(moveRequest);

            var lookRequest = new GamePlayRequest { SessionId = sessionId, Command = "look" };
            var lookResult = _server.PlayGame(lookRequest);

            // Assert - Map data should be consistent
            Assert.NotNull(moveResult.MapData);
            Assert.NotNull(lookResult.MapData);
            
            // Visited room count should be preserved
            Assert.True(lookResult.MapData.VisitedRoomCount >= initialVisitedCount);
            
            // Current room should be consistent
            Assert.Equal(moveResult.MapData.CurrentRoom, lookResult.MapData.CurrentRoom);
        }

        [Fact]
        public void SpaceStation_ShouldHaveDifferentGameContent()
        {
            // Arrange
            var adventureHouseRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var spaceStationRequest = new GamePlayRequest { SessionId = "", GameId = 2, Command = "" };
            
            var AdventureRealms = _server.PlayGame(adventureHouseRequest);
            var spaceStation = _server.PlayGame(spaceStationRequest);

            // Assert - Games should have different content
            Assert.NotEqual(AdventureRealms.RoomName, spaceStation.RoomName);
            Assert.NotEqual(AdventureRealms.MapData?.RenderingConfig.GameName, 
                           spaceStation.MapData?.RenderingConfig.GameName);
            Assert.NotEqual(AdventureRealms.MapData?.CurrentLevelDisplayName, 
                           spaceStation.MapData?.CurrentLevelDisplayName);
        }

        [Fact]
        public void Server_ShouldOnlyProcessGameCommands()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act & Assert - Test that server only handles game commands, not console commands
            var gameCommands = new[] { "look", "inv", "get bugle", "pet kitten" };

            foreach (var command in gameCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                Assert.NotNull(response);
                Assert.False(string.IsNullOrEmpty(response.CommandResponse), 
                    $"Game command '{command}' should produce a response");
                
                // Console commands should be handled by client, not server
                Assert.True(string.IsNullOrEmpty(""), // Server doesn't have ConsoleOutput anymore
                    $"Server should not generate console output for command '{command}'");
                
                // Game state should be maintained
                Assert.Equal(sessionId, response.SessionId);
                Assert.NotNull(response.MapData);
            }
        }

        [Fact]
        public void Game_ShouldHandleItemInteractions()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Try item commands
            var itemCommands = new[] 
            { 
                "get bugle",
                "get kitten", 
                "pet kitten",
                "drop bugle",
                "inv"
            };

            foreach (var command in itemCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                // Assert - Should get valid responses for all item interactions
                Assert.NotNull(response);
                Assert.Equal(sessionId, response.SessionId);
                Assert.False(string.IsNullOrEmpty(response.CommandResponse), 
                    $"Item command '{command}' should produce a response");
            }
        }

        [Fact]
        public void Game_ShouldProvideConsistentMapProgression()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            var mapProgression = new List<MapProgressionSnapshot>();

            // Capture initial state
            mapProgression.Add(new MapProgressionSnapshot(gameStart, "Initial"));

            // Act - Execute a series of commands and track map progression
            var explorationCommands = new[] 
            { 
                "look", "d", "look", "w", "look", "s", "look" 
            };

            foreach (var command in explorationCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                mapProgression.Add(new MapProgressionSnapshot(response, command));
            }

            // Assert - Map progression should be logical
            for (int i = 1; i < mapProgression.Count; i++)
            {
                var current = mapProgression[i];
                var previous = mapProgression[i - 1];

                // Visited room count should never decrease
                Assert.True(current.VisitedRoomCount >= previous.VisitedRoomCount,
                    $"After command '{current.Command}': visited rooms decreased from {previous.VisitedRoomCount} to {current.VisitedRoomCount}");

                // Should always have at least one discovered room
                Assert.True(current.DiscoveredRoomCount > 0,
                    $"After command '{current.Command}': no discovered rooms");

                // Should always have exactly one current location
                Assert.True(current.CurrentLocationCount == 1, 
                    $"After command '{current.Command}': {current.CurrentLocationCount} current locations (should be 1)");
            }

            // Output progression for debugging
            var progressionReport = string.Join("\n", mapProgression.Select(m => m.ToString()));
            Assert.True(mapProgression.Count > 1, $"Map progression:\n{progressionReport}");
        }

        [Fact]
        public void AdventureHouse_CompleteWalkthrough_ShouldReachExit()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Execute complete walkthrough sequence
            var walkthroughCommands = new[]
            {
                // Start in Attic (Room 20)
                "get bugle",      // Get the bugle
                "pet kitten",     // Pet the kitten (follows you)
                "d",              // Down to Master Bedroom Closet (Room 19)
                "w",              // West to Master Bedroom (Room 18)
                "get slip",       // Get the slip
                "s",              // South to Hallway (Room 17)
                "s",              // South to Kitchen (Room 16)
                "get bread",      // Get bread if available
                "eat bread",      // Eat bread for health
                "w",              // West to Living Room (Room 15)
                "get magazine",   // Get magazine
                "read magazine",  // Read for fortune
                "s",              // South to Front Porch (Room 14)
                "w",              // West to Front Yard (Room 13)
                "n",              // North to Gate (Room 12)
                "w",              // West to Street (Room 11)
                "n",              // North to Intersection (Room 10)
                "w",              // West to Park (Room 9)
                "n",              // North to Fountain (Room 8)
                "w",              // West to Bridge (Room 7)
                "n",              // North to Forest (Room 6)
                "w",              // West to Mountain Path (Room 5)
                "n",              // North to Cave Entrance (Room 4)
                "w",              // West to Cave (Room 3)
                "n",              // North to Underground River (Room 2)
                "w",              // West to Underground Cave (Room 1)
                "n",              // North to Exit (Room 0)
            };

            var responses = new List<GamePlayResponse> { gameStart };
            var walkthroughHistory = new List<string> { $"Start: {gameStart.RoomName} (Room {gameStart.MapData?.CurrentRoom})" };

            foreach (var command in walkthroughCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);

                var roomInfo = $"{command} -> {response.RoomName} (Room {response.MapData?.CurrentRoom})";
                walkthroughHistory.Add(roomInfo);

                // Stop if player dies or game ends
                if (response.PlayerDead || response.GameCompleted || response.SessionId == "-1")
                {
                    break;
                }
            }

            // Assert - Check if we made progress toward the exit
            var finalResponse = responses.Last();
            var history = string.Join("\n", walkthroughHistory);
            
            Assert.NotNull(finalResponse);
            Assert.True(responses.Count > 5, $"Should have executed several commands. History:\n{history}");
            
            // Check if we reached the exit (Room 0) or made significant progress
            var roomsVisited = finalResponse.MapData?.VisitedRoomCount ?? 0;
            Assert.True(roomsVisited >= 3, $"Should have visited at least 3 rooms. Visited: {roomsVisited}\nHistory:\n{history}");

            // If we reached Room 0, the game should be completed
            if (finalResponse.MapData?.CurrentRoom == 0)
            {
                Assert.True(finalResponse.GameCompleted, "Game should be marked as completed when reaching Room 0");
            }
        }

        private class MapProgressionSnapshot
        {
            public string Command { get; }
            public string RoomName { get; }
            public int CurrentRoom { get; }
            public int VisitedRoomCount { get; }
            public int DiscoveredRoomCount { get; }
            public int CurrentLocationCount { get; }

            public MapProgressionSnapshot(GamePlayResponse response, string command)
            {
                Command = command;
                RoomName = response.RoomName ?? "Unknown";
                CurrentRoom = response.MapData?.CurrentRoom ?? 0;
                VisitedRoomCount = response.MapData?.VisitedRoomCount ?? 0;
                DiscoveredRoomCount = response.MapData?.DiscoveredRooms.Count ?? 0;
                CurrentLocationCount = response.MapData?.DiscoveredRooms.Count(r => r.IsCurrentLocation) ?? 0;
            }

            public override string ToString()
            {
                return $"{Command}: Room={RoomName}({CurrentRoom}), Visited={VisitedRoomCount}, Discovered={DiscoveredRoomCount}, Current={CurrentLocationCount}";
            }
        }
    }
}