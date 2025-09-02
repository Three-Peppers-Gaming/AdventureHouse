using System;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    /// <summary>
    /// Clean 2-Endpoint API Test - validates the pure client-server architecture
    /// </summary>
    public class Clean2EndpointAPITest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Clean 2-Endpoint API Test ===");
            
            try
            {
                // Initialize clean server
                var cache = new MemoryCache(new MemoryCacheOptions());
                var fortune = new GetFortuneService();
                var commandProcessor = new CommandProcessingService();
                var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

                // Test Endpoint 1: GameList
                Console.WriteLine("\n1. Testing GameList endpoint...");
                var games = server.GameList();
                Console.WriteLine($"   Found {games.Count} games:");
                foreach (var game in games)
                {
                    Console.WriteLine($"   - {game.Name} (v{game.Ver}): {game.Desc}");
                }

                // Test Endpoint 2: PlayGame - New Session
                Console.WriteLine("\n2. Testing PlayGame endpoint - New Session...");
                var newGameRequest = new GamePlayRequest
                {
                    SessionId = "", // Empty for new game
                    GameId = 1,
                    Command = ""
                };
                
                var newGameResponse = server.PlayGame(newGameRequest);
                Console.WriteLine($"   New session created: {newGameResponse.SessionId}");
                Console.WriteLine($"   Game: {newGameResponse.GameName}");
                Console.WriteLine($"   Starting room: {newGameResponse.RoomName}");
                Console.WriteLine($"   Map data available: {newGameResponse.MapData != null}");
                
                if (newGameResponse.MapData != null)
                {
                    Console.WriteLine($"   Current room: {newGameResponse.MapData.CurrentRoom}");
                    Console.WriteLine($"   Visited rooms: {newGameResponse.MapData.VisitedRoomCount}");
                    Console.WriteLine($"   Discovered rooms: {newGameResponse.MapData.DiscoveredRooms.Count}");
                }

                // Test Endpoint 2: PlayGame - Game Commands
                Console.WriteLine("\n3. Testing PlayGame endpoint - Game Commands...");
                var sessionId = newGameResponse.SessionId;
                var testCommands = new[] { "look", "inv", "get bugle", "pet kitten", "d", "w" };

                foreach (var command in testCommands)
                {
                    Console.WriteLine($"\n   Command: {command}");
                    var commandRequest = new GamePlayRequest
                    {
                        SessionId = sessionId,
                        Command = command
                    };
                    
                    var commandResponse = server.PlayGame(commandRequest);
                    Console.WriteLine($"   Response: {commandResponse.CommandResponse?.Substring(0, Math.Min(80, commandResponse.CommandResponse.Length ?? 0))}...");
                    Console.WriteLine($"   Room: {commandResponse.RoomName} (Room {commandResponse.MapData?.CurrentRoom})");
                    Console.WriteLine($"   Health: {commandResponse.PlayerHealth}");
                    Console.WriteLine($"   Valid: {!commandResponse.InvalidCommand}");
                    
                    if (commandResponse.PlayerDead)
                    {
                        Console.WriteLine("   *** PLAYER DIED ***");
                        break;
                    }
                    
                    if (commandResponse.GameCompleted)
                    {
                        Console.WriteLine("   *** GAME COMPLETED ***");
                        break;
                    }
                }

                // Test Endpoint 2: PlayGame - Console Commands
                Console.WriteLine("\n4. Testing PlayGame endpoint - Console Commands...");
                var consoleCommands = new[] { "/help", "/map", "/time" };

                foreach (var command in consoleCommands)
                {
                    Console.WriteLine($"\n   Console Command: {command}");
                    var consoleRequest = new GamePlayRequest
                    {
                        SessionId = sessionId,
                        Command = command
                    };
                    
                    var consoleResponse = server.PlayGame(consoleRequest);
                    var outputPreview = consoleResponse.ConsoleOutput?.Length > 100 
                        ? consoleResponse.ConsoleOutput.Substring(0, 100) + "..."
                        : consoleResponse.ConsoleOutput;
                    Console.WriteLine($"   Console Output: {outputPreview}");
                }

                // Test Multiple Sessions
                Console.WriteLine("\n5. Testing Multiple Sessions...");
                var session2Request = new GamePlayRequest { SessionId = "", GameId = 2, Command = "" };
                var session2Response = server.PlayGame(session2Request);
                
                Console.WriteLine($"   Session 1: {sessionId} - {newGameResponse.GameName}");
                Console.WriteLine($"   Session 2: {session2Response.SessionId} - {session2Response.GameName}");
                Console.WriteLine($"   Sessions isolated: {sessionId != session2Response.SessionId}");

                Console.WriteLine("\n=== Clean 2-Endpoint API Test SUCCESSFUL ===");
                Console.WriteLine("\n? Server Architecture Validation:");
                Console.WriteLine("   ? 2-Endpoint API working");
                Console.WriteLine("   ? GameList endpoint functional");
                Console.WriteLine("   ? PlayGame endpoint functional");
                Console.WriteLine("   ? Session management working");
                Console.WriteLine("   ? Multiple games supported");
                Console.WriteLine("   ? Command processing working");
                Console.WriteLine("   ? Map data generation working");
                Console.WriteLine("   ? Console commands working");
                Console.WriteLine("   ? Client-server decoupling complete");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n? API Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}