using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureHouse.Services.AdventureServer.Models
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public string PlayerID { get; set; } = string.Empty;
        public int Room { get; set; } = 0;
        public int HealthCurrent { get; set; } = 0;
        public int HealthMax { get; set; } = 0;
        public int Steps { get; set; } = 0;
        public bool Verbose { get; set; } = false;
        public int Points { get; set; } = 0;
        public bool PlayerDead { get; set; } = false;
    }
}