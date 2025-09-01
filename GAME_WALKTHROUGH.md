# Adventure House Game System - Complete Walkthrough Guide

## ?? System Overview

The Adventure House framework supports multiple text-based adventure games with a modern service-oriented architecture built on .NET 9. The system features:

- **Multi-Adventure Support**: Currently includes Adventure House and Space Station adventures
- **Advanced UI**: Both classic console and enhanced Spectre.Console modes
- **Dynamic Mapping**: Real-time ASCII maps showing player location and visited areas
- **Rich Combat System**: Monster encounters with weapons and companion assistance
- **Comprehensive Command System**: Natural language processing with synonym support

## ?? Getting Started

### Application Flow
1. **Launch** ? **Welcome Screen** ? **Adventure Selection** ? **Game Loop**
2. Choose between Adventure House (classic escape) or Space Station (sci-fi thriller)
3. Navigate with simple two-word commands
4. Complete objectives to win

### Essential Commands
```
Movement:     north, south, east, west, up, down (or n, s, e, w, u, d)
Items:        get [item], drop [item], inv, look [item]
Actions:      eat [item], use [item], read [item], wave [item], throw [item]
Combat:       attack [monster], activate [item]
Companions:   pet [animal], shoo [animal]
System:       help, health, points, quit, newgame
Console:      /map, /chelp, /time, /intro, /classic, /clear, /history
```

---

## ?? Adventure House Walkthrough (Game ID: 1)

### Starting Conditions
- **Player**: Random gamer tag (e.g., "Cute Gamer", "Minecraft Steve")
- **Starting Location**: Attic (Room 20)
- **Health**: 200/200
- **Starting Points**: 1
- **Objective**: Escape the house before you starve

### Quick Start Commands
```bash
# Basic orientation
look
inv
health
d
```

### Complete Step-by-Step Walkthrough

#### Phase 1: Initial Exploration
```bash
# Start in Attic (Room 20)
look                    # You are in the house attic. This room is musty and dark.
get kitten             # A delightful fuzzy kitten
pet kitten             # The cute kitten begins to follow you
get bugle              # A bugle. You were never very good with instruments.
d                      # Down to Master Bedroom Closet (Room 19)
w                      # West to Master Bedroom (Room 18)
```

#### Phase 2: Master Bedroom Area
```bash
# In Master Bedroom (Room 18)
look                   # The master bedroom with an inviting king size bed
get slip               # A small slip of paper (fortune cookie slip)
read slip              # Shows random developer fortune
get flyswatter         # A sturdy plastic fly swatter (weapon for mosquito)
n                      # North to Master Bedroom Bath (Room 21)
s                      # Back to Master Bedroom (Room 18)
s                      # South to Upstairs North Hallway (Room 13)
```

#### Phase 3: Upper Floor Exploration
```bash
# Navigate upper floor
w                      # West to Upstairs Bath (Room 17)
get wand               # A small wooden wand (teleportation item)
e                      # Back to Upstairs North Hallway (Room 13)
e                      # East to Upstairs Hallway (Room 11)
s                      # South to Upstairs East Hallway (Room 12)
s                      # South to Spare Room (Room 15)
n                      # Back to Upstairs East Hallway (Room 12)
n                      # Back to Upstairs Hallway (Room 11)
```

#### Phase 4: Ground Floor Access
```bash
# Go downstairs
d                      # Down to Downstairs Hallway (Room 2)
s                      # South to Living Room (Room 4)
w                      # West to Family Room (Room 5)
n                      # North to Nook (Room 6)
get bread              # A small loaf of bread (health restoration)
w                      # West to Deck (Room 24)
get key                # A shiny brushed metal key (CRITICAL ITEM)
```

#### Phase 5: Kitchen and Dining Area
```bash
# Explore kitchen area
e                      # Back to Nook (Room 6)
n                      # North to Kitchen (Room 7)
get apple              # A nice, red apple (health restoration)
n                      # North to Utility Hall (Room 8)
w                      # West to Garage (Room 9)
e                      # Back to Utility Hall (Room 8)
e                      # East to Main Dining Room (Room 10)
get pie                # A small slice of apple pie (health restoration)
```

#### Phase 6: Monster Combat (Optional)
```bash
# Navigate to Entertainment Room for monster encounter
s                      # South to Main Entrance (Room 1)
s                      # South to Downstairs Hallway (Room 2)
u                      # Up to Upstairs Hallway (Room 11)
w                      # West to Upstairs North Hallway (Room 13)
s                      # South to Upstairs West Hallway (Room 14)
s                      # South to Entertainment Room (Room 23)

# Monster combat (if mosquito appears - 60% chance)
look                   # Check for mosquito
attack mosquito        # With flyswatter: "You successfully attack the mosquito!"
                      # Without weapon: "You need the right weapon to attack the mosquito."
```

#### Phase 7: Victory Sequence
```bash
# Use key to unlock entrance and escape
n                      # Back to Upstairs West Hallway (Room 14)
n                      # North to Upstairs North Hallway (Room 13)
e                      # East to Upstairs Hallway (Room 11)
d                      # Down to Downstairs Hallway (Room 2)
n                      # North to Main Entrance (Room 1)
use key                # "The key fits perfectly and the door unlocked"
e                      # East to EXIT! (Room 0)
```

### Alternative Victory Path (Magic Route)
```bash
# From any location with the wand
wave wand              # Teleports to Main Entrance (Room 1)
use key                # Unlock the door
e                      # East to victory
```

### Health Management
```bash
# When health gets low ("bad" status)
eat bread              # Restores 100 health
eat apple              # Restores 25 health  
eat pie                # Restores 100 health
```

### Pet Management
```bash
# Kitten companion commands
pet kitten             # "The cute kitten begins to follow you"
shoo kitten            # "The cute kitten looks disappointed and runs away"
```

---

## ?? Space Station Walkthrough (Game ID: 2)

### Starting Conditions
- **Player**: Random gamer tag
- **Starting Location**: Hibernation Chamber (Room 1)
- **Health**: 300/300
- **Starting Points**: 10
- **Objective**: Escape via the pod before life support fails

### Quick Start Commands
```bash
# Basic orientation
look
inv
health
e
```

### Complete Step-by-Step Walkthrough

#### Phase 1: Command Level Exploration (Level 1)
```bash
# Start in Hibernation Chamber (Room 1)
look                   # Emergency lighting casts an eerie red glow
get ration             # A military-grade emergency food ration
e                      # East to Command Center (Room 2)
look                   # Multiple screens show system alerts
e                      # East to Captain's Office (Room 3)
get keycard            # A general access keycard
s                      # South to Secure Office (Room 4) - LOCKED
use keycard            # "The keycard opens the secure office door"
s                      # Now can enter Secure Office (Room 4)
get card               # Escape pod authorization card (CRITICAL ITEM)
n                      # Back to Captain's Office (Room 3)
w                      # West to Command Center (Room 2)
s                      # South to Robot Room (Room 5)
activate robot         # "The robot's systems come online with a cheerful beep"
s                      # South to Command Lift (Room 6)
d                      # Down to Eng Lift (Room 36) on Level 4
```

#### Phase 2: Engineering Level (Level 4)
```bash
# Critical items collection on Engineering Level
w                      # West to Escape Pod Bay (Room 35) - CHECK DESTINATION
look                   # "requires special authorization to activate"
e                      # Back to Eng Lift (Room 36)
n                      # North to Maintenance Bay (Room 31)
get flashlight         # A high-powered LED flashlight
w                      # West to Engine Room (Room 30)
w                      # West to Engineering Corridor (Room 27)
w                      # West to Main Engineering (Room 28)
w                      # West to Power Core (Room 29)
s                      # South to Tool Storage (Room 32)
get blaster            # Standard-issue plasma blaster (CRITICAL WEAPON)
get toolkit            # Comprehensive engineering toolkit
e                      # East to Spare Parts Room (Room 33)
```

#### Phase 3: Crew Level Exploration (Level 2)
```bash
# Navigate to crew level via lift
e                      # East to Waste Processing (Room 34)
n                      # North to Engineering Corridor (Room 27)
e                      # East to Engine Room (Room 30)
n                      # North to Escape Pod Bay (Room 35)
e                      # East to Eng Lift (Room 36)
u                      # Up to Command Lift (Room 6) on Level 1
# Alternative: from Engineering, find crew lift access
# Go to Room 16 (Crew Lift) via appropriate path

# At Crew Level (assuming reached Room 7)
w                      # West to Crew Mess Hall (Room 8)
get energybar          # High-energy protein bar
e                      # East to Crew Corridor (Room 7)
e                      # East to Captain's Quarters (Room 9)
s                      # South to Captain's Shower (Room 10)
get fob                # The captain's security fob (CRITICAL ITEM)
```

#### Phase 4: Science Level & Monster Combat (Level 3)
```bash
# Navigate to Science Level (specific path varies)
# Assuming at Science Corridor (Room 17)
w                      # West to Hydroponics Lab (Room 18)
# Combat: Cakebite may appear (70% chance)
attack cakebite        # With blaster: "Your blaster proves effective!"
                      # Without blaster: "You need the right weapon"

e                      # East to Science Corridor (Room 17)
e                      # East to Experiment Chamber (Room 22)
# Combat: Frostling may appear (75% chance)
attack frostling       # Use blaster

w                      # West to Science Corridor (Room 17)
s                      # South to Science Computer Core (Room 20)
get log                # Commander's personal log (story item)
read log               # "The engineer used antimatter accelerator to bake cake..."
s                      # South to Teleport Room (Room 23)
get teleporter         # Personal teleportation device
use teleporter         # "You materialize instantly in the Command Center!"
```

#### Phase 5: Monster Hunting & Item Collection
```bash
# Continue monster encounters for points
# From Command Center (Room 2), navigate to Science Level
# Visit Containment Lab (Room 24)
attack goopling        # Strongest monster (3 attacks to kill)

# Visit Analysis Lab (Room 25)  
attack sweetspore      # Use blaster

# Return to Engineering via lifts
# Visit Waste Processing (Room 34)
attack batterling      # Final monster
```

#### Phase 6: Health & Companion Management
```bash
# Health management
eat ration             # Restores 100 health
eat energybar          # Restores 75 health
use medikit            # Restores 150 health (if found)

# Robot companion
activate robot         # "Robot begins following you faithfully"
# Robot assists in combat and provides messages
shoo robot             # "Robot returns to charging station"
```

#### Phase 7: Victory Sequence
```bash
# Final escape (ensure you have CARD)
# Navigate to Escape Pod Bay (Room 35)
use card               # "The authorization card unlocks the escape pod bay"
n                      # North to Room 0 (Escape Pod - Freedom!)
# Victory message: "?? SUCCESS! You are safely aboard the escape pod..."
```

### Monster Combat Reference
```bash
# All monsters defeated with BLASTER:
Cakebite (Room 18):    2 hits, 35% hit chance, 20 damage
Frostling (Room 22):   2 hits, 40% hit chance, 25 damage  
Goopling (Room 24):    3 hits, 45% hit chance, 30 damage
Sweetspore (Room 25):  2 hits, 30% hit chance, 15 damage
Batterling (Room 34):  2 hits, 25% hit chance, 18 damage
```

### Critical Items Checklist
- ? **KEYCARD** (Room 3) - Unlocks Secure Office
- ? **CARD** (Room 4) - Escape pod authorization  
- ? **BLASTER** (Room 32) - Defeats all monsters
- ? **FOB** (Room 10) - Captain's security (bonus points)
- ? **ROBOT** (Room 5) - Companion assistance

---

## ?? Advanced Game Features

### Console Commands (Type with / prefix)
```bash
/map                   # ASCII map showing current location (@)
/chelp                 # Console command help
/time                  # Current date/time
/intro                 # Replay introduction
/classic               # Toggle classic/enhanced mode
/clear                 # Clear screen
/history               # Show command history
/scroll                # Toggle scroll mode
```

### Debug Commands (DEBUG mode only)
```bash
validate               # Comprehensive adventure validation
check                  # Alias for validate  
verify                 # Alias for validate
validateadventure      # Full form of validation command
```

### Health Status System
```bash
Excellent (190-300):   Full health, no warnings
Good (100-189):        Healthy, normal status
Bad (20-99):           "You feel hungry, tired and sick"
Critical (1-19):       "You will die soon unless you eat something"
Dead (0):              Game over, use "newgame" to restart
```

### Points System
```bash
points                 # Check current score
# Points awarded for:
# - Visiting new rooms (varies by room)
# - Using key items successfully  
# - Defeating monsters
# - Completing story objectives
```

### Command Synonyms
```bash
Movement:     go, run, move, climb ? directional movement
Items:        take, pickup, grab ? get
             put ? drop
Combat:       fight, hit, kill, slay, strike ? attack
Activation:   start, power, boot, turn ? activate
Examination:  examine ? look
Inventory:    inventory, pack ? inv
Scoring:      score, result ? points
Restart:      restart ? quit
```

### Special Mechanics

#### Adventure House Unique Features
- **Magic System**: Wand teleportation, magic mushroom room
- **Pet System**: Kitten companion with following messages
- **Key Progression**: Must find key to unlock main entrance
- **Fortune System**: Random developer messages from slip

#### Space Station Unique Features  
- **Multi-Level Design**: 4 distinct levels with lift system
- **Robot Companion**: Activatable assistant for combat/exploration
- **Emergency Teleporter**: One-way beam to Command Center
- **Story Discovery**: Commander's log reveals station's fate
- **Authorization System**: Requires proper clearance for escape

### Troubleshooting Commands
```bash
help                   # In-game help with story context
health                 # Current health status
inv                    # Check inventory contents
look                   # Examine current room
newgame                # Restart current adventure
quit                   # End game (or kill player if alive)
```

---

## ?? Victory Conditions

### Adventure House Success
- **Requirement**: Use KEY to unlock Main Entrance, then go East
- **Reward**: "CONGRATULATIONS! YOU WIN!" + points summary
- **Message**: Escape before starvation, continue exploring option

### Space Station Success  
- **Requirement**: Use CARD in Escape Pod Bay, then enter pod
- **Reward**: "?? ESCAPE POD LAUNCHED SUCCESSFULLY!" 
- **Message**: Cosmic appreciation from Adventure House team

Both games support continued exploration after victory, allowing players to return and discover additional content, secrets, and story elements.

---

## ?? Testing Checklist

### Core System Tests
- [ ] Application launches with intro screen
- [ ] Adventure selection presents both options
- [ ] Map system shows player location (@) 
- [ ] Health decreases over time
- [ ] Commands process with synonyms
- [ ] Items can be picked up/dropped/used
- [ ] Monster combat works with appropriate weapons
- [ ] Victory conditions trigger properly

### Adventure-Specific Tests

#### Adventure House
- [ ] Starts in Attic (Room 20) with 200 health
- [ ] Kitten pet/shoo mechanics work
- [ ] Flyswatter defeats mosquito
- [ ] Wand teleportation functions
- [ ] Key unlocks main entrance
- [ ] Magic realm accessible

#### Space Station  
- [ ] Starts in Hibernation Chamber (Room 1) with 300 health
- [ ] Robot activation/following works
- [ ] Lift system connects levels
- [ ] Blaster defeats all goopling types
- [ ] Authorization card unlocks escape pod
- [ ] Story log reveals background

This comprehensive walkthrough provides exact commands for completing both adventures while testing all major game systems and mechanics.