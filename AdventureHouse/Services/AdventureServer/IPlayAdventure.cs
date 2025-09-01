using AdventureHouse.Services.Shared.Models;

namespace AdventureHouse.Services.AdventureServer
{
    public interface IPlayAdventure 
    {
        /// <summary>
        /// Create a new game instance for the specified game ID
        /// </summary>
        /// <param name="GameID">The ID of the game to create</param>
        /// <returns>Initial game state</returns>
        public GameMoveResult FrameWork_NewGame(int GameID);

        /// <summary>
        /// Get list of available games
        /// </summary>
        /// <returns>List of available games</returns>
        public List<Game> FrameWork_GetGames();

        /// <summary>
        /// Process a game move (including console commands)
        /// This is the single entry point for all client interactions
        /// </summary>
        /// <param name="gameMove">The move to process</param>
        /// <returns>Game state and any console output</returns>
        GameMoveResult FrameWork_GameMove(GameMove gameMove);

        /// <summary>
        /// Initialize a new game session with client display preferences
        /// This combines game creation with initial setup
        /// </summary>
        /// <param name="gameId">The game ID to start</param>
        /// <param name="useClassicMode">Client display mode preference</param>
        /// <param name="useScrollMode">Client scroll mode preference</param>
        /// <returns>Complete initial game state with intro and available games</returns>
        GameMoveResult FrameWork_StartGameSession(int gameId, bool useClassicMode = false, bool useScrollMode = false);
    }
}