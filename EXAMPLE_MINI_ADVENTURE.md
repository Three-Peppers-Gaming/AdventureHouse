# Example Mini-Adventure: "Escape the Tower"

This is a complete working example of a small adventure that demonstrates all the key concepts and patterns used in the Adventure House framework.

## Adventure Overview
- **Theme**: Escape from a wizard's tower
- **Rooms**: 6 rooms (plus exit and debug room)
- **Items**: 8 items including food, weapons, keys, and magic items
- **Monsters**: 1 dragon guarding the exit
- **Goal**: Find the magic key to unlock the exit and escape

## Complete Code Example

### TowerConfiguration.cs
```csharp
using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    public class TowerConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Escape the Tower";
        public string GameVersion => "1.0";
        public string GameDescription => "Escape from a mysterious wizard's tower before time runs out!";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomDisplayCharacters => new()
        {
            ["exit"] = 'X',
            ["entrance"] = 'E',
            ["laboratory"] = 'L',
            ["library"] = 'B',
            ["chamber"] = 'C',
            ["tower top"] = 'T',
            ["dungeon"] = 'D'
        };

        public Dictionary<string, char> RoomTypeCharacters => new()
        {
            ["room"] = 'R',
            ["hall"] = 'H',
            ["chamber"] = 'C',
            ["magical"] = '*',
#if DEBUG
            ["debug"] = '?'
#endif
        };

        public Dictionary<string, int> RoomNameToNumberMapping => new()
        {
            ["exit"] = 0,
            ["entrance hall"] = 1,
            ["spiral staircase"] = 2,
            ["wizard's laboratory"] = 3,
            ["ancient library"] = 4,
            ["magical chamber"] = 5,
            ["tower top"] = 6,
#if DEBUG
            ["debug room"] = 88
#endif
        };
        #endregion

        #region Game Settings
        public int StartingRoom => 1;
        public int MaxHealth => 150;
        public int HealthStep => 2;
        public int StartingPoints => 1;
        public string InitialPointsCheckList => "NewGame";
        
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';
        #endregion

        #region Map Configuration
        public Dictionary<int, MapLevel> RoomToLevelMapping => new()
        {
            [0] = MapLevel.Exit,
            [1] = MapLevel.GroundFloor,
            [2] = MapLevel.GroundFloor,
            [3] = MapLevel.UpperFloor,
            [4] = MapLevel.UpperFloor,
            [5] = MapLevel.UpperFloor,
            [6] = MapLevel.Attic,
#if DEBUG
            [88] = MapLevel.DebugLevel
#endif
        };

        public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
        {
            [MapLevel.GroundFloor] = (5, 3),
            [MapLevel.UpperFloor] = (5, 3),
            [MapLevel.Attic] = (3, 3),
#if DEBUG
            [MapLevel.DebugLevel] = (3, 3),
#endif
            [MapLevel.Exit] = (3, 3)
        };

        public Dictionary<int, (int X, int Y)> RoomPositions => new()
        {
            [0] = (2, 1),  // Exit
            [1] = (2, 1),  // Entrance Hall
            [2] = (2, 2),  // Spiral Staircase
            [3] = (1, 1),  // Laboratory
            [4] = (3, 1),  // Library
            [5] = (2, 0),  // Magical Chamber
            [6] = (1, 1),  // Tower Top
#if DEBUG
            [88] = (1, 1), // Debug Room
#endif
        };
        #endregion

        #region Helper Methods
        public char GetRoomDisplayChar(string roomName)
        {
            var cleanName = roomName?.ToLower() ?? string.Empty;
            
            if (RoomDisplayCharacters.TryGetValue(cleanName, out char exactChar))
                return exactChar;
            
            foreach (var (pattern, character) in RoomTypeCharacters)
            {
                if (cleanName.Contains(pattern))
                    return character;
            }
            
            return DefaultRoomCharacter;
        }

        public int GetRoomNumberFromName(string roomName)
        {
            var cleanName = roomName?.ToLower().Trim() ?? string.Empty;
            return RoomNameToNumberMapping.GetValueOrDefault(cleanName, -1);
        }

        public string GetLevelDisplayName(MapLevel level)
        {
            return level switch
            {
                MapLevel.GroundFloor => "Ground Floor",
                MapLevel.UpperFloor => "Upper Floor",
                MapLevel.Attic => "Tower Top",
                MapLevel.Exit => "Freedom!",
#if DEBUG
                MapLevel.DebugLevel => "Debug Level",
#endif
                _ => "Unknown Level"
            };
        }

        public MapLevel GetLevelForRoom(int roomNumber)
        {
            return RoomToLevelMapping.GetValueOrDefault(roomNumber, MapLevel.GroundFloor);
        }

        public (int X, int Y) GetRoomPosition(int roomNumber)
        {
            return RoomPositions.GetValueOrDefault(roomNumber, (0, 0));
        }
        #endregion

        #region Story Text
        public string GetAdventureHelpText()
        {
            return "You are trapped in a mysterious wizard's tower and must find a way to escape!\r\n\r\n" +
                   "COMMANDS: Use two-word commands like GO NORTH, GET APPLE, USE KEY, EAT BREAD, etc.\r\n" +
                   "MOVEMENT: GO followed by NORTH, SOUTH, EAST, WEST, UP, or DOWN\r\n" +
                   "ITEMS: GET to pick up items, DROP to put them down, INV to see inventory\r\n" +
                   "ACTIONS: EAT food to restore health, USE items to solve puzzles\r\n" +
                   "COMBAT: Some creatures guard the tower - find the right weapon to defeat them!\r\n" +
                   "GOAL: Find the magic key and escape through the exit before your health runs out.\r\n\r\n" +
                   "The tower is full of magical items and dangerous creatures. Explore carefully,\r\n" +
                   "collect useful items, and solve the puzzles to find your way to freedom!";
        }

        public string GetAdventureThankYouText()
        {
            return "?? CONGRATULATIONS! ??\r\n\r\n" +
                   "You have successfully escaped from the wizard's tower!\r\n" +
                   "Your courage and wit have served you well on this magical adventure.\r\n\r\n" +
                   "The fresh air outside tastes sweet after your time in the mysterious tower.\r\n" +
                   "You can now return to civilization with tales of your daring escape!\r\n\r\n" +
                   "Thank you for playing 'Escape the Tower'!\r\n" +
                   "Try exploring different paths to discover all the tower's secrets.";
        }

        public string GetWelcomeMessage(string gamerTag)
        {
            return $"Greetings, brave {gamerTag}!\r\n\r\n" +
                   "You awaken to find yourself trapped in a tall, mysterious tower.\r\n" +
                   "The walls are covered in strange symbols, and magical energy crackles in the air.\r\n" +
                   "You must find a way to escape before the tower's magic drains your life force!\r\n\r\n" +
                   "Use simple commands to explore, collect items, and solve puzzles.\r\n" +
                   "Type HELP for guidance on your quest for freedom.\r\n\r\n" +
                   "Good luck, and may your wits serve you well!\r\n\r\n";
        }

        public string MapLegendContent => "Tower Map Legend - ASCII representation of rooms and connections";
        public string MapLegendFooter => "Note: Only visited areas are shown on your map.";
        public string GetCompleteMapLegend() => MapLegendContent + "\r\n" + MapLegendFooter;
        
        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] => "Ground Floor",
            [MapLevel.UpperFloor] => "Upper Floor",
            [MapLevel.Attic] => "Tower Top",
            [MapLevel.Exit] => "Freedom!",
#if DEBUG
            [MapLevel.DebugLevel] => "Debug Level"
#endif
        };
        #endregion
    }
}
```

### TowerData.cs
```csharp
using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    public class TowerData
    {
        private readonly TowerConfiguration _config = new();

        public string GetAdventureHelpText() => _config.GetAdventureHelpText();
        public string GetAdventureThankYouText() => _config.GetAdventureThankYouText();
        public TowerConfiguration GetGameConfiguration() => _config;

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
                Player = new Player
                {
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

        private static List<Room> Rooms()
        {
            var rooms = new List<Room>
            {
                // Exit room (goal)
                new Room 
                { 
                    Number = 0, 
                    RoomPoints = 200, 
                    Name = "Exit", 
                    Desc = "Freedom at last! You have escaped from the wizard's tower!", 
                    N = 99, S = 99, E = 99, W = 1, U = 99, D = 99 
                },

                // Starting room
                new Room 
                { 
                    Number = 1, 
                    RoomPoints = 25, 
                    Name = "Entrance Hall", 
                    Desc = "A grand entrance hall with a locked exit door to the west. Mysterious symbols glow on the walls. You can go north up a spiral staircase.", 
                    N = 2, S = 99, E = 99, W = 99, U = 99, D = 99 
                },

                // Central hub
                new Room 
                { 
                    Number = 2, 
                    RoomPoints = 15, 
                    Name = "Spiral Staircase", 
                    Desc = "A twisting spiral staircase in the center of the tower. You can see doors to the east and west, stairs up to the north, and the entrance hall to the south.", 
                    N = 5, S = 1, E = 4, W = 3, U = 99, D = 99 
                },

                // Laboratory (has healing potion)
                new Room 
                { 
                    Number = 3, 
                    RoomPoints = 30, 
                    Name = "Wizard's Laboratory", 
                    Desc = "A mysterious laboratory filled with bubbling cauldrons, strange apparatus, and magical ingredients. The air shimmers with magical energy.", 
                    N = 99, S = 99, E = 2, W = 99, U = 99, D = 99 
                },

                // Library (has spell scroll)
                new Room 
                { 
                    Number = 4, 
                    RoomPoints = 30, 
                    Name = "Ancient Library", 
                    Desc = "Tall shelves filled with ancient tomes and magical scrolls. Dust motes dance in beams of ethereal light filtering through stained glass windows.", 
                    N = 99, S = 99, E = 99, W = 2, U = 99, D = 99 
                },

                // Magical chamber (has sword and leads to tower top)
                new Room 
                { 
                    Number = 5, 
                    RoomPoints = 40, 
                    Name = "Magical Chamber", 
                    Desc = "A circular chamber crackling with magical energy. Arcane symbols pulse with power on the floor. A ladder leads up to the tower's peak.", 
                    N = 99, S = 2, E = 99, W = 99, U = 6, D = 99 
                },

                // Tower top (has dragon and magic key)
                new Room 
                { 
                    Number = 6, 
                    RoomPoints = 50, 
                    Name = "Tower Top", 
                    Desc = "The highest point of the tower, open to the sky. Ancient magical artifacts are scattered around a powerful summoning circle. A fierce wind whips through your hair.", 
                    N = 99, S = 99, E = 99, W = 99, U = 99, D = 5 
                },

#if DEBUG
                // Debug room
                new Room 
                { 
                    Number = 88, 
                    RoomPoints = 0, 
                    Name = "Debug Room", 
                    Desc = "Developer testing area with debug items. Up leads to entrance, down leads to exit.", 
                    N = 99, S = 99, E = 99, W = 99, U = 1, D = 0 
                },
#endif
            };

            return rooms;
        }

        private static List<Item> Items()
        {
            var items = new List<Item>
            {
                // Food items for health restoration
                new Item 
                { 
                    Name = "BREAD", 
                    Description = "A stale loaf of bread, but still edible.", 
                    Location = 1, 
                    Action = "The bread is tough but fills your stomach.", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "30", 
                    ActionPoints = 10 
                },

                new Item 
                { 
                    Name = "POTION", 
                    Description = "A glowing red healing potion that radiates warmth.", 
                    Location = 3, 
                    Action = "The potion tastes sweet and fills you with energy!", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "75", 
                    ActionPoints = 25 
                },

                // Weapon for fighting the dragon
                new Item 
                { 
                    Name = "SWORD", 
                    Description = "A gleaming enchanted sword that hums with magical power.", 
                    Location = 5, 
                    Action = "You swing the magical sword with confidence and skill!", 
                    ActionVerb = "ATTACK", 
                    ActionResult = "WEAPON", 
                    ActionValue = "DRAGON", 
                    ActionPoints = 50 
                },

                // Magic key to unlock the exit (obtained after defeating dragon)
                new Item 
                { 
                    Name = "MAGICKEY", 
                    Description = "An ornate golden key that glows with mystical energy.", 
                    Location = 6, 
                    Action = "The magic key fits perfectly! The exit door unlocks with a brilliant flash of light!", 
                    ActionVerb = "USE", 
                    ActionResult = "UNLOCK", 
                    ActionValue = "1|W|0|The exit door stands open, revealing freedom beyond!|The magic key has unlocked your escape!", 
                    ActionPoints = 150 
                },

                // Magical scroll for teleportation
                new Item 
                { 
                    Name = "SCROLL", 
                    Description = "An ancient scroll covered in glowing magical runes.", 
                    Location = 4, 
                    Action = "The scroll crackles with energy as reality bends around you!", 
                    ActionVerb = "READ", 
                    ActionResult = "TELEPORT", 
                    ActionValue = "2", 
                    ActionPoints = 20 
                },

                // Crystal orb for fortune telling
                new Item 
                { 
                    Name = "ORB", 
                    Description = "A mystical crystal orb that swirls with inner light.", 
                    Location = 6, 
                    Action = "Visions of possible futures dance within the crystal orb.", 
                    ActionVerb = "READ", 
                    ActionResult = "FORTUNE", 
                    ActionValue = "1", 
                    ActionPoints = 15 
                },

#if DEBUG
                // Debug items for testing
                new Item 
                { 
                    Name = "DEBUGHEALTH", 
                    Description = "Developer's instant health restoration.", 
                    Location = 88, 
                    Action = "You feel completely restored!", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "1000", 
                    ActionPoints = 0 
                },

                new Item 
                { 
                    Name = "DEBUGKEY", 
                    Description = "Developer's master key.", 
                    Location = 88, 
                    Action = "The debug key opens all doors!", 
                    ActionVerb = "USE", 
                    ActionResult = "UNLOCK", 
                    ActionValue = "1|W|0|Debug exit unlocked!|All doors opened!", 
                    ActionPoints = 0 
                },
#endif
            };

            return items;
        }

        private static List<Message> Messages()
        {
            var messages = new List<Message>
            {
                // Movement blocking messages
                new Message { MessageTag = "North", Messsage = "You cannot go north from here." },
                new Message { MessageTag = "North", Messsage = "A magical barrier blocks your path north." },
                new Message { MessageTag = "South", Messsage = "The way south is blocked." },
                new Message { MessageTag = "South", Messsage = "You cannot proceed south." },
                new Message { MessageTag = "East", Messsage = "There is no passage to the east." },
                new Message { MessageTag = "East", Messsage = "A solid wall prevents you from going east." },
                new Message { MessageTag = "West", Messsage = "The way west is impassable." },
                new Message { MessageTag = "West", Messsage = "You cannot go west from this location." },
                new Message { MessageTag = "Up", Messsage = "There are no stairs leading up." },
                new Message { MessageTag = "Up", Messsage = "The ceiling blocks your way up." },
                new Message { MessageTag = "Down", Messsage = "You cannot go down from here." },
                new Message { MessageTag = "Down", Messsage = "The floor is solid - no way down." },

                // Action success messages
                new Message { MessageTag = "GetSuccess", Messsage = "You carefully pick up the @." },
                new Message { MessageTag = "GetSuccess", Messsage = "The @ fits nicely in your pack." },
                new Message { MessageTag = "DropSuccess", Messsage = "You gently place the @ on the ground." },
                new Message { MessageTag = "EatSuccessBig", Messsage = "The @ is incredibly satisfying and energizing!" },
                new Message { MessageTag = "EatSuccessMedium", Messsage = "The @ tastes good and helps restore your strength." },
                new Message { MessageTag = "EatSuccessSmall", Messsage = "The @ provides a small amount of nourishment." },

                // Action failure messages
                new Message { MessageTag = "GetFailed", Messsage = "You don't see a @ here to pick up." },
                new Message { MessageTag = "GetFailed", Messsage = "The @ seems to be missing from this area." },
                new Message { MessageTag = "DropFailed", Messsage = "You don't have a @ in your inventory." },
                new Message { MessageTag = "EatFailed", Messsage = "You cannot eat the @." },
                new Message { MessageTag = "EatFailed", Messsage = "The @ is not edible." },
                new Message { MessageTag = "UseFailed", Messsage = "The @ doesn't seem to work here." },
                new Message { MessageTag = "UseFailed", Messsage = "You can't figure out how to use the @ in this situation." },

                // Critical status messages
                new Message { MessageTag = "Dead", Messsage = "The tower's dark magic has drained your life force. You have perished!" },
                new Message { MessageTag = "Dead", Messsage = "Your strength fails you in this cursed place. Game over!" },
                new Message { MessageTag = "Bad", Messsage = "You feel weak and drained. The tower's magic is sapping your energy!" },
                new Message { MessageTag = "Bad", Messsage = "Your health is failing. You need food or healing soon!" },

                // Combat messages
                new Message { MessageTag = "MonsterAppear", Messsage = "A fearsome @ suddenly appears before you!" },
                new Message { MessageTag = "MonsterAppear", Messsage = "You encounter a dangerous @!" },
                new Message { MessageTag = "MonsterAttack", Messsage = "The @ attacks you with fierce aggression!" },
                new Message { MessageTag = "MonsterAttack", Messsage = "The @ lunges at you menacingly!" },
                new Message { MessageTag = "MonsterHit", Messsage = "The @ strikes you! You feel searing pain!" },
                new Message { MessageTag = "MonsterHit", Messsage = "The @'s attack connects! You are wounded!" },
                new Message { MessageTag = "MonsterMiss", Messsage = "The @ attacks but misses you completely!" },
                new Message { MessageTag = "MonsterMiss", Messsage = "You dodge the @'s attack skillfully!" },
                new Message { MessageTag = "MonsterDefeated", Messsage = "You have defeated the @! It vanishes in a puff of smoke!" },
                new Message { MessageTag = "MonsterDefeated", Messsage = "The @ is vanquished! Victory is yours!" },
                new Message { MessageTag = "AttackSuccess", Messsage = "You strike the @ with your @! A powerful blow!" },
                new Message { MessageTag = "AttackFailed", Messsage = "You need a proper weapon to defeat the @!" },
                new Message { MessageTag = "NoMonster", Messsage = "There is nothing here to attack." },

                // Miscellaneous messages
                new Message { MessageTag = "Any", Messsage = "That action doesn't seem to work here." },
                new Message { MessageTag = "Any", Messsage = "The tower's magic interferes with your action." },
                new Message { MessageTag = "DeadMove", Messsage = "You cannot move while dead. Try starting a new game." },
            };

            return messages;
        }

        private static List<Monster> Monsters()
        {
            var monsters = new List<Monster>
            {
                new Monster 
                { 
                    Key = "DRAGON", 
                    Name = "Ancient Dragon", 
                    Description = "A massive ancient dragon with scales that shimmer like starlight. Its eyes burn with eternal flame, and it guards the tower's greatest treasure - the magic key to freedom.",
                    RoomNumber = 6, // Tower Top
                    ObjectNameThatCanAttackThem = "SWORD", // Must have enchanted sword
                    AttacksToKill = 3, // Takes 3 hits to defeat
                    CurrentHealth = 3, // Initialize to AttacksToKill
                    CanHitPlayer = true,
                    HitOdds = 40, // 40% chance to hit player each turn
                    HealthDamage = 25, // Deals significant damage
                    AppearanceChance = 100, // Always appears when entering room
                    IsPresent = false, // Start as not present
                    PetAttackChance = 0 // No pets in this adventure
                }
            };

            return monsters;
        }
    }
}
```

## Adventure Flow

1. **Start**: Player begins in Entrance Hall (Room 1)
2. **Explore**: Collect bread for health, explore laboratory and library
3. **Gather Items**: Find healing potion, magical scroll, and enchanted sword
4. **Prepare**: Use items to restore health and prepare for final battle
5. **Combat**: Face the dragon at Tower Top (Room 6) with the enchanted sword
6. **Victory**: Defeat dragon to access the magic key
7. **Escape**: Use magic key to unlock exit door and escape to freedom

## Key Learning Points

### Room Design
- Each room has a clear purpose and atmosphere
- Connections create logical navigation flow
- Room points reward exploration

### Item Progression
- Basic food (bread) ? better healing (potion)
- Utility items (scroll for teleport, orb for fortune)
- Required progression item (sword) ? final key (magic key)

### Combat System
- Dragon requires specific weapon (sword)
- Multiple hits needed (3 attacks to kill)
- Significant challenge but fair with preparation

### Debug Support
- Debug room accessible from entrance and exit
- Debug items for testing health and unlocking
- Easy testing of all game mechanics

### Validation Features
- All room connections are valid (using 99 for blocked directions)
- Items placed in logical locations
- Monster references existing weapon
- Essential message tags included
- Clear win condition with magic key unlock

This example demonstrates all the core patterns and can serve as a template for creating larger, more complex adventures!