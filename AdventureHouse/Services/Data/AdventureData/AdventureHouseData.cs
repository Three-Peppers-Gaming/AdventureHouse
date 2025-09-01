using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.AdventureClient.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.Data.AdventureData
{
    public class AdventureHouseData
    {
        private readonly AdventureHouseConfiguration _config = new();

        public AdventureHouseConfiguration GetGameConfiguration() => _config;

        public PlayAdventureModel SetupAdventure(string instanceID)
        {
            var player = new Player
            {
                Name = "Gamer",
                Room = _config.StartingRoom, // Should be 20 (Outside)
                HealthCurrent = _config.MaxHealth,
                HealthMax = _config.MaxHealth,
                PlayerID = Guid.NewGuid().ToString(),
                Verbose = true,
                Points = _config.StartingPoints
            };

            var adventure = new PlayAdventureModel
            {
                GameID = 1,
                GameName = _config.GameName,
                GameHelp = _config.GetAdventureHelpText(),
                GameThanks = _config.GetAdventureThankYouText(),
                InstanceID = instanceID,
                WelcomeMessage = _config.GetWelcomeMessage("Gamer"),
                StartRoom = _config.StartingRoom, // Should be 20 (Outside)
                MaxHealth = _config.MaxHealth,
                HealthStep = _config.HealthStep,
                Player = player,
                Items = Items(),
                Rooms = Rooms(),
                Messages = Messages(),
                Monsters = Monsters(),
                GameActive = true,
                PointsCheckList = new List<string>(_config.InitialPointsCheckList)
            };

            return adventure;
        }

        private static List<Item> Items()
        {
            return new List<Item>
            {
                new() { Name = "BREAD", Description = "A small loaf of bread. Not quite a lunch, too big for an afternoon snack.", Location = 6, Action = "The bread was fresh and warm.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 5 },
                new() { Name = "BUGLE", Description = "A bugle. You were never very good with instruments.", Location = 20, Action = "You try to no avail to produce something that could constitute music.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "-1", ActionPoints = 1 },
                new() { Name = "APPLE", Description = "A nice, red apple that looks fresh, shiny and very appetizing.", Location = 7, Action = "Tastes just as good as it looked.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 10 },
                new() { Name = "KEY", Description = "A shiny brushed metal aesthetically pleasing key that has a good weight and feel. This must open something.", Location = 24, Action = "The key fits perfectly and the door unlocked with some effort.", ActionVerb = "USE", ActionResult = "UNLOCK", ActionValue = "1|E|0|This is the entrance. The door is unlocked.|This is the entrance. Door is now unlocked", ActionPoints = 100 },
                new() { Name = "WAND", Description = "A small wooden wand with G. Stilton scratched into the base.", Location = 17, Action = "You wave the wand and the room fades for a second.", ActionVerb = "WAVE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints = 10 },
                new() { Name = "PIE", Description = "A small slice of apple pie. It looks mouthwatering and fresh.", Location = 10, Action = "A little cold, but there is never really a good reason to turn down pie.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 10 },
                new() { Name = "KITTEN", Description = "A delightful fuzzy kitten", Location = 20, Action = "The little fuzzball, a black and white kitten just looks so adorable!", ActionVerb = "PET", ActionResult = "FOLLOW", ActionValue = "20", ActionPoints = 50 },
                new() { Name = "SLIP", Description = "A small slip of paper that looks like The fortune cookie slip from yesterdays DEVELOPER lunch. The words seem to fade in and out over and over.", Location = 18, Action = "the fortune cookie slip from yesterdays DEVELOPER lunch", ActionVerb = "READ", ActionResult = "FORTUNE", ActionValue = "1", ActionPoints = 33 },
                new() { Name = "ROCK", Description = "A magic rock", Location = 6, Action = "This looks more like a coprolite fossel than a rock. Might want to get rid of this thing soon.", ActionVerb = "THROW", ActionResult = "TELEPORT", ActionValue = "95", ActionPoints = 10 },
                new() { Name = "FLYSWATTER", Description = "A sturdy plastic fly swatter with a bright yellow handle. Perfect for dealing with pesky insects.", Location = 18, Action = "You swing the fly swatter through the air with a satisfying whoosh sound.", ActionVerb = "ATTACK", ActionResult = "WEAPON", ActionValue = "MOSQUITO", ActionPoints = 25 },
#if DEBUG
                new() { Name = "STICK", Description = "This is the developers helpful and magic stick.", Location = 0, Action = "This looks a lot a debugging tool that a developer would create to make his life easy.", ActionVerb = "WAVE", ActionResult = "TELEPORT", ActionValue = "88", ActionPoints = 0 },
                new() { Name = "AWAND", Description = "A small wooden wand.", Location = 99, Action = "You wave the wand and the room fades for a second.", ActionVerb = "WAVE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints = 1 },
                new() { Name = "ADONUT", Description = "A small glazed donut. Not quite a lunch, too big for a snack.", Location = 88, Action = "The bread was fresh and warm.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 5 },
                new() { Name = "ABUGLE", Description = "A You were never very good with instruments.", Location = 88, Action = "You try to no avail to produce something that could constitute music.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "-1", ActionPoints = 1 },
                new() { Name = "AAPPLE", Description = "A nice, red fruit that looks rather appetizing.", Location = 88, Action = "Tastes just as good as it looked.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 25 },
                new() { Name = "AKEY", Description = "A shiny, aesthetically pleasing key. Must open something.", Location = 88, Action = "The key fits perfectly and the door unlocked with some effort.", ActionVerb = "USE", ActionResult = "UNLOCK", ActionValue = "1|E|0|This is the entrance. The door is unlocked.|This is the entrance. Door is now unlocked", ActionPoints = 0 },
                new() { Name = "APIE", Description = "A small slice of apple pie. Mouthwatering.", Location = 88, Action = "A little cold, but there never really a good reason to turn down pie.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 10 }
#endif
            };
        }

        private static List<Room> Rooms()
        {
            return new List<Room>
            {
                new() { Number = 0, Name = "Exit!", Desc = "Exit! - You have found freedom!", N = 99, S = 99, E = 99, W = 1, U = 99, D = 99, RoomPoints = 100 },
                new() { Number = 1, Name = "Main Entrance", Desc = "This is the house entrance. The door is securely locked.", N = 10, S = 2, E = 99, W = 99, U = 99, D = 99, RoomPoints = 25 },
                new() { Number = 2, Name = "Downstairs Hallway", Desc = "This hall is at the bottom of the stairs.", N = 1, S = 4, E = 3, W = 99, U = 11, D = 99, RoomPoints = 50 },
                new() { Number = 3, Name = "Guest Bathroom", Desc = "Small half bathroom downstairs. It is too small and ensures guests leave sooner than later.", N = 99, S = 99, E = 5, W = 2, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 4, Name = "Living Room", Desc = "This is a simple living room on the southeast side of the house. The carpet is worn like lots of people have visited this room.", N = 2, S = 99, E = 99, W = 5, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 5, Name = "Family Room", Desc = "An inviting game room with a large TV and freshly broken Atari 2600.", N = 6, S = 99, E = 2, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 6, Name = "Nook", Desc = "This is a nook with a small dining table that has \"W. Robinett rulez\" scratched in top.", N = 7, S = 5, E = 99, W = 24, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 7, Name = "Kitchen", Desc = "There is a clean floor and a large granite counter.", N = 8, S = 6, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 8, Name = "Utility Hall", Desc = "This is a small hall with two large trash cans that are empty.", N = 99, S = 7, E = 10, W = 9, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 9, Name = "Garage", Desc = "The the big garage door is closed and bolted shut. The garage is empty and clean.", N = 99, S = 99, E = 8, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 10, Name = "Main Dining Room", Desc = "The dining room is on the northeast side of the house.", N = 99, S = 1, E = 99, W = 8, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 11, Name = "Upstairs Hallway", Desc = "This long hall is at the top of the stairs to the first floor.", N = 99, S = 12, E = 16, W = 13, U = 99, D = 2, RoomPoints = 5 },
                new() { Number = 12, Name = "Upstairs East Hallway", Desc = "Hall with two tables and computers. The Apple 2 and TRS-80 computers are broken.", N = 11, S = 15, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 13, Name = "Upstairs North Hallway", Desc = "The part of the hall has a large closet.", N = 18, S = 14, E = 11, W = 17, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 14, Name = "Upstairs West Hallway", Desc = "The long hallway has with a small closet. That door is nailed shut with an out of order sign on the door.", N = 13, S = 23, E = 99, W = 22, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 15, Name = "Spare Room", Desc = "The bedroom has a queen size bed and several KISS posters on the wall. It looks like it was a fun party room. The bed is covered in roses that look a bit old. The pedals like they were place there months ago.", N = 12, S = 99, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 16, Name = "Utility Room", Desc = "This is a laundry room with a washer and dryer. The dryer looks fine but the washer looks rusty and needs to be replaced.", N = 99, S = 99, E = 99, W = 11, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 17, Name = "Upstairs Bath", Desc = "The main bathroom with a large bathtub that is full of bubble bath. The water looks dirty and it stinks.", N = 99, S = 99, E = 13, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 18, Name = "Master Bedroom", Desc = "The master bedroom with an inviting king size bed. The room is messy and it seems like they had a party here.", N = 21, S = 13, E = 19, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 19, Name = "Master Bedroom Closet", Desc = "This is a long and narrow walk-in closet. A good place to put stairs to an attic.", N = 99, S = 99, E = 99, W = 18, U = 20, D = 99, RoomPoints = 5 },
                new() { Number = 20, Name = "Attic", Desc = "You are in the house attic. This room is musty and dark.", N = 99, S = 99, E = 99, W = 99, U = 99, D = 19, RoomPoints = 0 },
                new() { Number = 21, Name = "Master Bedroom Bath", Desc = "Warm master bedroom with a shower and tub.", N = 99, S = 18, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 22, Name = "Children's Room", Desc = "Clean children's room with twin beds.", N = 99, S = 99, E = 14, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 23, Name = "Entertainment Room", Desc = "This is a very inviting play room with games and toys.", N = 14, S = 99, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 24, Name = "Deck", Desc = "You are standing on a finely crafted wooden covered deck.", N = 99, S = 99, E = 6, W = 99, U = 99, D = 99, RoomPoints = 50 },
#if DEBUG
                new() { Number = 88, Name = "Debug Room", Desc = "The Magic Debug Room Up - Leads to the Door, Down leads to the Attic", N = 99, S = 99, E = 99, W = 99, U = 1, D = 20, RoomPoints = 50 },
#endif
                new() { Number = 93, Name = "Psychedelic Ladder", Desc = "You are on a what seems like and endless glowing ladder. You see magic spiraling vortex.", N = 99, S = 99, E = 19, W = 19, U = 95, D = 94, RoomPoints = 50 },
                new() { Number = 94, Name = "Memory Ladder", Desc = "You climbed on to the ladder and your memory of how to get back fades. You are on a what seems like and endless magic ladder.", N = 99, S = 99, E = 99, W = 99, U = 93, D = 95, RoomPoints = 50 },
                new() { Number = 95, Name = "Magic Mushroom", Desc = "A magic room. The walls sparkle and shine. This room seems like a very happy place. You see 4 doors and ladders leading up and down", N = 20, S = 20, E = 20, W = 20, U = 94, D = 93, RoomPoints = 50 }
            };
        }

        private static List<Message> Messages()
        {
            return new List<Message>
            {
                new() { MessageTag = "any", Messsage = "You can't do that here." },
                new() { MessageTag = "getsuccess", Messsage = "You pick up the @." },
                new() { MessageTag = "getfailed", Messsage = "You can't get that." },
                new() { MessageTag = "dropsuccess", Messsage = "You drop the @." },
                new() { MessageTag = "dropfailed", Messsage = "You don't have that." },
                new() { MessageTag = "petsuccess", Messsage = "The @ starts following you!" },
                new() { MessageTag = "petfailed", Messsage = "You can't pet that." },
                new() { MessageTag = "petfollow", Messsage = "Your faithful @ follows you." },
                new() { MessageTag = "shoosuccess", Messsage = "The @ runs away." },
                new() { MessageTag = "UseFailed", Messsage = "You can't use that here." },
                new() { MessageTag = "LookEmpty", Messsage = "Look at what?" },
                new() { MessageTag = "LookFailed", Messsage = "You don't see that here." },
                new() { MessageTag = "dead", Messsage = "You have died! Your adventure is over." },
                new() { MessageTag = "bad", Messsage = "You feel weak and dizzy." },
                new() { MessageTag = "DeadMove", Messsage = "Dead people don't move." },
                new() { MessageTag = "north", Messsage = "You can't go that way." },
                new() { MessageTag = "south", Messsage = "You can't go that way." },
                new() { MessageTag = "east", Messsage = "You can't go that way." },
                new() { MessageTag = "west", Messsage = "You can't go that way." },
                new() { MessageTag = "up", Messsage = "You can't go that way." },
                new() { MessageTag = "down", Messsage = "You can't go that way." },
                new() { MessageTag = "MonsterAppear", Messsage = "A wild @ suddenly appears!" },
                new() { MessageTag = "MonsterAppear", Messsage = "You hear a buzzing sound as a @ enters the room." },
                new() { MessageTag = "MonsterAppear", Messsage = "Suddenly, a @ flies into view!" },
                new() { MessageTag = "MonsterAttack", Messsage = "The @ attacks you!" },
                new() { MessageTag = "MonsterAttack", Messsage = "The @ swoops down and strikes!" },
                new() { MessageTag = "MonsterHit", Messsage = "The @ bites you! Ouch!" },
                new() { MessageTag = "MonsterHit", Messsage = "The @ stings you painfully!" },
                new() { MessageTag = "MonsterMiss", Messsage = "The @ misses you completely." },
                new() { MessageTag = "MonsterMiss", Messsage = "You dodge the @'s attack." },
                new() { MessageTag = "MonsterDefeated", Messsage = "You defeat the @! It flies away." },
                new() { MessageTag = "MonsterDefeated", Messsage = "The @ is vanquished!" },
                new() { MessageTag = "AttackSuccess", Messsage = "You successfully attack the @ with your @!" },
                new() { MessageTag = "AttackFailed", Messsage = "You need the right weapon to attack the @." },
                new() { MessageTag = "NoMonster", Messsage = "There's nothing here to attack." },
                new() { MessageTag = "PetAttackSuccess", Messsage = "Your brave @ leaps to your defense!" },
                new() { MessageTag = "PetAttackSuccess", Messsage = "The loyal @ springs into action!" },
                new() { MessageTag = "PetAttackSuccess", Messsage = "Your @ courageously attacks the enemy!" },
                new() { MessageTag = "PetAttackFailed", Messsage = "Your @ wants to help but looks uncertain." },
                new() { MessageTag = "PetAttackFailed", Messsage = "The @ hesitates, not sure how to help." },
                new() { MessageTag = "PetDefeated", Messsage = "Your heroic @ saves the day!" },
                new() { MessageTag = "PetDefeated", Messsage = "The brave @ emerges victorious!" }
            };
        }

        private static List<Monster> Monsters()
        {
            return new List<Monster>
            {
                new() 
                { 
                    Key = "spider", 
                    Name = "Giant Spider", 
                    Description = "A large, menacing spider blocks your path.", 
                    RoomNumber = 3, 
                    ObjectNameThatCanAttackThem = "Sword", 
                    AttacksToKill = 2, 
                    CanHitPlayer = true, 
                    HitOdds = 30, 
                    HealthDamage = 15, 
                    AppearanceChance = 40, 
                    IsPresent = false, 
                    CurrentHealth = 2, 
                    PetAttackChance = 25 
                },
                new() 
                { 
                    Key = "bat", 
                    Name = "Vampire Bat", 
                    Description = "A blood-thirsty bat swoops around the room.", 
                    RoomNumber = 7, 
                    ObjectNameThatCanAttackThem = "Hammer", 
                    AttacksToKill = 1, 
                    CanHitPlayer = true, 
                    HitOdds = 25, 
                    HealthDamage = 10, 
                    AppearanceChance = 35, 
                    IsPresent = false, 
                    CurrentHealth = 1, 
                    PetAttackChance = 30 
                },
                new() 
                { 
                    Key = "MOSQUITO", 
                    Name = "Mosquito", 
                    Description = "A large, annoying mosquito buzzing around the room. It looks like it wants to make you its next meal.", 
                    RoomNumber = 23, 
                    ObjectNameThatCanAttackThem = "FLYSWATTER", 
                    AttacksToKill = 1, 
                    CanHitPlayer = true, 
                    HitOdds = 30, 
                    HealthDamage = 5, 
                    AppearanceChance = 60, 
                    IsPresent = false, 
                    CurrentHealth = 1, 
                    PetAttackChance = 30 
                }
            };
        }
    }
}
