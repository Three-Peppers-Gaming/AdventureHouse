using AdventureHouse.Services.Models;
using AdventureServer.Services;

namespace AdventureHouse.Services.UI
{
    /// <summary>
    /// Interface for display services that handle UI rendering
    /// </summary>
    public interface IDisplayService
    {
        /// <summary>
        /// Display the game introduction screen
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayIntro(bool useClassicMode);

        /// <summary>
        /// Display the help screen
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayHelp(bool useClassicMode);

        /// <summary>
        /// Display the current game state
        /// </summary>
        /// <param name="gameResult">The game state result</param>
        /// <param name="useEnhanced">Whether to use enhanced display</param>
        /// <param name="scrollMode">Whether scrolling mode is enabled</param>
        void DisplayGameState(GameMoveResult gameResult, bool useEnhanced, bool scrollMode);

        /// <summary>
        /// Display the map
        /// </summary>
        /// <param name="mapState">The current map state</param>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayMap(MapState? mapState, bool useClassicMode);

        /// <summary>
        /// Show loading progress
        /// </summary>
        /// <param name="gameInitAction">Action to execute during loading</param>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void ShowLoadingProgress(Action gameInitAction, bool useClassicMode);

        /// <summary>
        /// Display date and time
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayDateTime(bool useClassicMode);

        /// <summary>
        /// Display command history
        /// </summary>
        /// <param name="commandHistory">List of command history</param>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayCommandHistory(List<string> commandHistory, bool useClassicMode);

        /// <summary>
        /// Display farewell message
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayFarewell(bool useClassicMode);

        /// <summary>
        /// Display error message
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayError(string errorMessage, bool useClassicMode);

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void ClearDisplay(bool useClassicMode);
    }
}