using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureHouse.Services.Models
{
    public class PlayAdventure
    {
        // TODO: add a lifetime for this object so we can clear out the instances or use a cache
        public int GameID { get; set; } = 0;
        public string GameName { get; set; } = string.Empty;
        public string GameHelp { get; set; } = string.Empty;
        public string GameThanks { get; set; } = string.Empty;
        public string InstanceID { get; set; } = string.Empty;
        public string WelcomeMessage { get; set; } = string.Empty;
        public int StartRoom { get; set; } = 0;
        public int MaxHealth { get; set; } = 0;
        public int HealthStep { get; set; } = 0;
        public Player Player { get; set; } = new();
        public List<Item> Items { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();
        public List<Monster> Monsters { get; set; } = new();
        public Boolean GameActive { get; set; } = false;
        public List<string> PointsCheckList { get; set; } = new();

    }
}
