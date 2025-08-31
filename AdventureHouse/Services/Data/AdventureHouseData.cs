using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureHouse.Services.Models;


namespace AdventureHouse.Services.Data.AdventureData
{
    public class AdventureHouseData
    {
        private readonly AdventureHouseConfiguration _config = new();

        public string GetAdventureHelpText()
        {
            return _config.GetAdventureHelpText();
        }

        public string GetAdventureThankYouText()
        {
            return _config.GetAdventureThankYouText();
        }

        public AdventureHouseConfiguration GetGameConfiguration()
        {
            return _config;
        }


        public PlayAdventure SetupAdventure(string gameid)
            { 

            var _gamerTag = new GamerTags().RandomTag();

            var _play = new PlayAdventure
            {
                GameID = 1,
                GameName = _config.GameName,
                GameHelp = _config.GetAdventureHelpText(),
                GameThanks= _config.GetAdventureThankYouText(),
                InstanceID = gameid,
                StartRoom = _config.StartingRoom,
                WelcomeMessage = _config.GetWelcomeMessage(_gamerTag),
                MaxHealth = _config.MaxHealth,
                HealthStep = _config.HealthStep,
                Items = Items(),
                Messages = Messages(),
                Rooms = Rooms(),
                Monsters = Monsters(),
                Player = new Player { Name = _gamerTag, PlayerID = gameid, HealthMax = _config.MaxHealth, HealthCurrent = _config.MaxHealth, Room = _config.StartingRoom, Steps = 2, Verbose = true, Points = _config.StartingPoints, PlayerDead = false },
                GameActive = true,
                PointsCheckList = new List<string> { _config.InitialPointsCheckList }
            };

                return _play;
            }

            private static List<Item> Items()
            {

                var _items = new List<Item>
            {
                new Item { Name="BREAD",  Description="A small loaf of bread. Not quite a lunch, too big for an afternoon snack.",Location=06, Action="The bread was fresh and warm.", ActionVerb = "EAT", ActionResult="HEALTH", ActionValue = "100", ActionPoints = 5},
                new Item { Name="BUGLE", Description="A bugle. You were never very good with instruments.",Location= 20, Action="You try to no avail to produce something that could constitute music.", ActionVerb ="USE", ActionResult = "HEALTH", ActionValue = "-1" , ActionPoints=1 },
                new Item { Name="APPLE",Description= "A nice, red apple that looks fresh, shiny and very appetizing.", Location = 07, Action="Tastes just as good as it looked.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 10 },
                new Item { Name="KEY",Description= "A shiny brushed metal aesthetically pleasing key that has a good weight and feel. This must open something.", Location = 24, Action= "The key fits perfectly and the door unlocked with some effort.", ActionVerb="USE", ActionResult = "UNLOCK", ActionValue = "1|E|0|This is the entrance. The door is unlocked.|This is the entrance. Door is now unlocked", ActionPoints=100},
                new Item { Name="WAND", Description= "A small wooden wand with G. Stilton scratched into the base.",Location = 17, Action="You wave the wand and the room fades for a second.", ActionVerb="WAVE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints=10 },
                new Item { Name="PIE", Description= "A small slice of apple pie. It looks mouthwatering and fresh.",Location = 10, Action= "A little cold, but there is never really a good reason to turn down pie.", ActionVerb="EAT", ActionResult="HEALTH", ActionValue ="100", ActionPoints = 10 },
                new Item { Name="BREAD",  Description="A small loaf of bread. Not quite a lunch, too big for a snack.",Location=99, Action="It's too big for a snack. Maybe later, for lunch.", ActionVerb = "EAT", ActionResult="HEALTH", ActionValue = "100", ActionPoints= 10 },
                new Item { Name="KITTEN", Description= "A delightful fuzzy kitten",Location = 20, Action= "The little fuzzball, a black and white kitten just looks so adorable!", ActionVerb="PET", ActionResult="FOLLOW", ActionValue ="20", ActionPoints=50 },
                new Item { Name="SLIP", Description= "A small slip of paper that looks like The fortune cookie slip from yesterdays DEVELOPER lunch. The words seem to fade in and out over and over.",Location = 18, Action= "the fortune cookie slip from yesterdays DEVELOPER lunch", ActionVerb="READ", ActionResult="FORTUNE", ActionValue ="1", ActionPoints=33 },
                new Item { Name="FLYSWATTER", Description= "A sturdy plastic fly swatter with a bright yellow handle. Perfect for dealing with pesky insects.",Location = 18, Action="You swing the fly swatter through the air with a satisfying whoosh sound.", ActionVerb="ATTACK", ActionResult="WEAPON", ActionValue ="MOSQUITO", ActionPoints=25 },
#if (DEBUG)                  
                    new Item { Name="STICK", Description= "This is the developers helpful and magic stick.",Location = 00, Action= "This looks a lot a debugging tool that a developer would create to make his life easy.", ActionVerb="WAVE", ActionResult="TELEPORT", ActionValue ="88", ActionPoints=0},
                    new Item { Name="AWAND", Description= "A small wooden wand.",Location = 99, Action="You wave the wand and the room fades for a second.", ActionVerb="WAVE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints=1},
                    new Item { Name="ADONUT",  Description="A small glazed donut. Not quite a lunch, too big for a snack.",Location=88, Action="The bread was fresh and warm.", ActionVerb = "EAT", ActionResult="HEALTH", ActionValue = "100", ActionPoints = 5},
                    new Item { Name="ABUGLE", Description="A You were never very good with instruments.",Location= 88, Action="You try to no avail to produce something that could constitute music.", ActionVerb ="USE", ActionResult = "HEALTH", ActionValue = "-1", ActionPoints =1 },
                    new Item { Name="AAPPLE",Description= "A nice, red fruit that looks rather appetizing.", Location = 88, Action="Tastes just as good as it looked.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "25", ActionPoints=25 },
                    new Item { Name="AKEY",Description= "A shiny, aesthetically pleasing key. Must open something.", Location = 88, Action= "The key fits perfectly and the door unlocked with some effort.", ActionVerb="USE", ActionResult = "UNLOCK", ActionValue = "1|E|0|This is the entrance. The door is unlocked.|This is the entrance. Door is now unlocked" },
                    new Item { Name="APIE", Description= "A small slice of apple pie. Mouthwatering.",Location = 88, Action= "A little cold, but there never really a good reason to turn down pie.", ActionVerb="EAT", ActionResult="HEALTH", ActionValue ="100", ActionPoints=10},
#endif
                    new Item { Name="ROCK", Description= "A magic rock",Location = 06, Action= "This looks more like a coprolite fossel than a rock. Might want to get rid of this thing soon.", ActionVerb="THROW", ActionResult="TELEPORT", ActionValue ="95", ActionPoints=10 }
           };

                return _items;
            }

            private static List<Room> Rooms()
            {
                var _rooms = new List<Room>
            {
                new Room { Number = 00, RoomPoints=100 ,Name = "Exit!", Desc="Exit! - You have found freedom!", N=99, S=99, E=99, W=01, U=99, D=99 },
                new Room { Number = 01, RoomPoints=25 ,Name = "Main Entrance", Desc="This is the house entrance. The door is securely locked.", N=10, S=02, E=99, W=99, U=99, D=99 },
                new Room { Number = 02, RoomPoints=50 ,Name = "Downstairs Hallway", Desc="This hall is at the bottom of the stairs.", N=01, S=04, E=03, W=99, U=11, D=99 },
                new Room { Number = 03, RoomPoints=5 ,Name = "Guest Bathroom", Desc="Small half bathroom downstairs. It is too small and ensures guests leave sooner than later.", N=99, S=99, E=05, W=02, U=99, D=99 },
                new Room { Number = 04, RoomPoints=5 ,Name = "Living Room", Desc="This is a simple living room on the southeast side of the house. The carpet is worn like lots of people have visited this room.", N=02, S=99, E=99, W=05, U=99, D=99 },
                new Room { Number = 05, RoomPoints=5 ,Name = "Family Room", Desc = "An inviting game room with a large TV and freshly broken Atari 2600.", N=06, S=99, E=02, W=99, U=99, D=99 },
                new Room { Number = 06, RoomPoints=5 ,Name = "Nook", Desc="This is a nook with a small dining table that has \"W. Robinett rulez\" scratched in top.", N=07, S=05, E=99, W=24, U=99, D=99 },
                new Room { Number = 07, RoomPoints=5 ,Name = "Kitchen", Desc="There is a clean floor and a large granite counter.", N=08, S=06, E=99, W=99, U=99, D=99 },
                new Room { Number = 08, RoomPoints=5 ,Name = "Utility Hall", Desc="This is a small hall with two large trash cans that are empty.", N=99, S=07, E=10, W=09, U=99, D=99 },
                new Room { Number = 09, RoomPoints=5 ,Name = "Garage", Desc="The the big garage door is closed and bolted shut. The garage is empty and clean.", N=99, S=99, E=08, W=99, U=99, D=99 },
                new Room { Number = 10, RoomPoints=5 ,Name = "Main Dining Room", Desc = "The dining room is on the northeast side of the house.", N = 99, S = 01, E = 99, W = 08, U = 99, D = 99 },
                new Room { Number = 11, RoomPoints=5 ,Name = "Upstairs Hallway", Desc = "This long hall is at the top of the stairs to the first floor.", N = 99, S = 12, E = 16, W = 13, U = 99, D = 02 },
                new Room { Number = 12, RoomPoints=5 ,Name = "Upstairs East Hallway", Desc = "Hall with two tables and computers. The Apple 2 and TRS-80 computers are broken.", N = 11, S = 15, E = 99, W = 99, U = 99, D = 99 },
                new Room { Number = 13, RoomPoints=5 ,Name = "Upstairs North Hallway", Desc = "The part of the hall has a large closet. ", N = 18, S = 14, E = 11, W = 17, U = 99, D = 99 },
                new Room { Number = 14, RoomPoints=5 ,Name = "Upstairs West Hallway", Desc = "The long hallway has with a small closet. That door is nailed shut with an out of order sign on the door. ", N = 13, S = 23, E = 99, W = 22, U = 99, D = 99 },
                new Room { Number = 15, RoomPoints=5 ,Name = "Spare Room", Desc = "The bedroom has a queen size bed and several KISS posters on the wall. It looks like it was a fun party room. The bed is covered in roses that look a bit old. The pedals like they were place there months ago.", N = 12, S = 99, E = 99, W = 99, U = 99, D = 99 },
                new Room { Number = 16, RoomPoints=5 ,Name = "Utility Room", Desc = "This is a laundry room with a washer and dryer. The dryer looks fine but the washer looks rusty and needs to be replaced.", N = 99, S = 99, E = 99, W = 11, U = 99, D = 99 },
                new Room { Number = 17, RoomPoints=5 ,Name = "Upstairs Bath", Desc = "The main bathroom with a large bathtub that is full of bubble bath. The water looks dirty and it stinks.", N = 99, S = 99, E = 13, W = 99, U = 99, D = 99 },
                new Room { Number = 18, RoomPoints=5 ,Name = "Master Bedroom", Desc = "The master bedroom with an inviting king size bed. The room is messy and it seems like they had a party here.", N = 21, S = 13, E = 19, W = 99, U = 99, D = 99 },
                new Room { Number = 19, RoomPoints=5 ,Name = "Master Bedroom Closet", Desc = "This is a long and narrow walk-in closet. A good place to put stairs to an attic.", N = 99, S = 99, E = 99, W = 18, U = 20, D = 99 },
#if (DEBUG)
                    new Room { Number = 20, RoomPoints=0 ,Name = "Attic", Desc = "You are in the Adventure house attic. This room is smelly and dark.", N = 99, S = 99, E = 99, W = 99, U = 88, D = 19 },
#else
                    new Room { Number = 20, RoomPoints=0 ,Name = "Attic", Desc = "You are in the house attic. This room is musty and dark.", N = 99, S = 99, E = 99, W = 99, U = 99, D = 19 },
#endif                
                new Room { Number = 21, RoomPoints=5 ,Name = "Master Bedroom Bath", Desc = "Warm master bedroom with a shower and tub.", N = 99, S = 18, E = 99, W = 99, U = 99, D = 99 },
                new Room { Number = 22, RoomPoints=5 ,Name = "Children's Room", Desc = "Clean children's room with twin beds.", N = 99, S = 99, E = 14, W = 99, U = 99, D = 99 },
                new Room { Number = 23, RoomPoints=5 ,Name = "Entertainment Room", Desc = "This is a very inviting play room with games and toys.", N = 14, S = 99, E = 99, W = 99, U = 99, D = 99 },
                new Room { Number = 24, RoomPoints=50 ,Name = "Deck", Desc = "You are standing on a finely crafted wooden covered deck.", N = 99, S = 99, E = 06, W = 99, U = 99, D = 99 },
#if (DEBUG)
                new Room { Number = 88, RoomPoints=50 ,Name = "Debug Room", Desc = "The Magic Debug Room Up - Leads to the Door, Down leads to the Attic", N = 99, S = 99, E = 99, W = 99, U = 1, D = 20 },
#endif
                new Room { Number = 93, RoomPoints=50 ,Name = "Psychedelic Ladder", Desc = "You are on a what seems like and endless glowing ladder. You see magic spiraling vortex. ", N = 99, S = 99, E = 19, W = 19, U = 95, D = 94 },
                new Room { Number = 94, RoomPoints=50 ,Name = "Memory Ladder", Desc = "You climbed on to the ladder and your memory of how to get back fades. You are on a what seems like and endless magic ladder.", N = 99, S = 99, E = 99, W = 99, U = 93, D = 95 },
                new Room { Number = 95, RoomPoints=50 ,Name = "Magic Mushroom", Desc = "A magic room. The walls sparkle and shine. This room seems like a very happy place. You see 4 doors and ladders leading up and down", N = 20, S = 20, E = 20, W = 20, U = 94, D = 93 }

            };

                return _rooms;
            }

            private static List<Message> Messages()
            {
                var _messsages = new List<Message>
            {
                new Message {MessageTag ="East", Messsage="There is no way to go @ from here." },
                new Message {MessageTag ="East", Messsage="Going @ from here is not possible." },
                new Message {MessageTag ="North", Messsage="You can't go @." },
                new Message {MessageTag ="North", Messsage="There's nothing @ from here." },
                new Message {MessageTag ="Up", Messsage="You can't go through the roof." },
                new Message {MessageTag ="Up", Messsage="There's a roof in the way." },
                new Message {MessageTag ="Up", Messsage="Ouch! You bump your head!"},
                new Message {MessageTag ="Down", Messsage="You can't dig through the floor." },
                new Message {MessageTag ="Down", Messsage="You sadly aren't a mole." },
                new Message {MessageTag ="Down", Messsage="You begin to dig but stop out of frustration." },
                new Message {MessageTag ="South", Messsage="There's no way to go @." },
                new Message {MessageTag ="South", Messsage="Going @ is not going to work." },
                new Message {MessageTag ="West", Messsage="There's no path leading @ from here." },
                new Message {MessageTag ="West", Messsage="It's a bad idea to go @. That leads to trouble."},
                new Message {MessageTag ="GetFailed", Messsage="The @ seems to be missing. Look again."},
                new Message {MessageTag ="GetFailed", Messsage="You reach for the @ through space and time but fail."},
                new Message {MessageTag ="PetFailed", Messsage="You reach for the @ but it jumps out of your hands."},
                new Message {MessageTag ="PetFailed", Messsage="The @ does not want to be picked up and put in your backpack."},
                new Message {MessageTag ="PetFailed", Messsage="The @ fails to respond at your harsh treatment."},
                new Message {MessageTag ="PetSuccess", Messsage="The cute @ begins to follow you."},
                new Message {MessageTag ="PetSuccess", Messsage="The little @ seems to like you and had become your friend."},
                new Message {MessageTag ="ShooSuccess", Messsage="The cute @ looks disappointed and sits then runs away."},
                new Message {MessageTag ="ShooSuccess", Messsage="The little @ is looks sad and runs to back to a safe hiding place."},
                new Message {MessageTag ="GetSuccess", Messsage="The @ seems to fit snugly in your pack."},
                new Message {MessageTag ="GetSuccess", Messsage="You store the @ in your inventory."},
                new Message {MessageTag ="DropFailed", Messsage="The @ is not in your inventory."},
                new Message {MessageTag ="DropSuccess", Messsage="You place the @ on the floor."},
                new Message {MessageTag ="UseFailed", Messsage="You try to use the @ and fail."},
                new Message {MessageTag ="UseFailed", Messsage="You can seem to figure out the @."},
                new Message {MessageTag ="UseFailed", Messsage="The @ is not intended to be used this way."},
                new Message {MessageTag ="EatFailed", Messsage="You try to eat the @ and your mouth refuses to chew."},
                new Message {MessageTag ="EatFailed", Messsage="Eating the @ won't work."},
                new Message {MessageTag ="EatFailed", Messsage="The @ is not as good as it looks."},
                new Message {MessageTag ="EatSuccessBig", Messsage="You try to eat the @ and it is very refreshing."},
                new Message {MessageTag ="EatSuccessMedium", Messsage="Eating the @ made you feel better."},
                new Message {MessageTag ="EatSuccessSmall", Messsage="The @ was good but not as filling as expected."},
                new Message {MessageTag ="ThrowFailed", Messsage="Throwing the @ did nothing."},
                new Message {MessageTag ="ThrowFailed", Messsage="No throwing @ here." },
                new Message {MessageTag ="ThrowFailed", Messsage="Throwing a @ seems silly."},
                new Message {MessageTag ="ThrowFailed", Messsage="You can't seem to throw the @"},
                new Message {MessageTag ="LookEmpty", Messsage="You look around and don't see anything special."},
                new Message {MessageTag ="LookFailed", Messsage="You don't see a @ here."},
                new Message {MessageTag ="WaveFailed", Messsage="Waving a @ seems silly."},
                new Message {MessageTag ="WaveFailed", Messsage="The @ attempts to wave back but fails."},
                new Message {MessageTag ="WaveSuccess", Messsage="Waving a @ worked! Poof!"},
                new Message {MessageTag ="WaveSuccess", Messsage="Waving the @ was a perfect idea! Zaaaap!"},
                new Message {MessageTag ="DeadMove", Messsage="You are dead. Don't do that!"},
                new Message {MessageTag ="DeadMove", Messsage="Only Zombies are allowed walk while dead."},
                new Message {MessageTag ="Any", Messsage="That seems like a bad idea. I can't let you do that" },
                new Message {MessageTag ="Any", Messsage="That did not work as expected." },
                new Message {MessageTag ="Any", Messsage="Nope." },
                new Message {MessageTag ="Any", Messsage="My Mom said I'm not allowed play that move." },
                new Message {MessageTag ="Any", Messsage="Try Again! Just kidding. That likely won't work!" },
                new Message {MessageTag ="Dead", Messsage = "You Died. I'd play Taps, but the bugle makes me feel sick R.I.P." },
                new Message {MessageTag ="Dead", Messsage = "You Died. Did that hurt? R.I.P." },
                new Message {MessageTag ="Bad", Messsage = "You will die soon unless you eat something." },
                new Message {MessageTag ="Bad", Messsage = "You feel hungry, tired and sick" },
                new Message {MessageTag ="Bad", Messsage = "You are Hangry!!" },
                new Message {MessageTag ="PetFollow", Messsage="The @ sits grinning and staring at you."},
                new Message {MessageTag ="PetFollow", Messsage="The @ races between your legs." },
                new Message {MessageTag ="PetFollow", Messsage="The @ stares silently." },
                new Message {MessageTag ="PetFollow", Messsage="The @ rolls over and begs for a belly rub." },
                new Message {MessageTag ="PetFollow", Messsage="The @ begins to sleep" },
                new Message {MessageTag ="PetFollow", Messsage = "The @ chases its tail." },
                new Message {MessageTag ="PetFollow", Messsage = "The @ sits and stars at its shadow." },
                new Message {MessageTag ="PetFollow", Messsage = "The @ chases a bug into a small hole." },
                new Message {MessageTag ="PetFollow", Messsage = "The @ sleeps quietly." },
                new Message {MessageTag ="PetFollow", Messsage = "The @ runs across the room and then returns to your feet." },
                new Message {MessageTag ="MonsterAppear", Messsage="A wild @ suddenly appears!"},
                new Message {MessageTag ="MonsterAppear", Messsage="You hear a buzzing sound as a @ enters the room."},
                new Message {MessageTag ="MonsterAppear", Messsage="Suddenly, a @ flies into view!"},
                new Message {MessageTag ="MonsterAttack", Messsage="The @ attacks you!"},
                new Message {MessageTag ="MonsterAttack", Messsage="The @ swoops down and strikes!"},
                new Message {MessageTag ="MonsterMiss", Messsage="The @ misses you completely."},
                new Message {MessageTag ="MonsterMiss", Messsage="You dodge the @'s attack."},
                new Message {MessageTag ="MonsterHit", Messsage="The @ bites you! Ouch!"},
                new Message {MessageTag ="MonsterHit", Messsage="The @ stings you painfully!"},
                new Message {MessageTag ="MonsterDefeated", Messsage="You defeat the @! It flies away."},
                new Message {MessageTag ="MonsterDefeated", Messsage="The @ is vanquished!"},
                new Message {MessageTag ="AttackSuccess", Messsage="You successfully attack the @ with your @!"},
                new Message {MessageTag ="AttackFailed", Messsage="You need the right weapon to attack the @."},
                new Message {MessageTag ="NoMonster", Messsage="There's nothing here to attack."},
                new Message {MessageTag ="PetAttackSuccess", Messsage="Your brave @ leaps to your defense!"},
                new Message {MessageTag ="PetAttackSuccess", Messsage="The loyal @ springs into action!"},
                new Message {MessageTag ="PetAttackSuccess", Messsage="Your @ courageously attacks the enemy!"},
                new Message {MessageTag ="PetAttackFailed", Messsage="Your @ wants to help but looks uncertain."},
                new Message {MessageTag ="PetAttackFailed", Messsage="The @ hesitates, not sure how to help."},
                new Message {MessageTag ="PetDefeated", Messsage="Your heroic @ saves the day!"},
                new Message {MessageTag ="PetDefeated", Messsage="The brave @ emerges victorious!"}
                };

                return _messsages;
            }

            private static List<Monster> Monsters()
            {
                var _monsters = new List<Monster>
                {
                    new Monster 
                    { 
                        Key = "MOSQUITO", 
                        Name = "Mosquito", 
                        Description = "A large, annoying mosquito buzzing around the room. It looks like it wants to make you its next meal.",
                        RoomNumber = 23, // Entertainment Room
                        ObjectNameThatCanAttackThem = "FLYSWATTER",
                        AttacksToKill = 1,
                        CurrentHealth = 1, // Initialize CurrentHealth to AttacksToKill
                        CanHitPlayer = true,
                        HitOdds = 30, // 30% chance to hit player
                        HealthDamage = 5,
                        AppearanceChance = 60, // 60% chance to appear when entering room
                        IsPresent = false, // Start as not present
                        PetAttackChance = 30 // 30% chance pet will attack when player lacks flyswatter
                    }
                };

                return _monsters;
            }
    }
}
