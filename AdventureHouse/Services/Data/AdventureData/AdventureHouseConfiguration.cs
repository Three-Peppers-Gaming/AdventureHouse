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
                   "while EATing an apple.\r\n\r\nConsole help type \"chelp\"";
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
        #endregion
    }
}