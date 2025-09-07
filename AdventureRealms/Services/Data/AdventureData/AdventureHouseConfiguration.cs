using System;
using System.Collections.Generic;
using AdventureRealms.Services.AdventureClient.Models;

namespace AdventureRealms.Services.Data.AdventureData
{
    /// <summary>
    /// Game-specific configuration for Adventure House game data and strings
    /// This class contains all the Adventure House specific text, mappings, and display rules
    /// </summary>
    public class AdventureHouseConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Adventure House";
        public string GameVersion => "3.5";
        public string GameDescription => "Figure out how to escape from the house.";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomCharacterMapping => new()
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
            ["nook"] = 'N',
            ["downstairs hallway"] = 'H',
            ["upstairs hallway"] = 'H',
            ["upstairs east hallway"] = 'H',
            ["upstairs north hallway"] = 'H',
            ["upstairs west hallway"] = 'H',
            ["utility hall"] = 'u',
            ["main dining room"] = 'd',
            ["spare room"] = 'm',
            ["utility room"] = 'u',
            ["master bedroom closet"] = 'c',
            ["master bedroom bath"] = 'b',
            ["entertainment room"] = 'e',
            ["psychedelic ladder"] = '|',
            ["memory ladder"] = '|',
            ["magic mushroom"] = '*'
        };

        public Dictionary<string, char> RoomDisplayCharacters => RoomCharacterMapping;

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
#if DEBUG
            ["debug room"] = 88,
#endif
            ["psychedelic ladder"] = 93,
            ["memory ladder"] = 94,
            ["magic mushroom"] = 95
        };
        #endregion

        #region Map Legend and Help Text
        public string MapLegend => @"
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

Note: Only visited rooms and paths between them are shown.
";
        
        public string MapLegendContent => MapLegend;
        public string MapLegendFooter => "";

        public string GetCompleteMapLegend() => MapLegend;

        public string GetAdventureHelpText()
        {
            return "You pause and recall your mother's bedtime story:\r\n\r\n" +
                   "Once upon a time a great explorer wandered into a mystery house. " +
                   "The adventurer visited rooms from EAST to WEST, going UP and DOWN stairs " +
                   "looking for items. The hero took actions such as LOOKing and GETting items.\r\n\r\n" +
                   "From time to time the hero would USE these items to explore further. " +
                   "The adventurer's backpack has infinite INVentory space. " +
                   "The hero would often get and need to EAT a snack.\r\n\r\n" +
                   "Your mom mentioned the explorer would WAVE things while EATing an apple. " +
                   "Sometimes the adventurer would encounter dangerous creatures. " +
                   "The brave explorer would ATTACK the beast with the right weapon.\r\n\r\n" +
                   "Type \"chelp\" for console commands.";
        }

        public string GetAdventureThankYouText()
        {
            return "CONGRATULATIONS! YOU WIN! (try the \"Points\" command)\r\n\r\n" +
                   "You have escaped the game before you starved to death!\r\n" +
                   "You can continue to explore by returning to the house \"west\".\r\n\r\n" +
                   "We hope you enjoyed this retro-style adventure game! " +
                   "We hidden some fun surprises in the text and objects. " +
                   "Try PETting Stormi the kitten - she'll follow you around! " +
                   "You can SHOO her away if needed.\r\n\r\n" +
                   "Have a Great Day!\r\n" +
                   "\"The Three Peppers\" - Steve, Stevie, and Anabella";
        }

        public string GetWelcomeMessage(string gamerTag)
        {
            return $"Dear {gamerTag}, \r\n\r\nThis is a simple 2 word adventure game. Use simple but HELPful commands to find your way out before you die.\r\n\r\nGood Luck!\r\n\r\nThe Management.\r\n\r\n\r\n";
        }
        #endregion

        #region Game Settings and Constants
        public int StartingRoom => 20; // Attic - Start here!
        public int MaxHealth => 200;
        public int HealthStep => 2;
        public int StartingPoints => 1;
        public List<string> InitialPointsCheckList => new() { "NewGame" };
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';
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

        public Dictionary<int, (int X, int Y)> RoomPositionMapping => new()
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

        public Dictionary<int, (int X, int Y)> RoomPositions => RoomPositionMapping;
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

        public int GetRoomNumberFromName(string roomName)
        {
            var cleanName = roomName?.ToLower().Trim() ?? string.Empty;
            
            // Check exact matches first
            if (RoomNameToNumberMapping.TryGetValue(cleanName, out int exactMatch))
                return exactMatch;
            
            // Check partial matches - look for rooms that contain key words from the room name
            foreach (var kvp in RoomNameToNumberMapping)
            {
                var mappingKey = kvp.Key;
                var roomNumber = kvp.Value;
                
                // If the clean name contains the mapping key or vice versa
                if (cleanName.Contains(mappingKey) || mappingKey.Contains(cleanName))
                {
                    return roomNumber;
                }
            }
            
            // If no match found, return -1
            return -1;
        }
        #endregion
    }
}