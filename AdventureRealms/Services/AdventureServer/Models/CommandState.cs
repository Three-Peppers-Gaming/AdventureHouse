using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureRealms.Services.AdventureServer.Models
{
    public class CommandState
    {
        public bool Valid { get; set; } = false;
        public string RawCommand { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty; 
        public string Modifier { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}