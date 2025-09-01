using AdventureHouse.Services.Data.AdventureData;
using AdventureHouse.Services.Models;
using AdventureServer.Services;

namespace AdventureHouse.Services.GameManagement
{
    /// <summary>
    /// Game state service that manages the current game state and map
    /// Now supports multiple adventure types
    /// </summary>
    public class GameStateService : IGameStateService
    {
        private MapState? _mapState;
        private IGameConfiguration? _gameConfig;

        public MapState? MapState => _mapState;
        public IGameConfiguration? GameConfig => _gameConfig;

        public GameMoveResult InitializeGame(AdventurHouse.Services.IPlayAdventure client, int gameId)
        {
            var gameResult = client.FrameWork_NewGame(gameId);
            
            // Get the appropriate game configuration and data based on game ID
            _gameConfig = GetGameConfiguration(gameId);
            var gameInstance = GetGameInstance(gameId, gameResult.InstanceID);
            
            if (_gameConfig != null && gameInstance != null)
            {
                _mapState = new MapState(_gameConfig, gameInstance.Rooms, gameInstance.StartRoom);
                
                // Ensure the starting room is marked as visited and current
                _mapState.UpdatePlayerPosition(gameInstance.StartRoom);
            }

            return gameResult;
        }

        private IGameConfiguration GetGameConfiguration(int gameId)
        {
            return gameId switch
            {
                1 => new AdventureHouseConfiguration(),
                2 => new SpaceStationConfiguration(),
                _ => new AdventureHouseConfiguration() // Default fallback
            };
        }

        private PlayAdventure? GetGameInstance(int gameId, string instanceId)
        {
            return gameId switch
            {
                1 => new AdventureHouseData().SetupAdventure(instanceId),
                2 => new SpaceStationData().SetupAdventure(instanceId),
                _ => new AdventureHouseData().SetupAdventure(instanceId) // Default fallback
            };
        }

        public void UpdateMapState(GameMoveResult gameResult)
        {
            if (_mapState == null || gameResult == null || _gameConfig == null) return;
            
            // Get current room number from the room name using the game-specific configuration
            var currentRoomNumber = _gameConfig.GetRoomNumberFromName(gameResult.RoomName);
            
            if (currentRoomNumber > 0)
            {
                _mapState.UpdatePlayerPosition(currentRoomNumber);
                
                // Update items visibility based on whether room has visible items
                bool hasItems = !string.IsNullOrEmpty(gameResult.ItemsMessage) && 
                               gameResult.ItemsMessage != "No Items" && 
                               !gameResult.ItemsMessage.Contains("nothing");
                _mapState.UpdateRoomItems(currentRoomNumber, hasItems);
            }
        }

        public int GetRoomNumberFromName(string roomName)
        {
            return _gameConfig?.GetRoomNumberFromName(roomName) ?? -1;
        }
    }
}