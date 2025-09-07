using System;
using AdventureRealms.Services.AdventureClient;
using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.Shared.FortuneService;
using AdventureRealms.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureRealms
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
            
            // Initialize server services
            var gameCache = new MemoryCache(new MemoryCacheOptions());
            var fortuneService = new GetFortuneService();
            var commandProcessingService = new CommandProcessingService();
            var server = new AdventureFrameworkService(gameCache, fortuneService, commandProcessingService);

            // Check for console/classic mode
            if (args.Length > 0 && (args[0] == "--console" || args[0] == "--classic" || args[0] == "-c"))
            {
                Console.WriteLine("Starting in classic console mode...");
                var consoleClient = new AdventureClientService();
                consoleClient.StartAdventure(server);
                return;
            }

            // Default to Terminal.Gui mode
            try
            {
                Console.WriteLine("Starting Adventure Realms with Terminal.Gui interface...");
                var guiClient = new TerminalGuiAdventureClient(server);
                guiClient.StartAdventure(server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start Terminal.Gui interface: {ex.Message}");
                Console.WriteLine("Falling back to classic console mode...");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                
                var consoleClient = new AdventureClientService();
                consoleClient.StartAdventure(server);
            }
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
