using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureRealms.Services.AdventureServer.Models
{
    public class Room
    {
        public int Number { get; set; } = 0;
        public string Name { get; set; } = string.Empty;    
        public string Desc { get; set; } = string.Empty;
        public int N { get; set; } = 0;
        public int S { get; set; } = 0;
        public int E { get; set; } = 0;
        public int W { get; set; } = 0;
        public int U { get; set; } = 0;
        public int D { get; set; } = 0;
        public int RoomPoints { get; set; } = 0; 
    }
}