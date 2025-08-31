using AdventureHouse.Services.UI;
using AdventureHouse.Services.Input;
using AdventureHouse.Services.GameManagement;

namespace AdventureHouse.Services.Commands
{
    /// <summary>
    /// Interface for console command processing
    /// </summary>
    public interface IConsoleCommandService
    {
        /// <summary>
        /// Process a console command
        /// </summary>
        /// <param name="normalizedCommand">The normalized command (lowercase, trimmed)</param>
        /// <param name="displayService">The display service</param>
        /// <param name="inputService">The input service</param>
        /// <param name="gameStateService">The game state service</param>
        /// <param name="useClassicMode">Whether classic mode is enabled</param>
        /// <param name="scrollMode">Whether scroll mode is enabled</param>
        /// <returns>A tuple indicating if the command was processed and the resulting action</returns>
        (bool wasProcessed, ConsoleCommandResult result) ProcessConsoleCommand(
            string normalizedCommand,
            IDisplayService displayService,
            IInputService inputService,
            IGameStateService gameStateService,
            bool useClassicMode,
            bool scrollMode);
    }

    /// <summary>
    /// Result of processing a console command
    /// </summary>
    public class ConsoleCommandResult
    {
        public ConsoleCommandAction Action { get; set; }
        public string? GameCommand { get; set; }
        public bool ToggleClassicMode { get; set; }
        public bool ToggleScrollMode { get; set; }

        public static ConsoleCommandResult Continue() => new() { Action = ConsoleCommandAction.Continue };
        public static ConsoleCommandResult ProcessAsGameCommand(string command) => new() { Action = ConsoleCommandAction.ProcessAsGameCommand, GameCommand = command };
        public static ConsoleCommandResult ToggleClassic() => new() { Action = ConsoleCommandAction.ToggleMode, ToggleClassicMode = true, GameCommand = "look" };
        public static ConsoleCommandResult ToggleScroll() => new() { Action = ConsoleCommandAction.ToggleMode, ToggleScrollMode = true };
        public static ConsoleCommandResult ClearAndLook() => new() { Action = ConsoleCommandAction.ProcessAsGameCommand, GameCommand = "look" };
    }

    /// <summary>
    /// Actions that can result from console command processing
    /// </summary>
    public enum ConsoleCommandAction
    {
        Continue,
        ProcessAsGameCommand,
        ToggleMode
    }
}