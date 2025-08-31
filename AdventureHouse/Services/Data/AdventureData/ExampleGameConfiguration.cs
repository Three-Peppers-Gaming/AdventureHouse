using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Example game-specific configuration showing how different games can have different level names
    /// This demonstrates the flexibility of the abstracted level naming system
    /// </summary>
    public class ExampleGameConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Example Adventure Game";
        public string GameVersion => "1.0";
        public string GameDescription => "A sample game showing different level naming conventions.";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomDisplayCharacters => new()
        {
            ["start"] = 'S',
            ["finish"] = 'F',
            ["treasure"] = 'T'
        };

        public Dictionary<string, char> RoomTypeCharacters => new()
        {
            ["corridor"] = 'C',
            ["chamber"] = 'H'
        };

        public Dictionary<string, int> RoomNameToNumberMapping => new()
        {
            ["start"] = 1,
            ["finish"] = 99
        };
        #endregion

        #region Map and Help Text
        public string MapLegendContent => "Example game map legend content.";
        public string MapLegendFooter => "Example footer text.";
        
        // EXAMPLE: Different level naming convention
        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] = "Dungeon Level 1",      // Instead of "Ground Floor"
            [MapLevel.UpperFloor] = "Dungeon Level 2",       // Instead of "Upper Floor" 
            [MapLevel.Attic] = "Tower Peak",                 // Instead of "Attic"
            [MapLevel.MagicRealm] = "Mystic Dimension",      // Instead of "Magic Realm"
#if DEBUG
            [MapLevel.DebugLevel] = "Developer Realm",       // Instead of "Debug Level"
#endif
            [MapLevel.Exit] = "Victory Gate!"               // Instead of "Freedom!"
        };

        public string GetCompleteMapLegend() => MapLegendContent + MapLegendFooter;
        public string GetAdventureHelpText() => "Example game help text.";
        public string GetAdventureThankYouText() => "Thanks for playing the example game!";
        public string GetWelcomeMessage(string gamerTag) => $"Welcome {gamerTag} to the example game!";
        #endregion

        #region Game Settings
        public int StartingRoom => 1;
        public int MaxHealth => 100;
        public int HealthStep => 1;
        public int StartingPoints => 0;
        public string InitialPointsCheckList => "Start";
        public int InventoryLocation => 1000;
        public int PetFollowLocation => 1001;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '?';
        #endregion

        #region Helper Methods
        public char GetRoomDisplayChar(string roomName)
        {
            var cleanName = roomName?.ToLower() ?? string.Empty;
            if (RoomDisplayCharacters.TryGetValue(cleanName, out char exactChar))
                return exactChar;
            
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
            return LevelDisplayNames.GetValueOrDefault(level, "Unknown Realm");
        }
        #endregion
    }
}