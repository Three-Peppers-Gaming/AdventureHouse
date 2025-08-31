using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureHouse.Services.Models
{
    public class Monster
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RoomNumber { get; set; } = 0;
        public string ObjectNameThatCanAttackThem { get; set; } = string.Empty;
        public int AttacksToKill { get; set; } = 1;
        public bool CanHitPlayer { get; set; } = false;
        public int HitOdds { get; set; } = 50; // 1-100, chance monster hits player
        public int HealthDamage { get; set; } = 10; // How much health monster takes from player
        public int AppearanceChance { get; set; } = 50; // 1-100, chance monster appears in room
        public bool IsPresent { get; set; } = false; // Whether monster is currently in the room
        public int CurrentHealth { get; set; } = 1; // Current health of monster (starts at AttacksToKill)
    }
}