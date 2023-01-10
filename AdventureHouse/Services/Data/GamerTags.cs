using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AdventureHouse.Services.Data.AdventureData
{
    public class GamerTags
    {
        public List<string> GetTags()
        {
            return Tags;
        }

        public string RandomTag()
        {
            return GetTags()[new Random().Next(0, GetTags().Count())];
        }

        public List<string> Tags => new List<string>
            {
                "Player One",
                "Cute Gamer",
                "Minecraft Steve",
                "Miner Alex",
                "Mr. Washer",
                "Radio Head",
                "Fuzz Ball",
                "Fluffy Frog",
                "Missing Droid",
                "Time Travler",
                "Mr. Smith",
                "Top Hatter",
                "Bat Master",
                "Red Winter",
                "Brian Boitano",
                "Alvin",
                "S Qubed",
                "Person X"
            };

  
    }
}
