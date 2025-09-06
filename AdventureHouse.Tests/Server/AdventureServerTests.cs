using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Tests.Server
{
    /// <summary>
    /// Unit tests for the Clean Adventure Server API
    /// Tests the 2-endpoint architecture with 100% client-server decoupling
    /// </summary>
    public class AdventureServerTests
    {
        private readonly IPlayAdventure _server;

        public AdventureServerTests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            _server = new AdventureFrameworkService(cache, fortune, commandProcessor);
        }

        [Fact]
        public void GameList_ShouldReturnAvailableGames()
        {
            // Act
            var games = _server.GameList();

            // Assert
            Assert.NotNull(games);
            Assert.True(games.Count >= 3, "Should have at least 3 games available");
            
            // Verify each game has required properties
            foreach (var game in games)
            {
                Assert.True(game.Id > 0);
                Assert.False(string.IsNullOrEmpty(game.Name));
                Assert.False(string.IsNullOrEmpty(game.Ver));
                Assert.False(string.IsNullOrEmpty(game.Desc));
            }
        }

        [Fact]
        public void PlayGame_ShouldStartNewGameSession()
        {
            // Arrange
            var request = new GamePlayRequest
            {
                SessionId = "", // Empty for new game
                GameId = 1,
                Command = ""
            };

            // Act
            var response = _server.PlayGame(request);

            // Assert
            Assert.NotNull(response);
            Assert.False(string.IsNullOrEmpty(response.SessionId));
            Assert.NotEqual("-1", response.SessionId);
            Assert.NotNull(response.MapData);
            
            var mapData = response.MapData;
            Assert.True(mapData.CurrentRoom > 0);
            Assert.False(string.IsNullOrEmpty(mapData.CurrentLevel));
            Assert.True(mapData.VisitedRoomCount > 0);
            Assert.NotEmpty(mapData.DiscoveredRooms);
            
            // Should have exactly one current location
            Assert.Single(mapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
        }

        [Fact]
        public void PlayGame_ShouldProcessGameCommands()
        {
            // Arrange - Start new game
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            var testCommands = new[] { "look", "inv", "get bugle", "pet kitten", "d", "w" };

            foreach (var command in testCommands)
            {
                // Act
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(sessionId, response.SessionId);
                Assert.False(string.IsNullOrEmpty(response.RoomName));
                Assert.False(string.IsNullOrEmpty(response.RoomDescription));
                Assert.NotNull(response.MapData);
                
                var mapData = response.MapData;
                Assert.True(mapData.VisitedRoomCount > 0);
                Assert.NotEmpty(mapData.DiscoveredRooms);
                Assert.Single(mapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
            }
        }

        [Fact]
        public void PlayGame_ShouldHandleInvalidSession()
        {
            // Act
            var request = new GamePlayRequest 
            { 
                SessionId = "invalid-session-id", 
                Command = "look" 
            };
            var response = _server.PlayGame(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("-1", response.SessionId);
            Assert.Contains("expired", response.CommandResponse.ToLower());
            Assert.True(response.InvalidCommand);
        }

        [Fact]
        public void PlayGame_ShouldSupportMultipleSessions()
        {
            // Arrange - Start multiple game sessions
            var session1Request = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var session2Request = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            
            var session1 = _server.PlayGame(session1Request);
            var session2 = _server.PlayGame(session2Request);

            // Assert - Sessions should be independent
            Assert.NotEqual(session1.SessionId, session2.SessionId);

            // Act - Perform different commands in each session
            var session1Move = _server.PlayGame(new GamePlayRequest 
            { 
                SessionId = session1.SessionId, 
                Command = "get bugle" 
            });

            var session2Move = _server.PlayGame(new GamePlayRequest 
            { 
                SessionId = session2.SessionId, 
                Command = "pet kitten" 
            });

            // Assert - Each session should maintain its own state
            Assert.NotEqual(session1Move.CommandResponse, session2Move.CommandResponse);
            Assert.Equal(session1.SessionId, session1Move.SessionId);
            Assert.Equal(session2.SessionId, session2Move.SessionId);
        }

        [Fact]
        public void AllGames_ShouldHaveValidMapData()
        {
            // Get all available games
            var games = _server.GameList();

            foreach (var game in games)
            {
                // Arrange
                var request = new GamePlayRequest { SessionId = "", GameId = game.Id, Command = "" };
                var response = _server.PlayGame(request);

                // Assert
                Assert.NotNull(response);
                Assert.NotNull(response.MapData);
                
                var mapData = response.MapData;
                Assert.True(mapData.CurrentRoom > 0);
                Assert.False(string.IsNullOrEmpty(mapData.CurrentLevel));
                Assert.False(string.IsNullOrEmpty(mapData.CurrentLevelDisplayName));
                Assert.NotEmpty(mapData.DiscoveredRooms);
                Assert.True(mapData.VisitedRoomCount > 0);
                Assert.NotNull(mapData.RenderingConfig);
                
                // Validate rendering config
                Assert.False(string.IsNullOrEmpty(mapData.RenderingConfig.GameName));
                Assert.True(mapData.RenderingConfig.PlayerChar != '\0');
                Assert.True(mapData.RenderingConfig.ItemsChar != '\0');
                Assert.True(mapData.RenderingConfig.DefaultRoomChar != '\0');
                
                // Should have exactly one current location
                Assert.Single(mapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
            }
        }


        [Fact]
        public void PlayGame_ShouldTrackGameProgression()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Execute a series of commands and track progression
            var explorationCommands = new[] 
            { 
                "look", "d", "look", "w", "look", "s", "look" 
            };

            var responses = new List<GamePlayResponse> { gameStart };

            foreach (var command in explorationCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);
            }

            // Assert - Map progression should be logical
            for (int i = 1; i < responses.Count; i++)
            {
                var current = responses[i];
                var previous = responses[i - 1];

                // Visited room count should never decrease
                Assert.True(current.MapData.VisitedRoomCount >= previous.MapData.VisitedRoomCount,
                    $"Visited rooms decreased from {previous.MapData.VisitedRoomCount} to {current.MapData.VisitedRoomCount}");

                // Should always have at least one discovered room
                Assert.True(current.MapData.DiscoveredRooms.Count > 0);

                // Should always have exactly one current location
                Assert.Single(current.MapData.DiscoveredRooms.Where(r => r.IsCurrentLocation));
            }
        }

        [Fact]
        public void DifferentGames_ShouldHaveDifferentContent()
        {
            // Arrange
            var games = _server.GameList();
            Assert.True(games.Count >= 2, "Need at least 2 games for comparison");

            var game1Request = new GamePlayRequest { SessionId = "", GameId = games[0].Id, Command = "" };
            var game2Request = new GamePlayRequest { SessionId = "", GameId = games[1].Id, Command = "" };
            
            var game1 = _server.PlayGame(game1Request);
            var game2 = _server.PlayGame(game2Request);

            // Assert
            Assert.NotEqual(game1.RoomName, game2.RoomName);
            Assert.NotEqual(game1.MapData.RenderingConfig.GameName, game2.MapData.RenderingConfig.GameName);
        }

        [Fact]
        public void PlayGame_ShouldDetectGameCompletion()
        {
            // This test would need to be expanded with specific win conditions for each game
            // For now, just test that the game completion flag exists and can be set
            
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);

            // Assert
            Assert.False(gameStart.GameCompleted); // New game should not be completed
            Assert.False(gameStart.PlayerDead); // New player should not be dead
        }
    }
}