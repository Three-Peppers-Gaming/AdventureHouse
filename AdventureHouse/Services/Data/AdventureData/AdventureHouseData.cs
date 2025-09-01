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
                new() { Name = "BREAD", Description = "A small loaf of Wonder Bread. Not quite a lunch, too big for an afternoon snack.", Location = 6, Action = "The bread was fresh and warm, just like mom used to make.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 5 },
                new() { Name = "BUGLE", Description = "A shiny brass bugle that looks like it came from a marching band. You were never very good with instruments.", Location = 20, Action = "You try to produce something musical but it sounds like a dying moose.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "-1", ActionPoints = 1 },
                new() { Name = "APPLE", Description = "A crisp Red Delicious apple that looks fresh, shiny and totally appetizing.", Location = 7, Action = "Tastes just as good as it looked. Totally rad!", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 10 },
                new() { Name = "KEY", Description = "A shiny brushed metal key that has serious weight and feel. This must open something important in this groovy house.", Location = 24, Action = "The key fits perfectly and the door unlocked with some effort. Far out!", ActionVerb = "USE", ActionResult = "UNLOCK", ActionValue = "1|E|0|This is the entrance. The door is unlocked.|This is the entrance. Door is now unlocked", ActionPoints = 100 },
                new() { Name = "WAND", Description = "A small wooden wand with 'G. Stilton' scratched into the base. Looks totally mystical.", Location = 17, Action = "You wave the wand and the room fades for a second like a scene from Star Wars.", ActionVerb = "WAVE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints = 10 },
                new() { Name = "PIE", Description = "A righteous slice of apple pie that looks mouthwatering and fresh from the oven.", Location = 10, Action = "A little cold, but there's never really a good reason to turn down pie. Totally awesome!", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 10 },
                new() { Name = "KITTEN", Description = "A delightful fuzzy kitten that's cuter than a Garfield cartoon", Location = 20, Action = "The little fuzzball, a black and white kitten, looks absolutely adorable and ready to be your sidekick!", ActionVerb = "PET", ActionResult = "FOLLOW", ActionValue = "20", ActionPoints = 50 },
                new() { Name = "SLIP", Description = "A small slip of paper that looks like a fortune cookie slip from yesterday's Chinese takeout. The words seem to fade in and out like a broken TV.", Location = 18, Action = "The fortune cookie slip from yesterday's Chinese takeout dinner", ActionVerb = "READ", ActionResult = "FORTUNE", ActionValue = "1", ActionPoints = 33 },
                new() { Name = "ROCK", Description = "A totally gnarly magic rock that glows like something from E.T.", Location = 6, Action = "This looks more like a fossilized dinosaur dropping than a rock. Might want to ditch this thing soon!", ActionVerb = "THROW", ActionResult = "TELEPORT", ActionValue = "95", ActionPoints = 10 },
                new() { Name = "FLYSWATTER", Description = "A sturdy plastic fly swatter with a bright yellow handle that screams 1980's kitchen chic. Perfect for dealing with pesky insects.", Location = 18, Action = "You swing the fly swatter through the air with a satisfying whoosh sound like you're Bruce Lee.", ActionVerb = "ATTACK", ActionResult = "WEAPON", ActionValue = "MOSQUITO", ActionPoints = 25 },
#if DEBUG
                new() { Name = "STICK", Description = "This is the developer's helpful and totally magical debugging stick.", Location = 0, Action = "This looks like a debugging tool that a radical programmer would create to make life easy.", ActionVerb = "WAVE", ActionResult = "TELEPORT", ActionValue = "88", ActionPoints = 0 },
                new() { Name = "AWAND", Description = "A small wooden wand that looks mystical.", Location = 99, Action = "You wave the wand and the room fades for a second.", ActionVerb = "WAVE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints = 1 },
                new() { Name = "ADONUT", Description = "A small glazed donut. Not quite a lunch, too big for a snack.", Location = 88, Action = "The donut was fresh and sweet.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 5 },
                new() { Name = "ABUGLE", Description = "A brass instrument. You were never very good with these.", Location = 88, Action = "You try to no avail to produce something that could constitute music.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "-1", ActionPoints = 1 },
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
                new() { Number = 0, Name = "Exit!", Desc = "Exit! - You have found freedom! The fresh air tastes like victory and freedom!", N = 99, S = 99, E = 99, W = 1, U = 99, D = 99, RoomPoints = 100 },
                new() { Number = 1, Name = "Main Entrance", Desc = "This is the house entrance with a heavy wooden door. The door is securely locked with multiple deadbolts.", N = 10, S = 2, E = 99, W = 99, U = 99, D = 99, RoomPoints = 25 },
                new() { Number = 2, Name = "Downstairs Hallway", Desc = "This hall is at the bottom of the stairs with wood paneling that screams 1980's suburban chic.", N = 1, S = 4, E = 3, W = 99, U = 11, D = 99, RoomPoints = 50 },
                new() { Number = 3, Name = "Guest Bathroom", Desc = "Small half bathroom downstairs with avocado green fixtures. It's too small and ensures guests leave sooner than later.", N = 99, S = 99, E = 5, W = 2, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 4, Name = "Living Room", Desc = "This is a rad living room on the southeast side of the house. The shag carpet is worn like lots of people have partied here.", N = 2, S = 99, E = 99, W = 5, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 5, Name = "Family Room", Desc = "An totally awesome game room with a large wood-grain TV and a freshly broken Atari 2600 that someone tried to fix with duct tape.", N = 6, S = 99, E = 2, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 6, Name = "Nook", Desc = "This is a groovy breakfast nook with a small dining table that has 'W. Robinett rulez' scratched into the top.", N = 7, S = 5, E = 99, W = 24, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 7, Name = "Kitchen", Desc = "There's a clean linoleum floor and large Formica counter. The appliances are harvest gold and totally retro.", N = 8, S = 6, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 8, Name = "Utility Hall", Desc = "This is a small hall with two large trash cans that are empty. The walls have fake wood paneling.", N = 99, S = 7, E = 10, W = 9, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 9, Name = "Garage", Desc = "The big garage door is closed and bolted shut. The garage is empty and clean except for some old motor oil stains.", N = 99, S = 99, E = 8, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 10, Name = "Main Dining Room", Desc = "The dining room is on the northeast side of the house with a formal table and china cabinet full of fancy dishes.", N = 99, S = 1, E = 99, W = 8, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 11, Name = "Upstairs Hallway", Desc = "This long hall is at the top of the stairs with burnt orange carpet that's totally groovy.", N = 99, S = 12, E = 16, W = 13, U = 99, D = 2, RoomPoints = 5 },
                new() { Number = 12, Name = "Upstairs East Hallway", Desc = "Hall with two tables featuring broken computers. The Apple 2 and TRS-80 computers look like they've seen better days.", N = 11, S = 15, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 13, Name = "Upstairs North Hallway", Desc = "This part of the hall has a large linen closet with flowered wallpaper that's totally rad.", N = 18, S = 14, E = 11, W = 17, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 14, Name = "Upstairs West Hallway", Desc = "The long hallway with a small closet. That door is nailed shut with an 'out of order' sign that looks hand-written.", N = 13, S = 23, E = 99, W = 22, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 15, Name = "Spare Room", Desc = "The bedroom has a queen size waterbed and several KISS posters on the wall. It looks like it was a totally gnarly party room. The bed is covered in rose petals that look months old.", N = 12, S = 99, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 16, Name = "Utility Room", Desc = "This is a laundry room with a matching washer and dryer set in avocado green. The dryer looks fine but the washer is rustier than a forgotten Pinto.", N = 99, S = 99, E = 99, W = 11, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 17, Name = "Upstairs Bath", Desc = "The main bathroom with a large bathtub full of bubble bath that's gone flat. The water looks murky and smells funky.", N = 99, S = 99, E = 13, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 18, Name = "Master Bedroom", Desc = "The master bedroom with an inviting king size bed and a mirrored ceiling. The room is messy like someone threw an epic house party.", N = 21, S = 13, E = 19, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 19, Name = "Master Bedroom Closet", Desc = "This is a long and narrow walk-in closet filled with polyester suits and platform shoes. A perfect place for stairs to an attic.", N = 99, S = 99, E = 99, W = 18, U = 20, D = 99, RoomPoints = 5 },
                new() { Number = 20, Name = "Attic", Desc = "You are in the house attic. This room is musty, dark, and filled with boxes of old 8-track tapes and Christmas decorations.", N = 99, S = 99, E = 99, W = 99, U = 99, D = 19, RoomPoints = 0 },
                new() { Number = 21, Name = "Master Bedroom Bath", Desc = "Totally tubular master bathroom with a shower, tub, and gold-flecked mirror tiles that scream luxury.", N = 99, S = 18, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 22, Name = "Children's Room", Desc = "Clean children's room with twin beds and walls covered in Star Wars and E.T. posters.", N = 99, S = 99, E = 14, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 23, Name = "Entertainment Room", Desc = "This is a totally rad play room with board games, a pinball machine, and toys scattered around like a tornado hit.", N = 14, S = 99, E = 99, W = 99, U = 99, D = 99, RoomPoints = 5 },
                new() { Number = 24, Name = "Deck", Desc = "You are standing on a finely crafted wooden deck with a view of the backyard and a barbecue grill that's seen better days.", N = 99, S = 99, E = 6, W = 99, U = 99, D = 99, RoomPoints = 50 },
#if DEBUG
                new() { Number = 88, Name = "Debug Room", Desc = "The Magic Debug Room - Up leads to the Door, Down leads to the Attic. This place is more mysterious than the Bermuda Triangle.", N = 99, S = 99, E = 99, W = 99, U = 1, D = 20, RoomPoints = 50 },
#endif
                new() { Number = 93, Name = "Psychedelic Ladder", Desc = "You are on what seems like an endless glowing ladder with swirling colors. You see a magic spiraling vortex that's totally far out.", N = 99, S = 99, E = 19, W = 19, U = 95, D = 94, RoomPoints = 50 },
                new() { Number = 94, Name = "Memory Ladder", Desc = "You climbed onto this ladder and your memory of how to get back fades like a bad dream. You're on an endless magic ladder that defies reality.", N = 99, S = 99, E = 99, W = 99, U = 93, D = 95, RoomPoints = 50 },
                new() { Number = 95, Name = "Magic Mushroom", Desc = "A totally awesome magic room where the walls sparkle and shine like a disco ball. This room feels like the happiest place in the universe. You see 4 doors and ladders leading up and down.", N = 20, S = 20, E = 20, W = 20, U = 94, D = 93, RoomPoints = 50 }
            };
        }

        private static List<Message> Messages()
        {
            return new List<Message>
            {
                // Generic action responses with 80's flair
                new() { MessageTag = "any", Messsage = "That's totally bogus, dude! You can't do that here." },
                new() { MessageTag = "any", Messsage = "No way, José! That doesn't work in this room." },
                new() { MessageTag = "any", Messsage = "Nice try, but that's not happening, sport." },
                
                // Get item success messages
                new() { MessageTag = "getsuccess", Messsage = "Rad! You snagged the @." },
                new() { MessageTag = "getsuccess", Messsage = "Totally awesome! The @ is now yours." },
                new() { MessageTag = "getsuccess", Messsage = "Groovy! You pick up the @ like a champ." },
                new() { MessageTag = "getsuccess", Messsage = "Far out! The @ fits perfectly in your Members Only jacket pocket." },
                
                // Get item failed messages
                new() { MessageTag = "getfailed", Messsage = "Gag me with a spoon! You can't get that." },
                new() { MessageTag = "getfailed", Messsage = "No dice, ace! That's not happening." },
                new() { MessageTag = "getfailed", Messsage = "Dream on! You can't pick that up." },
                
                // Drop item success messages
                new() { MessageTag = "dropsuccess", Messsage = "You drop the @ like it's hot potato." },
                new() { MessageTag = "dropsuccess", Messsage = "The @ hits the shag carpet with a thud." },
                new() { MessageTag = "dropsuccess", Messsage = "You set down the @ next to your Walkman." },
                
                // Drop item failed messages
                new() { MessageTag = "dropfailed", Messsage = "Duh! You don't have that, Einstein." },
                new() { MessageTag = "dropfailed", Messsage = "What are you, spacing out? You're not carrying that." },
                
                // Pet success messages
                new() { MessageTag = "petsuccess", Messsage = "Totally tubular! The @ starts following you like you're the Fonz!" },
                new() { MessageTag = "petsuccess", Messsage = "Awesome possum! The @ thinks you're the bee's knees!" },
                new() { MessageTag = "petsuccess", Messsage = "Gnarly! The @ is now your faithful sidekick!" },
                
                // Pet failed messages
                new() { MessageTag = "petfailed", Messsage = "Not cool, man. You can't pet that." },
                new() { MessageTag = "petfailed", Messsage = "What's your damage? That's not pettable." },
                
                // Pet following messages
                new() { MessageTag = "petfollow", Messsage = "Your righteous @ follows you like a loyal companion." },
                new() { MessageTag = "petfollow", Messsage = "The adorable @ trots behind you, purring contentedly." },
                new() { MessageTag = "petfollow", Messsage = "Your fuzzy @ companion stays close, ready for adventure." },
                
                // Shoo success messages
                new() { MessageTag = "shoosuccess", Messsage = "Bummer! The @ scampers away like it just heard Pac-Man died." },
                new() { MessageTag = "shoosuccess", Messsage = "The @ splits faster than a banana in a blender!" },
                
                // Use item failed messages
                new() { MessageTag = "UseFailed", Messsage = "That's whack! You can't use that here, chief." },
                new() { MessageTag = "UseFailed", Messsage = "No way in heck! That won't work in this room." },
                
                // Look command messages
                new() { MessageTag = "LookEmpty", Messsage = "Look at what, space cadet?" },
                new() { MessageTag = "LookEmpty", Messsage = "Hellooo? Look at WHAT exactly?" },
                
                new() { MessageTag = "LookFailed", Messsage = "I don't see that anywhere, Sherlock." },
                new() { MessageTag = "LookFailed", Messsage = "That's not here, genius. Check your glasses!" },
                
                // Death and health messages
                new() { MessageTag = "dead", Messsage = "Game over, man! You've totally bought the farm!" },
                new() { MessageTag = "dead", Messsage = "Bummer to the max! You've gone to that big arcade in the sky!" },
                new() { MessageTag = "dead", Messsage = "Wipeout! Your adventure ends here, dude." },
                
                new() { MessageTag = "bad", Messsage = "Whoa! You're feeling gnarly and need some grub, pronto!" },
                new() { MessageTag = "bad", Messsage = "Bogus! You're getting weak as a wet noodle." },
                new() { MessageTag = "bad", Messsage = "Yikes! You're feeling totally spaced out and hungry." },
                
                // Movement blocked messages with 80's references
                new() { MessageTag = "DeadMove", Messsage = "Dead dudes don't boogie, pal." },
                
                new() { MessageTag = "north", Messsage = "No can do, buckaroo! There's no way north." },
                new() { MessageTag = "north", Messsage = "That's a no-go, chief! The wall blocks your path like a brick house." },
                new() { MessageTag = "north", Messsage = "Nope! Going north is more impossible than beating Pac-Man." },
                
                new() { MessageTag = "south", Messsage = "Southern comfort ain't happening here, sport!" },
                new() { MessageTag = "south", Messsage = "Can't go south! It's blocked tighter than a new jar of Tang." },
                new() { MessageTag = "south", Messsage = "South is a no-show, amigo!" },
                
                new() { MessageTag = "east", Messsage = "Eastward bound? Not today, cowboy!" },
                new() { MessageTag = "east", Messsage = "Going east is out of the question, like wearing bell-bottoms to a punk show." },
                new() { MessageTag = "east", Messsage = "East? That's more blocked than a clogged lava lamp." },
                
                new() { MessageTag = "west", Messsage = "Westward ho? More like westward NO!" },
                new() { MessageTag = "west", Messsage = "Can't head west! It's blocked like Saturday Night Fever on a Monday." },
                new() { MessageTag = "west", Messsage = "West is totally off-limits, dude!" },
                
                new() { MessageTag = "up", Messsage = "Up, up, and away? Not here, Superman!" },
                new() { MessageTag = "up", Messsage = "Going up is harder than programming a VCR." },
                new() { MessageTag = "up", Messsage = "Can't go up! It's more impossible than getting MTV to play music videos." },
                
                new() { MessageTag = "down", Messsage = "Down and out? Just out, my friend!" },
                new() { MessageTag = "down", Messsage = "Going down is blocked like a broken Rubik's Cube." },
                new() { MessageTag = "down", Messsage = "Can't go down! That path is deader than disco." },
                
                // Monster encounter messages
                new() { MessageTag = "MonsterAppear", Messsage = "Holy guacamole! A wild @ suddenly appears like a bad hair day!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Zoinks! You hear a gnarly buzzing as a @ enters the room." },
                new() { MessageTag = "MonsterAppear", Messsage = "Tubular terror! A @ flies into view faster than a DeLorean!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Radical but scary! A @ appears like it just escaped from Stranger Things!" },
                
                new() { MessageTag = "MonsterAttack", Messsage = "The @ attacks you like it's possessed by the spirit of a broken Atari!" },
                new() { MessageTag = "MonsterAttack", Messsage = "Bogus! The @ swoops down like a kamikaze pilot!" },
                new() { MessageTag = "MonsterAttack", Messsage = "The @ strikes faster than you can say 'Where's the beef?'" },
                
                new() { MessageTag = "MonsterHit", Messsage = "Ouch! The @ bites you harder than a Rubik's Cube solution!" },
                new() { MessageTag = "MonsterHit", Messsage = "Ow! The @ stings you more painfully than listening to elevator music!" },
                new() { MessageTag = "MonsterHit", Messsage = "Yikes! The @ nails you like a bad perm on picture day!" },
                
                new() { MessageTag = "MonsterMiss", Messsage = "Totally awesome! The @ misses you completely, like a broken Nintendo cartridge!" },
                new() { MessageTag = "MonsterMiss", Messsage = "Rad dodge! You avoid the @'s attack like you're Neo from The Matrix!" },
                new() { MessageTag = "MonsterMiss", Messsage = "Groovy! The @ whiffs harder than a disco comeback attempt!" },
                
                new() { MessageTag = "MonsterDefeated", Messsage = "Totally excellent! You defeat the @! It flies away like it's late for aerobics class!" },
                new() { MessageTag = "MonsterDefeated", Messsage = "Radical victory! The @ is vanquished faster than you can say 'gag me with a spoon!'" },
                new() { MessageTag = "MonsterDefeated", Messsage = "Awesome sauce! The @ retreats like it just realized it left its Walkman on!" },
                
                // Combat success/failure messages
                new() { MessageTag = "AttackSuccess", Messsage = "Totally gnarly! You successfully whack the @ with your @!" },
                new() { MessageTag = "AttackSuccess", Messsage = "Righteous hit! Your @ connects with the @ like a perfect Pac-Man chomp!" },
                new() { MessageTag = "AttackSuccess", Messsage = "Excellent! You nail the @ with your @ like you're the Karate Kid!" },
                
                new() { MessageTag = "AttackFailed", Messsage = "Bummer, dude! You need the right weapon to attack the @. This isn't Dungeons & Dragons!" },
                new() { MessageTag = "AttackFailed", Messsage = "No dice! You can't hurt the @ without the proper gear, sport!" },
                new() { MessageTag = "AttackFailed", Messsage = "That's bogus! The @ laughs at your feeble attempt!" },
                
                new() { MessageTag = "NoMonster", Messsage = "There's nothing here to attack, Rambo!" },
                new() { MessageTag = "NoMonster", Messsage = "Attack what? The air? You're not in a ninja movie, chief!" },
                new() { MessageTag = "NoMonster", Messsage = "Chill out! There are no bad guys here to fight." },
                
                // Pet combat assistance messages
                new() { MessageTag = "PetAttackSuccess", Messsage = "Totally rad! Your brave @ leaps to your defense like Lassie saving Timmy!" },
                new() { MessageTag = "PetAttackSuccess", Messsage = "Awesome possum! The loyal @ springs into action like a furry superhero!" },
                new() { MessageTag = "PetAttackSuccess", Messsage = "Far out! Your @ courageously attacks the enemy like it's auditioning for a action flick!" },
                
                new() { MessageTag = "PetAttackFailed", Messsage = "Your @ wants to help but looks more confused than someone trying to program a VCR." },
                new() { MessageTag = "PetAttackFailed", Messsage = "The @ hesitates, looking as uncertain as a breakdancer on roller skates." },
                new() { MessageTag = "PetAttackFailed", Messsage = "Your @ tries to help but seems as lost as a tourist without a map." },
                
                new() { MessageTag = "PetDefeated", Messsage = "Totally tubular! Your heroic @ saves the day like a fuzzy Chuck Norris!" },
                new() { MessageTag = "PetDefeated", Messsage = "Righteous! The brave @ emerges victorious, ready for its close-up!" },
                new() { MessageTag = "PetDefeated", Messsage = "Excellent! Your @ wins the day and deserves a treat from the fridge!" }
            };
        }

        private static List<Monster> Monsters()
        {
            return new List<Monster>
            {
                new() 
                { 
                    Key = "MOSQUITO", 
                    Name = "Mosquito", 
                    Description = "A totally gnarly, oversized mosquito buzzing around the room like it escaped from a B-grade horror movie. It looks hungry and you're on the menu!", 
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
