

namespace AdventureHouse.Services.AdventureClient
{
    /// <summary>
    /// Interface for the Adventure Client - handles all user interaction and display
    /// This is completely separate from the Adventure Server which handles game logic
    /// </summary>
    public interface IAdventureClient
    {
        /// <summary>
        /// Start the adventure client with the given server
        /// </summary>
        /// <param name="adventureServer">The adventure server to connect to</param>
        void StartAdventure(AdventureServer.IPlayAdventure adventureServer);
    }
}