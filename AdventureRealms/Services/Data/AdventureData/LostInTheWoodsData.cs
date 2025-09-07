using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureRealms.Services.AdventureServer.Models;
using AdventureRealms.Services.AdventureClient.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.Data.AdventureData
{
    /// <summary>
    /// Lost in the Woods adventure - find your way home through the enchanted forest
    /// A fairy tale adventure with creatures, items, and a journey back to safety
    /// </summary>
    public class LostInTheWoodsData
    {
        private readonly LostInTheWoodsConfiguration _config = new();

        public string GetAdventureHelpText()
        {
            return _config.GetAdventureHelpText();
        }

        public string GetAdventureThankYouText()
        {
            return _config.GetAdventureThankYouText();
        }

        public LostInTheWoodsConfiguration GetGameConfiguration()
        {
            return _config;
        }

        public PlayAdventureModel SetupAdventure(string instanceID)
        {
            var gamerTag = new GamerTags().RandomTag();

            var player = new Player
            {
                Name = gamerTag,
                Room = _config.StartingRoom, // Grandma's House
                HealthCurrent = _config.MaxHealth,
                HealthMax = _config.MaxHealth,
                PlayerID = Guid.NewGuid().ToString(),
                Verbose = true,
                Points = _config.StartingPoints
            };

            var adventure = new PlayAdventureModel
            {
                GameID = 4, // New game ID for Lost in the Woods
                GameName = _config.GameName,
                GameHelp = _config.GetAdventureHelpText(),
                GameThanks = _config.GetAdventureThankYouText(),
                InstanceID = instanceID,
                WelcomeMessage = _config.GetWelcomeMessage(gamerTag),
                StartRoom = _config.StartingRoom,
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

        private static List<Room> Rooms()
        {
            return new List<Room>
            {
                // Exit room (goal)
                new() { Number = 0, Name = "Exit!", Desc = "Congratulations! You've made it home safely! Your family rushes out to greet you, relieved that you found your way back through the enchanted forest. The adventure is complete!", N = 99, S = 99, E = 99, W = 28, U = 99, D = 99, RoomPoints = 200 },

                // Starting point - Grandma's House
                new() { Number = 1, Name = "Grandma's House", Desc = "You're standing outside Grandma's cozy cottage, your belly still warm from the delicious Wolf Stew. The forest path stretches east into the woods. You can see your BBGun leaning against the porch and a sandwich wrapped in cloth on the table.", N = 99, S = 99, E = 2, W = 99, U = 99, D = 99, RoomPoints = 0 },

                // Main forest path
                new() { Number = 2, Name = "Forest Path", Desc = "A well-worn dirt path winds through tall pine trees. Sunlight filters through the canopy above, creating dancing shadows on the forest floor. The path continues east deeper into the woods, or you can head west back to Grandma's.", N = 12, S = 13, E = 3, W = 1, U = 99, D = 99, RoomPoints = 5 },

                new() { Number = 3, Name = "Deep Woods", Desc = "The forest grows thicker here, with ancient oaks and maples towering overhead. A babbling brook can be heard to the south, and an old oak tree stands majestically to the north. The path continues east.", N = 6, S = 9, E = 4, W = 2, U = 99, D = 99, RoomPoints = 10 },

                new() { Number = 4, Name = "Clearing", Desc = "A peaceful forest clearing where wildflowers bloom in patches of sunlight. A babbling brook flows gently to the south, and you can hear birds chirping in the trees. Paths lead in multiple directions.", N = 7, S = 10, E = 5, W = 3, U = 99, D = 99, RoomPoints = 15 },

                new() { Number = 5, Name = "Meadow", Desc = "A beautiful meadow filled with colorful wildflowers and buzzing bees. The sweet scent of flowers fills the air. A rocky outcrop rises to the north, and the path continues east through the meadow.", N = 8, S = 11, E = 14, W = 4, U = 99, D = 99, RoomPoints = 20 },

                // Northern forest areas
                new() { Number = 6, Name = "Old Oak Tree", Desc = "An enormous old oak tree dominates this area, its massive trunk covered in moss and carved initials from generations of forest visitors. Squirrels chatter in its branches. A sunlit glade lies to the west.", N = 99, S = 3, E = 7, W = 12, U = 99, D = 99, RoomPoints = 15 },

                new() { Number = 7, Name = "Babbling Brook", Desc = "A crystal-clear brook babbles merrily over smooth stones. Small fish dart in the clear water, and dragonflies hover above the surface. The water looks safe to drink. You could follow the brook north or south.", N = 6, S = 4, E = 8, W = 99, U = 99, D = 99, RoomPoints = 10 },

                new() { Number = 8, Name = "Rocky Outcrop", Desc = "A jumble of large boulders creates natural steps leading upward. From here you can see over much of the forest canopy. The view is breathtaking, but you notice storm clouds gathering in the distance.", N = 99, S = 5, E = 15, W = 7, U = 99, D = 99, RoomPoints = 25 },

                // Southern forest areas  
                new() { Number = 9, Name = "Fallen Log", Desc = "A massive fallen tree trunk blocks much of this area, covered in green moss and mushrooms. Small woodland creatures scurry about. A spider's web glistens with dew to the east.", N = 3, S = 18, E = 10, W = 99, U = 99, D = 99, RoomPoints = 10 },

                new() { Number = 10, Name = "Spider's Web", Desc = "An enormous spider's web stretches between two trees, shimmering with morning dew. The web is beautiful but ominous - you can see something large moving in its center. A dark cave entrance yawns to the east.", N = 4, S = 99, E = 11, W = 9, U = 99, D = 99, RoomPoints = 15 },

                new() { Number = 11, Name = "Dark Cave", Desc = "The entrance to a dark, mysterious cave. You can hear the echo of dripping water and the flutter of bat wings from within. Strange symbols are carved around the entrance. Do you dare enter?", N = 5, S = 99, E = 16, W = 10, U = 99, D = 99, RoomPoints = 30 },

                // Western forest areas
                new() { Number = 12, Name = "Sunlit Glade", Desc = "A beautiful glade where golden sunlight streams through the trees. Wildflowers bloom in abundance here, and butterflies dance from flower to flower. This feels like a peaceful, magical place.", N = 99, S = 2, E = 6, W = 17, U = 99, D = 99, RoomPoints = 20 },

                new() { Number = 13, Name = "Berry Patch", Desc = "A tangled patch filled with berry bushes. You can see both blue berries and green berries growing here. The blue ones look delicious, but you're not sure about the green ones. Small animal tracks crisscross the area.", N = 2, S = 18, E = 9, W = 99, U = 99, D = 99, RoomPoints = 15 },

                // Eastern forest areas - deeper into the woods
                new() { Number = 14, Name = "Muddy Bog", Desc = "The ground becomes soft and muddy here, with cattails and marsh grasses growing in standing water. Your feet sink slightly with each step. Strange sounds echo from the twisted roots to the north.", N = 15, S = 16, E = 19, W = 5, U = 99, D = 99, RoomPoints = 10 },

                new() { Number = 15, Name = "Twisted Roots", Desc = "Ancient tree roots twist and turn above ground, creating a maze-like area that's difficult to navigate. The roots seem to move slightly in your peripheral vision. A crystal spring bubbles to the north.", N = 20, S = 14, E = 99, W = 8, U = 99, D = 99, RoomPoints = 20 },

                new() { Number = 16, Name = "Hollow Tree", Desc = "A huge hollow tree with an opening large enough to walk through. Inside, you can see scratches on the bark and what looks like a small nest. Something rustles in the branches above.", N = 14, S = 21, E = 99, W = 11, U = 99, D = 99, RoomPoints = 15 },

                // Far western areas
                new() { Number = 17, Name = "Moss Covered Stones", Desc = "A circle of ancient stones covered in thick, soft moss. The stones seem to have been placed here intentionally long ago. There's a mystical feeling to this place, as if it holds old forest magic.", N = 12, S = 18, E = 99, W = 99, U = 99, D = 99, RoomPoints = 25 },

                new() { Number = 18, Name = "Wildflower Field", Desc = "An explosion of colorful wildflowers covers this area - red poppies, blue cornflowers, yellow daisies, and purple lupine create a natural rainbow. Bees buzz contentedly among the blooms.", N = 17, S = 99, E = 13, W = 99, U = 99, D = 99, RoomPoints = 20 },

                // Far eastern areas - getting close to home
                new() { Number = 19, Name = "Thorny Thicket", Desc = "Dense brambles and thorny vines create a nearly impenetrable barrier. You'll need to be careful navigating through here - the thorns look sharp enough to tear clothing and skin.", N = 20, S = 21, E = 22, W = 14, U = 99, D = 99, RoomPoints = 15 },

                new() { Number = 20, Name = "Crystal Spring", Desc = "A magical crystal-clear spring bubbles up from deep underground. The water is so pure it almost glows, and drinking it makes you feel refreshed and energized. This must be an enchanted spring.", N = 23, S = 19, E = 99, W = 15, U = 99, D = 99, RoomPoints = 30 },

                new() { Number = 21, Name = "Ancient Stump", Desc = "The massive stump of what must have been a truly enormous tree. The stump is so large you could hold a small gathering on top of it. Rings in the wood suggest this tree was hundreds of years old.", N = 19, S = 24, E = 99, W = 16, U = 99, D = 99, RoomPoints = 15 },

                // Final eastern areas - almost home
                new() { Number = 22, Name = "Rabbit Warren", Desc = "A hillside honeycombed with rabbit burrows. Dozens of fluffy rabbit tails disappear into holes as you approach. The rabbits seem nervous, thumping warnings to each other with their hind feet.", N = 23, S = 24, E = 25, W = 19, U = 99, D = 99, RoomPoints = 10 },

                new() { Number = 23, Name = "Bird's Nest", Desc = "High in the trees above, you can see several large bird nests. Colorful feathers drift down occasionally, and you hear the sweet songs of forest birds. A moonbeam clearing lies to the east.", N = 26, S = 22, E = 99, W = 20, U = 99, D = 99, RoomPoints = 15 },

                new() { Number = 24, Name = "Squirrel's Cache", Desc = "You've discovered a squirrel's winter cache - nuts and seeds are buried and stored throughout this area. The industrious squirrels chatter at you from the trees, protective of their hard work.", N = 22, S = 27, E = 99, W = 21, U = 99, D = 99, RoomPoints = 10 },

                // Final approach to home
                new() { Number = 25, Name = "Vine Covered Arch", Desc = "Flowering vines have grown over fallen branches to create a natural archway. Walking through it feels like passing through a green tunnel filled with the sweet scent of flowers. You sense you're getting close to home.", N = 26, S = 27, E = 28, W = 22, U = 99, D = 99, RoomPoints = 25 },

                new() { Number = 26, Name = "Moonbeam Clearing", Desc = "Even in daylight, this clearing seems to glow with an ethereal light. Legend says this is where moonbeams first touch the earth each night. The peaceful energy here makes you feel brave and confident.", N = 99, S = 25, E = 99, W = 23, U = 99, D = 99, RoomPoints = 30 },

                new() { Number = 27, Name = "Whisper Hollow", Desc = "A small hollow where the wind creates gentle whispers through the leaves. Local folklore says the trees here share secrets and provide guidance to lost travelers. The whispers seem to point you eastward.", N = 25, S = 99, E = 99, W = 24, U = 99, D = 99, RoomPoints = 25 },

                // Home at last!
                new() { Number = 28, Name = "Home Clearing", Desc = "At last! You've reached the familiar clearing near your home. You can see smoke rising from your family's chimney in the distance, and hear the comforting sounds of home. Just a bit further east and you'll be safe!", N = 99, S = 99, E = 0, W = 25, U = 99, D = 99, RoomPoints = 50 }
            };
        }

        private static List<Item> Items()
        {
            return new List<Item>
            {
                // Starting items at Grandma's House
                new() { Name = "BBGUN", Description = "A shiny Red Rider BB gun, perfect for a young adventurer. It's lightweight but sturdy, and makes you feel brave and ready for anything the forest might throw at you.", Location = 1, Action = "You take aim and fire a BB at your target. Bulls-eye!", ActionVerb = "USE", ActionResult = "WEAPON", ActionValue = "15", ActionPoints = 10 },
                
                new() { Name = "SANDWICH", Description = "A delicious sandwich wrapped in a clean cloth, prepared by Grandma with love. It contains fresh bread, cheese, and what looks like leftover Wolf Stew meat.", Location = 1, Action = "The sandwich is incredibly satisfying and reminds you of Grandma's love. You feel much better!", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "50", ActionPoints = 25 },

                // Food items scattered throughout the forest
                new() { Name = "BREAD", Description = "A small loaf of crusty bread, probably dropped by another traveler. It's still fresh and would make a good snack.", Location = 7, Action = "The bread is filling and gives you energy for your journey.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 10 },

                new() { Name = "BLUEBERRIES", Description = "Plump, juicy blueberries that glow with health and vitality. These forest berries are known for their healing properties.", Location = 13, Action = "The sweet blueberries burst with flavor and make you feel healthier and more energetic!", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "30", ActionPoints = 15 },

                new() { Name = "GREENBERRIES", Description = "Strange green berries that don't look quite right. They have an odd shine and unusual smell. You're not sure if they're safe to eat.", Location = 13, Action = "The green berries taste bitter and make you feel queasy. You probably shouldn't have eaten those.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "-20", ActionPoints = 5 },

                // Special companion
                new() { Name = "KITTEN", Description = "A small, lost kitten with soft gray fur and bright green eyes. It looks scared and hungry, mewing pitifully. It seems to want to follow you home.", Location = 11, Action = "You gently pet the kitten and it purrs contentedly, rubbing against your hand. Its purring is surprisingly soothing.", ActionVerb = "PET", ActionResult = "HEALTH", ActionValue = "10", ActionPoints = 20 },

                // Useful items scattered around
                new() { Name = "ROPE", Description = "A length of sturdy rope, perhaps left by a previous traveler. It could be useful for climbing or tying things.", Location = 8, Action = "The rope could be useful, but you're not sure what to tie up right now.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "0", ActionPoints = 5 },

                new() { Name = "MUSHROOM", Description = "A large, colorful mushroom growing at the base of a tree. You're not sure if it's edible or magical... or poisonous.", Location = 9, Action = "The mushroom has a strange, earthy taste. You feel a bit dizzy but not too bad.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "-5", ActionPoints = 5 },

                new() { Name = "FEATHER", Description = "A beautiful, iridescent feather that must have come from a magical forest bird. It seems to shimmer with its own light.", Location = 23, Action = "You wave the feather and feel a gentle breeze. It seems to have a calming effect.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "5", ActionPoints = 10 },

                new() { Name = "ACORN", Description = "A large, perfect acorn from the mighty oak tree. Squirrels chatter angrily when they see you pick it up - it must be precious to them.", Location = 6, Action = "You nibble on the acorn. It's crunchy and nutty, but not very filling.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "5", ActionPoints = 5 },

                new() { Name = "CRYSTAL", Description = "A small, clear crystal that seems to have formed naturally in the spring. It pulses with a faint inner light and feels warm to the touch.", Location = 20, Action = "The crystal glows brightly when you hold it, filling you with magical energy!", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 30 },

                new() { Name = "FLOWERS", Description = "A small bouquet of beautiful wildflowers in various colors. They smell lovely and would make a nice gift for someone special.", Location = 18, Action = "The flowers smell wonderful and remind you of home. You feel happier holding them.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "10", ActionPoints = 10 },

                new() { Name = "STICK", Description = "A sturdy wooden stick, perfect for walking or defending yourself. It's been naturally carved smooth by weather and time.", Location = 15, Action = "You swing the stick experimentally. It makes a good walking stick and could be used as a club.", ActionVerb = "USE", ActionResult = "WEAPON", ActionValue = "8", ActionPoints = 5 },

#if DEBUG
                // Debug items for testing
                new() { Name = "DEBUGTELEPORT", Description = "Debug teleporter wand.", Location = 88, Action = "You wave the wand and feel a magical whoosh!", ActionVerb = "USE", ActionResult = "TELEPORT", ActionValue = "28", ActionPoints = 0 },
                new() { Name = "DEBUGHEALTH", Description = "Debug health potion.", Location = 88, Action = "You feel completely restored!", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "1000", ActionPoints = 0 }
#endif
            };
        }

        private static List<Message> Messages()
        {
            return new List<Message>
            {
                // Health messages
                new() { MessageTag = "excellent", Messsage = "You feel fantastic and ready for any adventure!" },
                new() { MessageTag = "good", Messsage = "You're feeling good and confident about your journey." },
                new() { MessageTag = "fair", Messsage = "You're doing okay, but could use some food or rest." },
                new() { MessageTag = "poor", Messsage = "You're feeling tired and weak. You need food or help soon." },
                new() { MessageTag = "bad", Messsage = "You feel terrible and desperately need help or healing." },

                // Movement blocked messages with forest/fairy tale theme
                new() { MessageTag = "DeadMove", Messsage = "You're too exhausted to move. Your forest adventure has come to an end." },
                
                new() { MessageTag = "north", Messsage = "The thick forest blocks your path to the north." },
                new() { MessageTag = "north", Messsage = "Thorny brambles prevent you from going north." },
                new() { MessageTag = "north", Messsage = "A steep rocky cliff blocks the way north." },
                
                new() { MessageTag = "south", Messsage = "Dense undergrowth blocks your path to the south." },
                new() { MessageTag = "south", Messsage = "A deep ravine prevents you from going south." },
                new() { MessageTag = "south", Messsage = "Tangled roots make it impossible to head south." },
                
                new() { MessageTag = "east", Messsage = "You can't go east - the way is blocked by fallen trees." },
                new() { MessageTag = "east", Messsage = "A thick wall of thorns blocks your eastward path." },
                new() { MessageTag = "east", Messsage = "The forest is too dense to travel east from here." },
                
                new() { MessageTag = "west", Messsage = "You can't head west - there's no path through the thick woods." },
                new() { MessageTag = "west", Messsage = "A muddy bog blocks your way to the west." },
                new() { MessageTag = "west", Messsage = "Heavy undergrowth prevents westward travel." },
                
                new() { MessageTag = "up", Messsage = "The trees are too thick to climb up here." },
                new() { MessageTag = "up", Messsage = "There's nothing to climb up to from this location." },
                
                new() { MessageTag = "down", Messsage = "The ground is solid here - you can't go down." },
                new() { MessageTag = "down", Messsage = "There's no way to descend from this location." },

                // Item interaction messages
                new() { MessageTag = "ItemGet", Messsage = "You pick up the @ and add it to your pack." },
                new() { MessageTag = "ItemGet", Messsage = "You carefully collect the @ for your journey." },
                new() { MessageTag = "ItemGet", Messsage = "The @ looks useful, so you take it with you." },
                
                new() { MessageTag = "ItemDrop", Messsage = "You set the @ down carefully." },
                new() { MessageTag = "ItemDrop", Messsage = "You place the @ on the ground." },
                
                new() { MessageTag = "ItemNotFound", Messsage = "You don't see any @ here in the forest." },
                new() { MessageTag = "ItemNotFound", Messsage = "There's no @ to be found in this area." },
                
                new() { MessageTag = "ItemNotCarried", Messsage = "You don't have any @ in your pack." },
                new() { MessageTag = "ItemNotCarried", Messsage = "You're not carrying a @ right now." },

                // Combat messages
                new() { MessageTag = "MonsterAppear", Messsage = "A dangerous @ emerges from the shadows of the forest!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Watch out! A @ blocks your path through the woods!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Suddenly, a @ appears from behind the trees!" },

                new() { MessageTag = "MonsterAttack", Messsage = "The @ attacks you fiercely!" },
                new() { MessageTag = "MonsterAttack", Messsage = "The @ lunges at you with wild fury!" },
                new() { MessageTag = "MonsterAttack", Messsage = "The @ strikes out with vicious intent!" },

                new() { MessageTag = "MonsterDefeat", Messsage = "You successfully defeat the @! The forest path is clear again." },
                new() { MessageTag = "MonsterDefeat", Messsage = "The @ retreats into the depths of the forest, defeated." },
                new() { MessageTag = "MonsterDefeat", Messsage = "Your brave attack sends the @ fleeing into the woods!" },

                new() { MessageTag = "PlayerAttack", Messsage = "You bravely attack the @ with your @!" },
                new() { MessageTag = "PlayerAttack", Messsage = "You strike at the @ using your @!" },
                new() { MessageTag = "PlayerAttack", Messsage = "You defend yourself against the @ with your @!" },

                // Special forest messages
                new() { MessageTag = "ForestMagic", Messsage = "You feel the ancient magic of the forest surrounding you." },
                new() { MessageTag = "ForestMagic", Messsage = "The trees seem to whisper encouragement as you pass." },
                new() { MessageTag = "ForestMagic", Messsage = "A gentle forest breeze fills you with hope." },

                new() { MessageTag = "AnimalFriend", Messsage = "A friendly forest animal watches you with curious eyes." },
                new() { MessageTag = "AnimalFriend", Messsage = "Small woodland creatures scurry about, going about their daily business." },
                new() { MessageTag = "AnimalFriend", Messsage = "You hear the cheerful sounds of forest animals nearby." },

                // Generic messages
                new() { MessageTag = "NoTarget", Messsage = "You don't see anything like that here." },
                new() { MessageTag = "NoWeapon", Messsage = "You need a weapon to attack with!" },
                new() { MessageTag = "InvalidCommand", Messsage = "I don't understand that command." }
            };
        }

        private static List<Monster> Monsters()
        {
            return new List<Monster>
            {
                new() { 
                    Key = "SPIDER",
                    Name = "Spider", 
                    Description = "A large, hairy forest spider with eight gleaming eyes and venomous fangs. Its web glistens ominously in the dappled forest light.", 
                    RoomNumber = 10, 
                    ObjectNameThatCanAttackThem = "BBGUN",
                    AttacksToKill = 2,
                    CurrentHealth = 2,
                    CanHitPlayer = true,
                    HitOdds = 35,
                    HealthDamage = 12,
                    AppearanceChance = 70,
                    IsPresent = false,
                    PetAttackChance = 40
                },
                
                new() { 
                    Key = "BAT",
                    Name = "Bat", 
                    Description = "A large vampire bat with leathery wings and sharp fangs. It hangs upside down, watching you with beady red eyes, ready to swoop down at any moment.", 
                    RoomNumber = 11, 
                    ObjectNameThatCanAttackThem = "BBGUN",
                    AttacksToKill = 1,
                    CurrentHealth = 1,
                    CanHitPlayer = true,
                    HitOdds = 25,
                    HealthDamage = 8,
                    AppearanceChance = 60,
                    IsPresent = false,
                    PetAttackChance = 50
                },
                
                new() { 
                    Key = "SNAKE",
                    Name = "Snake", 
                    Description = "A thick forest snake with diamond patterns on its scales and a forked tongue that flicks out menacingly. It coils and uncoils, ready to strike.", 
                    RoomNumber = 19, 
                    ObjectNameThatCanAttackThem = "STICK",
                    AttacksToKill = 2,
                    CurrentHealth = 2,
                    CanHitPlayer = true,
                    HitOdds = 30,
                    HealthDamage = 10,
                    AppearanceChance = 65,
                    IsPresent = false,
                    PetAttackChance = 35
                },

                new() { 
                    Key = "WOLF",
                    Name = "Wolf", 
                    Description = "A fierce gray wolf with yellow eyes and bared fangs. It growls low in its throat, blocking your path through the forest with predatory intent.", 
                    RoomNumber = 22, 
                    ObjectNameThatCanAttackThem = "BBGUN",
                    AttacksToKill = 3,
                    CurrentHealth = 3,
                    CanHitPlayer = true,
                    HitOdds = 40,
                    HealthDamage = 15,
                    AppearanceChance = 80,
                    IsPresent = false,
                    PetAttackChance = 25
                }
            };
        }
    }
}
