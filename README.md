# Adventure Realms

A collection of classic text adventure games built with .NET 9.0, featuring both console and modern Terminal.Gui interfaces.

## Games Included

- **Adventure House** - Explore a house and escape before you starve
- **Abandoned Space Station** - Navigate a mysterious space station and find the escape pod
- **Future Family Space Apartment** - Explore a futuristic apartment with advanced technology
- **Lost in the Woods** - Find your way home through an enchanted forest

## Features

- **Dual Interface Modes**: Choose between classic console mode and modern graphical Terminal.Gui interface
- **Professional UI**: Real-time map display, organized content areas, mouse support
- **Cross-Platform**: Runs on Windows, macOS, and Linux
- **Self-Contained**: Can be built as standalone executables requiring no .NET installation
- **Comprehensive Testing**: Full test suite ensuring game completion and mechanics validation

## Quick Start

### Prerequisites
- .NET 9.0 SDK or later

### Running the Application

#### Development Mode
```bash
# Clone the repository
git clone https://github.com/StevenSSparks/AdventureRealms.git
cd AdventureRealms

# Run in development mode
dotnet run -f net9.0 --project AdventureRealms/AdventureRealms.csproj
```

#### Build and Run
```bash
# Build the project
dotnet build

# Run the built application
dotnet run --project AdventureRealms
```

## Building Release Versions

### Self-Contained Standalone Executables

Build standalone executables that include the .NET runtime and require no .NET installation on the target machine:

#### macOS (Apple Silicon - ARM64)
```bash
dotnet publish AdventureRealms/AdventureRealms.csproj -c Release -f net9.0 --self-contained true -r osx-arm64 -o ./publish/osx-arm64
```

#### macOS (Intel - x64)
```bash
dotnet publish AdventureRealms/AdventureRealms.csproj -c Release -f net9.0 --self-contained true -r osx-x64 -o ./publish/osx-x64
```

#### Windows (x64)
```bash
dotnet publish AdventureRealms/AdventureRealms.csproj -c Release -f net9.0 --self-contained true -r win-x64 -o ./publish/win-x64
```

#### Windows (ARM64)
```bash
dotnet publish AdventureRealms/AdventureRealms.csproj -c Release -f net9.0 --self-contained true -r win-arm64 -o ./publish/win-arm64
```

#### Linux (x64)
```bash
dotnet publish AdventureRealms/AdventureRealms.csproj -c Release -f net9.0 --self-contained true -r linux-x64 -o ./publish/linux-x64
```

#### Linux (ARM64)
```bash
dotnet publish AdventureRealms/AdventureRealms.csproj -c Release -f net9.0 --self-contained true -r linux-arm64 -o ./publish/linux-arm64
```

### Command Parameters Explained

- `-c Release` - Release configuration (optimized build)
- `-f net9.0` - Target framework (required for multi-target projects)
- `--self-contained true` - Includes .NET runtime in the output
- `-r <runtime>` - Target platform runtime identifier
- `-o <path>` - Output directory for the published application

### Running Published Applications

After building, run the standalone executable directly:

```bash
# macOS/Linux
./publish/osx-x64/AdventureRealms

# Windows
./publish/win-x64/AdventureRealms.exe
```

## Testing

Run the comprehensive test suite to validate game mechanics:

```bash
# Run all tests
dotnet test

# Run specific game walkthrough tests
dotnet test --filter "CompleteGameWalkthroughTests"

# Run item interaction tests
dotnet test --filter "ItemInteractionTests"
```

## Game Commands

### Movement
- `n`, `s`, `e`, `w`, `u`, `d` (or full names: north, south, east, west, up, down)

### Actions
- `look` - Examine your surroundings
- `get <item>` / `take <item>` - Pick up items
- `drop <item>` - Drop items from inventory
- `use <item>` - Use items in your inventory
- `inv` / `inventory` - View your current inventory

### Special Commands
- `help` - Show game-specific help
- `map` - Display current level map (where available)
- `eat <item>` - Consume food items
- `pet <animal>` - Interact with animals
- `attack <monster>` - Combat actions

## Interface Modes

### Console Mode
Traditional text-based interface with enhanced colors via Spectre.Console.

### Terminal.Gui Mode
Modern graphical text interface featuring:
- Real-time map display
- Organized content panels (game text, map, items, status)
- Mouse support and keyboard navigation
- Professional visual design with color schemes

## Architecture

### Clean Separation
- **AdventureServer**: Pure game logic with zero UI dependencies
- **AdventureClient**: Pure UI/UX with zero game logic dependencies
- **Shared**: Clean communication contracts between client and server

### Multi-Interface Support
The architecture supports multiple interface types:
- Console mode (implemented)
- Terminal.Gui mode (implemented)
- Future: Web, mobile, and cloud deployment ready

## Development

### Project Structure
```
AdventureRealms/
├── AdventureRealms/           # Main application
│   ├── Services/
│   │   ├── AdventureServer/   # Game logic
│   │   ├── AdventureClient/   # UI implementations
│   │   └── Shared/           # Communication contracts
├── AdventureRealms.Tests/     # Comprehensive test suite
└── Setup/                     # Installation projects
```

### Key Technologies
- .NET 9.0
- Terminal.Gui (for modern text UI)
- Spectre.Console (for enhanced console output)
- xUnit (for testing)
- MemoryCache (for session management)

## License

Free to use for non-commercial purposes.

## Credits

Developed by **Steven S. Sparks**
- GitHub: https://github.com/StevenSSparks
- Adventure Realms: Classic text adventure gaming, modernized

---

*Thank you for exploring Adventure Realms! We hope you enjoy these classic-style text adventures with modern enhancements.*
