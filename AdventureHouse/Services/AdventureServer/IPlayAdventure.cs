using AdventureHouse.Services.Shared.Models;

namespace AdventureHouse.Services.AdventureServer
{
    /// <summary>
    /// Clean Adventure Server API with 100% client-server decoupling
    /// Only 2 endpoints needed for complete game functionality
    /// </summary>
    public interface IPlayAdventure 
    {
        /// <summary>
        /// Endpoint 1: Get list of available games
        /// Client calls this to display game selection menu
        /// </summary>
        /// <returns>List of available games with metadata</returns>
        List<Game> GameList();

        /// <summary>
        /// Endpoint 2: Play game - handles all game interactions
        /// This single endpoint handles: new game creation, moves, commands, everything
        /// </summary>
        /// <param name="request">Game play request with session ID and command</param>
        /// <returns>Complete game state response</returns>
        GamePlayResponse PlayGame(GamePlayRequest request);
    }
}