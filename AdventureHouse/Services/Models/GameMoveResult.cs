using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureHouse.Services.Models
{
    public class GameMoveResult
    {
        public string InstanceID { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string RoomMessage { get; set; } = string.Empty;
        public string ItemsMessage { get; set; } = string.Empty;
        public string HealthReport { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;

    }
}
