using AdventureHouse.Services.AdventureServer;
using AdventureHouse.Services.AdventureClient;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Tests.Client
{
    /// <summary>
    /// Tests for Terminal.Gui Adventure Client functionality
    /// Validates that the GUI client can be instantiated and basic operations work
    /// </summary>
    public class TerminalGuiClientTests
    {
        [Fact]
        public void TerminalGuiClient_ShouldBeInstantiatable()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act
            var client = new TerminalGuiAdventureClient(server);

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void TerminalGuiClient_ShouldImplementIAdventureClient()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act
            var client = new TerminalGuiAdventureClient(server);

            // Assert
            Assert.IsAssignableFrom<IAdventureClient>(client);
        }

        [Fact]
        public void TerminalGuiClient_ShouldHandleServerConnection()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act - Verify server has games available
            var games = server.GameList();

            // Assert
            Assert.NotNull(games);
            Assert.True(games.Count > 0, "Server should have games available for GUI client");

            // Verify we can create the client with this server
            var client = new TerminalGuiAdventureClient(server);
            Assert.NotNull(client);
        }

        [Fact]
        public void TerminalGuiClient_ShouldProvideGameSwitchingCapability()
        {
            // This test verifies the structure exists for game switching
            // Actual UI testing would require a more complex testing framework

            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act
            var client = new TerminalGuiAdventureClient(server);

            // Assert - Verify the class has the expected structure for game switching
            var clientType = typeof(TerminalGuiAdventureClient);
            
            // Check for private fields that indicate game switching capability
            var currentGameIdField = clientType.GetField("_currentGameId", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var availableGamesField = clientType.GetField("_availableGames", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.NotNull(currentGameIdField);
            Assert.NotNull(availableGamesField);
        }

        [Fact]
        public void TerminalGuiClient_ShouldProvideTerminalWindowCapability()
        {
            // This test verifies the structure exists for terminal window feature
            
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act
            var client = new TerminalGuiAdventureClient(server);

            // Assert - Verify the class has methods for terminal functionality
            var clientType = typeof(TerminalGuiAdventureClient);
            
            var openTerminalMethod = clientType.GetMethod("OpenTerminalWindow", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.NotNull(openTerminalMethod);
        }
    }
}