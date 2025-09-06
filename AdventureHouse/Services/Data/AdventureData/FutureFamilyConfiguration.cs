using System;
using System.Collections.Generic;
using AdventureHouse.Services.AdventureClient.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Configuration for Future Family Space Apartment escape adventure
    /// A satirical take on 1960s futuristic family life in a sky-high apartment
    /// </summary>
    public class FutureFamilyConfiguration : IGameConfiguration
    {
        #region Game Identity
        public string GameName => "Future Family Space Apartment";
        public string GameVersion => "1.0";
        public string GameDescription => "Escape from your retro-futuristic sky apartment before Space Control discovers you've been late for work at Galaxy Widgets Corp!";
        #endregion

        #region Room Display Configuration
        public Dictionary<string, char> RoomDisplayCharacters => new()
        {
            ["exit"] = 'T',      // Transport Tube
            ["entrance"] = 'L',  // Landing Pad
            ["living"] = 'F',    // Family Room
            ["kitchen"] = 'K',   // Kitchen
            ["bedroom"] = 'B',   // Bedroom
            ["bathroom"] = 'W',  // Wash Room
            ["storage"] = 'S',   // Storage
            ["utility"] = 'U',   // Utility
            ["elevator"] = 'E',  // Elevator
            ["balcony"] = 'O',   // Outside Balcony
            ["garage"] = 'G',    // Air Car Garage
            ["office"] = 'A',    // Home Office
            ["laundry"] = 'Y',   // Laundry Room
            ["closet"] = 'C',    // Closet
            ["nursery"] = 'N',   // Nursery
            ["debug"] = 'D'      // Debug Room
        };

        public Dictionary<string, char> RoomTypeCharacters => new()
        {
            ["family"] = 'F',
            ["service"] = 'S',
            ["private"] = 'P',
            ["utility"] = 'U',
            ["transport"] = 'T'
        };

        public Dictionary<string, int> RoomNameToNumberMapping => new()
        {
            ["exit"] = 0,
            ["tube"] = 0,
            ["entrance"] = 1,
            ["landing"] = 1,
            ["family"] = 2,
            ["living"] = 2,
            ["kitchen"] = 3,
            ["dining"] = 4,
            ["hallway"] = 5,
            ["bathroom"] = 6,
            ["wash"] = 6,
            ["master"] = 7,
            ["bedroom"] = 7,
            ["closet"] = 8,
            ["balcony"] = 9,
            ["garage"] = 10,
            ["storage"] = 11,
            ["laundry"] = 12,
            ["utility"] = 13,
            ["office"] = 14,
            ["nursery"] = 15,
            ["elevator"] = 16,
            ["roof"] = 17,
            ["basement"] = 18,
            ["mechanical"] = 19,
            ["shelter"] = 20
        };
        #endregion

        #region Game Settings
        public int StartingRoom => 7; // Master Bedroom - wake up late!
        public int MaxHealth => 175;
        public int HealthStep => 2;
        public int StartingPoints => 1;
        public List<string> InitialPointsCheckList => new() { "WakeUpLate" };

        // Constants
        public int InventoryLocation => 9999;
        public int PetFollowLocation => 9998;
        public int NoConnectionValue => 99;
        public char DefaultRoomCharacter => '.';
        #endregion

        #region Map Configuration
        public Dictionary<int, MapLevel> RoomToLevelMapping => new()
        {
            [0] = MapLevel.Exit,
            [1] = MapLevel.UpperFloor,    // Landing Pad (top level)
            [2] = MapLevel.UpperFloor,    // Family Room
            [3] = MapLevel.UpperFloor,    // Kitchen
            [4] = MapLevel.UpperFloor,    // Dining Room
            [5] = MapLevel.UpperFloor,    // Hallway
            [6] = MapLevel.UpperFloor,    // Bathroom
            [7] = MapLevel.GroundFloor,   // Master Bedroom (main level)
            [8] = MapLevel.GroundFloor,   // Master Closet
            [9] = MapLevel.GroundFloor,   // Balcony
            [10] = MapLevel.GroundFloor,  // Air Car Garage
            [11] = MapLevel.GroundFloor,  // Storage Room
            [12] = MapLevel.GroundFloor,  // Laundry Room
            [13] = MapLevel.GroundFloor,  // Utility Room
            [14] = MapLevel.GroundFloor,  // Home Office
            [15] = MapLevel.GroundFloor,  // Nursery
            [16] = MapLevel.Attic,        // Elevator Shaft
            [17] = MapLevel.Attic,        // Roof Access
            [18] = MapLevel.MagicRealm,   // Basement
            [19] = MapLevel.MagicRealm,   // Mechanical Room
            [20] = MapLevel.MagicRealm,   // Emergency Shelter
#if DEBUG
            [88] = MapLevel.Attic         // Debug Room
#endif
        };

        public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
        {
            [MapLevel.GroundFloor] = (8, 6),    // Main apartment level
            [MapLevel.UpperFloor] = (8, 4),     // Upper family areas
            [MapLevel.Attic] = (6, 3),          // Service areas
            [MapLevel.MagicRealm] = (6, 3),     // Basement and shelter
            [MapLevel.Exit] = (3, 3)            // Transport tube exit
        };

        public Dictionary<int, (int X, int Y)> RoomPositions => new()
        {
            // Exit level
            [0] = (1, 1),   // Transport Tube Exit

            // Upper Floor (Family Areas)
            [1] = (4, 1),   // Landing Pad
            [2] = (2, 2),   // Family Room
            [3] = (4, 2),   // Kitchen
            [4] = (6, 2),   // Dining Room
            [5] = (4, 3),   // Upper Hallway
            [6] = (6, 3),   // Bathroom

            // Ground Floor (Main Level)
            [7] = (2, 2),   // Master Bedroom
            [8] = (1, 2),   // Master Closet
            [9] = (3, 1),   // Balcony
            [10] = (1, 1),  // Air Car Garage
            [11] = (5, 1),  // Storage Room
            [12] = (6, 2),  // Laundry Room
            [13] = (7, 2),  // Utility Room
            [14] = (2, 3),  // Home Office
            [15] = (4, 3),  // Nursery
            [16] = (6, 3),  // Elevator Shaft

            // Attic (Service Areas)
            [17] = (3, 1),  // Roof Access

            // Basement/Underground
            [18] = (2, 1),  // Basement
            [19] = (4, 1),  // Mechanical Room
            [20] = (3, 2),  // Emergency Shelter

#if DEBUG
            [88] = (3, 2)   // Debug Room
#endif
        };

        public Dictionary<MapLevel, string> LevelDisplayNames => new()
        {
            [MapLevel.GroundFloor] = "Main Apartment",
            [MapLevel.UpperFloor] = "Upper Level",
            [MapLevel.Attic] = "Service Level",
            [MapLevel.MagicRealm] = "Underground",
            [MapLevel.Exit] = "Transport Tube!"
        };
        #endregion

        #region Interface Properties
        public Dictionary<string, char> RoomCharacterMapping => RoomDisplayCharacters;
        public Dictionary<int, (int X, int Y)> RoomPositionMapping => RoomPositions;
        public string MapLegend => GetCompleteMapLegend();
        #endregion

        #region Help and Story Text
        public string GetAdventureHelpText()
        {
            return @"=== FUTURE FAMILY SPACE APARTMENT ESCAPE ===

THE SITUATION:
You're George Spacely, a typical space-age family man living in 2087. You've overslept AGAIN 
and your boss Mr. Cogswell at Galaxy Widgets Corp is going to fire you if you're late one 
more time! Your wife Jane is visiting her mother on Mars, and you need to get to work before 
Space Control discovers you've been goofing off.

YOUR MISSION:
Escape from your sky-high apartment to catch the transport tube to Galaxy Widgets Corp.
You'll need to find your work clothes, grab some breakfast, and locate your transport pass.
Watch out for your robot maid Robotina - she's been acting up lately!

STORY:
The year is 2087, and the future isn't quite what they promised. Sure, you live in a 
floating apartment 2,000 feet above Neo Angeles, but the robot servants are glitchy, 
the food pills taste terrible, and your flying car keeps breaking down. Your son Elroy 
is at Space School, your daughter Judy is at the Cosmic Mall, and your wife Jane is 
visiting her mother on Mars. It's just you, your neurotic robot dog Nutso, and a 
malfunctioning house full of ""convenient"" future technology.

GOAL:
Find the Transport Pass and reach the Transport Tube to escape to work at Galaxy Widgets Corp!

CONTROLS:
• Move: N, S, E, W, U (up), D (down)
• Get items: GET [item] or TAKE [item]
• Use items: USE [item] or specific action verbs
• Drop items: DROP [item]
• Look: LOOK or LOOK [item]
• Inventory: I or INVENTORY
• Pet your robot dog: PET NUTSO
• Attack (if needed): ATTACK [monster] WITH [weapon]
• Map: MAP (shows current level layout)
• Help: HELP or ?
• Quit: QUIT or EXIT

TIPS:
• Explore all rooms - you never know what you'll find
• Some doors require special items to unlock
• Robot appliances can be helpful or harmful
• Your robot dog Nutso might help in dangerous situations
• Future food is surprisingly nutritious
• Read everything - the future is documented extensively

Remember: In the space age, even getting to work is an adventure!
This groovy retro-future world of 2087 awaits your exploration.
Can you make it to Galaxy Widgets Corp before Mr. Cogswell fires you?

The transport tube to success is waiting... if you can reach it!";
        }

        public string GetAdventureThankYouText()
        {
            return @"?? CONGRATULATIONS, SPACE CADET! ??

You've successfully escaped from your Future Family Space Apartment and made it to the 
Galaxy Widgets Corp transport tube! Mr. Cogswell will have to wait another day to fire you.

Your adventure through the retro-futuristic world of 2087 is complete. You've:
• Navigated the perils of space-age apartment living
• Mastered futuristic technology (sort of)
• Survived another day in the space suburbs
• Kept your job at Galaxy Widgets Corp (for now)

The year 2087 may have flying cars and robot maids, but some things never change - 
like oversleeping, rushing to work, and dealing with glitchy technology!

Thanks for playing Future Family Space Apartment!
Now get to work before Mr. Cogswell starts yelling about quarterly reports!

?? MISSION ACCOMPLISHED! ??

(Jane will never believe you made it to work on time today...)";
        }

        public string GetWelcomeMessage(string gamerTag)
        {
            return $@"Welcome to the Future Family Space Apartment, {gamerTag}!

?? THE YEAR IS 2087 ??

You are George Spacely, everyman of the space age! You live 2,000 feet above Neo Angeles 
in a fully automated apartment with your wife Jane, son Elroy, daughter Judy, robot maid 
Robotina, and neurotic robot dog Nutso.

But today is NOT your day...

?? EMERGENCY SITUATION ??
You've overslept AGAIN! Your antigrav alarm clock malfunctioned, Jane is visiting her 
mother on Mars, and you're supposed to be at Galaxy Widgets Corp in 30 minutes or 
Mr. Cogswell will fire you!

The kids are at their respective space activities, but Robotina has been acting strange 
lately, and Nutso seems more nervous than usual. Something's not right in your 
futuristic paradise...

Your mission: Get dressed, grab breakfast, find your transport pass, and escape 
to the transport tube before Space Control notices you're goofing off!

Welcome to the groovy but glitchy world of 2087, {gamerTag}!
Can you survive another day in space suburbia?

Type HELP for commands, or just start exploring your space-age home!";
        }
        #endregion

        #region Map Legend
        public string GetCompleteMapLegend()
        {
            return @"Future Family Space Apartment - Map Legend:

APARTMENT AREAS:
T = Transport Tube (EXIT!)
L = Landing Pad (apartment entrance)
F = Family Room (main living area)
K = Kitchen (space-age cooking)
B = Bedroom areas
W = Wash Room (bathroom)
S = Storage areas
E = Elevator Shaft
O = Outside Balcony (great view!)
G = Air Car Garage
A = Home Office (work from space!)
Y = Laundry Room
C = Closets
N = Nursery
U = Utility areas
D = Debug Room (developer only)

LEVEL NAMES:
Main Apartment = Primary living spaces
Upper Level = Family and dining areas  
Service Level = Maintenance and elevators
Underground = Basement and emergency areas
Transport Tube! = FREEDOM!

Your goal is to reach the Transport Tube (T) to escape to Galaxy Widgets Corp!
Navigate through your retro-futuristic apartment and avoid the glitchy robot maid!";
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
            var lowerName = roomName.ToLower();
            foreach (var kvp in RoomNameToNumberMapping)
            {
                if (lowerName.Contains(kvp.Key))
                    return kvp.Value;
            }
            return 1; // Default to starting room
        }
        #endregion
    }
}