using AdventureRealms.Services.AdventureClient.Models;

namespace AdventureRealms.Services.Data.AdventureData
{
    /// <summary>
    /// Lost in the Woods adventure configuration
    /// A fairy tale adventure through an enchanted forest
    /// </summary>
    public class LostInTheWoodsConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Lost in the Woods";
        public string GameVersion => "1.0";
        public string GameDescription => "A fairy tale adventure where you must find your way home through an enchanted forest after leaving grandma's house.";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomCharacterMapping => new()
        {
            ["grandma's house"] = 'G',
            ["forest"] = 'F',
            ["clearing"] = 'C',
            ["path"] = 'P',
            ["creek"] = '~',
            ["bridge"] = '=',
            ["cave"] = 'X',
            ["tree"] = 'T',
            ["berry"] = 'B',
            ["home clearing"] = 'H',
            ["spider"] = 'S',
            ["bat"] = 'b',
            ["snake"] = 's',
            ["wolf"] = 'W'
        };

        public Dictionary<string, char> RoomDisplayCharacters => RoomCharacterMapping;
        public Dictionary<string, char> RoomTypeCharacters => RoomCharacterMapping;

        public Dictionary<string, int> RoomNameToNumberMapping => new()
        {
            ["grandma's house"] = 1,
            ["forest path"] = 2,
            ["quiet forest"] = 3,
            ["dark woods"] = 4,
            ["sunny glade"] = 5,
            ["twisted trees"] = 6,
            ["small creek"] = 7,
            ["berry patch"] = 8,
            ["ancient oak"] = 9,
            ["spider's web"] = 10,
            ["bat cave"] = 11,
            ["deep woods"] = 12,
            ["fallen log"] = 13,
            ["mossy rocks"] = 14,
            ["thorny thicket"] = 15,
            ["peaceful grove"] = 16,
            ["old bridge"] = 17,
            ["rushing water"] = 18,
            ["snake's hollow"] = 19,
            ["mushroom circle"] = 20,
            ["tall grass"] = 21,
            ["wolf's den"] = 22,
            ["rocky outcrop"] = 23,
            ["pine trees"] = 24,
            ["forest edge"] = 25,
            ["meadow"] = 26,
            ["flower field"] = 27,
            ["babbling brook"] = 28,
            ["home clearing"] = 29
        };
        #endregion

        #region Game Settings
        public int StartingRoom => 1; // Grandma's House
        public int MaxHealth => 150;
        public int HealthStep => 2;
        public int StartingPoints => 1;
        public List<string> InitialPointsCheckList => new() { "NewGame" };
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => 'F';
        #endregion

        #region Map Configuration
        public Dictionary<int, MapLevel> RoomToLevelMapping => new()
        {
            [0] = MapLevel.Exit,
            [1] = MapLevel.GroundFloor, [2] = MapLevel.GroundFloor, [3] = MapLevel.GroundFloor,
            [4] = MapLevel.GroundFloor, [5] = MapLevel.GroundFloor, [6] = MapLevel.GroundFloor,
            [7] = MapLevel.GroundFloor, [8] = MapLevel.GroundFloor, [9] = MapLevel.GroundFloor,
            [10] = MapLevel.GroundFloor, [11] = MapLevel.GroundFloor, [12] = MapLevel.GroundFloor,
            [13] = MapLevel.GroundFloor, [14] = MapLevel.GroundFloor, [15] = MapLevel.GroundFloor,
            [16] = MapLevel.GroundFloor, [17] = MapLevel.GroundFloor, [18] = MapLevel.GroundFloor,
            [19] = MapLevel.GroundFloor, [20] = MapLevel.GroundFloor, [21] = MapLevel.GroundFloor,
            [22] = MapLevel.GroundFloor, [23] = MapLevel.GroundFloor, [24] = MapLevel.GroundFloor,
            [25] = MapLevel.GroundFloor, [26] = MapLevel.GroundFloor, [27] = MapLevel.GroundFloor,
            [28] = MapLevel.GroundFloor, [29] = MapLevel.GroundFloor
        };

        public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
        {
            [MapLevel.GroundFloor] = (10, 8),   // Large forest area
            [MapLevel.Exit] = (3, 3)
        };

        public Dictionary<int, (int X, int Y)> RoomPositionMapping => new()
        {
            // Forest journey layout - winding path through the woods
            [1] = (1, 1),   // Grandma's House (start)
            [2] = (2, 1),   // Forest Path
            [3] = (3, 1),   // Quiet Forest
            [4] = (4, 1),   // Dark Woods
            [5] = (4, 2),   // Sunny Glade
            [6] = (4, 3),   // Twisted Trees
            [7] = (3, 3),   // Small Creek
            [8] = (2, 3),   // Berry Patch
            [9] = (1, 3),   // Ancient Oak
            [10] = (1, 4),  // Spider's Web
            [11] = (1, 5),  // Bat Cave
            [12] = (2, 5),  // Deep Woods
            [13] = (3, 5),  // Fallen Log
            [14] = (4, 5),  // Mossy Rocks
            [15] = (5, 5),  // Thorny Thicket
            [16] = (6, 5),  // Peaceful Grove
            [17] = (7, 5),  // Old Bridge
            [18] = (8, 5),  // Rushing Water
            [19] = (8, 4),  // Snake's Hollow
            [20] = (8, 3),  // Mushroom Circle
            [21] = (8, 2),  // Tall Grass
            [22] = (8, 1),  // Wolf's Den
            [23] = (7, 1),  // Rocky Outcrop
            [24] = (6, 1),  // Pine Trees
            [25] = (6, 2),  // Forest Edge
            [26] = (6, 3),  // Meadow
            [27] = (6, 4),  // Flower Field
            [28] = (7, 4),  // Babbling Brook
            [29] = (9, 4),  // Home Clearing (end)
            [0] = (5, 1)    // Exit
        };

        public Dictionary<int, (int X, int Y)> RoomPositions => RoomPositionMapping;
        #endregion

        #region Map Display Configuration
        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] = "Forest Floor",
            [MapLevel.Exit] = "Safety!"
        };

        public string MapLegend => "🌲 Lost in the Woods Map Legend 🌲\r\n" +
                                  "G = Grandma's House   F = Forest Areas    C = Clearings\r\n" +
                                  "~ = Creek/Water       = = Bridge          X = Cave\r\n" +
                                  "T = Big Trees         B = Berry Patches   H = Home\r\n" +
                                  "S = Spider            b = Bat Cave        s = Snake\r\n" +
                                  "W = Wolf's Den        P = Forest Paths    \r\n" +
                                  "@ = Your Location     ? = Unexplored Area";

        public string MapLegendContent => MapLegend;
        public string MapLegendFooter => "Follow the forest paths to find your way home safely!";
        #endregion

        #region Story Text
        public string GetAdventureHelpText()
        {
            return "🌲 Lost in the Woods - A Fairy Tale Adventure 🌲\r\n\r\n" +
                   "After enjoying Wolf Stew at Grandma's house, you head home through the forest.\r\n" +
                   "But the birds have eaten your breadcrumb trail! Now you must find another way home.\r\n\r\n" +
                   "COMMANDS: Use two-word commands like GO NORTH, GET APPLE, USE KEY, EAT BREAD, etc.\r\n" +
                   "MOVEMENT: GO followed by NORTH, SOUTH, EAST, WEST, UP, or DOWN\r\n" +
                   "ITEMS: GET to pick up items, DROP to put them down, INV to see inventory\r\n" +
                   "SPECIAL: PET animals you find, SHOO away pests, USE items when needed\r\n" +
                   "COMBAT: Some forest creatures may attack - use your BB Gun or other items!\r\n\r\n" +
                   "GOAL: Navigate through 29 forest locations to reach the Home Clearing safely.\r\n" +
                   "Watch out for spiders, bats, snakes, and wolves! Look for helpful items like\r\n" +
                   "berries for health, a sandwich for sustenance, and maybe a lost kitten to pet.\r\n\r\n" +
                   "Good luck finding your way home through the enchanted forest!";
        }

        public string GetAdventureThankYouText()
        {
            return "🏠 Congratulations! You made it home safely! 🏠\r\n\r\n" +
                   "After a perilous journey through the enchanted forest, dodging spiders and wolves,\r\n" +
                   "collecting berries and helping a lost kitten, you've finally reached the safety\r\n" +
                   "of your own clearing. The warm lights of home welcome you back from your adventure.\r\n\r\n" +
                   "Thank you for playing Lost in the Woods!\r\n" +
                   "Perhaps next time you'll remember to leave better breadcrumbs... 🍞";
        }

        public string GetWelcomeMessage(string playerName)
        {
            return $"Welcome {playerName} to the enchanted forest! 🌲\r\n\r\n" +
                   "You've just finished a delicious meal of Wolf Stew at Grandma's house, but now\r\n" +
                   "it's time to head home. Unfortunately, the birds have eaten all your breadcrumbs!\r\n" +
                   "You'll need to find another way through the forest to reach your home clearing.";
        }
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

        public string GetLevelDisplayName(MapLevel level)
        {
            return LevelDisplayNames.GetValueOrDefault(level, "Unknown Forest");
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
            return RoomNameToNumberMapping.GetValueOrDefault(cleanName, -1);
        }

        public string GetCompleteMapLegend()
        {
            return MapLegendContent + "\r\n" + MapLegendFooter;
        }
        #endregion
    }
}
