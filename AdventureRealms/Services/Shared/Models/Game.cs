using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureRealms.Services.Shared.Models
{
    public class Game
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string Ver { get; set; } = string.Empty;
    }
}