using System;
using System.Collections.Generic;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    public class MapDebugTest
    {
        public static void RunMapTest()
        {
            Console.WriteLine("=== Map Debug Test ===");
            
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
            Console.WriteLine($"   MapData exists: {gameResult.MapData != null}");

            if (gameResult.MapData != null)
            {
                Console.WriteLine("2. Analyzing initial MapData...");
                var mapDataType = gameResult.MapData.GetType();
                Console.WriteLine($"   MapData type: {mapDataType.Name}");
                
                var mapStateProperty = mapDataType.GetProperty("MapState");
                if (mapStateProperty != null)
                {
                    var mapStateValue = mapStateProperty.GetValue(gameResult.MapData);
                    if (mapStateValue is AdventureHouse.Services.AdventureClient.Models.MapState mapState)
                    {
                        Console.WriteLine($"   MapState CurrentRoom: {mapState.CurrentRoom}");
                        Console.WriteLine($"   MapState Level: {mapState.CurrentLevel}");
                        Console.WriteLine($"   MapState VisitedRooms: {mapState.VisitedRoomCount}");
                        
                        var asciiMap = mapState.GenerateCurrentLevelMap();
                        Console.WriteLine($"   ASCII Map length: {asciiMap?.Length ?? 0}");
                        if (!string.IsNullOrWhiteSpace(asciiMap))
                        {
                            Console.WriteLine("   Generated ASCII Map:");
                            Console.WriteLine(asciiMap);
                        }
                        else
                        {
                            Console.WriteLine("   ASCII Map is empty or null");
                        }
                    }
                }
            }

            // Try a movement command
            Console.WriteLine("\n3. Testing Movement Command...");
            var moveResult = server.FrameWork_GameMove(new GameMove 
            { 
                InstanceID = gameResult.InstanceID, 
                Move = "south"
            });

            Console.WriteLine($"   Move result room: {moveResult.RoomName}");
            Console.WriteLine($"   MapData exists after move: {moveResult.MapData != null}");

            if (moveResult.MapData != null)
            {
                var mapDataType = moveResult.MapData.GetType();
                var mapStateProperty = mapDataType.GetProperty("MapState");
                if (mapStateProperty != null)
                {
                    var mapStateValue = mapStateProperty.GetValue(moveResult.MapData);
                    if (mapStateValue is AdventureHouse.Services.AdventureClient.Models.MapState mapState)
                    {
                        Console.WriteLine($"   MapState after move - CurrentRoom: {mapState.CurrentRoom}");
                        Console.WriteLine($"   MapState after move - VisitedRooms: {mapState.VisitedRoomCount}");
                        
                        var asciiMap = mapState.GenerateCurrentLevelMap();
                        if (!string.IsNullOrWhiteSpace(asciiMap))
                        {
                            Console.WriteLine("   ASCII Map after movement:");
                            Console.WriteLine(asciiMap);
                        }
                    }
                }
            }
            
            Console.WriteLine("\n=== Map Debug Test Complete ===");
        }
    }
}