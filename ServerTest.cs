using System;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    /// <summary>
    /// Simple server test to validate the map functionality with specific commands
    /// Tests: pet kitten, get bugle, d (down), w (west)
    /// </summary>
    public class SimpleServerTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Adventure Server Test ===");
            
            // Initialize services
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Start a game session
            Console.WriteLine("1. Starting Game Session...");
            var gameResult = server.FrameWork_StartGameSession(1, false, false);
            Console.WriteLine($"   Game started: {gameResult.InstanceID}");
            Console.WriteLine($"   Current room: {gameResult.RoomName}");
            Console.WriteLine($"   Items: {gameResult.ItemsMessage}");
            Console.WriteLine($"   MapData exists: {gameResult.MapData != null}");

            if (gameResult.MapData is PlayerMapData initialMapData)
            {
                Console.WriteLine("2. Analyzing initial MapData...");
                Console.WriteLine($"   MapData CurrentRoom: {initialMapData.CurrentRoom}");
                Console.WriteLine($"   MapData Level: {initialMapData.CurrentLevel}");
                Console.WriteLine($"   MapData VisitedRooms: {initialMapData.VisitedRoomCount}");
                Console.WriteLine($"   MapData DiscoveredRooms: {initialMapData.DiscoveredRooms.Count}");
            }

            // Test the exact commands: pet kitten, get bugle, d, w
            var testCommands = new[]
            {
                "pet kitten",
                "get bugle", 
                "d",  // down
                "w"   // west
            };

            Console.WriteLine("\n3. Testing specific commands...");
            foreach (var command in testCommands)
            {
                Console.WriteLine($"\n--- Command: {command} ---");
                var moveResult = server.FrameWork_GameMove(new GameMove 
                { 
                    InstanceID = gameResult.InstanceID, 
                    Move = command
                });

                Console.WriteLine($"   Result room: {moveResult.RoomName}");
                Console.WriteLine($"   Room message: {moveResult.RoomMessage?.Substring(0, Math.Min(100, moveResult.RoomMessage.Length))}...");
                Console.WriteLine($"   Items: {moveResult.ItemsMessage}");
                Console.WriteLine($"   Health: {moveResult.HealthReport}");

                if (moveResult.MapData is PlayerMapData mapData)
                {
                    Console.WriteLine($"   MapData CurrentRoom: {mapData.CurrentRoom}");
                    Console.WriteLine($"   MapData VisitedRooms: {mapData.VisitedRoomCount}");
                    Console.WriteLine($"   MapData DiscoveredRooms: {mapData.DiscoveredRooms.Count}");
                    
                    // Show current location details
                    var currentRoom = mapData.DiscoveredRooms.FirstOrDefault(r => r.IsCurrentLocation);
                    if (currentRoom != null)
                    {
                        Console.WriteLine($"   Current location: {currentRoom.Name} (Room {currentRoom.RoomNumber})");
                        Console.WriteLine($"   Has items: {currentRoom.HasItems}");
                        Console.WriteLine($"   Connections: {currentRoom.Connections.Count}");
                        
                        // Show connections
                        if (currentRoom.Connections.Any())
                        {
                            Console.WriteLine($"   Available connections:");
                            foreach (var conn in currentRoom.Connections)
                            {
                                Console.WriteLine($"     {conn.Direction} -> Room {conn.TargetRoom}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("   MapData is null or wrong type");
                }
            }

            // Show final game state
            Console.WriteLine("\n4. Final state summary...");
            var finalState = server.FrameWork_GameMove(new GameMove 
            { 
                InstanceID = gameResult.InstanceID, 
                Move = "look"
            });

            if (finalState.MapData is PlayerMapData finalMapData)
            {
                Console.WriteLine($"   Final room: {finalState.RoomName} (Room {finalMapData.CurrentRoom})");
                Console.WriteLine($"   Total rooms visited: {finalMapData.VisitedRoomCount}");
                Console.WriteLine($"   Game: {finalMapData.RenderingConfig.GameName}");
                
                Console.WriteLine("\n   All discovered rooms:");
                foreach (var room in finalMapData.DiscoveredRooms.OrderBy(r => r.RoomNumber))
                {
                    var marker = room.IsCurrentLocation ? "@ " : "  ";
                    var items = room.HasItems ? " [+]" : "";
                    Console.WriteLine($"   {marker}{room.Name} (Room {room.RoomNumber}){items}");
                }
            }
            
            Console.WriteLine("\n=== Adventure Server Test Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}