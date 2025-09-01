namespace AdventureHouse.Services.AdventureClient.Input
{
    /// <summary>
    /// Interface for input services that handle user input and command history for the Adventure Client
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Initialize the command buffer with common commands
        /// </summary>
        void InitializeCommandBuffer();

        /// <summary>
        /// Get user input with command history support
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        /// <returns>The user input string</returns>
        string GetUserInput(bool useClassicMode);

        /// <summary>
        /// Get the command history
        /// </summary>
        List<string> CommandHistory { get; }

        /// <summary>
        /// Add a command to the history
        /// </summary>
        /// <param name="command">The command to add</param>
        void AddToHistory(string command);
    }
}