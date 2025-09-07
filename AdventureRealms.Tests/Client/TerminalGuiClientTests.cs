using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.AdventureClient;
using AdventureRealms.Services.Shared.FortuneService;
using AdventureRealms.Services.Shared.CommandProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureRealms.Tests.Client
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
            // This test verifies the client can be instantiated and has game switching capabilities
            // We test this by verifying the client implements the interface and can be created

            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act
            var client = new TerminalGuiAdventureClient(server);

            // Assert - Verify the client can be created and implements proper interface
            Assert.NotNull(client);
            Assert.IsAssignableFrom<IAdventureClient>(client);
            
            // Verify the client has the game state management components
            var clientType = typeof(TerminalGuiAdventureClient);
            
            // Check for the game state field that now holds game switching state
            var gameStateField = clientType.GetField("_gameState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.NotNull(gameStateField);
        }

        [Fact]
        public void TerminalGuiClient_ShouldProvideTerminalWindowCapability()
        {
            // This test verifies the client provides proper UI functionality
            // Since we refactored to use component architecture, we test for the main interface
            
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var fortune = new GetFortuneService();
            var commandProcessor = new CommandProcessingService();
            var server = new AdventureFrameworkService(cache, fortune, commandProcessor);

            // Act
            var client = new TerminalGuiAdventureClient(server);

            // Assert - Verify the client has UI component management
            var clientType = typeof(TerminalGuiAdventureClient);
            
            // Check for UI components field that manages Terminal.Gui functionality
            var uiComponentsField = clientType.GetField("_uiComponents", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.NotNull(uiComponentsField);
            
            // Verify the StartAdventure method exists (main entry point for terminal functionality)
            var startAdventureMethod = clientType.GetMethod("StartAdventure", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            Assert.NotNull(startAdventureMethod);
        }
    }
}