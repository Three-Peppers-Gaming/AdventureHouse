using System.Diagnostics;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Tests.Performance
{
    /// <summary>
    /// Performance tests for the Clean Adventure Server API
    /// Tests scalability, concurrency, and performance of the 2-endpoint architecture
    /// </summary>
    public class ServerPerformanceTests
    {
        private readonly IPlayAdventure _server;

        public ServerPerformanceTests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            _server = new AdventureFrameworkService(cache, fortune, commandProcessor);
        }

        [Fact]
        public void Server_ShouldHandleQuickSuccessiveCommands()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var initialState = _server.PlayGame(newGameRequest);
            var sessionId = initialState.SessionId;
            
            Assert.NotNull(initialState.MapData);
            Assert.True(initialState.MapData.VisitedRoomCount > 0);

            var commands = new[] { "look", "inv", "get bugle", "look", "inv" };
            var results = new List<GamePlayResponse>();

            var stopwatch = Stopwatch.StartNew();

            // Act - Execute commands quickly
            foreach (var command in commands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var result = _server.PlayGame(request);
                results.Add(result);
            }

            stopwatch.Stop();

            // Assert - All commands should complete quickly
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
                $"Commands took {stopwatch.ElapsedMilliseconds}ms, should be < 1000ms");

            // All results should be valid
            foreach (var result in results)
            {
                Assert.NotNull(result);
                Assert.Equal(sessionId, result.SessionId);
                Assert.NotNull(result.MapData);
            }
        }

        [Fact]
        public void Server_ShouldIsolateConcurrentSessions()
        {
            // Arrange - Start two independent sessions
            var session1Request = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var session2Request = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            
            var session1 = _server.PlayGame(session1Request);
            var session2 = _server.PlayGame(session2Request);

            Assert.NotEqual(session1.SessionId, session2.SessionId);

            // Act - Perform different actions in each session
            var session1MoveRequest = new GamePlayRequest { SessionId = session1.SessionId, Command = "get bugle" };
            var session1Move = _server.PlayGame(session1MoveRequest);

            var session2MoveRequest = new GamePlayRequest { SessionId = session2.SessionId, Command = "pet kitten" };
            var session2Move = _server.PlayGame(session2MoveRequest);

            // Get final states
            var session1FinalRequest = new GamePlayRequest { SessionId = session1.SessionId, Command = "look" };
            var session1Final = _server.PlayGame(session1FinalRequest);

            var session2FinalRequest = new GamePlayRequest { SessionId = session2.SessionId, Command = "look" };
            var session2Final = _server.PlayGame(session2FinalRequest);

            // Assert - Sessions should remain isolated
            Assert.Equal(session1Final.MapData.CurrentRoom, session1.MapData?.CurrentRoom);
            Assert.Equal(session2Final.MapData.CurrentRoom, session2.MapData?.CurrentRoom);

            // Actions should have different effects
            Assert.NotEqual(session1Move.CommandResponse, session2Move.CommandResponse);
        }

        [Fact]
        public void Server_ShouldHandleAllGamesEfficiently()
        {
            // Arrange
            var games = _server.GameList();
            var stopwatch = Stopwatch.StartNew();

            foreach (var game in games)
            {
                // Act - Start each game and perform basic operations
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = game.Id, Command = "" };
                var gameStart = _server.PlayGame(newGameRequest);
                var gameName = game.Name;

                Assert.NotNull(gameStart);
                Assert.NotNull(gameStart.MapData);
                
                // Verify the game name is reflected in the map data
                Assert.Contains(gameName.Split(' ')[0], gameStart.MapData.RenderingConfig.GameName);

                // Perform a few basic commands (only game commands, no console commands)
                var commands = new[] { "look", "inv" };
                foreach (var command in commands)
                {
                    var request = new GamePlayRequest { SessionId = gameStart.SessionId, Command = command };
                    var result = _server.PlayGame(request);
                    Assert.NotNull(result);
                    Assert.Equal(gameStart.SessionId, result.SessionId);
                }
            }

            stopwatch.Stop();

            // Assert - Should handle all games quickly
            Assert.True(stopwatch.ElapsedMilliseconds < 5000, 
                $"All games took {stopwatch.ElapsedMilliseconds}ms, should be < 5000ms");
        }

        [Fact]
        public void Server_ShouldMaintainMapStateConsistency()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            var mapDataSnapshots = new List<PlayerMapData> { gameStart.MapData };

            // Act - Execute multiple commands and capture map state
            var commands = new[] { "look", "inv", "d", "look", "w", "inv", "look" };
            foreach (var command in commands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var result = _server.PlayGame(request);
                mapDataSnapshots.Add(result.MapData);
            }

            // Assert - Map state should be consistent throughout
            foreach (var mapData in mapDataSnapshots)
            {
                Assert.NotNull(mapData);
                Assert.True(mapData.VisitedRoomCount > 0);
                Assert.NotEmpty(mapData.DiscoveredRooms);
                Assert.NotNull(mapData.RenderingConfig);
                
                // Should always have exactly one current location
                Assert.Single(mapData.DiscoveredRooms, r => r.IsCurrentLocation);
                
                // All discovered rooms should have valid data
                foreach (var room in mapData.DiscoveredRooms)
                {
                    Assert.True(room.RoomNumber > 0);
                    Assert.False(string.IsNullOrEmpty(room.Name));
                    Assert.NotNull(room.Connections);
                    
                    // All connections should point to discovered rooms
                    foreach (var connection in room.Connections)
                    {
                        Assert.Contains(connection.TargetRoom, 
                            mapData.DiscoveredRooms.Select(r => r.RoomNumber));
                    }
                }
            }
        }

        [Fact]
        public void Server_ShouldHandleInvalidInputsGracefully()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act & Assert - Test various invalid inputs
            var invalidCommands = new[] 
            { 
                "xyzabc123",           // Nonsense command
                "get nonexistentitem", // Invalid item
                "go invalidirection",  // Invalid direction
                "/invalidconsole",     // Invalid console command
                "",                    // Empty command
                new string('a', 1000)  // Very long command
            };

            foreach (var command in invalidCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var result = _server.PlayGame(request);
                
                // Should not crash and should return valid response
                Assert.NotNull(result);
                Assert.Equal(sessionId, result.SessionId); // Session should remain valid
                Assert.NotNull(result.MapData);
                
                // Most invalid commands should be marked as invalid
                // (though some might be handled gracefully with explanatory messages)
            }
        }

        [Fact]
        public void Server_ShouldScaleWithMultipleGames()
        {
            // Arrange - Create multiple concurrent game sessions
            var sessionCount = 10;
            var sessions = new List<string>();
            var stopwatch = Stopwatch.StartNew();

            // Act - Create multiple sessions
            for (int i = 0; i < sessionCount; i++)
            {
                var gameId = (i % 3) + 1; // Rotate through all 3 games
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = gameId, Command = "" };
                var gameStart = _server.PlayGame(newGameRequest);
                sessions.Add(gameStart.SessionId);
                
                Assert.NotEqual("-1", gameStart.SessionId);
                Assert.NotNull(gameStart.MapData);
            }

            // Execute commands in all sessions
            foreach (var sessionId in sessions)
            {
                var commands = new[] { "look", "inv", "d", "look" };
                foreach (var command in commands)
                {
                    var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                    var result = _server.PlayGame(request);
                    Assert.NotNull(result);
                    Assert.Equal(sessionId, result.SessionId);
                }
            }

            stopwatch.Stop();

            // Assert - Should handle multiple sessions efficiently
            Assert.True(stopwatch.ElapsedMilliseconds < 3000, 
                $"Multiple sessions took {stopwatch.ElapsedMilliseconds}ms, should be < 3000ms");
            
            // All sessions should be unique
            Assert.Equal(sessionCount, sessions.Distinct().Count());
        }

        [Fact]
        public void Server_ShouldOnlyHandleGameCommands()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act & Assert - Test that server processes game commands (even if game logic makes them invalid)
            var gameCommands = new[] { "look", "inv", "get bugle", "pet kitten", "d", "w" };

            foreach (var command in gameCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                Assert.NotNull(response);
                
                // Game commands should NOT be rejected by server (InvalidCommand should be false)
                // Even if the game logic finds them invalid (like "look" without parameters)
                Assert.False(response.InvalidCommand, 
                    $"Game command '{command}' should not be rejected by server (InvalidCommand=true indicates server rejection)");
                
                Assert.Equal(sessionId, response.SessionId);
                Assert.NotNull(response.MapData);
            }

            // Test that console commands are properly rejected by server
            var consoleCommands = new[] { "/help", "/map", "/time", "map", "resign" };

            foreach (var command in consoleCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                Assert.NotNull(response);
                Assert.True(response.InvalidCommand, 
                    $"Console command '{command}' should be rejected by server");
                Assert.Contains("Console commands are not supported", response.CommandResponse);
                Assert.Equal(sessionId, response.SessionId); // Session should remain valid
            }

            // Test that the 'help' command goes to the server and returns adventure-specific help
            var helpRequest = new GamePlayRequest { SessionId = sessionId, Command = "help" };
            var helpResponse = _server.PlayGame(helpRequest);
            
            Assert.NotNull(helpResponse);
            Assert.False(helpResponse.InvalidCommand, "The 'help' command should be processed by server as a game command");
            Assert.False(string.IsNullOrEmpty(helpResponse.CommandResponse), "Server should return adventure-specific help text");
            Assert.Contains("adventure", helpResponse.CommandResponse.ToLower()); // Help response should contain adventure-specific content
            Assert.Equal(sessionId, helpResponse.SessionId);
        }
    }
}