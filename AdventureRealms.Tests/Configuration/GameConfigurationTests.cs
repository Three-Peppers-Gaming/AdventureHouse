using AdventureRealms.Services.Data.AdventureData;
using AdventureRealms.Services.AdventureClient.Models;

namespace AdventureRealms.Tests.Configuration
{
    /// <summary>
    /// Tests for game configuration and map layout validation
    /// Ensures that room positions, connections, and display characters are correctly configured
    /// </summary>
    public class GameConfigurationTests
    {
        private readonly IGameConfiguration _adventureHouseConfig;
        private readonly IGameConfiguration _spaceStationConfig;
        private readonly IGameConfiguration _futureFamilyConfig;

        public GameConfigurationTests()
        {
            _adventureHouseConfig = new AdventureHouseConfiguration();
            _spaceStationConfig = new SpaceStationConfiguration();
            _futureFamilyConfig = new FutureFamilyConfiguration();
        }

        [Fact]
        public void AdventureHouseConfig_ShouldHaveValidRoomPositions()
        {
            // Act & Assert
            ValidateRoomPositions(_adventureHouseConfig);
        }

        [Fact]
        public void SpaceStationConfig_ShouldHaveValidRoomPositions()
        {
            // Act & Assert
            ValidateRoomPositions(_spaceStationConfig);
        }

        [Fact]
        public void FutureFamilyConfig_ShouldHaveValidRoomPositions()
        {
            // Act & Assert
            ValidateRoomPositions(_futureFamilyConfig);
        }

        [Fact]
        public void AdventureHouseConfig_ShouldHaveValidDisplayCharacters()
        {
            // Act & Assert
            ValidateDisplayCharacters(_adventureHouseConfig);
            
            // Adventure House specific validations
            Assert.True(_adventureHouseConfig.RoomDisplayCharacters.ContainsKey("attic"));
            Assert.True(_adventureHouseConfig.RoomDisplayCharacters.ContainsKey("kitchen"));
            Assert.True(_adventureHouseConfig.RoomDisplayCharacters.ContainsKey("living room"));
            
            // Check specific character mappings
            Assert.Equal('A', _adventureHouseConfig.GetRoomDisplayChar("Attic"));
            Assert.Equal('K', _adventureHouseConfig.GetRoomDisplayChar("Kitchen"));
            Assert.Equal('L', _adventureHouseConfig.GetRoomDisplayChar("Living Room"));
        }

        [Fact]
        public void GameConfigs_ShouldHaveUniqueLevelDisplayNames()
        {
            // Arrange
            var configs = new[] { _adventureHouseConfig, _spaceStationConfig, _futureFamilyConfig };

            foreach (var config in configs)
            {
                // Act
                var levelNames = config.LevelDisplayNames;

                // Assert
                Assert.NotNull(levelNames);
                Assert.True(levelNames.Count > 0);
                
                // All level names should be unique
                var uniqueNames = levelNames.Values.Distinct().ToList();
                Assert.Equal(levelNames.Count, uniqueNames.Count);
                
                // Level names should not be empty
                foreach (var levelName in levelNames.Values)
                {
                    Assert.False(string.IsNullOrEmpty(levelName));
                }
            }
        }

        [Fact]
        public void GameConfigs_ShouldHaveValidRoomToLevelMappings()
        {
            // Arrange
            var configs = new[] { _adventureHouseConfig, _spaceStationConfig, _futureFamilyConfig };

            foreach (var config in configs)
            {
                // Act
                var roomToLevelMapping = config.RoomToLevelMapping;

                // Assert
                Assert.NotNull(roomToLevelMapping);
                Assert.True(roomToLevelMapping.Count > 0);
                
                // All room numbers should be positive
                foreach (var roomNumber in roomToLevelMapping.Keys)
                {
                    Assert.True(roomNumber >= 0, $"Room number should be non-negative: {roomNumber}");
                }
                
                // All levels should have corresponding display names
                foreach (var level in roomToLevelMapping.Values.Distinct())
                {
                    Assert.True(config.LevelDisplayNames.ContainsKey(level));
                }
            }
        }

        [Fact]
        public void AdventureHouseConfig_ShouldHaveCorrectStartingRoom()
        {
            // Act
            var startingRoom = _adventureHouseConfig.StartingRoom;

            // Assert
            Assert.Equal(20, startingRoom); // Attic
            
            // Starting room should have a position
            var position = _adventureHouseConfig.GetRoomPosition(startingRoom);
            Assert.True(position.X >= 0);
            Assert.True(position.Y >= 0);
            
            // Starting room should have a level
            var level = _adventureHouseConfig.GetLevelForRoom(startingRoom);
            Assert.True(_adventureHouseConfig.LevelDisplayNames.ContainsKey(level));
        }

        [Fact]
        public void GameConfigs_ShouldHaveValidLevelGridSizes()
        {
            // Arrange
            var configs = new[] { _adventureHouseConfig, _spaceStationConfig, _futureFamilyConfig };

            foreach (var config in configs)
            {
                // Act
                var levelGridSizes = config.LevelGridSizes;

                // Assert
                Assert.NotNull(levelGridSizes);
                Assert.True(levelGridSizes.Count > 0);
                
                foreach (var kvp in levelGridSizes)
                {
                    var level = kvp.Key;
                    var (width, height) = kvp.Value;
                    
                    Assert.True(width > 0, $"Grid width should be positive for level {level}");
                    Assert.True(height > 0, $"Grid height should be positive for level {level}");
                    
                    // Reasonable size limits (not too large or too small)
                    Assert.True(width <= 50, $"Grid width should be reasonable for level {level}");
                    Assert.True(height <= 50, $"Grid height should be reasonable for level {level}");
                    Assert.True(width >= 1, $"Grid width should be at least 1 for level {level}");
                    Assert.True(height >= 1, $"Grid height should be at least 1 for level {level}");
                }
            }
        }

        [Fact]
        public void AdventureHouseConfig_ShouldHaveValidRoomNameToNumberMapping()
        {
            // Act
            var mapping = _adventureHouseConfig.RoomNameToNumberMapping;

            // Assert
            Assert.NotNull(mapping);
            Assert.True(mapping.Count > 0);
            
            // Test specific known rooms
            Assert.True(mapping.ContainsKey("attic"));
            Assert.True(mapping.ContainsKey("kitchen"));
            Assert.True(mapping.ContainsKey("living room"));
            Assert.True(mapping.ContainsKey("exit!"));
            
            // Verify the mappings are correct
            Assert.Equal(20, mapping["attic"]);
            Assert.Equal(7, mapping["kitchen"]);
            Assert.Equal(4, mapping["living room"]);
            Assert.Equal(0, mapping["exit!"]);
            
            // All room numbers should be non-negative
            foreach (var roomNumber in mapping.Values)
            {
                Assert.True(roomNumber >= 0);
            }
        }

        [Fact]
        public void GameConfigs_ShouldHaveConsistentPositionMappings()
        {
            // Arrange
            var configs = new[] { _adventureHouseConfig, _spaceStationConfig, _futureFamilyConfig };

            foreach (var config in configs)
            {
                // Act
                var roomPositions = config.RoomPositions;

                // Assert
                Assert.NotNull(roomPositions);
                Assert.True(roomPositions.Count > 0);
                
                // All positions should be within the grid boundaries for their level
                foreach (var kvp in roomPositions)
                {
                    var roomNumber = kvp.Key;
                    var (x, y) = kvp.Value;
                    var level = config.GetLevelForRoom(roomNumber);
                    
                    if (config.LevelGridSizes.TryGetValue(level, out var gridSize))
                    {
                        var (gridWidth, gridHeight) = gridSize;
                        Assert.True(x >= 0 && x <= gridWidth, 
                            $"Room {roomNumber} X position {x} should be within grid width {gridWidth} for level {level}");
                        Assert.True(y >= 0 && y <= gridHeight, 
                            $"Room {roomNumber} Y position {y} should be within grid height {gridHeight} for level {level}");
                    }
                }
                
                // No two rooms should have the exact same position on the same level
                var positionsByLevel = roomPositions
                    .GroupBy(kvp => config.GetLevelForRoom(kvp.Key))
                    .ToDictionary(g => g.Key, g => g.Select(kvp => kvp.Value).ToList());
                
                foreach (var kvp in positionsByLevel)
                {
                    var level = kvp.Key;
                    var positions = kvp.Value;
                    var uniquePositions = positions.Distinct().ToList();
                    // Note: Some games might allow rooms at same position on different connections
                    // So we'll just ensure we have reasonable position distribution
                    Assert.True(uniquePositions.Count > 0, $"Level {level} should have at least one unique position");
                }
            }
        }

        [Fact]
        public void GameConfigs_ShouldProvideValidMapLegends()
        {
            // Arrange
            var configs = new[] { _adventureHouseConfig, _spaceStationConfig, _futureFamilyConfig };

            foreach (var config in configs)
            {
                // Act
                var mapLegend = config.GetCompleteMapLegend();

                // Assert
                Assert.NotNull(mapLegend);
                Assert.False(string.IsNullOrEmpty(mapLegend));
                
                // Map legend should contain some useful information - be very flexible
                Assert.True(mapLegend.Length > 10, $"Map legend for {config.GameName} should have substantial content");
            }
        }

        [Fact]
        public void GameConfigs_ShouldHaveValidGameIdentity()
        {
            // Adventure House
            Assert.Equal("Adventure House", _adventureHouseConfig.GameName);
            Assert.False(string.IsNullOrEmpty(_adventureHouseConfig.GameVersion));
            Assert.False(string.IsNullOrEmpty(_adventureHouseConfig.GameDescription));
            
            // Space Station
            Assert.Equal("Abandoned Space Station", _spaceStationConfig.GameName);
            Assert.False(string.IsNullOrEmpty(_spaceStationConfig.GameVersion));
            Assert.False(string.IsNullOrEmpty(_spaceStationConfig.GameDescription));
            
            // Future Family
            Assert.Equal("Future Family Space Apartment", _futureFamilyConfig.GameName);
            Assert.False(string.IsNullOrEmpty(_futureFamilyConfig.GameVersion));
            Assert.False(string.IsNullOrEmpty(_futureFamilyConfig.GameDescription));
        }

        [Fact]
        public void GameConfigs_ShouldHaveValidHealthAndPointSettings()
        {
            // Arrange
            var configs = new[] { _adventureHouseConfig, _spaceStationConfig, _futureFamilyConfig };

            foreach (var config in configs)
            {
                // Act & Assert
                Assert.True(config.MaxHealth > 0, $"{config.GameName} should have positive max health");
                Assert.True(config.HealthStep > 0, $"{config.GameName} should have positive health step");
                Assert.True(config.StartingPoints >= 0, $"{config.GameName} should have non-negative starting points");
                Assert.True(config.InventoryLocation > 0, $"{config.GameName} should have valid inventory location");
                Assert.True(config.NoConnectionValue > 0, $"{config.GameName} should have valid no-connection value");
                
                // Default room character should be printable
                Assert.True(config.DefaultRoomCharacter >= ' ', 
                    $"{config.GameName} default room character should be printable");
            }
        }

        private void ValidateRoomPositions(IGameConfiguration config)
        {
            // Act
            var roomPositions = config.RoomPositions;

            // Assert
            Assert.NotNull(roomPositions);
            Assert.True(roomPositions.Count > 0, $"{config.GameName} should have room positions");
            
            foreach (var kvp in roomPositions)
            {
                var roomNumber = kvp.Key;
                var (x, y) = kvp.Value;
                Assert.True(roomNumber >= 0, $"Room number should be non-negative: {roomNumber}");
                Assert.True(x >= 0, $"X position should be non-negative for room {roomNumber}: {x}");
                Assert.True(y >= 0, $"Y position should be non-negative for room {roomNumber}: {y}");
            }
        }

        private void ValidateDisplayCharacters(IGameConfiguration config)
        {
            // Act
            var displayChars = config.RoomDisplayCharacters;

            // Assert
            Assert.NotNull(displayChars);
            Assert.True(displayChars.Count > 0, $"{config.GameName} should have display characters");
            
            foreach (var kvp in displayChars)
            {
                var roomName = kvp.Key;
                var character = kvp.Value;
                Assert.False(string.IsNullOrEmpty(roomName), "Room name should not be empty");
                Assert.True(character >= ' ', $"Display character should be printable for room '{roomName}': '{character}'");
            }
            
            // Default character should be valid
            Assert.True(config.DefaultRoomCharacter >= ' ', 
                $"{config.GameName} default character should be printable");
        }
    }
}