using AdventureHouse.Services;
using AdventureHouse.Services.Models;
using AdventureServer.Services;
using AdventurHouse.Services;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Adventure House";
            Console.SetWindowSize(80, 30);

            // Create services manually in dependency order
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var getFortuneService = new GetFortuneService();
            var gameInstanceService = new GameInstanceService(memoryCache);
            var commandProcessingService = new CommandProcessingService();
            var playerManagementService = new PlayerManagementService();
            var messageService = new MessageService();
            
            // Create services that depend on others
            var monsterManagementService = new MonsterManagementService(playerManagementService, messageService);
            var itemManagementService = new ItemManagementService(playerManagementService, null, messageService); // RoomManagementService will be set later
            var roomManagementService = new RoomManagementService(commandProcessingService, monsterManagementService, messageService);
            
            // Update itemManagementService with roomManagementService dependency
            // Note: This creates a circular dependency issue - we may need to refactor the services
            
            // Create the main framework service with all dependencies
            var adventureFrameworkService = new AdventureFrameworkService(
                memoryCache,
                getFortuneService,
                gameInstanceService,
                commandProcessingService,
                playerManagementService,
                monsterManagementService,
                itemManagementService,
                roomManagementService,
                messageService);
            
            // Start the game
            PlayAdventureClient.PlayAdventure(adventureFrameworkService);
        }
    }
}