using System;
using System.Collections.Generic;
using Spectre.Console;

namespace AdventureRealms.Services.AdventureClient.Models
{
    /// <summary>
    /// Centralized configuration for all UI-related static content and settings
    /// </summary>
    public static class UIConfiguration
    {
        #region Application Information
        public static readonly string WelcomeTitle = "Adventure Realms!";
        public static readonly string DeveloperName = "Steve Sparks";
        public static readonly string GameDescription = "Simple two word text adventure games. Try to escape from them before you DIE!";
        public static readonly string CompanyName = "Steven Sparks";
        public static readonly string CopyrightNotice = "Copyright © 2025 Steven Sparks";
        #endregion

        #region Console Commands Configuration
        public static readonly Dictionary<string, string> ConsoleCommands = new()
        {
            ["chelp"] = "Display this console commands help",
            ["help"] = "Display in-game adventure help",
            ["map"] = "Display ASCII map of current level",
            ["clear"] = "Clear the screen and scroll buffer",
            ["classic"] = "Toggle classic console mode",
            ["intro"] = "Display game information",
            ["scroll"] = "Toggle scrolling mode",
            ["time"] = "Display system date and time",
            ["history"] = "Show recent command history",
            ["resign"] = "Exit game"
        };

        public static readonly string[] CommonGameCommands = {
            // Console commands - BOTH with and without forward slashes
            "chelp", "/chelp", "/help", "clear", "classic", "intro", "scroll", "time", "resign", "map", "/map",
            
            // Game commands - basic
            "help", "look", "get", "drop", "use", "eat", "read", "wave", "throw",
            "inv", "inventory", "pet", "shoo", "points", "health", "quit", "newgame",
            
            // Directions
            "north", "south", "east", "west", "up", "down", "n", "s", "e", "w", "u", "d",
            "go north", "go south", "go east", "go west", "go up", "go down"
        };
        #endregion

        #region Display Text Configuration
        public static readonly string AttentionMessage = "ATTENTION: To exit type \"resign\", For console help type \"chelp\", For game help type \"help\"";
        
        public static readonly string[] KeyboardInstructions = {
            "* Use Up/Down arrows for command history.",
            "* Use Left/Right arrows to edit",
            "* ESC to clear line"
        };

        public static readonly string[] EnhancedCommandLineFeatures = {
            "* Up/Down arrows: Navigate command history",
            "* Left/Right arrows: Edit current command", 
            "* Home/End: Jump to start/end of line",
            "* ESC: Clear current line",
            "* Backspace/Delete: Edit text",
            "* Enter: Execute command"
        };

        public static readonly string ContinuePrompt = "Press any key to continue...";
        public static readonly string ContinueOrWaitPrompt = "Press Enter to continue or wait 5 seconds...";
        public static readonly string NextActionPrompt = "Next Action? > ";
        #endregion

        #region Health Status Color Mapping
        public static readonly Dictionary<string, string> HealthColorMapping = new()
        {
            ["great"] = "green",
            ["okay"] = "yellow", 
            ["bad"] = "orange1",
            ["horrible"] = "red",
            ["dead"] = "darkred"
        };

        public static readonly Dictionary<string, ConsoleColor> HealthColorMappingClassic = new()
        {
            ["great"] = ConsoleColor.Green,
            ["okay"] = ConsoleColor.Yellow,
            ["bad"] = ConsoleColor.DarkYellow,
            ["horrible"] = ConsoleColor.Red,
            ["dead"] = ConsoleColor.DarkRed
        };
        #endregion

        #region Map Configuration
        public static readonly string MapLegendContent = @"
This legend is now provided by game-specific configuration classes.
See AdventureHouseConfiguration for Adventure House specific legend.
";

#if DEBUG
        public static readonly string MapLegendDebugAddition = @"Debug content provided by game configuration.
";
#endif

        public static readonly string MapLegendFooter = @"
General map footer text goes here.
";

        public static readonly Dictionary<string, char> RoomDisplayCharacters = new()
        {
            // This dictionary is now empty - game-specific mappings moved to game configuration classes
        };

        public static readonly Dictionary<string, char> RoomTypeCharacters = new()
        {
            // This dictionary is now empty - game-specific mappings moved to game configuration classes
        };

        public static readonly Dictionary<string, int> RoomNameToNumberMapping = new()
        {
            // This dictionary is now empty - game-specific mappings moved to game configuration classes
        };
        #endregion

        #region Map Level Display Names - MOVED TO GAME CONFIGURATION
        // Level display names are now provided by game-specific configuration classes
        // See IGameConfiguration.LevelDisplayNames for game-specific level names
        // This allows different games to have their own level naming conventions
        #endregion

        #region UI Display Settings
        public static readonly double TextWrapRatio = 0.9;
        public static readonly int MaxCommandLength = 100;
        public static readonly int MaxHistoryDisplay = 10;
        public static readonly int SafeConsoleWidth = 120;
        public static readonly int MinMapWidth = 4;
        public static readonly int MinMapHeight = 3;
        public static readonly int RoomBoxWidth = 4;
        public static readonly int RoomBoxHeight = 3;
        public static readonly char DefaultRoomChar = '.';
        public static readonly int NoConnectionValue = 99;
        #endregion

        #region Pause and Timing Settings
        public static readonly int DefaultPauseMilliseconds = 5000;
        public static readonly int ConsoleRefreshDelay = 50;
        public static readonly int LoadingProgressDelay = 100;
        #endregion

        #region ASCII Character Sets
        public static readonly int MinPrintableAscii = 32;
        public static readonly int MaxPrintableAscii = 126;
        
        public static readonly char[] PathCharacters = { '.', ':', '^', 'v' };
        public static readonly char[] BoxDrawingCharacters = { '+', '-', '|' };
        public static readonly char PlayerCharacter = '@';
        public static readonly char ItemsIndicator = '+';
        public static readonly char SpaceCharacter = ' ';
        #endregion

        #region Spectreconsole Color Scheme
        public static readonly Color PrimaryBorderColor = Color.Green;
        public static readonly Color SecondaryBorderColor = Color.Blue;
        public static readonly Color AccentBorderColor = Color.Yellow;
        public static readonly Color WarningColor = Color.Red;
        public static readonly Color InfoColor = Color.Cyan1;
        public static readonly Color SuccessColor = Color.Green;
        public static readonly Color HeaderColor = Color.Yellow;
        #endregion

        #region Classic Console Color Scheme
        public static readonly ConsoleColor ClassicPrimaryColor = ConsoleColor.Green;
        public static readonly ConsoleColor ClassicSecondaryColor = ConsoleColor.White;
        public static readonly ConsoleColor ClassicAccentColor = ConsoleColor.Yellow;
        public static readonly ConsoleColor ClassicWarningColor = ConsoleColor.Red;
        public static readonly ConsoleColor ClassicInfoColor = ConsoleColor.Cyan;
        public static readonly ConsoleColor ClassicHeaderColor = ConsoleColor.DarkCyan;
        public static readonly ConsoleColor ClassicTextColor = ConsoleColor.Gray;
        #endregion

        #region Error Messages
        public static readonly string ClientErrorMessage = "Client Error:";
        public static readonly string UnknownRoomMessage = "Unknown Room";
        public static readonly string NoMapAvailableMessage = "No map available for this level.";
        public static readonly string EmptyLevelMessage = "Empty level.";
        public static readonly string DebugNotAvailableMessage = "Debug level not available in release mode.";
        public static readonly string SetupGameMessage = "Setting up your adventure...";
        public static readonly string GameExitMessage = "Thanks for playing Adventure Realms!";
        #endregion

        #region Helper Methods for UI Text Generation
        /// <summary>
        /// Get the complete map legend including debug content if applicable
        /// NOTE: This method now returns placeholder text - use game configuration for actual legend
        /// </summary>
        public static string GetCompleteMapLegend()
        {
            var legend = MapLegendContent;
#if DEBUG
            legend += MapLegendDebugAddition;
#endif
            legend += MapLegendFooter;
            return legend;
        }

        /// <summary>
        /// Get health color for Spectre.Console based on health status
        /// </summary>
        public static string GetHealthColor(string healthStatus)
        {
            var status = healthStatus?.ToLower() ?? string.Empty;
            return HealthColorMapping.GetValueOrDefault(status, "white");
        }

        /// <summary>
        /// Get health color for classic console based on health status
        /// </summary>
        public static ConsoleColor GetHealthColorClassic(string healthStatus)
        {
            var status = healthStatus?.ToLower() ?? string.Empty;
            return HealthColorMappingClassic.GetValueOrDefault(status, ConsoleColor.White);
        }

        /// <summary>
        /// Get room number from room name using the mapping
        /// NOTE: This method now requires game-specific configuration to be passed in
        /// </summary>
        public static int GetRoomNumberFromName(string roomName)
        {
            // Game-specific mappings should now be handled by game configuration classes
            // This method is kept for backward compatibility but returns -1 for unknown rooms
            return -1;
        }

        /// <summary>
        /// Get room display character based on room name
        /// NOTE: This method now requires game-specific configuration to be passed in
        /// </summary>
        public static char GetRoomDisplayChar(string roomName)
        {
            // Game-specific character mappings should now be handled by game configuration classes
            // This method is kept for backward compatibility but returns default character
            return DefaultRoomChar;
        }

        /// <summary>
        /// Get level display name
        /// NOTE: This method now requires game-specific configuration to be passed in
        /// Use IGameConfiguration.GetLevelDisplayName() for game-specific level names
        /// </summary>
        public static string GetLevelDisplayName(MapLevel level)
        {
            // Level display names should now be handled by game configuration classes
            // This method is kept for backward compatibility but returns generic names
            return level switch
            {
                MapLevel.GroundFloor => "Level 1",
                MapLevel.UpperFloor => "Level 2", 
                MapLevel.Attic => "Level 3",
                MapLevel.MagicRealm => "Level 4",
#if DEBUG
                MapLevel.DebugLevel => "Debug",
#endif
                MapLevel.Exit => "Exit",
                _ => "Unknown Level"
            };
        }

        /// <summary>
        /// Format date/time display text
        /// </summary>
        public static string GetDateTimeDisplay()
        {
            return $"Date and Time: {DateTime.Now:F}";
        }

        /// <summary>
        /// Get console commands help text for classic mode
        /// </summary>
        public static string[] GetConsoleCommandsHelpClassic()
        {
            var help = new List<string>();
            foreach (var (command, description) in ConsoleCommands)
            {
                help.Add($"{command.PadRight(8)} - {description}");
            }
            return help.ToArray();
        }
        #endregion
    }
}