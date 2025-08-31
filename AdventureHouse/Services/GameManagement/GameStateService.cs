using AdventureHouse.Services.Data.AdventureData;
using AdventureHouse.Services.Models;
using AdventureServer.Services;

namespace AdventureHouse.Services.GameManagement
{
    /// <summary>
    /// Game state service that manages the current game state and map
    /// </summary>
    public class GameStateService : IGameStateService
    {
        private MapState? _mapState;
        private readonly AdventureHouseConfiguration _gameConfig = new();

        public MapState? MapState => _mapState;
        public AdventureHouseConfiguration GameConfig => _gameConfig;

        public GameMoveResult InitializeGame(AdventurHouse.Services.IPlayAdventure client, int gameId)
        {
            var gameResult = client.FrameWork_NewGame(gameId);
            
            // Get the game instance service from DI if available
            if (client is AdventureFrameworkService frameworkService)
            {
                // For now, we'll need to get the instance data differently
                // Since GameInstance_GetObject is no longer directly accessible
                // We'll create a temporary solution
                var gameData = new AdventureHouseData();
                var tempInstance = gameData.SetupAdventure(gameResult.InstanceID);
                _mapState = new MapState(_gameConfig, tempInstance.Rooms, tempInstance.StartRoom);
            }
            else
            {
                // Fallback - create map state with default rooms
                var gameData = new AdventureHouseData();
                var tempInstance = gameData.SetupAdventure(Guid.NewGuid().ToString());
                _mapState = new MapState(_gameConfig, tempInstance.Rooms, _gameConfig.StartingRoom);
            }

            return gameResult;
        }

        public void UpdateMapState(GameMoveResult gameResult)
        {
            if (_mapState == null || gameResult == null) return;
            
            // Since we don't have direct access to current room number from GameMoveResult,
            // we'll need to extract it from the room name or maintain it separately
            // For now, let's create a simple room name to number mapping
            var currentRoomNumber = GetRoomNumberFromName(gameResult.RoomName);
            
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
            return _gameConfig.GetRoomNumberFromName(roomName);
        }
    }
}