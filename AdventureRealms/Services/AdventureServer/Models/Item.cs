using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureRealms.Services.AdventureServer.Models
{
    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Location { get; set; } = 0; // valid locations are room number, 9998 for used and 9999 for inventory of player
        public string Action { get; set; } = string.Empty;
        public int ActionPoints { get; set; } = 0;
        public string ActionVerb { get; set; } = string.Empty;
        public string ActionResult { get; set; } = string.Empty;
        public string ActionValue { get; set; } = string.Empty;
    }
}