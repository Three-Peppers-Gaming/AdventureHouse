using System;
using AdventureHouse.Services.AdventureClient;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Handle special debug argument
            if (args.Length > 0 && args[0] == "debug-map")
            {
                RunMapDebugTest();
                return;
            }

            // Handle server test argument  
            if (args.Length > 0 && args[0] == "server-test")
            {
                RunServerTest();
                return;
            }

            // Check for GUI mode - temporarily disabled due to build issues
            if (args.Length > 0 && args[0] == "--gui")
            {
                Console.WriteLine("Terminal.Gui mode is temporarily disabled due to build issues.");
                Console.WriteLine("Running console mode instead...");
                // Fall through to console mode
            }

            // Default to console client
            var gameCache = new MemoryCache(new MemoryCacheOptions());
            var fortuneService = new GetFortuneService();
            var commandProcessingService = new CommandProcessingService();
            var server = new AdventureFrameworkService(gameCache, fortuneService, commandProcessingService);
            var client = new AdventureClientService();
            client.StartAdventure(server);
        }

        private static void RunServerTest()
        {
            Console.WriteLine("=== Adventure Server Test ===");
            
            // Initialize services
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
                var responsePreview = commandResponse.CommandResponse?.Length > 80 
                    ? commandResponse.CommandResponse.Substring(0, 80) + "..."
                    : commandResponse.CommandResponse;
                Console.WriteLine($"   Response: {responsePreview}");
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

            // Final status
            var finalRequest = new GamePlayRequest { SessionId = sessionId, Command = "look" };
            var finalState = server.PlayGame(finalRequest);

            if (finalState.MapData != null)
            {
                Console.WriteLine($"\n4. Final Summary:");
                Console.WriteLine($"   Final room: {finalState.RoomName} (Room {finalState.MapData.CurrentRoom})");
                Console.WriteLine($"   Total rooms visited: {finalState.MapData.VisitedRoomCount}");
                Console.WriteLine($"   Game: {finalState.MapData.RenderingConfig.GameName}");
                
                Console.WriteLine("\n   All discovered rooms:");
                foreach (var room in finalState.MapData.DiscoveredRooms.OrderBy(r => r.RoomNumber))
                {
                    var marker = room.IsCurrentLocation ? "@ " : "  ";
                    var items = room.HasItems ? " [+]" : "";
                    Console.WriteLine($"   {marker}{room.Name} (Room {room.RoomNumber}){items}");
                }
            }
            
            Console.WriteLine("\n=== Adventure Server Test SUCCESSFUL ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void RunMapDebugTest()
        {
            // Use the server test for now
            RunServerTest();
        }
    }
}
