using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.AdventureClient.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Space Station adventure data - contains all rooms, items, messages, and monsters
    /// A thrilling escape adventure aboard an abandoned space station
    /// </summary>
    public class SpaceStationData
    {
        private readonly SpaceStationConfiguration _config = new();

        public string GetAdventureHelpText()
        {
            return _config.GetAdventureHelpText();
        }

        public string GetAdventureThankYouText()
        {
            return _config.GetAdventureThankYouText();
        }

        public SpaceStationConfiguration GetGameConfiguration()
        {
            return _config;
        }

        public PlayAdventureModel SetupAdventure(string gameid)
        {
            var _gamerTag = new GamerTags().RandomTag();

            var _play = new PlayAdventureModel
            {
                GameID = 2,
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
                    Steps = 3, 
                    Verbose = true, 
                    Points = _config.StartingPoints, 
                    PlayerDead = false 
                },
                GameActive = true,
                PointsCheckList = _config.InitialPointsCheckList
            };

            return _play;
        }

        private static List<Room> Rooms()
        {
            var _rooms = new List<Room>
            {
                // Exit room
                new Room { Number = 0, RoomPoints = 1000, Name = "Escape Pod - Freedom!", 
                    Desc = "?? SUCCESS! You are safely aboard the escape pod, drifting toward rescue. The abandoned space station grows smaller behind you.", 
                    N = 99, S = 99, E = 99, W = 99, U = 99, D = 99 },

                // LEVEL 1 - COMMAND DECK (Rooms 1-6)
                new Room { Number = 1, RoomPoints = 25, Name = "Hibernation Chamber", 
                    Desc = "You awaken in the hibernation chamber. Emergency lighting casts an eerie red glow. The other hibernation pods are empty and powered down. Something is very wrong.", 
                    N = 99, S = 99, E = 2, W = 99, U = 99, D = 99 },

                new Room { Number = 2, RoomPoints = 50, Name = "Command Center", 
                    Desc = "The station's command center is eerily quiet. Multiple screens show system alerts in flashing red. The captain's chair sits empty, and communication systems appear to be offline.", 
                    N = 99, S = 5, E = 3, W = 1, U = 99, D = 99 },

                new Room { Number = 3, RoomPoints = 25, Name = "Captain's Office", 
                    Desc = "The captain's personal office contains a desk covered in scattered reports. A secure door leads south, and another door leads west to what appears to be a specialized room.", 
                    N = 99, S = 4, E = 99, W = 2, U = 99, D = 5 },

                new Room { Number = 4, RoomPoints = 75, Name = "Secure Office", 
                    Desc = "A high-security office with reinforced walls. The door requires special authorization to unlock. Inside are classified documents and a secure storage compartment.", 
                    N = 3, S = 99, E = 99, W = 99, U = 99, D = 99 },

                new Room { Number = 5, RoomPoints = 100, Name = "Robot Room", 
                    Desc = "A specialized maintenance room housing the station's service robot. The robot sits dormant in its charging station, awaiting activation commands.", 
                    N = 2, S = 6, E = 99, W = 99, U = 3, D = 99 },

                new Room { Number = 6, RoomPoints = 10, Name = "Command Lift", 
                    Desc = "The primary lift connecting the command deck to other levels. Control panels show access to Engineering (Level 4). The lift hums with emergency power.", 
                    N = 5, S = 99, E = 99, W = 99, U = 99, D = 36 },

                // LEVEL 2 - CREW QUARTERS (Rooms 7-16)
                new Room { Number = 7, RoomPoints = 25, Name = "Crew Corridor", 
                    Desc = "The main corridor of the crew quarters level. Personal belongings are scattered about, suggesting a hasty evacuation. Emergency lighting flickers occasionally.", 
                    N = 99, S = 99, E = 9, W = 8, U = 99, D = 99 },

                new Room { Number = 8, RoomPoints = 30, Name = "Crew Mess Hall", 
                    Desc = "The crew dining area with tables still set for a meal that was never finished. Half-eaten food has spoiled, and there's an unpleasant odor in the air.", 
                    N = 99, S = 15, E = 7, W = 99, U = 99, D = 99 },

                new Room { Number = 9, RoomPoints = 50, Name = "Captain's Quarters", 
                    Desc = "The captain's private living quarters. The room is well-appointed but shows signs of a hurried departure. Personal items are strewn about.", 
                    N = 99, S = 10, E = 99, W = 7, U = 99, D = 99 },

                new Room { Number = 10, RoomPoints = 100, Name = "Captain's Shower", 
                    Desc = "The captain's private bathroom facilities. Water still drips from the shower head. A small storage compartment is built into the wall.", 
                    N = 9, S = 99, E = 99, W = 99, U = 99, D = 99 },

                new Room { Number = 11, RoomPoints = 20, Name = "Crew Quarters A", 
                    Desc = "A standard crew sleeping quarters with bunk beds. Personal effects suggest this room housed two crew members who left in a hurry.", 
                    N = 99, S = 99, E = 12, W = 99, U = 99, D = 99 },

                new Room { Number = 12, RoomPoints = 20, Name = "Crew Quarters B", 
                    Desc = "Another crew sleeping area. A datapad lies open on one of the beds, displaying what appears to be a personal journal entry.", 
                    N = 99, S = 99, E = 13, W = 11, U = 99, D = 99 },

                new Room { Number = 13, RoomPoints = 20, Name = "Crew Quarters C", 
                    Desc = "The third crew quarters. Unlike the others, this room appears to have been sealed hastily. Warning tape blocks the entrance to an adjoining compartment.", 
                    N = 99, S = 99, E = 14, W = 12, U = 99, D = 99 },

                new Room { Number = 14, RoomPoints = 30, Name = "Crew Lounge", 
                    Desc = "A recreational area with entertainment systems and comfortable seating. A half-finished game of cards sits on the table, drinks still in their glasses.", 
                    N = 99, S = 99, E = 99, W = 13, U = 99, D = 99 },

                new Room { Number = 15, RoomPoints = 40, Name = "Crew Medical Bay", 
                    Desc = "The crew's medical facility with basic treatment equipment. Medical supplies are scattered, and one of the examination tables shows signs of recent use.", 
                    N = 8, S = 99, E = 16, W = 99, U = 99, D = 99 },

                new Room { Number = 16, RoomPoints = 10, Name = "Crew Lift", 
                    Desc = "A secondary lift connecting crew quarters to other levels. The control panel indicates access to Science Labs (Level 3) and Engineering (Level 4).", 
                    N = 99, S = 99, E = 99, W = 15, U = 23, D = 36 },

                // LEVEL 3 - SCIENCE LABS (Rooms 17-26)
                new Room { Number = 17, RoomPoints = 25, Name = "Science Corridor", 
                    Desc = "The main corridor of the science level. Laboratory equipment and specimen containers line the walls. Some containers appear to be empty or broken.", 
                    N = 99, S = 20, E = 22, W = 18, U = 99, D = 99 },

                new Room { Number = 18, RoomPoints = 40, Name = "Hydroponics Lab", 
                    Desc = "The station's food production facility. Plant growth chambers line the walls, but many plants appear withered or strangely mutated.", 
                    N = 99, S = 24, E = 17, W = 99, U = 99, D = 99 },

                new Room { Number = 19, RoomPoints = 30, Name = "Research Lab", 
                    Desc = "A general research laboratory with various scientific instruments. Experiment notes are scattered about, many marked with urgent warnings.", 
                    N = 20, S = 99, E = 99, W = 99, U = 99, D = 99 },

                new Room { Number = 20, RoomPoints = 50, Name = "Science Computer Core", 
                    Desc = "The central computer system for the science level. Multiple terminals are still active, displaying research data and alarming biological readings.", 
                    N = 17, S = 23, E = 21, W = 19, U = 99, D = 99 },

                new Room { Number = 21, RoomPoints = 60, Name = "Observatory", 
                    Desc = "A stellar observation deck with a magnificent view of space. Star charts and navigation data cover the workstations. Earth is visible in the distance.", 
                    N = 99, S = 25, E = 99, W = 20, U = 99, D = 99 },

                new Room { Number = 22, RoomPoints = 75, Name = "Experiment Chamber", 
                    Desc = "A sealed experimental chamber with reinforced viewing windows. Strange organic residue coats the interior surfaces. Warning signs are posted everywhere.", 
                    N = 99, S = 26, E = 99, W = 17, U = 99, D = 99 },

                new Room { Number = 23, RoomPoints = 200, Name = "Teleport Room", 
                    Desc = "An advanced teleportation facility. The control panel glows with available destinations. A direct beam to the Command Level is available for emergency use.", 
                    N = 20, S = 99, E = 99, W = 99, U = 99, D = 16 },

                new Room { Number = 24, RoomPoints = 35, Name = "Containment Lab", 
                    Desc = "A high-security biological containment facility. Several containment units are open and empty. Biohazard warnings flash on multiple displays.", 
                    N = 18, S = 99, E = 99, W = 99, U = 99, D = 99 },

                new Room { Number = 25, RoomPoints = 40, Name = "Analysis Lab", 
                    Desc = "An analytical laboratory with advanced scanning equipment. Test samples are still being processed, showing unusual biological activity.", 
                    N = 21, S = 99, E = 26, W = 99, U = 99, D = 99 },

                new Room { Number = 26, RoomPoints = 25, Name = "Science Storage", 
                    Desc = "A storage area for scientific equipment and specimens. Many containers are labeled with cryptic codes. Some storage units appear to have been forced open.", 
                    N = 22, S = 99, E = 99, W = 25, U = 99, D = 99 },

                // LEVEL 4 - ENGINEERING (Rooms 27-40)
                new Room { Number = 27, RoomPoints = 25, Name = "Engineering Corridor", 
                    Desc = "The main engineering corridor vibrates with the hum of machinery. Pipes and conduits run along the ceiling, some showing signs of damage or hasty repairs.", 
                    N = 99, S = 34, E = 30, W = 28, U = 99, D = 99 },

                new Room { Number = 28, RoomPoints = 50, Name = "Main Engineering", 
                    Desc = "The heart of the station's engineering operations. Control consoles monitor power distribution and life support systems. Many systems show warning status.", 
                    N = 99, S = 33, E = 27, W = 29, U = 99, D = 99 },

                new Room { Number = 29, RoomPoints = 75, Name = "Power Core", 
                    Desc = "The station's main power generation facility. The fusion reactor hums ominously behind protective barriers. Power output appears unstable.", 
                    N = 99, S = 32, E = 28, W = 99, U = 99, D = 99 },

                new Room { Number = 30, RoomPoints = 40, Name = "Engine Room", 
                    Desc = "The propulsion control center. The engines are currently offline, leaving the station in a decaying orbit. Thruster controls are locked down.", 
                    N = 99, S = 35, E = 31, W = 27, U = 99, D = 99 },

                new Room { Number = 31, RoomPoints = 30, Name = "Maintenance Bay", 
                    Desc = "A general maintenance area filled with tools and spare parts. Work orders are posted showing various urgent repairs that were never completed.", 
                    N = 99, S = 36, E = 99, W = 30, U = 99, D = 99 },

                new Room { Number = 32, RoomPoints = 25, Name = "Tool Storage", 
                    Desc = "Storage for engineering tools and equipment. Tool racks are partially empty, suggesting crew members grabbed emergency equipment during evacuation.", 
                    N = 29, S = 37, E = 33, W = 99, U = 99, D = 99 },

                new Room { Number = 33, RoomPoints = 30, Name = "Spare Parts Room", 
                    Desc = "A storage area for replacement components. Inventory lists show missing critical parts. Some storage areas appear to have been searched frantically.", 
                    N = 28, S = 38, E = 34, W = 32, U = 99, D = 99 },

                new Room { Number = 34, RoomPoints = 35, Name = "Waste Processing", 
                    Desc = "The station's waste management facility. The air smells foul, and some processing units appear to be backed up or malfunctioning.", 
                    N = 27, S = 39, E = 35, W = 33, U = 99, D = 99 },

                new Room { Number = 35, RoomPoints = 500, Name = "Escape Pod Bay", 
                    Desc = "The emergency evacuation facility. A single escape pod remains in its launch tube, but the access panel shows it requires special authorization to activate.", 
                    N = 30, S = 40, E = 36, W = 34, U = 99, D = 99 },

                new Room { Number = 36, RoomPoints = 10, Name = "Eng Lift", 
                    Desc = "The engineering level lift providing access to all station levels. This is the primary vertical transportation hub for the entire station.", 
                    N = 31, S = 99, E = 99, W = 35, U = 6, D = 99 },

                new Room { Number = 37, RoomPoints = 30, Name = "Auxiliary Engine", 
                    Desc = "Backup propulsion systems for emergency maneuvering. These systems appear to have been activated recently, suggesting someone tried to change the station's course.", 
                    N = 32, S = 99, E = 38, W = 99, U = 99, D = 99 },

                new Room { Number = 38, RoomPoints = 25, Name = "Cooling Systems", 
                    Desc = "Environmental control systems that regulate station temperature. Coolant pipes show signs of stress, and temperature warnings flash on monitoring displays.", 
                    N = 33, S = 99, E = 39, W = 37, U = 99, D = 99 },

                new Room { Number = 39, RoomPoints = 40, Name = "Reactor Room", 
                    Desc = "Secondary reactor control systems. Radiation warning signs are prominent, and a Geiger counter clicks ominously. Access requires protective equipment.", 
                    N = 34, S = 99, E = 40, W = 38, U = 99, D = 99 },

                new Room { Number = 40, RoomPoints = 20, Name = "Airlock Chamber", 
                    Desc = "An emergency airlock leading to the outer hull. The chamber shows signs of recent use, and space suits hang on the wall, one missing.", 
                    N = 35, S = 99, E = 99, W = 39, U = 99, D = 99 },

#if DEBUG
                new Room { Number = 88, RoomPoints = 0, Name = "Debug Station", 
                    Desc = "Developer's debug space station. All levels accessible from here.", 
                    N = 1, S = 16, E = 23, W = 36, U = 0, D = 35 },
#endif
            };

            return _rooms;
        }

        private static List<Item> Items()
        {
            var _items = new List<Item>
            {
                // Food items for health
                new Item { 
                    Name = "RATION", 
                    Description = "A military-grade emergency food ration. Designed for long-term storage and maximum nutrition.", 
                    Location = 1, 
                    Action = "The ration is surprisingly tasty and very nutritious. You feel much better.", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "100", 
                    ActionPoints = 15 
                },

                new Item { 
                    Name = "VITAMINS", 
                    Description = "A bottle of essential vitamins for space travelers. The label warns against exceeding recommended dosage.", 
                    Location = 15, 
                    Action = "The vitamins provide a moderate boost to your health and energy levels.", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "50", 
                    ActionPoints = 10 
                },

                new Item { 
                    Name = "ENERGYBAR", 
                    Description = "A high-energy protein bar designed for emergency situations. It looks dense and nutritious.", 
                    Location = 8, 
                    Action = "The energy bar is chewy but satisfying. You feel your strength returning.", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "75", 
                    ActionPoints = 12 
                },

                // Key story items
                new Item { 
                    Name = "FOB", 
                    Description = "The captain's security fob with his personal identification code. Essential for accessing secure areas.", 
                    Location = 10, 
                    Action = "You scan the captain's fob and feel a sense of progress toward escape.", 
                    ActionVerb = "USE", 
                    ActionResult = "HEALTH", 
                    ActionValue = "10", 
                    ActionPoints = 50 
                },

                new Item { 
                    Name = "CARD", 
                    Description = "An escape pod authorization card from the secure office. This grants access to emergency evacuation procedures.", 
                    Location = 4, 
                    Action = "The authorization card unlocks the escape pod bay. Freedom awaits!", 
                    ActionVerb = "USE", 
                    ActionResult = "UNLOCK", 
                    ActionValue = "35|N|0|The escape pod is now accessible! Board it to escape!|Escape pod activated successfully!", 
                    ActionPoints = 1000 
                },

                // Robot activation
                new Item { 
                    Name = "ROBOT", 
                    Description = "A sophisticated service robot designed to assist with station operations. It appears to be in standby mode.", 
                    Location = 5, 
                    Action = "The robot's systems come online with a cheerful beep. It will now follow you and provide assistance.", 
                    ActionVerb = "ACTIVATE", 
                    ActionResult = "FOLLOW", 
                    ActionValue = "5", 
                    ActionPoints = 100 
                },

                // Weapons for monsters
                new Item { 
                    Name = "BLASTER", 
                    Description = "A standard-issue plasma blaster for dealing with hostile life forms. The power cell shows full charge.", 
                    Location = 32, 
                    Action = "You fire the plasma blaster with a satisfying buzz. The energy beam cuts through the air.", 
                    ActionVerb = "ATTACK", 
                    ActionResult = "WEAPON", 
                    ActionValue = "GOOPLING", 
                    ActionPoints = 25 
                },

                // Teleport items
                new Item { 
                    Name = "TELEPORTER", 
                    Description = "A personal teleportation device. The display shows it can beam you to the Command Center.", 
                    Location = 23, 
                    Action = "The teleporter activates with a bright flash. You materialize instantly in the Command Center!", 
                    ActionVerb = "USE", 
                    ActionResult = "TELEPORT", 
                    ActionValue = "2", 
                    ActionPoints = 50 
                },

                // Story items and logs
                new Item { 
                    Name = "LOG", 
                    Description = "The commander's personal log containing crucial information about the station's abandonment.", 
                    Location = 20, 
                    Action = "COMMANDER'S LOG: The engineer, after being demoted to cook, used the antimatter accelerator to bake a birthday cake. The reaction created the goopling creatures that overran the station.", 
                    ActionVerb = "READ", 
                    ActionResult = "HEALTH", 
                    ActionValue = "5", 
                    ActionPoints = 200 
                },

                new Item { 
                    Name = "JOURNAL", 
                    Description = "A crew member's personal journal documenting the final days aboard the station.", 
                    Location = 12, 
                    Action = "Day 15: Strange creatures appearing in the science labs. Day 16: Evacuation ordered. Day 17: I'm hiding... they're everywhere...", 
                    ActionVerb = "READ", 
                    ActionResult = "HEALTH", 
                    ActionValue = "5", 
                    ActionPoints = 75 
                },

                // Utility items
                new Item { 
                    Name = "FLASHLIGHT", 
                    Description = "A high-powered LED flashlight. Essential for exploring dark areas of the station.", 
                    Location = 31, 
                    Action = "The flashlight illuminates dark corners, revealing hidden details in the shadows.", 
                    ActionVerb = "USE", 
                    ActionResult = "HEALTH", 
                    ActionValue = "5", 
                    ActionPoints = 20 
                },

                new Item { 
                    Name = "KEYCARD", 
                    Description = "A general access keycard for standard ship functions. Many doors require this for entry.", 
                    Location = 3, 
                    Action = "The keycard opens the secure office door with a satisfying beep.", 
                    ActionVerb = "USE", 
                    ActionResult = "UNLOCK", 
                    ActionValue = "3|S|4|The secure office door is now open.|Security door unlocked!", 
                    ActionPoints = 100 
                },

                new Item { 
                    Name = "SAMPLE", 
                    Description = "A sealed sample container labeled 'GOOPLING SPECIMEN - EXTREMELY DANGEROUS'. The contents seem to be moving.", 
                    Location = 22, 
                    Action = "You carefully examine the sample. The creature inside writhes menacingly but appears contained.", 
                    ActionVerb = "USE", 
                    ActionResult = "HEALTH", 
                    ActionValue = "-10", 
                    ActionPoints = 15 
                },

                new Item { 
                    Name = "MEDIKIT", 
                    Description = "An advanced medical kit containing various healing supplies and emergency medications.", 
                    Location = 15, 
                    Action = "The medical kit provides excellent treatment for your injuries. You feel significantly better.", 
                    ActionVerb = "USE", 
                    ActionResult = "HEALTH", 
                    ActionValue = "150", 
                    ActionPoints = 30 
                },

                new Item { 
                    Name = "TOOLKIT", 
                    Description = "A comprehensive engineering toolkit with various instruments for repairs and maintenance.", 
                    Location = 32, 
                    Action = "You use the toolkit to make minor repairs and improvements. Very satisfying work.", 
                    ActionVerb = "USE", 
                    ActionResult = "HEALTH", 
                    ActionValue = "25", 
                    ActionPoints = 20 
                },

#if DEBUG
                // Debug items
                new Item { 
                    Name = "DEBUGBEAM", 
                    Description = "Developer's teleportation device for testing.", 
                    Location = 88, 
                    Action = "The debug beam teleports you instantly.", 
                    ActionVerb = "USE", 
                    ActionResult = "TELEPORT", 
                    ActionValue = "1", 
                    ActionPoints = 0 
                },

                new Item { 
                    Name = "SUPERRATION", 
                    Description = "Developer's super nutrition pack.", 
                    Location = 88, 
                    Action = "The super ration completely restores your health.", 
                    ActionVerb = "EAT", 
                    ActionResult = "HEALTH", 
                    ActionValue = "1000", 
                    ActionPoints = 0 
                },

                new Item { 
                    Name = "DEBUGBLASTER", 
                    Description = "Developer's super weapon.", 
                    Location = 88, 
                    Action = "The debug blaster destroys any goopling instantly.", 
                    ActionVerb = "ATTACK", 
                    ActionResult = "WEAPON", 
                    ActionValue = "GOOPLING", 
                    ActionPoints = 0 
                },
#endif
            };

            return _items;
        }

        private static List<Message> Messages()
        {
            var _messages = new List<Message>
            {
                // Direction blocking messages
                new Message { MessageTag = "North", Messsage = "The corridor north is blocked by debris." },
                new Message { MessageTag = "North", Messsage = "A sealed bulkhead prevents passage north." },
                new Message { MessageTag = "North", Messsage = "Emergency barriers block the path north." },

                new Message { MessageTag = "South", Messsage = "The southern passage is sealed." },
                new Message { MessageTag = "South", Messsage = "Damaged sections prevent movement south." },
                new Message { MessageTag = "South", Messsage = "Warning lights block the south corridor." },

                new Message { MessageTag = "East", Messsage = "The eastern path is inaccessible." },
                new Message { MessageTag = "East", Messsage = "Emergency lockdown prevents eastward movement." },
                new Message { MessageTag = "East", Messsage = "Structural damage blocks the way east." },

                new Message { MessageTag = "West", Messsage = "The western corridor is blocked." },
                new Message { MessageTag = "West", Messsage = "You cannot proceed west due to safety barriers." },
                new Message { MessageTag = "West", Messsage = "Emergency protocols prevent westward passage." },

                new Message { MessageTag = "Up", Messsage = "The lift shaft above is damaged." },
                new Message { MessageTag = "Up", Messsage = "No access upward from this location." },
                new Message { MessageTag = "Up", Messsage = "The ceiling prevents upward movement." },

                new Message { MessageTag = "Down", Messsage = "The floor is solid - no downward access." },
                new Message { MessageTag = "Down", Messsage = "No downward passage available here." },
                new Message { MessageTag = "Down", Messsage = "The deck plating blocks downward movement." },

                // Action result messages
                new Message { MessageTag = "GetSuccess", Messsage = "You carefully store the @ in your equipment pack." },
                new Message { MessageTag = "GetSuccess", Messsage = "The @ fits securely in your survival gear." },
                new Message { MessageTag = "GetSuccess", Messsage = "You acquire the @ for your mission." },

                new Message { MessageTag = "GetFailed", Messsage = "The @ is not available here." },
                new Message { MessageTag = "GetFailed", Messsage = "You cannot find a @ in this location." },
                new Message { MessageTag = "GetFailed", Messsage = "The @ appears to be missing." },

                new Message { MessageTag = "DropSuccess", Messsage = "You place the @ carefully on the deck." },
                new Message { MessageTag = "DropSuccess", Messsage = "The @ is now secured in this location." },

                new Message { MessageTag = "DropFailed", Messsage = "You don't have a @ to drop." },
                new Message { MessageTag = "DropFailed", Messsage = "The @ is not in your inventory." },

                new Message { MessageTag = "UseFailed", Messsage = "The @ cannot be used in this situation." },
                new Message { MessageTag = "UseFailed", Messsage = "You're not sure how to use the @ here." },
                new Message { MessageTag = "UseFailed", Messsage = "The @ doesn't seem to work properly." },

                new Message { MessageTag = "EatSuccessBig", Messsage = "The @ is incredibly nourishing and restores significant health." },
                new Message { MessageTag = "EatSuccessMedium", Messsage = "The @ provides good nutrition and helps your recovery." },
                new Message { MessageTag = "EatSuccessSmall", Messsage = "The @ offers modest nourishment." },

                new Message { MessageTag = "EatFailed", Messsage = "The @ is not edible." },
                new Message { MessageTag = "EatFailed", Messsage = "You cannot consume the @." },
                new Message { MessageTag = "EatFailed", Messsage = "The @ is not suitable for consumption." },

                new Message { MessageTag = "LookEmpty", Messsage = "You scan the area but see nothing of particular interest." },
                new Message { MessageTag = "LookEmpty", Messsage = "The area appears clear of useful items." },

                new Message { MessageTag = "LookFailed", Messsage = "You don't see a @ here." },
                new Message { MessageTag = "LookFailed", Messsage = "There's no @ visible in this location." },

                // Essential status messages
                new Message { MessageTag = "Dead", Messsage = "Your life support has failed. Mission terminated." },
                new Message { MessageTag = "Dead", Messsage = "Critical systems failure. You have perished aboard the station." },
                new Message { MessageTag = "Dead", Messsage = "Your health has reached zero. The station claims another victim." },

                new Message { MessageTag = "Bad", Messsage = "Warning: Your health is critically low. Seek medical attention immediately." },
                new Message { MessageTag = "Bad", Messsage = "Life support systems are failing. You need nutrition soon." },
                new Message { MessageTag = "Bad", Messsage = "Emergency: Your condition is deteriorating rapidly." },

                // Robot companion messages
                new Message { MessageTag = "PetFollow", Messsage = "The robot hums quietly as it follows your commands." },
                new Message { MessageTag = "PetFollow", Messsage = "Your robotic companion scans the environment efficiently." },
                new Message { MessageTag = "PetFollow", Messsage = "The service robot maintains perfect formation behind you." },
                new Message { MessageTag = "PetFollow", Messsage = "Beep boop! The robot reports all systems nominal." },
                new Message { MessageTag = "PetFollow", Messsage = "The robot's optical sensors track your movements carefully." },
                new Message { MessageTag = "PetFollow", Messsage = "Your mechanical aide processes environmental data silently." },

                new Message { MessageTag = "PetSuccess", Messsage = "The robot activates and begins following you faithfully." },
                new Message { MessageTag = "PetSuccess", Messsage = "Your new robotic companion is now operational." },

                new Message { MessageTag = "ShooSuccess", Messsage = "The robot deactivates and returns to its charging station." },
                new Message { MessageTag = "ShooSuccess", Messsage = "Your robotic companion powers down and remains here." },

                // Monster encounter messages
                new Message { MessageTag = "MonsterAppear", Messsage = "A hideous @ emerges from the shadows!" },
                new Message { MessageTag = "MonsterAppear", Messsage = "Warning! A hostile @ blocks your path!" },
                new Message { MessageTag = "MonsterAppear", Messsage = "Danger! A @ materializes before you!" },

                new Message { MessageTag = "MonsterAttack", Messsage = "The @ attacks with vicious intent!" },
                new Message { MessageTag = "MonsterAttack", Messsage = "The @ strikes out aggressively!" },

                new Message { MessageTag = "MonsterHit", Messsage = "The @ injures you with its attack!" },
                new Message { MessageTag = "MonsterHit", Messsage = "You take damage from the @!" },

                new Message { MessageTag = "MonsterMiss", Messsage = "The @ misses its attack!" },
                new Message { MessageTag = "MonsterMiss", Messsage = "You dodge the @'s strike!" },

                new Message { MessageTag = "MonsterDefeated", Messsage = "The @ is destroyed! You are victorious!" },
                new Message { MessageTag = "MonsterDefeated", Messsage = "You defeat the @! It dissolves into goo." },

                new Message { MessageTag = "AttackSuccess", Messsage = "Your @ proves effective against the @!" },
                new Message { MessageTag = "AttackSuccess", Messsage = "You successfully attack the @ with your @!" },

                new Message { MessageTag = "AttackFailed", Messsage = "You need the right weapon to defeat the @." },
                new Message { MessageTag = "AttackFailed", Messsage = "Your attack against the @ is ineffective." },

                new Message { MessageTag = "NoMonster", Messsage = "There's nothing hostile here to attack." },
                new Message { MessageTag = "NoMonster", Messsage = "No threats detected in this area." },

                // Robot combat assistance
                new Message { MessageTag = "PetAttackSuccess", Messsage = "Your robot companion assists in the battle!" },
                new Message { MessageTag = "PetAttackSuccess", Messsage = "The service robot provides tactical support!" },
                new Message { MessageTag = "PetAttackSuccess", Messsage = "Your mechanical ally joins the fight!" },

                new Message { MessageTag = "PetAttackFailed", Messsage = "The robot's combat protocols are limited." },
                new Message { MessageTag = "PetAttackFailed", Messsage = "Your robotic companion lacks offensive capabilities." },

                new Message { MessageTag = "PetDefeated", Messsage = "With the robot's help, you emerge victorious!" },
                new Message { MessageTag = "PetDefeated", Messsage = "Your robotic ally proves invaluable in combat!" },

                // General failure messages
                new Message { MessageTag = "Any", Messsage = "That action is not possible in this situation." },
                new Message { MessageTag = "Any", Messsage = "The station's systems do not respond to that command." },
                new Message { MessageTag = "Any", Messsage = "Emergency protocols prevent that action." },
                new Message { MessageTag = "Any", Messsage = "Your survival instincts tell you that's not wise." },

                // Death prevention
                new Message { MessageTag = "DeadMove", Messsage = "You cannot move while your life support has failed." },
                new Message { MessageTag = "DeadMove", Messsage = "Mission terminated. No further actions possible." },
            };

            return _messages;
        }

        private static List<Monster> Monsters()
        {
            var _monsters = new List<Monster>
            {
                // Cakebite - appears in Hydroponics Lab
                new Monster 
                { 
                    Key = "CAKEBITE", 
                    Name = "Cakebite", 
                    Description = "A grotesque creature that appears to be made of animated cake batter mixed with biological matter. It oozes frosting and snaps with cake-cutter teeth.",
                    RoomNumber = 18, // Hydroponics Lab
                    ObjectNameThatCanAttackThem = "BLASTER",
                    AttacksToKill = 2,
                    CurrentHealth = 2,
                    CanHitPlayer = true,
                    HitOdds = 35,
                    HealthDamage = 20,
                    AppearanceChance = 70,
                    IsPresent = false,
                    PetAttackChance = 40
                },

                // Frostling - appears in Experiment Chamber
                new Monster 
                { 
                    Key = "FROSTLING", 
                    Name = "Frostling", 
                    Description = "A crystalline creature born from frozen cake ingredients exposed to antimatter. It radiates cold and moves with jerky, unnatural motions.",
                    RoomNumber = 22, // Experiment Chamber
                    ObjectNameThatCanAttackThem = "BLASTER",
                    AttacksToKill = 2,
                    CurrentHealth = 2,
                    CanHitPlayer = true,
                    HitOdds = 40,
                    HealthDamage = 25,
                    AppearanceChance = 75,
                    IsPresent = false,
                    PetAttackChance = 35
                },

                // Goopling - appears in Containment Lab
                new Monster 
                { 
                    Key = "GOOPLING", 
                    Name = "Goopling", 
                    Description = "A writhing mass of organic goo that resulted from the antimatter cake experiment. It pulsates with malevolent energy and seems hungry for living tissue.",
                    RoomNumber = 24, // Containment Lab
                    ObjectNameThatCanAttackThem = "BLASTER",
                    AttacksToKill = 3,
                    CurrentHealth = 3,
                    CanHitPlayer = true,
                    HitOdds = 45,
                    HealthDamage = 30,
                    AppearanceChance = 80,
                    IsPresent = false,
                    PetAttackChance = 30
                },

                // Sweetspore - appears in Analysis Lab
                new Monster 
                { 
                    Key = "SWEETSPORE", 
                    Name = "Sweetspore", 
                    Description = "A fungal creature that grew from spilled cake mix contaminated with alien spores. It releases toxic clouds of sugary particles when threatened.",
                    RoomNumber = 25, // Analysis Lab
                    ObjectNameThatCanAttackThem = "BLASTER",
                    AttacksToKill = 2,
                    CurrentHealth = 2,
                    CanHitPlayer = true,
                    HitOdds = 30,
                    HealthDamage = 15,
                    AppearanceChance = 65,
                    IsPresent = false,
                    PetAttackChance = 45
                },

                // Batterling - appears in Waste Processing
                new Monster 
                { 
                    Key = "BATTERLING", 
                    Name = "Batterling", 
                    Description = "A disgusting creature formed from cake batter mixed with waste processing systems. It smells terrible and moves with disturbing squelching sounds.",
                    RoomNumber = 34, // Waste Processing
                    ObjectNameThatCanAttackThem = "BLASTER",
                    AttacksToKill = 2,
                    CurrentHealth = 2,
                    CanHitPlayer = true,
                    HitOdds = 25,
                    HealthDamage = 18,
                    AppearanceChance = 60,
                    IsPresent = false,
                    PetAttackChance = 50
                }
            };

            return _monsters;
        }
    }
}