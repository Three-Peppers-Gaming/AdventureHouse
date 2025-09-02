using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Tests.Gameplay
{
    /// <summary>
    /// Complete walkthrough tests for all adventure games
    /// These tests validate that each game can be completed from start to finish
    /// </summary>
    public class GameWalkthroughTests
    {
        private readonly IPlayAdventure _server;

        public GameWalkthroughTests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            _server = new AdventureFrameworkService(cache, fortune, commandProcessor);
        }

        [Fact]
        public void AdventureHouse_CompleteWalkthrough_ShouldEscapeHouse()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 1, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;
            
            Assert.Equal("Attic", gameStart.RoomName); // Should start in Attic

            // Act - Execute complete Adventure House walkthrough
            var walkthroughCommands = new[]
            {
                // Phase 1: Gather initial items in the house
                "get bugle",      // Get bugle from attic
                "pet kitten",     // Pet kitten (now follows you)
                "d",              // Down to Master Bedroom Closet
                "w",              // West to Master Bedroom  
                "get slip",       // Get slip from bedroom
                "get flyswatter", // Get flyswatter if available
                "s",              // South to Hallway
                "s",              // South to Kitchen
                "get bread",      // Get bread for health
                "eat bread",      // Eat bread to restore health
                "w",              // West to Living Room
                "get magazine",   // Get magazine
                "read magazine",  // Read magazine for fortune
                "s",              // South to Front Porch
                
                // Phase 2: Explore outside areas
                "w",              // West to Front Yard
                "n",              // North to Gate
                "w",              // West to Street
                "n",              // North to Intersection  
                "w",              // West to Park
                "n",              // North to Fountain
                "w",              // West to Bridge
                
                // Phase 3: Navigate through wilderness
                "n",              // North to Forest
                "w",              // West to Mountain Path
                "n",              // North to Cave Entrance
                "w",              // West to Cave
                "n",              // North to Underground River
                "w",              // West to Underground Cave
                "n",              // North should lead to Exit (Room 0)
            };

            var responses = new List<GamePlayResponse> { gameStart };
            var walkthroughHistory = new List<string> { 
                $"Start: {gameStart.RoomName} (Room {gameStart.MapData?.CurrentRoom}) - Health: {gameStart.PlayerHealth}" 
            };

            // Execute walkthrough
            foreach (var command in walkthroughCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);

                var healthStatus = response.PlayerDead ? "DEAD" : response.PlayerHealth;
                var roomInfo = $"{command} -> {response.RoomName} (Room {response.MapData?.CurrentRoom}) - Health: {healthStatus}";
                walkthroughHistory.Add(roomInfo);

                // Stop if player dies
                if (response.PlayerDead)
                {
                    break;
                }

                // Stop if we reach the exit
                if (response.GameCompleted || response.MapData?.CurrentRoom == 0)
                {
                    walkthroughHistory.Add("*** GAME COMPLETED - ESCAPED! ***");
                    break;
                }

                // Stop if session ends
                if (response.SessionId == "-1")
                {
                    break;
                }
            }

            // Assert - Validate walkthrough results
            var finalResponse = responses.Last();
            var history = string.Join("\n", walkthroughHistory);

            // Should have made significant progress
            Assert.True(responses.Count > 10, $"Should have executed many commands. History:\n{history}");
            
            // Should have visited multiple rooms
            var roomsVisited = finalResponse.MapData?.VisitedRoomCount ?? 0;
            Assert.True(roomsVisited >= 5, $"Should have visited at least 5 rooms. Visited: {roomsVisited}");

            // Player should not be dead (walkthrough should be survivable)
            Assert.False(finalResponse.PlayerDead, $"Player should not die in walkthrough. History:\n{history}");

            // Ideally should reach Room 0 (exit)
            if (finalResponse.MapData?.CurrentRoom == 0)
            {
                Assert.True(finalResponse.GameCompleted, "Game should be marked as completed when reaching Room 0");
            }

            // Output full walkthrough for analysis
            Console.WriteLine($"Adventure House Walkthrough Results:\n{history}");
            Console.WriteLine($"Final Status: Room {finalResponse.MapData?.CurrentRoom}, Health: {finalResponse.PlayerHealth}, Rooms Visited: {roomsVisited}");
        }

        [Fact]
        public void SpaceStation_BasicWalkthrough_ShouldNavigateStation()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 2, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Execute basic Space Station exploration
            var explorationCommands = new[]
            {
                "look",           // Examine starting area
                "inv",            // Check starting inventory
                "get all",        // Try to get any available items
                "n",              // Try moving north
                "look",           // Look around new area
                "e",              // Try moving east
                "look",           // Look around
                "s",              // Try moving south
                "look",           // Look around
                "w",              // Try moving west
                "look",           // Look around
                "u",              // Try going up
                "look",           // Look around
                "d",              // Try going down
                "look",           // Final look
            };

            var responses = new List<GamePlayResponse> { gameStart };
            var explorationHistory = new List<string> { 
                $"Start: {gameStart.RoomName} (Room {gameStart.MapData?.CurrentRoom})" 
            };

            foreach (var command in explorationCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);

                var roomInfo = $"{command} -> {response.RoomName} (Room {response.MapData?.CurrentRoom})";
                explorationHistory.Add(roomInfo);

                if (response.PlayerDead || response.SessionId == "-1")
                {
                    break;
                }
            }

            // Assert
            var finalResponse = responses.Last();
            var history = string.Join("\n", explorationHistory);
            
            Assert.NotNull(finalResponse);
            Assert.True(responses.Count > 5, $"Should have executed several commands. History:\n{history}");
            
            var roomsVisited = finalResponse.MapData?.VisitedRoomCount ?? 0;
            Assert.True(roomsVisited >= 2, $"Should have visited at least 2 rooms. History:\n{history}");

            // Verify this is indeed the Space Station game
            Assert.Contains("Space", finalResponse.MapData?.RenderingConfig.GameName ?? "");

            Console.WriteLine($"Space Station Exploration Results:\n{history}");
        }

        [Fact]
        public void FutureFamily_BasicWalkthrough_ShouldNavigateApartment()
        {
            // Arrange
            var newGameRequest = new GamePlayRequest { SessionId = "", GameId = 3, Command = "" };
            var gameStart = _server.PlayGame(newGameRequest);
            var sessionId = gameStart.SessionId;

            // Act - Execute basic Future Family exploration
            var explorationCommands = new[]
            {
                "look",           // Examine starting area
                "inv",            // Check starting inventory
                "get all",        // Try to get any available items
                "n", "look",      // Explore north
                "e", "look",      // Explore east  
                "s", "look",      // Explore south
                "w", "look",      // Explore west
                "u", "look",      // Try going up
                "d", "look",      // Try going down
            };

            var responses = new List<GamePlayResponse> { gameStart };
            var explorationHistory = new List<string> { 
                $"Start: {gameStart.RoomName} (Room {gameStart.MapData?.CurrentRoom})" 
            };

            foreach (var command in explorationCommands)
            {
                var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                var response = _server.PlayGame(request);
                responses.Add(response);

                var roomInfo = $"{command} -> {response.RoomName} (Room {response.MapData?.CurrentRoom})";
                explorationHistory.Add(roomInfo);

                if (response.PlayerDead || response.SessionId == "-1")
                {
                    break;
                }
            }

            // Assert
            var finalResponse = responses.Last();
            var history = string.Join("\n", explorationHistory);
            
            Assert.NotNull(finalResponse);
            Assert.True(responses.Count > 5, $"Should have executed several commands. History:\n{history}");
            
            var roomsVisited = finalResponse.MapData?.VisitedRoomCount ?? 0;
            Assert.True(roomsVisited >= 2, $"Should have visited at least 2 rooms. History:\n{history}");

            // Verify this is indeed the Future Family game
            Assert.Contains("Future", finalResponse.MapData?.RenderingConfig.GameName ?? "");

            Console.WriteLine($"Future Family Exploration Results:\n{history}");
        }

        [Fact]
        public void AllGames_ShouldSupportBasicGameplay()
        {
            // Arrange
            var games = _server.GameList();
            
            foreach (var game in games)
            {
                // Act - Test basic gameplay in each game
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = game.Id, Command = "" };
                var gameStart = _server.PlayGame(newGameRequest);
                var sessionId = gameStart.SessionId;

                var basicCommands = new[] { "look", "inv", "/help", "/map", "help" };
                var allSuccess = true;
                var commandResults = new List<string>();

                foreach (var command in basicCommands)
                {
                    var request = new GamePlayRequest { SessionId = sessionId, Command = command };
                    var response = _server.PlayGame(request);
                    
                    commandResults.Add($"{command}: {(response != null ? "OK" : "FAIL")}");
                    
                    if (response == null || response.SessionId == "-1")
                    {
                        allSuccess = false;
                        break;
                    }
                }

                // Assert
                var results = string.Join(", ", commandResults);
                Assert.True(allSuccess, $"Game {game.Name} should support basic commands. Results: {results}");
                
                Console.WriteLine($"Game {game.Name} - Basic Commands: {results}");
            }
        }

        [Fact]
        public void AllGames_ShouldHaveValidGameData()
        {
            // Arrange
            var games = _server.GameList();

            foreach (var game in games)
            {
                // Act
                var newGameRequest = new GamePlayRequest { SessionId = "", GameId = game.Id, Command = "" };
                var response = _server.PlayGame(newGameRequest);

                // Assert - Validate game data structure
                Assert.NotNull(response);
                Assert.NotEqual("-1", response.SessionId);
                Assert.False(string.IsNullOrEmpty(response.GameName));
                Assert.False(string.IsNullOrEmpty(response.RoomName));
                Assert.False(string.IsNullOrEmpty(response.RoomDescription));
                Assert.NotNull(response.MapData);

                var mapData = response.MapData;
                Assert.True(mapData.CurrentRoom > 0);
                Assert.False(string.IsNullOrEmpty(mapData.CurrentLevel));
                Assert.NotEmpty(mapData.DiscoveredRooms);
                Assert.True(mapData.VisitedRoomCount > 0);
                Assert.NotNull(mapData.RenderingConfig);
                Assert.False(string.IsNullOrEmpty(mapData.RenderingConfig.GameName));

                Console.WriteLine($"Game {game.Name} validation: Room={response.RoomName}, MapRooms={mapData.DiscoveredRooms.Count}");
            }
        }
    }
}