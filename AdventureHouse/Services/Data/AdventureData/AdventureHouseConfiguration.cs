using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Game-specific configuration for Adventure House game data and strings
    /// This class contains all the Adventure House specific text, mappings, and display rules
    /// </summary>
    public class AdventureHouseConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Adventure House 3.5 (.Net 9.0)";
        public string GameVersion => "3.5";
        public string GameDescription => "Figure out how to escape from the house.";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomDisplayCharacters => new()
        {
            ["exit!"] = 'X',
            ["main entrance"] = 'E',
            ["kitchen"] = 'K',
            ["living room"] = 'L',
            ["family room"] = 'F',
            ["guest bathroom"] = 'B',
            ["upstairs bath"] = 'U',
            ["master bedroom"] = 'M',
            ["children's room"] = 'C',
            ["attic"] = 'A',
            ["deck"] = 'D',
            ["garage"] = 'G',
            ["nook"] = 'N'
        };

        public Dictionary<string, char> RoomTypeCharacters => new()
        {
            ["hallway"] = 'H',
            ["bathroom"] = 'b',
            ["bath"] = 'b',
            ["bedroom"] = 'm',
            ["dining"] = 'd',
            ["utility"] = 'u',
            ["closet"] = 'c',
            ["entertainment"] = 'e',
            ["magic"] = '*',
            ["ladder"] = '|',
#if DEBUG
            ["debug"] = '?'
#endif
        };

        public Dictionary<string, int> RoomNameToNumberMapping => new()
        {
            ["exit!"] = 0,
            ["main entrance"] = 1,
            ["downstairs hallway"] = 2,
            ["guest bathroom"] = 3,
            ["living room"] = 4,
            ["family room"] = 5,
            ["nook"] = 6,
            ["kitchen"] = 7,
            ["utility hall"] = 8,
            ["garage"] = 9,
            ["main dining room"] = 10,
            ["upstairs hallway"] = 11,
            ["upstairs east hallway"] = 12,
            ["upstairs north hallway"] = 13,
            ["upstairs west hallway"] = 14,
            ["spare room"] = 15,
            ["utility room"] = 16,
            ["upstairs bath"] = 17,
            ["master bedroom"] = 18,
            ["master bedroom closet"] = 19,
            ["attic"] = 20,
            ["master bedroom bath"] = 21,
            ["children's room"] = 22,
            ["entertainment room"] = 23,
            ["deck"] = 24,
            ["debug room"] = 88,
            ["psychedelic ladder"] = 93,
            ["memory ladder"] = 94,
            ["magic mushroom"] = 95
        };
        #endregion

        #region Map Legend and Help Text
        public string MapLegendContent => @"
MAP LEGEND (ASCII ONLY - BOXED ROOMS WITH PATHS):
+---+  = Room Box     @ = Your Location (replaces room char)    
|E  |  = Entrance     |K| = Kitchen        |L| = Living Room
|M+ |  = Room + Items |C| = Children Room  |B| = Guest Bathroom  
|U  |  = Upstairs Bath|H| = Hallway        |D| = Deck/Dining     
|G  |  = Garage       |A| = Attic          |*| = Magic Room      
||  |  = Ladder       |.| = Other Room

PATH CONNECTIONS:
. . .  = Horizontal path (East/West)
:      = Vertical path (North/South)  
  :    
^      = Stairs/ladder going Up
v      = Stairs/ladder going Down
";

#if DEBUG
        public string MapLegendDebugAddition => @"|?  |  = Debug Room
";
#endif

        public string MapLegendFooter => @"
Note: Only visited rooms and paths between them are shown.
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
            return "You pause and recall your mother's bedtime story:\r\n" +
                   "Once upon a time a great explorer wandered into a mystery house. " +
                   "The adventurer visited rooms from the EAST to the WEST. " +
                   "Going UP and DOWN stairs and ladders looking for items. The hero " +
                   "took many actions such as LOOKing and GETting items found in their path. " +
                   "From time to time the hero would USE these items to explore further. " +
                   "Sometimes these things would need to be used in a specific way. " +
                   "Fortunately for the adventurer, their backpack has infinite INVentory " +
                   "space and they could carry almost anything. The adventure seemed never " +
                   "to end unless a specific exit or item was found. There were times when " +
                   "the adventurer would die, but since this is a story you can always " +
                   "RESTART from the beginning. The hero would often get and need to EAT " +
                   "a snack. The rest of the story fades from your mind, but you do recall " +
                   "your mom talking about the explorer who would WAVE things " +
                   "while EATing an apple.\r\n\r\n" +
                   "Your mother also mentioned that sometimes the adventurer would encounter " +
                   "dangerous creatures in the house. When this happened, the brave explorer " +
                   "would need to find the right weapon to ATTACK the beast. Without the proper " +
                   "weapon, the creatures could not be defeated, and they might strike back! " +
                   "The explorer could always flee to another room to escape danger.\r\n\r\n" +
                   "Console help type \"chelp\"";
        }

        public string GetAdventureThankYouText()
        {
            return "CONGRATULATIONS! YOU WIN! (try the \"Points\" command)\r\n\r\n" +
                   "You have been able to escape the game before you starved to death!\r\n" +
                   "You can continue to explore by returning to the house \"west\".\r\n\r\n"
                   + "We hope you have enjoyed this retro-style adventure game! "
                   + "We have hidden some fun little surprises in the text and objects. "
                   + "Or personal favorite is Stormi the kitten following you around the house after you "
                   + "give her a nice PET. You can always SHOO her away if she is too much of a pest. "
                   + "If you get a chance to read the SLIP a few times it can make for a few laughs." +
                   "\r\n\r\n" +
                   "Have a Great Day and Let is know what you think!\r\n\r\n" +
                   "\"The Three Peppers\" - Steve, Stevie, and Anabella\r\n\r\n";
        }

        public string GetWelcomeMessage(string gamerTag)
        {
            return $"Dear {gamerTag}, \r\n\r\nThis is a simple 2 word adventure game. Use simple but HELPful commands to find your way out before you die.\r\n\r\nGood Luck!\r\n\r\nThe Management.\r\n\r\n\r\n";
        }
        #endregion

        #region Map Level Display Names
        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] = "Ground Floor",
            [MapLevel.UpperFloor] = "Upper Floor", 
            [MapLevel.Attic] = "Attic",
            [MapLevel.MagicRealm] = "Magic Realm",
#if DEBUG
            [MapLevel.DebugLevel] = "Debug Level",
#endif
            [MapLevel.Exit] = "Freedom!"
        };
        #endregion

        #region Game Settings and Constants
        public int StartingRoom => 20;
        public int MaxHealth => 200;
        public int HealthStep => 2;
        public int StartingPoints => 1;
        public string InitialPointsCheckList => "NewGame";

        // Special location constants for Adventure House
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get room display character based on room name for Adventure House game
        /// </summary>
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

        /// <summary>
        /// Get room number from room name using Adventure House mapping
        /// </summary>
        public int GetRoomNumberFromName(string roomName)
        {
            var cleanName = roomName?.ToLower().Trim() ?? string.Empty;
            return RoomNameToNumberMapping.GetValueOrDefault(cleanName, -1);
        }

        /// <summary>
        /// Get level display name for Adventure House game
        /// </summary>
        public string GetLevelDisplayName(MapLevel level)
        {
            return LevelDisplayNames.GetValueOrDefault(level, "Unknown Level");
        }

        /// <summary>
        /// Get which level a room belongs to
        /// </summary>
        public MapLevel GetLevelForRoom(int roomNumber)
        {
            return RoomToLevelMapping.GetValueOrDefault(roomNumber, MapLevel.GroundFloor);
        }

        /// <summary>
        /// Get the grid position for a room on its level
        /// </summary>
        public (int X, int Y) GetRoomPosition(int roomNumber)
        {
            return RoomPositions.GetValueOrDefault(roomNumber, (0, 0));
        }
        #endregion

        #region Map Configuration
        public Dictionary<int, MapLevel> RoomToLevelMapping => new()
        {
            [0] = MapLevel.Exit,
            [1] = MapLevel.GroundFloor, [2] = MapLevel.GroundFloor, [3] = MapLevel.GroundFloor,
            [4] = MapLevel.GroundFloor, [5] = MapLevel.GroundFloor, [6] = MapLevel.GroundFloor,
            [7] = MapLevel.GroundFloor, [8] = MapLevel.GroundFloor, [9] = MapLevel.GroundFloor,
            [10] = MapLevel.GroundFloor, [24] = MapLevel.GroundFloor,
            [11] = MapLevel.UpperFloor, [12] = MapLevel.UpperFloor, [13] = MapLevel.UpperFloor,
            [14] = MapLevel.UpperFloor, [15] = MapLevel.UpperFloor, [16] = MapLevel.UpperFloor,
            [17] = MapLevel.UpperFloor, [18] = MapLevel.UpperFloor, [19] = MapLevel.UpperFloor,
            [21] = MapLevel.UpperFloor, [22] = MapLevel.UpperFloor, [23] = MapLevel.UpperFloor,
            [20] = MapLevel.Attic,
            [93] = MapLevel.MagicRealm, [94] = MapLevel.MagicRealm, [95] = MapLevel.MagicRealm,
#if DEBUG
            [88] = MapLevel.DebugLevel
#endif
        };

        public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
        {
            [MapLevel.GroundFloor] = (8, 7),   // Wide layout for house floors
            [MapLevel.UpperFloor] = (8, 9),    // Upper floor layout
            [MapLevel.Attic] = (3, 3),         // Small single room
            [MapLevel.MagicRealm] = (3, 5),    // Vertical magic realm
#if DEBUG
            [MapLevel.DebugLevel] = (3, 3),    // Small debug area
#endif
            [MapLevel.Exit] = (3, 3)           // Exit area
        };

        public Dictionary<int, (int X, int Y)> RoomPositions => new()
        {
            // Ground Floor Layout
            [1] = (5, 2),   // Main Entrance
            [2] = (5, 4),   // Downstairs Hallway
            [3] = (7, 4),   // Guest Bathroom
            [4] = (5, 6),   // Living Room
            [5] = (3, 6),   // Family Room
            [6] = (3, 4),   // Nook
            [7] = (3, 2),   // Kitchen
            [8] = (3, 0),   // Utility Hall
            [9] = (1, 0),   // Garage
            [10] = (5, 0),  // Main Dining Room
            [24] = (1, 4),  // Deck

            // Upper Floor Layout
            [11] = (5, 4),  // Upstairs Hallway
            [12] = (5, 6),  // Upstairs East Hallway
            [13] = (3, 4),  // Upstairs North Hallway
            [14] = (3, 6),  // Upstairs West Hallway
            [15] = (5, 8),  // Spare Room
            [16] = (7, 4),  // Utility Room
            [17] = (1, 4),  // Upstairs Bath
            [18] = (3, 2),  // Master Bedroom
            [19] = (5, 2),  // Master Bedroom Closet
            [21] = (3, 0),  // Master Bedroom Bath
            [22] = (1, 6),  // Children's Room
            [23] = (3, 8),  // Entertainment Room

            // Attic Layout
            [20] = (1, 1),  // Attic (center of small map)

            // Magic Realm Layout
            [93] = (1, 1),  // Psychedelic Ladder (top)
            [94] = (1, 3),  // Memory Ladder (bottom)
            [95] = (1, 2),  // Magic Mushroom (center)

#if DEBUG
            // Debug Layout
            [88] = (1, 1),  // Debug Room (center)
#endif

            // Exit Layout
            [0] = (1, 1)    // Exit (center)
        };
        #endregion
    }
}