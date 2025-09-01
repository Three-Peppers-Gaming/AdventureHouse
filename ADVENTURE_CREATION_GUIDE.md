# Adventure Creation Guide for Copilot Sessions

This guide provides comprehensive instructions for creating new adventures in the Adventure House framework. Follow these patterns and conventions to ensure compatibility with the existing game engine.

## Table of Contents
1. [Project Structure Overview](#project-structure-overview)
2. [Adventure Data Components](#adventure-data-components)
3. [Configuration Class Pattern](#configuration-class-pattern)
4. [Data Creation Patterns](#data-creation-patterns)
5. [Game Mechanics Integration](#game-mechanics-integration)
6. [Debug Features](#debug-features)
7. [Testing and Validation](#testing-and-validation)
8. [Common Pitfalls and Solutions](#common-pitfalls-and-solutions)

## Project Structure Overview

The Adventure House framework uses a service-based architecture with the following key components:

```
AdventureHouse/
??? Services/
?   ??? Data/
?   ?   ??? AdventureData/
?   ?       ??? IGameConfiguration.cs (Interface)
?   ?       ??? AdventureHouseConfiguration.cs (Example implementation)
?   ?       ??? [YourAdventure]Configuration.cs (New adventure config)
?   ?       ??? [YourAdventure]Data.cs (New adventure data)
?   ??? Models/ (Shared game models)
?   ??? AdventureFrameworkService.cs (Main game engine)
```

## Adventure Data Components

Every adventure requires these core components:

### 1. Rooms (`List<Room>`)
- **Required Properties**: Number, Name, Desc, N/S/E/W/U/D connections
- **Convention**: Use `99` for blocked directions (NoConnectionValue)
- **Best Practice**: Include room points for scoring system

### 2. Items (`List<Item>`)
- **Required Properties**: Name, Description, Location
- **Action System**: ActionVerb, ActionResult, ActionValue, ActionPoints
- **Location Conventions**:
  - Room numbers: Place item in specific room
  - `9999`: Player inventory (InventoryLocation)
  - `9998`: Pet following location (PetFollowLocation)
  - `99`: Unused/stored items (NoConnectionValue)

### 3. Messages (`List<Message>`)
- **Purpose**: Provide variety in game responses
- **Convention**: Multiple messages per tag for randomization
- **Essential Tags**: "dead", "bad", "petfollow"
- **Action Tags**: "GetSuccess", "GetFailed", "EatSuccess", etc.

### 4. Monsters (`List<Monster>`)
- **Combat System**: AttacksToKill, CanHitPlayer, HitOdds, HealthDamage
- **Weapon System**: ObjectNameThatCanAttackThem (item name)
- **Behavior**: AppearanceChance, PetAttackChance

## Configuration Class Pattern

Create a new configuration class implementing `IGameConfiguration`:

```csharp
public class [YourAdventure]Configuration : IGameConfiguration
{
    #region Game Identity
    public string GameName => "[Your Adventure Name]";
    public string GameVersion => "1.0";
    public string GameDescription => "[Brief description]";
    #endregion

    #region Room Display Configuration
    public Dictionary<string, char> RoomDisplayCharacters => new()
    {
        ["exit"] = 'X',
        ["entrance"] = 'E',
        // Add room-specific display characters
    };

    public Dictionary<string, char> RoomTypeCharacters => new()
    {
        ["hallway"] = 'H',
        ["bedroom"] = 'B',
        // Add room type patterns
    };

    public Dictionary<string, int> RoomNameToNumberMapping => new()
    {
        ["exit"] = 0,
        ["entrance"] = 1,
        // Map all room names to numbers
    };
    #endregion

    #region Game Settings
    public int StartingRoom => 1; // Adjust as needed
    public int MaxHealth => 200;
    public int HealthStep => 2;
    public int StartingPoints => 1;
    public string InitialPointsCheckList => "NewGame";
    
    // Constants - keep these values
    public int InventoryLocation => 9999;
    public int PetFollowLocation => 9998;
    public int NoConnectionValue => 99;
    public char DefaultRoomCharacter => '.';
    #endregion

    #region Map Configuration
    public Dictionary<int, MapLevel> RoomToLevelMapping => new()
    {
        [0] = MapLevel.Exit,
        [1] = MapLevel.GroundFloor,
        // Map rooms to appropriate levels
    };

    public Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes => new()
    {
        [MapLevel.GroundFloor] = (8, 7),
        [MapLevel.UpperFloor] = (8, 9),
        // Define grid sizes for each level
    };

    public Dictionary<int, (int X, int Y)> RoomPositions => new()
    {
        [0] = (4, 3), // Exit position
        [1] = (4, 6), // Entrance position
        // Define X,Y coordinates for each room
    };
    #endregion

    #region Help and Story Text
    public string GetAdventureHelpText()
    {
        return "[Your adventure-specific help text with story and commands]";
    }

    public string GetAdventureThankYouText()
    {
        return "[Congratulations message when player wins]";
    }

    public string GetWelcomeMessage(string gamerTag)
    {
        return $"Welcome {gamerTag}, [adventure-specific welcome message]";
    }
    #endregion

    // Implement remaining interface methods...
}
```

## Data Creation Patterns

### Room Creation Pattern
```csharp
private static List<Room> Rooms()
{
    var rooms = new List<Room>
    {
        // Exit room (always room 0)
        new Room { 
            Number = 0, 
            RoomPoints = 100, 
            Name = "Exit", 
            Desc = "You have escaped!", 
            N = 99, S = 99, E = 99, W = 1, U = 99, D = 99 
        },
        
        // Starting room
        new Room { 
            Number = 1, 
            RoomPoints = 25, 
            Name = "Entrance", 
            Desc = "The adventure begins here.", 
            N = 2, S = 99, E = 99, W = 0, U = 99, D = 99 
        },
        
        // Add more rooms following this pattern
    };
    return rooms;
}
```

### Item Creation Pattern
```csharp
private static List<Item> Items()
{
    var items = new List<Item>
    {
        // Food item example
        new Item { 
            Name = "APPLE", 
            Description = "A fresh red apple.", 
            Location = 1, 
            Action = "Tastes delicious and refreshing.", 
            ActionVerb = "EAT", 
            ActionResult = "HEALTH", 
            ActionValue = "25", 
            ActionPoints = 10 
        },
        
        // Tool/weapon item example
        new Item { 
            Name = "SWORD", 
            Description = "A sharp steel sword.", 
            Location = 2, 
            Action = "You swing the sword with confidence.", 
            ActionVerb = "ATTACK", 
            ActionResult = "WEAPON", 
            ActionValue = "DRAGON", 
            ActionPoints = 50 
        },
        
        // Key/unlock item example
        new Item { 
            Name = "KEY", 
            Description = "An ornate golden key.", 
            Location = 3, 
            Action = "The key turns smoothly in the lock.", 
            ActionVerb = "USE", 
            ActionResult = "UNLOCK", 
            ActionValue = "1|N|2|The door is now open.|Door unlocked!", 
            ActionPoints = 100 
        },

#if DEBUG
        // Debug items (only in debug builds)
        new Item { 
            Name = "DEBUGWAND", 
            Description = "Developer's magic wand.", 
            Location = 0, 
            Action = "Reality bends to your will.", 
            ActionVerb = "WAVE", 
            ActionResult = "TELEPORT", 
            ActionValue = "88", 
            ActionPoints = 0 
        },
#endif
    };
    return items;
}
```

### Action Result Types
- **HEALTH**: Modify player health (ActionValue = health change amount)
- **TELEPORT**: Move player to room (ActionValue = room number)
- **UNLOCK**: Unlock doors (ActionValue = "roomNum|direction|newRoom|newDesc1|newDesc2")
- **WEAPON**: Attack monsters (ActionValue = monster key name)
- **FOLLOW**: Pet follows player (ActionValue = item location)
- **FORTUNE**: Display random fortune (ActionValue = fortune type)

### Message Creation Pattern
```csharp
private static List<Message> Messages()
{
    var messages = new List<Message>
    {
        // Direction blocking messages
        new Message { MessageTag = "North", Messsage = "You can't go north from here." },
        new Message { MessageTag = "North", Messsage = "There's a wall blocking your path north." },
        
        // Action result messages
        new Message { MessageTag = "GetSuccess", Messsage = "You pick up the @." },
        new Message { MessageTag = "GetSuccess", Messsage = "The @ fits perfectly in your pack." },
        
        // Essential status messages
        new Message { MessageTag = "Dead", Messsage = "You have died. Game over!" },
        new Message { MessageTag = "Bad", Messsage = "You feel weak and need food soon." },
        
        // Pet messages (if adventure has pets)
        new Message { MessageTag = "PetFollow", Messsage = "Your pet companion follows loyally." },
        
        // Monster combat messages
        new Message { MessageTag = "MonsterAppear", Messsage = "A fearsome @ blocks your path!" },
        new Message { MessageTag = "AttackSuccess", Messsage = "You defeat the @ with your @!" },
    };
    return messages;
}
```

### Monster Creation Pattern
```csharp
private static List<Monster> Monsters()
{
    var monsters = new List<Monster>
    {
        new Monster 
        { 
            Key = "DRAGON", 
            Name = "Dragon", 
            Description = "A massive fire-breathing dragon guards the treasure.",
            RoomNumber = 5, // Room where monster appears
            ObjectNameThatCanAttackThem = "SWORD", // Required weapon
            AttacksToKill = 3, // How many hits to defeat
            CurrentHealth = 3, // Initialize to AttacksToKill
            CanHitPlayer = true,
            HitOdds = 40, // 40% chance to hit player each turn
            HealthDamage = 20, // Damage dealt to player
            AppearanceChance = 80, // 80% chance to appear when entering room
            IsPresent = false, // Start as not present
            PetAttackChance = 25 // 25% chance pet will help attack
        },
    };
    return monsters;
}
```

## Game Mechanics Integration

### Health System
- Use positive ActionValue for healing items
- Use negative ActionValue for harmful items
- MaxHealth and HealthStep control health mechanics

### Point System
- Set RoomPoints for rooms (first visit awards points)
- Set ActionPoints for item usage
- Points contribute to final score

### Combat System
- Monsters require specific weapons (ObjectNameThatCanAttackThem)
- Pet companions can assist in combat (PetAttackChance)
- Multiple hits may be required (AttacksToKill)

### Movement System
- Use room connections (N/S/E/W/U/D) for movement
- Use 99 for blocked directions
- TELEPORT items can bypass normal movement

## Debug Features

Always include debug features for testing:

```csharp
#if DEBUG
// Debug room for testing
new Room { 
    Number = 88, 
    Name = "Debug Room", 
    Desc = "Developer testing area", 
    U = 1, D = 0 // Connect to entrance and exit
},

// Debug items for testing
new Item { 
    Name = "DEBUGHEALTH", 
    Location = 88, 
    ActionVerb = "EAT", 
    ActionResult = "HEALTH", 
    ActionValue = "1000" 
},
#endif
```

The framework provides a `validateadventure` command in debug mode that will check your adventure configuration for common issues.

## Testing and Validation

### Using the Validation Command
In debug builds, use these commands to validate your adventure:
- `validateadventure` (full command)
- `validate` (short form)
- `check` (synonym)
- `verify` (synonym)

### Common Validation Checks
1. **Room Connectivity**: All room connections point to existing rooms
2. **Item Locations**: All items are placed in valid rooms or special locations
3. **Monster Weapons**: All monsters reference existing items as weapons
4. **Starting Room**: The starting room exists in the room list
5. **Essential Messages**: Required message tags are present

### Manual Testing Checklist
- [ ] Can reach exit room from starting room
- [ ] All items can be picked up and used
- [ ] Combat system works with weapons and monsters
- [ ] Health system functions (eating/damage)
- [ ] All rooms are accessible or intentionally isolated
- [ ] Point system awards points correctly
- [ ] Help text is informative and complete

## Common Pitfalls and Solutions

### 1. Room Connection Issues
**Problem**: Room connections creating infinite loops or dead ends
**Solution**: Draw a map on paper first, use validation command

### 2. Item Action Format
**Problem**: UNLOCK ActionValue format is complex
**Format**: "roomNumber|direction|newRoom|newDescription1|newDescription2"
**Example**: "1|N|2|The door is open.|Door unlocked successfully"

### 3. Monster Weapon References
**Problem**: Monster weapon names don't match item names exactly
**Solution**: Use exact item names (case-sensitive), validate with debug command

### 4. Missing Essential Messages
**Problem**: Game crashes due to missing message tags
**Solution**: Include at minimum: "dead", "bad", "petfollow" message tags

### 5. Debug vs Release Differences
**Problem**: Debug items/rooms not available in release builds
**Solution**: Use `#if DEBUG` preprocessor directives consistently

### 6. Map Level Configuration
**Problem**: Room positions overlap or don't make visual sense
**Solution**: Plan grid layout carefully, test with map display

## Adventure Creation Checklist

- [ ] Create new configuration class implementing IGameConfiguration
- [ ] Create new data class with all four data methods (Rooms, Items, Messages, Monsters)
- [ ] Define starting room and exit room (always room 0)
- [ ] Plan room connectivity and test navigation paths
- [ ] Create meaningful items with appropriate actions
- [ ] Include variety in message responses
- [ ] Design combat encounters with appropriate weapons
- [ ] Add debug rooms and items for testing
- [ ] Configure map display and room positioning
- [ ] Write help text explaining the adventure's story and mechanics
- [ ] Test with validation command in debug mode
- [ ] Playtest complete adventure from start to finish

## Integration Steps

1. Create your configuration and data classes
2. Update the main service to use your new adventure data
3. Test thoroughly in debug mode with validation
4. Create adventure-specific unit tests if needed
5. Document any special mechanics or Easter eggs

Remember: The Adventure House framework is designed to be flexible while maintaining consistency. Follow these patterns, and your new adventure will integrate seamlessly with the existing game engine!