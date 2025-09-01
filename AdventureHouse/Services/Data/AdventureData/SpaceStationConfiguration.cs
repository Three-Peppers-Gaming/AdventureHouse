using System;
using System.Collections.Generic;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.AdventureClient.Models;



namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Space Station adventure configuration - A thrilling escape from an abandoned space station
    /// </summary>
    public class SpaceStationConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Abandoned Space Station: Project Escape";
        public string GameVersion => "1.0";
        public string GameDescription => "Escape from an abandoned space station overrun by mysterious creatures.";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomCharacterMapping => new()
        {
            ["hibernation chamber"] = 'H',
            ["command center"] = 'C',
            ["captain's office"] = 'O',
            ["secure office"] = 'S',
            ["robot room"] = 'R',
            ["captain's quarters"] = 'Q',
            ["captain's shower"] = 'W',
            ["crew mess hall"] = 'M',
            ["crew quarters"] = 'B',
            ["crew lounge"] = 'L',
            ["crew medical bay"] = 'D',
            ["hydroponics lab"] = 'P',
            ["research lab"] = 'L',
            ["science computer core"] = 'C',
            ["observatory"] = 'V',
            ["experiment chamber"] = 'E',
            ["teleport room"] = 'T',
            ["main engineering"] = 'E',
            ["power core"] = 'P',
            ["engine room"] = 'N',
            ["maintenance bay"] = 'A',
            ["tool storage"] = 'T',
            ["spare parts room"] = 'S',
            ["waste processing"] = 'W',
            ["escape pod bay"] = 'X',
            ["command lift"] = '|',
            ["crew lift"] = '|',
            ["eng lift"] = '|',
            ["science lift"] = '|'
        };

        public Dictionary<string, char> RoomDisplayCharacters => RoomCharacterMapping;

        public Dictionary<string, char> RoomTypeCharacters => new()
        {
            ["corridor"] = 'H',
            ["hallway"] = 'H',
            ["lift"] = '|',
            ["chamber"] = 'C',
            ["quarters"] = 'Q',
            ["bay"] = 'B',
            ["lab"] = 'L',
            ["storage"] = 'S',
            ["core"] = 'C',
            ["room"] = 'R',
            ["medical"] = '+',
            ["engine"] = 'E',
            ["waste"] = 'W',
            ["pod"] = 'P',
#if DEBUG
            ["debug"] = '?'
#endif
        };

        public Dictionary<string, int> RoomNameToNumberMapping => new()
        {
            // Level 1 - Command
            ["hibernation chamber"] = 1,
            ["command center"] = 2,
            ["captain's office"] = 3,
            ["secure office"] = 4,
            ["robot room"] = 5,
            ["command lift"] = 6,

            // Level 2 - Crew  
            ["crew corridor"] = 7,
            ["crew mess hall"] = 8,
            ["captain's quarters"] = 9,
            ["captain's shower"] = 10,
            ["crew quarters a"] = 11,
            ["crew quarters b"] = 12,
            ["crew quarters c"] = 13,
            ["crew lounge"] = 14,
            ["crew medical bay"] = 15,
            ["crew lift"] = 16,

            // Level 3 - Science
            ["science corridor"] = 17,
            ["hydroponics lab"] = 18,
            ["research lab"] = 19,
            ["science computer core"] = 20,
            ["observatory"] = 21,
            ["experiment chamber"] = 22,
            ["teleport room"] = 23,
            ["containment lab"] = 24,
            ["analysis lab"] = 25,
            ["science storage"] = 26,

            // Level 4 - Engineering
            ["engineering corridor"] = 27,
            ["main engineering"] = 28,
            ["power core"] = 29,
            ["engine room"] = 30,
            ["maintenance bay"] = 31,
            ["tool storage"] = 32,
            ["spare parts room"] = 33,
            ["waste processing"] = 34,
            ["escape pod bay"] = 35,
            ["eng lift"] = 36,
            ["auxiliary engine"] = 37,
            ["cooling systems"] = 38,
            ["reactor room"] = 39,
            ["airlock chamber"] = 40,

#if DEBUG
            ["debug station"] = 88
#endif
        };
        #endregion

        #region Map Legend and Help Text
        public string MapLegend => MapLegendContent + MapLegendFooter;
        
        public string MapLegendContent => @"
SPACE STATION MAP LEGEND:
+---+  = Room          @ = Your Location    |H| = Hibernation
|C  |  = Command       |O| = Captain Office |S| = Secure Room 
|Q+ |  = Quarters      |M| = Mess Hall      |L| = Lab/Lounge
|E  |  = Engineering   |P| = Power/Pod      |T| = Teleport
|R  |  = Robot Room    |D| = Medical Bay    |V| = Observatory
||  |  = Lift          |A| = Maintenance    |N| = Engine Room
|W  |  = Waste/Shower  |.| = Other Room     |X| = Escape Pod

LEVELS:
Level 1: Command Deck    Level 2: Crew Quarters
Level 3: Science Labs    Level 4: Engineering

PATH CONNECTIONS:
. . .  = Corridor (East/West)    :     = Vertical passage  
^      = Lift going Up           v     = Lift going Down
";

#if DEBUG
        public string MapLegendDebugAddition => @"|?  |  = Debug Station
";
#endif

        public string MapLegendFooter => @"
Note: Only visited areas and connections are shown.
Emergency systems may reveal additional pathways.
";

        public string GetCompleteMapLegend()
        {
            var legend = MapLegendContent;
#if DEBUG
            legend += MapLegendDebugAddition;
#endif
            legend += MapLegendFooter;
            return legend;
        }
        #endregion

        #region Game Help and Story Text
        public string GetAdventureHelpText()
        {
            return "EMERGENCY PROTOCOLS ACTIVATED\r\n\r\n" +
                   "You have awakened from hibernation aboard the space station to find it " +
                   "mysteriously abandoned. The emergency lighting casts eerie shadows through " +
                   "the corridors, and strange sounds echo from the ventilation systems.\r\n\r\n" +
                   "Your survival depends on finding the ESCAPE POD, but it requires proper " +
                   "authorization. You'll need to explore the station's four levels:\r\n" +
                   "• COMMAND DECK (Level 1) - Administrative and control systems\r\n" +
                   "• CREW QUARTERS (Level 2) - Living spaces and personal areas\r\n" +
                   "• SCIENCE LABS (Level 3) - Research facilities and experiments\r\n" +
                   "• ENGINEERING (Level 4) - Power systems and the escape pod\r\n\r\n" +
                   "Navigation Commands: NORTH, SOUTH, EAST, WEST, UP, DOWN\r\n" +
                   "Actions: LOOK, GET, USE, EAT, ATTACK, ACTIVATE, READ\r\n" +
                   "Inventory: INV shows your items, DROP releases items\r\n" +
                   "Special: The station's robot can be activated to assist you!\r\n\r\n" +
                   "WARNING: Reports indicate hostile life forms may have emerged from " +
                   "contaminated experiments. Stay alert and arm yourself appropriately.\r\n\r\n" +
                   "Emergency systems show limited time before life support fails.\r\n" +
                   "Find the escape pod authorization and evacuate immediately!\r\n\r\n" +
                   "Console commands: type \"chelp\" for system help";
        }

        public string GetAdventureThankYouText()
        {
            return "? ESCAPE POD LAUNCHED SUCCESSFULLY! ?\r\n\r\n" +
                   "CONGRATULATIONS, SURVIVOR!\r\n\r\n" +
                   "You have successfully escaped the abandoned space station before it " +
                   "became completely overrun by the mysterious gooplings! Your quick thinking " +
                   "and resourcefulness saved your life.\r\n\r\n" +
                   "As you drift safely toward the nearest rescue beacon, you reflect on " +
                   "the strange events that led to the station's abandonment. The commander's " +
                   "log revealed the tragic irony - an engineer demoted to cook, attempting " +
                   "to win back favor by using an antimatter accelerator to bake a birthday " +
                   "cake, accidentally creating the goopling creatures that doomed the station.\r\n\r\n" +
                   "Your robot companion's assistance proved invaluable in your escape. " +
                   "Perhaps in the future, space stations will have better safety protocols... " +
                   "and stricter kitchen equipment regulations!\r\n\r\n" +
                   "Thank you for playing 'Abandoned Space Station: Project Escape'!\r\n" +
                   "May the stars guide you safely home.\r\n\r\n" +
                   "Created with cosmic appreciation by the Adventure House team.";
        }

        public string GetWelcomeMessage(string gamerTag)
        {
            return $"HIBERNATION CHAMBER LOG: Occupant {gamerTag} awakening...\r\n\r\n" +
                   "Emergency protocols have been activated. Station status: UNKNOWN.\r\n" +
                   "All crew personnel are unaccounted for. Life support systems operating " +
                   "on emergency power.\r\n\r\n" +
                   "Recommendation: Investigate station status and locate emergency evacuation " +
                   "procedures immediately.\r\n\r\n" +
                   $"Good luck, {gamerTag}. The station's fate may depend on your actions.\r\n\r\n" +
                   "End transmission.\r\n\r\n";
        }
        #endregion

        #region Map Level Display Names
        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] = "Level 1: Command Deck",
            [MapLevel.UpperFloor] = "Level 2: Crew Quarters", 
            [MapLevel.Attic] = "Level 3: Science Labs",
            [MapLevel.MagicRealm] = "Level 4: Engineering",
#if DEBUG
            [MapLevel.DebugLevel] = "Debug Station",
#endif
            [MapLevel.Exit] = "Escape Pod - Freedom!"
        };
        #endregion

        #region Game Settings and Constants
        public int StartingRoom => 1; // Hibernation Chamber - Start here, not room 20!
        public int MaxHealth => 300;
        public int HealthStep => 3;
        public int StartingPoints => 10;
        public List<string> InitialPointsCheckList => new() { "Awakened" };

        // Special location constants
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';
        #endregion

        #region Helper Methods
        public char GetRoomDisplayChar(string roomName)
        {
            var cleanName = roomName?.ToLower() ?? string.Empty;
            
            // Check exact matches first
            if (RoomDisplayCharacters.TryGetValue(cleanName, out char exactChar))
                return exactChar;
            
            // Then check partial matches for room types
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
            return LevelDisplayNames.GetValueOrDefault(level, "Unknown Level");
        }

        public MapLevel GetLevelForRoom(int roomNumber)
        {
            return RoomToLevelMapping.GetValueOrDefault(roomNumber, MapLevel.GroundFloor);
        }

        public (int X, int Y) GetRoomPosition(int roomNumber)
        {
            return RoomPositionMapping.GetValueOrDefault(roomNumber, (0, 0));
        }
        #endregion

        #region Map Configuration
        public Dictionary<int, MapLevel> RoomToLevelMapping => new()
        {
            // Exit
            [0] = MapLevel.Exit,
            
            // Level 1 - Command (rooms 1-6)
            [1] = MapLevel.GroundFloor, [2] = MapLevel.GroundFloor, [3] = MapLevel.GroundFloor,
            [4] = MapLevel.GroundFloor, [5] = MapLevel.GroundFloor, [6] = MapLevel.GroundFloor,
            
            // Level 2 - Crew (rooms 7-16)
            [7] = MapLevel.UpperFloor, [8] = MapLevel.UpperFloor, [9] = MapLevel.UpperFloor,
            [10] = MapLevel.UpperFloor, [11] = MapLevel.UpperFloor, [12] = MapLevel.UpperFloor,
            [13] = MapLevel.UpperFloor, [14] = MapLevel.UpperFloor, [15] = MapLevel.UpperFloor,
            [16] = MapLevel.UpperFloor,
            
            // Level 3 - Science (rooms 17-26)
            [17] = MapLevel.Attic, [18] = MapLevel.Attic, [19] = MapLevel.Attic,
            [20] = MapLevel.Attic, [21] = MapLevel.Attic, [22] = MapLevel.Attic,
            [23] = MapLevel.Attic, [24] = MapLevel.Attic, [25] = MapLevel.Attic,
            [26] = MapLevel.Attic,
            
            // Level 4 - Engineering (rooms 27-40)
            [27] = MapLevel.MagicRealm, [28] = MapLevel.MagicRealm, [29] = MapLevel.MagicRealm,
            [30] = MapLevel.MagicRealm, [31] = MapLevel.MagicRealm, [32] = MapLevel.MagicRealm,
            [33] = MapLevel.MagicRealm, [34] = MapLevel.MagicRealm, [35] = MapLevel.MagicRealm,
            [36] = MapLevel.MagicRealm, [37] = MapLevel.MagicRealm, [38] = MapLevel.MagicRealm,
            [39] = MapLevel.MagicRealm, [40] = MapLevel.MagicRealm,

#if DEBUG
            [88] = MapLevel.DebugLevel
#endif
        };

        public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
        {
            [MapLevel.GroundFloor] = (6, 4),    // Command Deck
            [MapLevel.UpperFloor] = (8, 5),     // Crew Quarters
            [MapLevel.Attic] = (8, 6),          // Science Labs
            [MapLevel.MagicRealm] = (10, 7),    // Engineering (largest level)
#if DEBUG
            [MapLevel.DebugLevel] = (3, 3),
#endif
            [MapLevel.Exit] = (3, 3)
        };

        public Dictionary<int, (int X, int Y)> RoomPositionMapping => new()
        {
            // Level 1 - Command Deck Layout (6x4)
            [1] = (1, 1),  // Hibernation Chamber
            [2] = (3, 1),  // Command Center
            [3] = (5, 1),  // Captain's Office
            [4] = (5, 3),  // Secure Office
            [5] = (3, 3),  // Robot Room
            [6] = (1, 3),  // Command Lift

            // Level 2 - Crew Quarters Layout (8x5)
            [7] = (4, 1),   // Crew Corridor
            [8] = (2, 1),   // Crew Mess Hall
            [9] = (6, 1),   // Captain's Quarters
            [10] = (6, 3),  // Captain's Shower
            [11] = (1, 3),  // Crew Quarters A
            [12] = (3, 3),  // Crew Quarters B
            [13] = (5, 3),  // Crew Quarters C
            [14] = (7, 3),  // Crew Lounge
            [15] = (2, 5),  // Crew Medical Bay
            [16] = (6, 5),  // Crew Lift

            // Level 3 - Science Labs Layout (8x6)
            [17] = (4, 1),  // Science Corridor
            [18] = (1, 1),  // Hydroponics Lab
            [19] = (2, 3),  // Research Lab
            [20] = (4, 3),  // Science Computer Core
            [21] = (6, 3),  // Observatory
            [22] = (7, 1),  // Experiment Chamber
            [23] = (4, 5),  // Teleport Room
            [24] = (1, 5),  // Containment Lab
            [25] = (6, 5),  // Analysis Lab
            [26] = (7, 5),  // Science Storage

            // Level 4 - Engineering Layout (10x7)
            [27] = (5, 1),  // Engineering Corridor
            [28] = (3, 1),  // Main Engineering
            [29] = (1, 1),  // Power Core
            [30] = (7, 1),  // Engine Room
            [31] = (9, 1),  // Maintenance Bay
            [32] = (1, 3),  // Tool Storage
            [33] = (3, 3),  // Spare Parts Room
            [34] = (5, 3),  // Waste Processing
            [35] = (7, 3),  // Escape Pod Bay
            [36] = (9, 3),  // Eng Lift
            [37] = (1, 5),  // Auxiliary Engine
            [38] = (3, 5),  // Cooling Systems
            [39] = (5, 5),  // Reactor Room
            [40] = (7, 5),  // Airlock Chamber

#if DEBUG
            [88] = (1, 1),  // Debug Station
#endif

            // Exit
            [0] = (1, 1)    // Escape Pod Exit
        };

        public Dictionary<int, (int X, int Y)> RoomPositions => RoomPositionMapping;
        #endregion
    }
}