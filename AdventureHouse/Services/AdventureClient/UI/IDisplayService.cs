using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureClient.Models;

namespace AdventureHouse.Services.AdventureClient.UI
{
    /// <summary>
    /// Interface for display services that handle UI rendering for the Adventure Client.
    /// Now supports both console-based rendering and Terminal.Gui rendering.
    /// </summary>
    public interface IDisplayService
    {
        /// <summary>
        /// Display the game introduction screen
        /// </summary>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayIntro(bool useClassicMode);

        /// <summary>
        /// Display adventure selection screen and get user choice
        /// </summary>
        /// <param name="availableGames">List of available games to choose from</param>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        /// <returns>The selected game ID</returns>
        int DisplayAdventureSelection(List<Game> availableGames, bool useClassicMode);

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
        /// <param name="mapText">The map text to display</param>
        /// <param name="useClassicMode">Whether to use classic console mode</param>
        void DisplayMap(string mapText, bool useClassicMode);

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

        /// <summary>
        /// Display a simple message
        /// </summary>
        /// <param name="message">The message to display</param>
        void DisplayMessage(string message);

        /// <summary>
        /// Display console output from the server
        /// </summary>
        /// <param name="output">The output to display</param>
        void DisplayConsoleOutput(string output);

        // Terminal.Gui specific methods for bridging to the new UI system

        /// <summary>
        /// Create a Terminal.Gui map view from MapModel data
        /// </summary>
        Terminal.Gui.FrameView CreateTerminalGuiMapView(MapModel mapModel, Terminal.Gui.Rect bounds);

        /// <summary>
        /// Create a Terminal.Gui status view from game data
        /// </summary>
        Terminal.Gui.FrameView CreateTerminalGuiStatusView(MapModel mapModel, string healthStatus, Terminal.Gui.Rect bounds);

        /// <summary>
        /// Create a Terminal.Gui legend view
        /// </summary>
        Terminal.Gui.FrameView CreateTerminalGuiLegendView(MapModel mapModel, Terminal.Gui.Rect bounds);

        /// <summary>
        /// Update an existing Terminal.Gui map view with new data
        /// </summary>
        void UpdateTerminalGuiMapView(Terminal.Gui.FrameView mapView, MapModel mapModel);

        /// <summary>
        /// Update an existing Terminal.Gui status view with new data
        /// </summary>
        void UpdateTerminalGuiStatusView(Terminal.Gui.FrameView statusView, MapModel mapModel, string healthStatus);

        /// <summary>
        /// Render map to string for Terminal.Gui TextView display
        /// </summary>
        string RenderMapToString(MapModel mapModel, int width = 80, int height = 40);
    }
}