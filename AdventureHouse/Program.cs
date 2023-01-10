using AdventureHouse.Services;
using AdventureHouse.Services.Models;
using AdventureServer.Services;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse
{
    internal class Program
    {







        static void Main(string[] args)
        {
            Console.Title = "Adventure House";
            Console.SetWindowSize(80, 30);
           


            // Create and instance of the Adventure Framework Service
            var _advfw = new AdventureFrameworkService(new MemoryCache(new MemoryCacheOptions()), new GetFortuneService());
            PlayAdventureClient.PlayAdventure(_advfw);
            
        }
    }
}