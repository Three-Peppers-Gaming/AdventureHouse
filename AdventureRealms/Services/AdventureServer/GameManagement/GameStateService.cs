using AdventureRealms.Services.Data.AdventureData;
using AdventureRealms.Services.AdventureServer;
using AdventureRealms.Services.AdventureClient.Models;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    /// <summary>
    /// Game state service that manages the current game state and map
    /// Now supports multiple adventure types
    /// </summary>
    public class GameStateService : IGameStateService
    {
        private AdventureClient.Models.MapState? _mapState;
        private IGameConfiguration? _gameConfig;

        public AdventureClient.Models.MapState? MapState => _mapState;
        public IGameConfiguration? GameConfig => _gameConfig;

        public GameMoveResult InitializeGame(IPlayAdventure client, int gameId)
        {
            // Use new clean API to start a game session
            var newGameRequest = new GamePlayRequest 
            { 
                SessionId = "", 
                GameId = gameId, 
                Command = "" 
            };
            var gameResponse = client.PlayGame(newGameRequest);
            
            Console.WriteLine($"Initializing game with ID {gameId}, SessionID: {gameResponse.SessionId}");
            
            // Convert to legacy GameMoveResult for compatibility
            var gameResult = new GameMoveResult
            {
                InstanceID = gameResponse.SessionId,
                RoomName = gameResponse.RoomName,
                RoomMessage = gameResponse.RoomDescription,
                ItemsMessage = gameResponse.ItemsInRoom,
                HealthReport = gameResponse.PlayerHealth,
                PlayerName = gameResponse.PlayerName,
                MapData = gameResponse.MapData
            };
            
            // Get the appropriate game configuration and data based on game ID
            _gameConfig = GetGameConfiguration(gameId);
            var gameInstance = GetGameInstance(gameId, gameResponse.SessionId);
            
            Console.WriteLine($"Game config: {_gameConfig?.GameName}, Game instance: {gameInstance?.GameName}");
            
            if (_gameConfig != null && gameInstance != null)
            {
                Console.WriteLine($"Creating map state with starting room {gameInstance.StartRoom}");
                _mapState = new AdventureClient.Models.MapState(_gameConfig, gameInstance.Rooms, gameInstance.StartRoom);
                
                // Ensure the starting room is marked as visited and current
                _mapState.UpdatePlayerPosition(gameInstance.StartRoom);
                
                Console.WriteLine($"Map state created. Current level: {_mapState.CurrentLevel}, Current room: {_mapState.CurrentRoom}, Visited rooms: {_mapState.VisitedRoomCount}");
            }
            else
            {
                Console.WriteLine("Failed to create map state - missing game config or instance");
            }

            return gameResult;
        }

        private IGameConfiguration GetGameConfiguration(int gameId)
        {
            return gameId switch
            {
                1 => new AdventureHouseConfiguration(),
                2 => new SpaceStationConfiguration(),
                3 => new FutureFamilyConfiguration(),
                _ => new AdventureHouseConfiguration() // Default fallback
            };
        }

        private PlayAdventureModel? GetGameInstance(int gameId, string instanceId)
        {
            return gameId switch
            {
                1 => new AdventureHouseData().SetupAdventure(instanceId),
                2 => new SpaceStationData().SetupAdventure(instanceId),
                3 => new FutureFamilyData().SetupAdventure(instanceId),
                _ => new AdventureHouseData().SetupAdventure(instanceId) // Default fallback
            };
        }

        public void UpdateMapState(GameMoveResult gameResult)
        {
            if (_mapState == null || gameResult == null || _gameConfig == null) 
            {
                // Debug: Log why we're skipping update
                Console.WriteLine($"UpdateMapState skipped: MapState={_mapState != null}, GameResult={gameResult != null}, GameConfig={_gameConfig != null}");
                return;
            }
            
            // Get current room number from the room name using the game-specific configuration
            var currentRoomNumber = _gameConfig.GetRoomNumberFromName(gameResult.RoomName);
            
            // Debug: Log room mapping
            Console.WriteLine($"Room name '{gameResult.RoomName}' mapped to room number {currentRoomNumber}");
            
            // If direct name mapping fails, try to find the room by checking actual room data
            if (currentRoomNumber <= 0)
            {
                Console.WriteLine($"Failed to map room name '{gameResult.RoomName}' to a valid room number");
                return; // Skip updating if we can't find the room
            }
            
            _mapState.UpdatePlayerPosition(currentRoomNumber);
            
            // Update items visibility based on whether room has visible items
            bool hasItems = !string.IsNullOrEmpty(gameResult.ItemsMessage) && 
                           gameResult.ItemsMessage != "No Items" && 
                           !gameResult.ItemsMessage.Contains("nothing") &&
                           !gameResult.ItemsMessage.Contains("no items");
            _mapState.UpdateRoomItems(currentRoomNumber, hasItems);
            
            Console.WriteLine($"Map state updated: Player in room {currentRoomNumber}, has items: {hasItems}");
        }

        public int GetRoomNumberFromName(string roomName)
        {
            return _gameConfig?.GetRoomNumberFromName(roomName) ?? -1;
        }
    }
}