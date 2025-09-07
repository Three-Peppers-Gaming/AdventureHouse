using AdventureRealms.Services.Data.AdventureData;
using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureClient.Models;
using AdventureRealms.Services.AdventureServer;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    /// <summary>
    /// Interface for game state management services
    /// </summary>
    public interface IGameStateService
    {
        /// <summary>
        /// Initialize a new game
        /// </summary>
        /// <param name="client">The game client</param>
        /// <param name="gameId">The game ID to start</param>
        /// <returns>The initial game move result</returns>
        GameMoveResult InitializeGame(IPlayAdventure client, int gameId);

        /// <summary>
        /// Update the map state based on current game result
        /// </summary>
        /// <param name="gameResult">The current game result</param>
        void UpdateMapState(GameMoveResult gameResult);

        /// <summary>
        /// Get the current map state
        /// </summary>
        MapState? MapState { get; }

        /// <summary>
        /// Get the current game configuration (now supports multiple game types)
        /// </summary>
        IGameConfiguration? GameConfig { get; }

        /// <summary>
        /// Get room number from room name
        /// </summary>
        /// <param name="roomName">The room name</param>
        /// <returns>The room number, or -1 if not found</returns>
        int GetRoomNumberFromName(string roomName);
    }
}