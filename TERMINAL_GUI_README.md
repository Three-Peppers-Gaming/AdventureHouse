# Adventure House - Terminal.Gui Implementation

This document describes the Terminal.Gui implementation added to the Adventure House multiplayer text adventure game client.

## Overview

The codebase now supports two interface modes:
1. **Console Mode**: Traditional text-based interface using Spectre.Console for enhanced formatting
2. **Terminal.Gui Mode**: Modern terminal interface with panels, menus, and interactive elements

## Architecture

The Terminal.Gui implementation follows the existing architecture patterns and maintains separation of concerns:

### Key Components

#### 1. MapModel (`Services/AdventureClient/Models/MapModel.cs`)
- **Purpose**: Renderer-agnostic data model for map information
- **Key Features**:
  - Holds room data, visited rooms, and player position
  - Supports multiple map levels
  - Provides connection data for path rendering
  - Can be used by different UI frameworks (console, Terminal.Gui, web, etc.)

#### 2. TerminalGuiRenderer (`Services/AdventureClient/UI/TerminalGuiRenderer.cs`)
- **Purpose**: Converts MapModel data into Terminal.Gui visual elements
- **Key Features**:
  - Renders ASCII maps with box-drawing characters
  - Creates Terminal.Gui FrameView components
  - Supports dynamic updates without recreating views
  - Uses color schemes for different room states

#### 3. TerminalGuiAdventureClient (`Services/AdventureClient/TerminalGuiAdventureClient.cs`)
- **Purpose**: Main Terminal.Gui client implementing IAdventureClient interface
- **Key Features**:
  - Game selection dialog
  - Multi-panel layout (map, description, legend, input, status)
  - Menu bar with additional features
  - Command history with arrow key navigation
  - Real-time game state updates

## Terminal.Gui Concepts Demonstrated

### Application Lifecycle
```csharp
Application.Init();        // Initialize terminal system
Application.Run(window);   // Enter main event loop
Application.Shutdown();    // Clean up resources
```

### View Hierarchy
- **Window**: Top-level container
- **FrameView**: Panels with borders and titles
- **TextView**: Scrollable text display
- **TextField**: User input field
- **Label**: Static text display
- **MenuBar**: Application menu system

### Layout System
```csharp
// Automatic sizing using Dim.Fill()
Width = Dim.Fill()

// Percentage-based layout
var leftWidth = (int)(totalWidth * 0.6);

// Responsive positioning
X = leftWidth, Y = topHeight
```

### Event Handling
```csharp
// Key press events
_inputField.KeyPress += OnInputKeyPress;

// Button click events
button.Clicked += () => { /* action */ };

// Menu item selection
new MenuItem("_Help", "", ShowHelp)
```

## Usage

### Command Line Options
```bash
# Interactive mode selection
AdventureRealms

# Direct Terminal.Gui mode
AdventureRealms --gui
AdventureRealms -g

# Show help
AdventureRealms --help
AdventureRealms -h
```

### Interface Layout

```
???????????????????????????????????????????????????????????????
? Game | View                                                   ? Menu Bar
???????????????????????????????????????????????????????????????
? ??????????????????????? ??????????????????????????????????? ?
? ? Map                 ? ? Game Description                ? ?
? ?                     ? ?                                 ? ?
? ? ASCII map display   ? ? Room description, items,        ? ?
? ? with rooms and      ? ? game messages, and response     ? ?
? ? connections         ? ? to player commands              ? ?
? ?                     ? ?                                 ? ?
? ??????????????????????? ??????????????????????????????????? ?
? ??????????????????????? ??????????????????????????????????? ?
? ? Legend              ? ? Command Input                   ? ?
? ?                     ? ?                                 ? ?
? ? Map symbols and     ? ? Enter command: [input field]   ? ?
? ? color explanations  ? ?                                 ? ?
? ??????????????????????? ??????????????????????????????????? ?
???????????????????????????????????????????????????????????????
? Status: Location | Health | Commands                        ? Status Bar
???????????????????????????????????????????????????????????????
```

### Game Controls

**Input Field Navigation:**
- `?/?` - Command history navigation
- `ESC` - Clear input field
- `Enter` - Execute command

**Menu Bar:**
- `Alt+G` - Game menu (Help, Map, Quit)
- `Alt+V` - View menu (Clear, History)

**Game Commands:**
- `go <direction>` - Move (north, south, east, west, up, down)
- `look` - Examine current location
- `get <item>` - Pick up items
- `drop <item>` - Drop items
- `inv` - Show inventory
- `help` - Game help
- `resign` - Quit game

## Map Rendering

### ASCII Map Format
```
+---+ +---+
|@ +| |.  |  @ = Player location
+---+ +---+  + = Items available
  :            : = Vertical connection
+---+          . = Horizontal connection  
|^  |          ^ = Stairs up
+---+          v = Stairs down
```

### Color Coding
- **Green**: Visited rooms
- **Cyan**: Current player location
- **White**: Rooms with items available

### Map Features
- Only visited rooms are displayed
- Connections shown between visited rooms
- Real-time updates as player explores
- Multi-level support with level indicators

## Technical Implementation

### Separation of Concerns

1. **Game Logic**: Handled entirely by the server (AdventureFrameworkService)
2. **Data Model**: MapModel provides renderer-agnostic data structure
3. **Rendering**: TerminalGuiRenderer handles all Terminal.Gui-specific code
4. **UI Logic**: TerminalGuiAdventureClient manages application flow and events

### Compatibility

The Terminal.Gui implementation:
- ? Implements the same IAdventureClient interface as console mode
- ? Uses the existing server API without modifications
- ? Supports all existing game features
- ? Maintains command history and game state
- ? Works with all existing adventure games

### Future Extensibility

The modular design allows for easy addition of new renderers:
- Web-based renderer using the same MapModel
- Mobile app renderer
- GUI desktop application
- VR/AR interfaces

## Development Notes

### Key Design Decisions

1. **MapModel as Data Transfer Object**: Keeps rendering logic separate from game logic
2. **Renderer Pattern**: TerminalGuiRenderer can be swapped for other implementations
3. **Event-Driven Updates**: UI updates only when game state changes
4. **Graceful Fallback**: Console mode remains as backup option

### Terminal.Gui Version
- **Target Version**: 1.19.0
- **Key Features Used**: FrameView, TextView, TextField, MenuBar, Dialog
- **Layout System**: Automatic sizing with Dim.Fill() and percentage-based positioning

### Error Handling
- Terminal.Gui initialization errors fall back to console mode
- Game server errors are displayed in the description panel
- UI errors are logged and don't crash the application

## Examples

### Creating a Custom Renderer

```csharp
public class WebRenderer
{
    public string RenderMapToHtml(MapModel mapModel)
    {
        // Convert map data to HTML/CSS
        var visitedRooms = mapModel.GetVisitedRoomsForCurrentLevel();
        // ... rendering logic
        return htmlString;
    }
}
```

### Adding New Map Features

```csharp
// In MapModel.cs
public class MapRoomData
{
    // Add new properties
    public bool HasMonster { get; set; }
    public string RoomColor { get; set; }
    
    // Rendering logic remains in renderer classes
}
```

This implementation demonstrates modern terminal UI development while maintaining the simplicity and extensibility of the original text adventure game architecture.