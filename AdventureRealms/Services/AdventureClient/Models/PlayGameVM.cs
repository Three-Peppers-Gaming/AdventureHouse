using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureRealms.Services.AdventureClient.Models
{
    public class PlayGameVM
    {
        public string GameID { get; set; } = string.Empty;
        public string GamerTag { get; set; }= string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Buffer { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Items { get; set; } = string.Empty;
        public string HealthReport { get; set; } = string.Empty;
    }
}