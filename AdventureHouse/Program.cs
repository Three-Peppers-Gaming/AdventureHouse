using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.AdventureClient;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Adventure House";
            Console.SetWindowSize(80, 30);

            // Create the core dependencies for the Adventure Server
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var getFortuneService = new GetFortuneService();
            var commandProcessingService = new CommandProcessingService();

            // Create the Adventure Server (contains all game logic)
            var adventureServer = new AdventureFrameworkService(
                memoryCache,
                getFortuneService,
                commandProcessingService);

            // Create the Adventure Client (handles all UI and user interaction)
            var adventureClient = new AdventureClientService();

            // Start the adventure - client connects to server
            adventureClient.StartAdventure(adventureServer);
        }
    }
}
