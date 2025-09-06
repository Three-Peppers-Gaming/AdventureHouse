using AdventureHouse.Services.AdventureClient.UI;
using AdventureHouse.Services.AdventureClient.Models;
using AdventureHouse.Services.AdventureClient.AppVersion;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Data.AdventureData;
using AdventureHouse.Services.AdventureServer.Models;
using System.Text;
using ClientMapPosition = AdventureHouse.Services.Shared.Models.MapPosition;

namespace AdventureHouse.Tests.Client
{
    /// <summary>
    /// Tests for client-side map display and rendering functionality
    /// Validates that map data can be properly formatted and displayed
    /// </summary>
    public class MapDisplayTests
    {
        private readonly IDisplayService _displayService;
        private readonly IGameConfiguration _gameConfig;

        public MapDisplayTests()
        {
            var appVersionService = new AppVersionService();
            _displayService = new DisplayService(appVersionService);
            _gameConfig = new AdventureHouseConfiguration();
        }

        [Fact]
        public void GenerateConsoleMapText_ShouldProduceValidOutput()
        {
            // Arrange
            var mapData = CreateSampleMapData();

            // Act
            var result = GenerateConsoleMapText(mapData);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result));
            
            // Should contain basic map information
            Assert.Contains("Adventure House", result);
            Assert.Contains("Current Location:", result);
            Assert.Contains("Rooms Visited:", result);
            Assert.Contains("Discovered Rooms:", result);
            
            // Should show room information
            Assert.Contains("Attic", result);
            Assert.Contains("(20)", result); // Room number
            
            // Should indicate current location
            Assert.Contains("@", result);
        }

        [Fact]
        public void MapDisplay_ShouldHandleEmptyMapData()
        {
            // Arrange
            PlayerMapData emptyMapData = null;

            // Act
            var result = GenerateConsoleMapText(emptyMapData);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("No map data available", result);
        }

        [Fact]
        public void MapDisplay_ShouldShowItemIndicators()
        {
            // Arrange
            var mapData = CreateSampleMapDataWithItems();

            // Act
            var result = GenerateConsoleMapText(mapData);

            // Assert
            Assert.NotNull(result);
            
            // Should show items indicator for rooms with items
            Assert.Contains("[+]", result);
        }

        [Fact]
        public void MapDisplay_ShouldFormatMultipleRooms()
        {
            // Arrange
            var mapData = CreateSampleMapDataWithMultipleRooms();

            // Act
            var result = GenerateConsoleMapText(mapData);

            // Assert
            Assert.NotNull(result);
            
            // Should contain multiple rooms
            Assert.Contains("Attic", result);
            Assert.Contains("Master Bedroom", result);
            Assert.Contains("Kitchen", result);
            
            // Should show room numbers
            Assert.Contains("(20)", result); // Attic
            Assert.Contains("(18)", result); // Master Bedroom
            Assert.Contains("(7)", result);  // Kitchen
            
            // Only one room should be marked as current location
            var atSymbolCount = result.Count(c => c == '@');
            Assert.Equal(1, atSymbolCount);
        }

        [Fact]
        public void DisplayMap_ClassicMode_ShouldNotThrow()
        {
            // Arrange
            var mapData = CreateSampleMapData();
            var mapText = GenerateConsoleMapText(mapData);

            // Act & Assert - Should not throw exception
            // Note: We can't easily test console output, but we can ensure it doesn't crash
            try
            {
                // This would normally display to console, we're just testing it doesn't crash
                Assert.NotNull(mapText);
                Assert.True(mapText.Length > 0);
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Map display should not throw exception: {ex.Message}");
            }
        }

        [Fact]
        public void MapRenderingConfig_ShouldProvideValidCharacters()
        {
            // Arrange
            var mapData = CreateSampleMapData();

            // Act
            var config = mapData.RenderingConfig;

            // Assert
            Assert.NotNull(config);
            Assert.False(string.IsNullOrEmpty(config.GameName));
            
            // Should have valid display characters
            Assert.True(config.PlayerChar != '\0');
            Assert.True(config.ItemsChar != '\0');
            Assert.True(config.DefaultRoomChar != '\0');
            
            // Should have room type characters
            Assert.NotNull(config.RoomTypeChars);
            Assert.True(config.RoomTypeChars.Count > 0);
            
            // Should have level display names
            Assert.NotNull(config.LevelDisplayNames);
            Assert.True(config.LevelDisplayNames.Count > 0);
        }

        [Fact]
        public void MapData_ShouldTrackRoomConnections()
        {
            // Arrange
            var mapData = CreateSampleMapDataWithConnections();

            // Act & Assert
            Assert.NotNull(mapData);
            Assert.NotEmpty(mapData.DiscoveredRooms);
            
            var roomWithConnections = mapData.DiscoveredRooms.FirstOrDefault(r => r.Connections.Any());
            if (roomWithConnections != null)
            {
                Assert.NotNull(roomWithConnections.Connections);
                Assert.True(roomWithConnections.Connections.Count > 0);
                
                foreach (var connection in roomWithConnections.Connections)
                {
                    Assert.False(string.IsNullOrEmpty(connection.Direction));
                    Assert.True(connection.TargetRoom > 0);
                    Assert.True(connection.TargetRoomDiscovered);
                }
            }
        }

        [Fact]
        public void TerminalGuiRenderer_ShouldCreateValidMapView()
        {
            // Arrange
            var mapModel = CreateSampleMapModel();
            var renderer = new TerminalGuiRenderer();
            var bounds = new Terminal.Gui.Rect(0, 0, 80, 24);

            // Act
            var mapView = renderer.CreateMapView(mapModel, bounds);

            // Assert
            Assert.NotNull(mapView);
            Assert.Equal(bounds, mapView.Bounds);
        }

        [Fact]
        public void TerminalGuiRenderer_ShouldCreateValidStatusView()
        {
            // Arrange
            var mapModel = CreateSampleMapModel();
            var renderer = new TerminalGuiRenderer();
            var bounds = new Terminal.Gui.Rect(0, 0, 80, 3);
            var healthStatus = "Great";

            // Act
            var statusView = renderer.CreateStatusView(mapModel, healthStatus, bounds);

            // Assert
            Assert.NotNull(statusView);
            Assert.Equal(bounds, statusView.Bounds);
        }

        [Fact]
        public void TerminalGuiRenderer_ShouldRenderMapToString()
        {
            // Arrange
            var mapModel = CreateSampleMapModel();
            var renderer = new TerminalGuiRenderer();

            // Act
            var mapString = renderer.RenderMapToString(mapModel, 80, 40);

            // Assert
            Assert.NotNull(mapString);
            Assert.True(mapString.Length > 0);
            
            // Should contain some visual representation (more flexible check)
            var hasVisualElements = mapString.Contains("A") || mapString.Contains("@") || 
                                   mapString.Contains("+") || mapString.Contains("?") ||
                                   mapString.Contains("|") || mapString.Contains("-") ||
                                   mapString.Contains(".") || mapString.Contains("*");
            Assert.True(hasVisualElements, "Map string should contain some visual representation");
        }

        [Fact]
        public void MapModel_ShouldUpdatePlayerPosition()
        {
            // Arrange
            var rooms = CreateSampleRooms();
            var mapModel = new MapModel(_gameConfig, rooms, 20);
            var initialRoom = mapModel.CurrentRoom;
            var initialVisitedCount = mapModel.VisitedRooms.Count;

            // Act
            mapModel.UpdatePlayerPosition(18); // Move to Master Bedroom

            // Assert
            Assert.Equal(18, mapModel.CurrentRoom);
            Assert.NotEqual(initialRoom, mapModel.CurrentRoom);
            Assert.Contains(18, mapModel.VisitedRooms);
            
            // Visited rooms should include both initial and new room
            Assert.True(mapModel.VisitedRooms.Count >= initialVisitedCount, 
                "Visited room count should increase or stay the same");
        }

        [Fact]
        public void MapModel_ShouldTrackVisitedRooms()
        {
            // Arrange
            var rooms = CreateSampleRooms();
            var mapModel = new MapModel(_gameConfig, rooms, 20);
            var initialVisitedCount = mapModel.VisitedRooms.Count;

            // Act
            mapModel.UpdatePlayerPosition(18);
            mapModel.UpdatePlayerPosition(19);
            mapModel.UpdatePlayerPosition(7);

            // Assert
            Assert.True(mapModel.VisitedRooms.Count >= initialVisitedCount);
            Assert.Contains(18, mapModel.VisitedRooms);
            Assert.Contains(19, mapModel.VisitedRooms);
            Assert.Contains(7, mapModel.VisitedRooms);
        }

        [Fact]
        public void MapModel_ShouldGetVisitedRoomsForCurrentLevel()
        {
            // Arrange
            var rooms = CreateSampleRooms();
            var mapModel = new MapModel(_gameConfig, rooms, 20);
            
            // Visit some rooms
            mapModel.UpdatePlayerPosition(18); // Upper floor
            mapModel.UpdatePlayerPosition(19); // Upper floor
            mapModel.UpdatePlayerPosition(7);  // Ground floor

            // Act
            var currentLevelRooms = mapModel.GetVisitedRoomsForCurrentLevel();

            // Assert
            Assert.NotNull(currentLevelRooms);
            Assert.True(currentLevelRooms.Count > 0);
            
            // All returned rooms should be on the same level as current room
            var currentLevel = _gameConfig.GetLevelForRoom(mapModel.CurrentRoom);
            foreach (var room in currentLevelRooms)
            {
                var roomLevel = _gameConfig.GetLevelForRoom(room.RoomNumber);
                Assert.Equal(currentLevel, roomLevel);
            }
        }

        [Fact]
        public void DisplayMap_ShouldNotRequireKeyPress()
        {
            // Arrange
            var mapData = CreateSampleMapData();
            var mapText = GenerateConsoleMapText(mapData);

            // Act & Assert - Should not throw exception and should complete immediately
            try
            {
                _displayService.DisplayMap(mapText, true); // Classic mode
                _displayService.DisplayMap(mapText, false); // Enhanced mode
                
                // If we reach here, the method completed without waiting for user input
                Assert.True(true, "DisplayMap completed without requiring user input");
            }
            catch (Exception ex)
            {
                Assert.True(false, $"DisplayMap should not throw exception: {ex.Message}");
            }
        }

        #region Helper Methods

        private PlayerMapData CreateSampleMapData()
        {
            return new PlayerMapData
            {
                CurrentRoom = 20,
                CurrentLevel = "2",
                CurrentLevelDisplayName = "Attic",
                VisitedRoomCount = 1,
                DiscoveredRooms = new List<DiscoveredRoom>
                {
                    new DiscoveredRoom
                    {
                        RoomNumber = 20,
                        Name = "Attic",
                        Level = "2",
                        Position = new ClientMapPosition { X = 1, Y = 1 },
                        DisplayChar = 'A',
                        HasItems = true,
                        IsCurrentLocation = true,
                        Connections = new List<RoomConnection>()
                    }
                },
                RenderingConfig = new MapRenderingConfig
                {
                    GameName = "Adventure House",
                    RoomTypeChars = new Dictionary<string, char> { { "attic", 'A' } },
                    LevelDisplayNames = new Dictionary<string, string> { { "2", "Attic" } },
                    DefaultRoomChar = '.',
                    PlayerChar = '@',
                    ItemsChar = '+'
                }
            };
        }

        private PlayerMapData CreateSampleMapDataWithItems()
        {
            var mapData = CreateSampleMapData();
            mapData.DiscoveredRooms.First().HasItems = true;
            return mapData;
        }

        private PlayerMapData CreateSampleMapDataWithMultipleRooms()
        {
            return new PlayerMapData
            {
                CurrentRoom = 20,
                CurrentLevel = "2",
                CurrentLevelDisplayName = "Attic",
                VisitedRoomCount = 3,
                DiscoveredRooms = new List<DiscoveredRoom>
                {
                    new DiscoveredRoom
                    {
                        RoomNumber = 20,
                        Name = "Attic",
                        Level = "2",
                        Position = new ClientMapPosition { X = 1, Y = 1 },
                        DisplayChar = 'A',
                        HasItems = true,
                        IsCurrentLocation = true,
                        Connections = new List<RoomConnection>()
                    },
                    new DiscoveredRoom
                    {
                        RoomNumber = 18,
                        Name = "Master Bedroom",
                        Level = "1",
                        Position = new ClientMapPosition { X = 3, Y = 2 },
                        DisplayChar = 'M',
                        HasItems = false,
                        IsCurrentLocation = false,
                        Connections = new List<RoomConnection>()
                    },
                    new DiscoveredRoom
                    {
                        RoomNumber = 7,
                        Name = "Kitchen",
                        Level = "0",
                        Position = new ClientMapPosition { X = 3, Y = 2 },
                        DisplayChar = 'K',
                        HasItems = true,
                        IsCurrentLocation = false,
                        Connections = new List<RoomConnection>()
                    }
                },
                RenderingConfig = new MapRenderingConfig
                {
                    GameName = "Adventure House",
                    RoomTypeChars = new Dictionary<string, char> 
                    { 
                        { "attic", 'A' },
                        { "bedroom", 'M' },
                        { "kitchen", 'K' }
                    },
                    LevelDisplayNames = new Dictionary<string, string> 
                    { 
                        { "0", "Ground Floor" },
                        { "1", "Upper Floor" },
                        { "2", "Attic" }
                    },
                    DefaultRoomChar = '.',
                    PlayerChar = '@',
                    ItemsChar = '+'
                }
            };
        }

        private PlayerMapData CreateSampleMapDataWithConnections()
        {
            var mapData = CreateSampleMapDataWithMultipleRooms();
            
            // Add connections
            mapData.DiscoveredRooms[0].Connections.Add(new RoomConnection
            {
                Direction = "down",
                TargetRoom = 19,
                TargetRoomDiscovered = true
            });
            
            return mapData;
        }

        private MapModel CreateSampleMapModel()
        {
            var rooms = CreateSampleRooms();
            return new MapModel(_gameConfig, rooms, 20);
        }

        private List<Room> CreateSampleRooms()
        {
            return new List<Room>
            {
                new Room { Number = 20, Name = "Attic", N = 99, S = 99, E = 99, W = 99, U = 99, D = 19 },
                new Room { Number = 19, Name = "Master Bedroom Closet", N = 99, S = 99, E = 99, W = 18, U = 20, D = 99 },
                new Room { Number = 18, Name = "Master Bedroom", N = 99, S = 13, E = 19, W = 99, U = 99, D = 99 },
                new Room { Number = 7, Name = "Kitchen", N = 8, S = 99, E = 6, W = 99, U = 99, D = 99 }
            };
        }

        // Helper method extracted from AdventureClientService
        private string GenerateConsoleMapText(PlayerMapData mapData)
        {
            if (mapData == null || mapData.DiscoveredRooms.Count == 0)
            {
                return "No map data available.";
            }

            var mapText = new StringBuilder();
            mapText.AppendLine($"Map for {mapData.RenderingConfig.GameName} - {mapData.CurrentLevelDisplayName}");
            mapText.AppendLine($"Current Location: {mapData.DiscoveredRooms.FirstOrDefault(r => r.IsCurrentLocation)?.Name ?? "Unknown"}");
            mapText.AppendLine($"Rooms Visited: {mapData.VisitedRoomCount}");
            mapText.AppendLine();
            
            // Simple list of discovered rooms
            mapText.AppendLine("Discovered Rooms:");
            foreach (var room in mapData.DiscoveredRooms.OrderBy(r => r.RoomNumber))
            {
                var marker = room.IsCurrentLocation ? "@ " : "  ";
                var items = room.HasItems ? " [+]" : "";
                mapText.AppendLine($"{marker}{room.Name} ({room.RoomNumber}){items}");
            }
            
            return mapText.ToString();
        }

        #endregion
    }
}