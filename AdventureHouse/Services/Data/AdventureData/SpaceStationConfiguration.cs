using System;
using System.Collections.Generic;
using AdventureHouse.Services.AdventureClient.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Space Station adventure configuration - A thrilling escape from an abandoned space station
    /// </summary>
    public class SpaceStationConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Abandoned Space Station";
        public string GameVersion => "1.0";
        public string GameDescription => "Escape from an abandoned space station overrun by mysterious creatures!";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomDisplayCharacters => new()
        {
            ["hibernation"] = 'H',
            ["command"] = 'C',
            ["office"] = 'O',
            ["secure"] = 'S',
            ["robot"] = 'R',
            ["quarters"] = 'Q',
            ["shower"] = 'W',
            ["mess"] = 'M',
            ["bedroom"] = 'B',
            ["lounge"] = 'L',
            ["medical"] = 'D',
            ["hydroponics"] = 'P',
            ["research"] = 'L',
            ["computer"] = 'C',
            ["observatory"] = 'V',
            ["experiment"] = 'E',
            ["teleport"] = 'T',
            ["engineering"] = 'E',
            ["power"] = 'P',
            ["engine"] = 'N',
            ["maintenance"] = 'A',
            ["storage"] = 'S',
            ["waste"] = 'W',
            ["escape"] = 'X',
            ["lift"] = '|',
            ["exit"] = 'X',
            ["entrance"] = 'H',
            ["debug"] = 'D'
        };

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
            ["exit"] = 0,
            ["escape"] = 0,
            // Level 1 - Command
            ["hibernation"] = 1,
            ["command"] = 2,
            ["office"] = 3,
            ["secure"] = 4,
            ["robot"] = 5,
            ["commandlift"] = 6,

            // Level 2 - Crew  
            ["corridor"] = 7,
            ["mess"] = 8,
            ["quarters"] = 9,
            ["shower"] = 10,
            ["quartersa"] = 11,
            ["quartersb"] = 12,
            ["quartersc"] = 13,
            ["lounge"] = 14,
            ["medical"] = 15,
            ["crewlift"] = 16,

            // Level 3 - Science
            ["science"] = 17,
            ["hydroponics"] = 18,
            ["research"] = 19,
            ["computer"] = 20,
            ["observatory"] = 21,
            ["experiment"] = 22,
            ["teleport"] = 23,
            ["containment"] = 24,
            ["analysis"] = 25,
            ["storage"] = 26,

            // Level 4 - Engineering
            ["engineering"] = 27,
            ["main"] = 28,
            ["power"] = 29,
            ["engine"] = 30,
            ["maintenance"] = 31,
            ["tools"] = 32,
            ["parts"] = 33,
            ["waste"] = 34,
            ["pod"] = 35,
            ["englift"] = 36,
            ["auxiliary"] = 37,
            ["cooling"] = 38,
            ["reactor"] = 39,
            ["airlock"] = 40,

#if DEBUG
            ["debug"] = 88
#endif
        };
        #endregion

        #region Game Settings
        public int StartingRoom => 1; // Hibernation Chamber
        public int MaxHealth => 300;
        public int HealthStep => 3;
        public int StartingPoints => 10;
        public List<string> InitialPointsCheckList => new() { "Awakened" };

        // Constants
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';
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
            [88] = MapLevel.Attic
#endif
        };

        public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
        {
            [MapLevel.GroundFloor] = (6, 4),    // Command Deck
            [MapLevel.UpperFloor] = (8, 5),     // Crew Quarters
            [MapLevel.Attic] = (8, 6),          // Science Labs
            [MapLevel.MagicRealm] = (10, 7),    // Engineering (largest level)
            [MapLevel.Exit] = (3, 3)
        };

        public Dictionary<int, (int X, int Y)> RoomPositions => new()
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

        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] = "Command Deck",
            [MapLevel.UpperFloor] = "Crew Quarters", 
            [MapLevel.Attic] = "Science Labs",
            [MapLevel.MagicRealm] = "Engineering",
            [MapLevel.Exit] = "Escape Pod!"
        };
        #endregion

        #region Interface Properties
        public Dictionary<string, char> RoomCharacterMapping => RoomDisplayCharacters;
        public Dictionary<int, (int X, int Y)> RoomPositionMapping => RoomPositions;
        public string MapLegend => GetCompleteMapLegend();
        public string MapLegendContent => @"SPACE STATION MAP LEGEND:
H = Hibernation   C = Command      O = Office       S = Secure/Storage
Q = Quarters      M = Mess Hall    L = Lab/Lounge   E = Engineering/Experiment
P = Power/Pod     R = Robot Room   D = Medical      V = Observatory
T = Teleport      A = Maintenance  N = Engine       W = Waste/Shower
X = Escape Pod    | = Lift         . = Other Room

LEVELS:
Command Deck = Administrative and control systems
Crew Quarters = Living spaces and personal areas  
Science Labs = Research facilities and experiments
Engineering = Power systems and escape pod";

        public string MapLegendFooter => @"
Your goal is to reach the Escape Pod (X) to escape the station!
Navigate through the four levels and find the authorization needed.";
        #endregion

        #region Help and Story Text
        public string GetAdventureHelpText()
        {
            return @"=== ABANDONED SPACE STATION ESCAPE ===

EMERGENCY PROTOCOLS ACTIVATED

You have awakened from hibernation aboard the space station to find it mysteriously abandoned. 
The emergency lighting casts eerie shadows through the corridors, and strange sounds echo from 
the ventilation systems.

Your survival depends on finding the ESCAPE POD, but it requires proper authorization. 
You'll need to explore the station's four levels:

• COMMAND DECK (Level 1) - Administrative and control systems
• CREW QUARTERS (Level 2) - Living spaces and personal areas
• SCIENCE LABS (Level 3) - Research facilities and experiments
• ENGINEERING (Level 4) - Power systems and the escape pod

CONTROLS:
• Move: N, S, E, W, U (up), D (down)
• Get items: GET [item] or TAKE [item]
• Use items: USE [item] or specific action verbs
• Drop items: DROP [item]
• Look: LOOK or LOOK [item]
• Inventory: I or INVENTORY
• Activate robot: ACTIVATE ROBOT
• Attack creatures: ATTACK [monster] WITH [weapon]
• Map: MAP (shows current level layout)
• Help: HELP or ?
• Quit: QUIT or EXIT

WARNING: Reports indicate hostile life forms may have emerged from contaminated experiments. 
Stay alert and arm yourself appropriately.

Emergency systems show limited time before life support fails.
Find the escape pod authorization and evacuate immediately!";
        }

        public string GetAdventureThankYouText()
        {
            return @"?? ESCAPE POD LAUNCHED SUCCESSFULLY! ??

CONGRATULATIONS, SURVIVOR!

You have successfully escaped the abandoned space station before it became completely overrun 
by the mysterious creatures! Your quick thinking and resourcefulness saved your life.

As you drift safely toward the nearest rescue beacon, you reflect on the strange events that 
led to the station's abandonment. The commander's log revealed the tragic irony - an engineer 
demoted to cook, attempting to win back favor by using an antimatter accelerator to bake a 
birthday cake, accidentally creating the hostile creatures that doomed the station.

Your robot companion's assistance proved invaluable in your escape. Perhaps in the future, 
space stations will have better safety protocols... and stricter kitchen equipment regulations!

Thank you for playing 'Abandoned Space Station'!
May the stars guide you safely home.

?? MISSION ACCOMPLISHED! ??";
        }

        public string GetWelcomeMessage(string gamerTag)
        {
            return $@"Welcome to the Abandoned Space Station, {gamerTag}!

?? HIBERNATION CHAMBER LOG: Occupant {gamerTag} awakening... ??

Emergency protocols have been activated. Station status: UNKNOWN.
All crew personnel are unaccounted for. Life support systems operating on emergency power.

Strange readings detected throughout the station. Multiple system failures reported.
Hostile life forms may be present.

Recommendation: Investigate station status and locate emergency evacuation procedures immediately.

Your mission: Find the escape pod authorization and evacuate before life support fails!

Good luck, {gamerTag}. The station's fate may depend on your actions.

Type HELP for commands, or start exploring to uncover the mystery!";
        }
        #endregion

        #region Interface Implementation
        public char GetRoomDisplayChar(string roomName)
        {
            var lowerName = roomName.ToLower();
            foreach (var kvp in RoomDisplayCharacters)
            {
                if (lowerName.Contains(kvp.Key))
                    return kvp.Value;
            }
            return DefaultRoomCharacter;
        }

        public string GetLevelDisplayName(MapLevel level)
        {
            return LevelDisplayNames.TryGetValue(level, out var name) ? name : level.ToString();
        }

        public MapLevel GetLevelForRoom(int roomNumber)
        {
            return RoomToLevelMapping.TryGetValue(roomNumber, out var level) ? level : MapLevel.GroundFloor;
        }

        public (int X, int Y) GetRoomPosition(int roomNumber)
        {
            return RoomPositions.TryGetValue(roomNumber, out var position) ? position : (1, 1);
        }

        public int GetRoomNumberFromName(string roomName)
        {
            var cleanName = roomName?.ToLower().Trim() ?? string.Empty;
            
            // Check exact matches first
            foreach (var kvp in RoomNameToNumberMapping)
            {
                if (cleanName.Contains(kvp.Key))
                    return kvp.Value;
            }
            
            // Check partial matches for common words
            var words = cleanName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                foreach (var kvp in RoomNameToNumberMapping)
                {
                    if (kvp.Key.Contains(word) && word.Length > 2) // Avoid matching very short words
                    {
                        return kvp.Value;
                    }
                }
            }
            
            return 1; // Default to starting room if no match found
        }

        public string GetCompleteMapLegend()
        {
            return MapLegendContent + MapLegendFooter;
        }
        #endregion
    }
}