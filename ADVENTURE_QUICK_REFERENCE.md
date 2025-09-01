# Adventure Creation Quick Reference

This is a quick reference guide with code templates and common patterns for creating adventures in the Adventure House framework.

## Quick Start Template

### 1. Configuration Class Template
```csharp
using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    public class [AdventureName]Configuration : IGameConfiguration
    {
        public string GameName => "[Adventure Title]";
        public string GameVersion => "1.0";
        public string GameDescription => "[Brief description for game selection]";

        // Copy room display patterns from AdventureHouseConfiguration.cs
        public Dictionary<string, char> RoomDisplayCharacters => new() { /* room mappings */ };
        public Dictionary<string, char> RoomTypeCharacters => new() { /* type patterns */ };
        public Dictionary<string, int> RoomNameToNumberMapping => new() { /* name->number */ };

        public int StartingRoom => 1;
        public int MaxHealth => 200;
        public int HealthStep => 2;
        public int StartingPoints => 1;
        public string InitialPointsCheckList => "NewGame";

        // Keep these constants
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';

        // Implement all other required interface methods...
    }
}
```

### 2. Data Class Template
```csharp
using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    public class [AdventureName]Data
    {
        private readonly [AdventureName]Configuration _config = new();

        public string GetAdventureHelpText() => _config.GetAdventureHelpText();
        public string GetAdventureThankYouText() => _config.GetAdventureThankYouText();
        public [AdventureName]Configuration GetGameConfiguration() => _config;

        public PlayAdventure SetupAdventure(string gameid)
        {
            var _gamerTag = new GamerTags().RandomTag();
            return new PlayAdventure
            {
                GameID = 1,
                GameName = _config.GameName,
                GameHelp = _config.GetAdventureHelpText(),
                GameThanks = _config.GetAdventureThankYouText(),
                InstanceID = gameid,
                StartRoom = _config.StartingRoom,
                WelcomeMessage = _config.GetWelcomeMessage(_gamerTag),
                MaxHealth = _config.MaxHealth,
                HealthStep = _config.HealthStep,
                Items = Items(),
                Messages = Messages(),
                Rooms = Rooms(),
                Monsters = Monsters(),
                Player = new Player { 
                    Name = _gamerTag, 
                    PlayerID = gameid, 
                    HealthMax = _config.MaxHealth, 
                    HealthCurrent = _config.MaxHealth, 
                    Room = _config.StartingRoom, 
                    Steps = 2, 
                    Verbose = true, 
                    Points = _config.StartingPoints, 
                    PlayerDead = false 
                },
                GameActive = true,
                PointsCheckList = new List<string> { _config.InitialPointsCheckList }
            };
        }

        private static List<Room> Rooms() { /* implement */ }
        private static List<Item> Items() { /* implement */ }
        private static List<Message> Messages() { /* implement */ }
        private static List<Monster> Monsters() { /* implement */ }
    }
}
```

## Common Room Patterns

### Standard Room
```csharp
new Room { 
    Number = 1, 
    RoomPoints = 25, 
    Name = "Room Name", 
    Desc = "Room description.", 
    N = 2,      // Connected room or 99 for blocked
    S = 99,     // 99 = no connection
    E = 99, 
    W = 0,      // 0 = exit room
    U = 99,     // Up (stairs/ladder)
    D = 99      // Down
}
```

### Exit Room (Always Room 0)
```csharp
new Room { 
    Number = 0, 
    RoomPoints = 100, 
    Name = "Exit!", 
    Desc = "You have escaped!", 
    N = 99, S = 99, E = 99, W = 1, U = 99, D = 99 
}
```

### Debug Room
```csharp
#if DEBUG
new Room { 
    Number = 88, 
    RoomPoints = 0, 
    Name = "Debug Room", 
    Desc = "Developer testing area", 
    N = 99, S = 99, E = 99, W = 99, U = 1, D = 0 
}
#endif
```

## Common Item Patterns

### Food Item (Healing)
```csharp
new Item { 
    Name = "BREAD", 
    Description = "A loaf of fresh bread.", 
    Location = 1,           // Room number
    Action = "The bread tastes fresh and warm.", 
    ActionVerb = "EAT", 
    ActionResult = "HEALTH", 
    ActionValue = "50",     // Health restored
    ActionPoints = 10       // Points awarded
}
```

### Weapon Item
```csharp
new Item { 
    Name = "SWORD", 
    Description = "A sharp steel sword.", 
    Location = 2, 
    Action = "You swing the sword skillfully.", 
    ActionVerb = "ATTACK", 
    ActionResult = "WEAPON", 
    ActionValue = "DRAGON",  // Monster key this attacks
    ActionPoints = 25 
}
```

### Key/Unlock Item
```csharp
new Item { 
    Name = "KEY", 
    Description = "A brass key.", 
    Location = 3, 
    Action = "The key turns with a satisfying click.", 
    ActionVerb = "USE", 
    ActionResult = "UNLOCK", 
    // Format: "roomNum|direction|newRoom|newDesc1|newDesc2"
    ActionValue = "1|N|2|The door is now open.|The door unlocks!", 
    ActionPoints = 100 
}
```

### Teleport Item
```csharp
new Item { 
    Name = "WAND", 
    Description = "A magical wand.", 
    Location = 4, 
    Action = "The wand glows and reality shifts.", 
    ActionVerb = "WAVE", 
    ActionResult = "TELEPORT", 
    ActionValue = "10",      // Room number to teleport to
    ActionPoints = 15 
}
```

### Pet Item
```csharp
new Item { 
    Name = "KITTEN", 
    Description = "A cute fuzzy kitten.", 
    Location = 5, 
    Action = "The kitten purrs and follows you.", 
    ActionVerb = "PET", 
    ActionResult = "FOLLOW", 
    ActionValue = "20",      // Follow behavior value
    ActionPoints = 50 
}
```

### Fortune/Random Item
```csharp
new Item { 
    Name = "CRYSTAL", 
    Description = "A mystical crystal ball.", 
    Location = 6, 
    Action = "The crystal shows visions of the future.", 
    ActionVerb = "READ", 
    ActionResult = "FORTUNE", 
    ActionValue = "1",       // Fortune type
    ActionPoints = 25 
}
```

## Essential Message Tags

### Movement Blocking Messages
```csharp
// Direction messages (provide variety)
new Message { MessageTag = "North", Messsage = "You can't go north." },
new Message { MessageTag = "North", Messsage = "There's a wall to the north." },
new Message { MessageTag = "South", Messsage = "No path leads south." },
new Message { MessageTag = "East", Messsage = "The way east is blocked." },
new Message { MessageTag = "West", Messsage = "You cannot go west." },
new Message { MessageTag = "Up", Messsage = "There's nothing above you." },
new Message { MessageTag = "Down", Messsage = "You can't go down here." },
```

### Action Result Messages
```csharp
// Success messages
new Message { MessageTag = "GetSuccess", Messsage = "You pick up the @." },
new Message { MessageTag = "GetSuccess", Messsage = "The @ fits in your pack." },
new Message { MessageTag = "DropSuccess", Messsage = "You place the @ down." },
new Message { MessageTag = "EatSuccessBig", Messsage = "The @ is very satisfying!" },
new Message { MessageTag = "EatSuccessMedium", Messsage = "The @ tastes good." },
new Message { MessageTag = "EatSuccessSmall", Messsage = "The @ helps a little." },

// Failure messages
new Message { MessageTag = "GetFailed", Messsage = "You can't find the @." },
new Message { MessageTag = "DropFailed", Messsage = "You don't have the @." },
new Message { MessageTag = "EatFailed", Messsage = "You can't eat the @." },
new Message { MessageTag = "UseFailed", Messsage = "The @ doesn't work here." },
```

### Critical Status Messages
```csharp
// Required for game engine
new Message { MessageTag = "Dead", Messsage = "You have died! Game over." },
new Message { MessageTag = "Bad", Messsage = "You feel weak and need food." },
new Message { MessageTag = "PetFollow", Messsage = "Your pet companion stays close." },
```

### Combat Messages
```csharp
new Message { MessageTag = "MonsterAppear", Messsage = "A @ appears!" },
new Message { MessageTag = "MonsterAttack", Messsage = "The @ attacks you!" },
new Message { MessageTag = "MonsterHit", Messsage = "The @ hits you! Ouch!" },
new Message { MessageTag = "MonsterMiss", Messsage = "The @ misses you." },
new Message { MessageTag = "MonsterDefeated", Messsage = "You defeat the @!" },
new Message { MessageTag = "AttackSuccess", Messsage = "You hit the @ with your @!" },
new Message { MessageTag = "AttackFailed", Messsage = "You need a weapon to fight the @." },
new Message { MessageTag = "NoMonster", Messsage = "There's nothing to attack here." },
```

## Monster Template
```csharp
new Monster 
{ 
    Key = "MONSTER_KEY",                    // Unique identifier
    Name = "Monster Name", 
    Description = "Description of the monster.",
    RoomNumber = 5,                         // Room where it appears
    ObjectNameThatCanAttackThem = "WEAPON", // Required weapon item name
    AttacksToKill = 2,                      // Hits needed to defeat
    CurrentHealth = 2,                      // Initialize to AttacksToKill
    CanHitPlayer = true,                    // Can it damage player?
    HitOdds = 30,                          // 0-100, chance to hit player
    HealthDamage = 10,                     // Damage dealt to player
    AppearanceChance = 70,                 // 0-100, chance to appear in room
    IsPresent = false,                     // Start as not present
    PetAttackChance = 25                   // 0-100, chance pet helps attack
}
```

## Common Action Result Values

| ActionResult | ActionValue Format | Example | Purpose |
|--------------|-------------------|---------|---------|
| HEALTH | Number | "50" or "-10" | Heal/damage player |
| TELEPORT | Room number | "5" | Move to room 5 |
| UNLOCK | "room\|dir\|newRoom\|desc1\|desc2" | "1\|N\|2\|Open\|Unlocked" | Open doors |
| WEAPON | Monster key | "DRAGON" | Attack monster |
| FOLLOW | Behavior value | "20" | Pet follows player |
| FORTUNE | Fortune type | "1" | Random message |

## Validation Tips

Use the debug command `validateadventure` to check:
- ? All room connections are valid
- ? All items are in valid locations  
- ? All monsters reference existing weapons
- ? Starting room exists
- ? Essential message tags present

## Room Connection Planning

Draw your adventure map first:
```
[Exit:0] ? [Room1:1] ? [Room2:2]
             ?           ?
          [Room3:3] ? [Room4:4]
```

Then implement connections:
- Room 1: W=0 (exit), E=2, S=3
- Room 2: W=1, S=4  
- Room 3: N=1, E=4
- Room 4: N=2, W=3

## Debug Items Template
```csharp
#if DEBUG
// Health restoration
new Item { Name = "DEBUGHEALTH", Location = 88, ActionVerb = "EAT", 
          ActionResult = "HEALTH", ActionValue = "1000", ActionPoints = 0 },

// Teleport to exit
new Item { Name = "DEBUGEXIT", Location = 88, ActionVerb = "WAVE", 
          ActionResult = "TELEPORT", ActionValue = "0", ActionPoints = 0 },

// Teleport to start  
new Item { Name = "DEBUGSTART", Location = 88, ActionVerb = "WAVE", 
          ActionResult = "TELEPORT", ActionValue = "1", ActionPoints = 0 },
#endif
```

Remember: Always test your adventure with the validation command and playtest from start to finish!