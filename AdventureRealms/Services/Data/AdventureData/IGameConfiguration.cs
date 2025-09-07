using System.Collections.Generic;
using AdventureRealms.Services.AdventureClient.Models;

namespace AdventureRealms.Services.Data.AdventureData
{
    /// <summary>
    /// Interface for game-specific configurations
    /// Each adventure game can implement this to provide its own room mappings, characters, and layout
    /// </summary>
    public interface IGameConfiguration
    {
        /// <summary>
        /// Game information
        /// </summary>
        string GameName { get; }
        string GameVersion { get; }
        string GameDescription { get; }

        /// <summary>
        /// Room display character mappings for the map
        /// </summary>
        Dictionary<string, char> RoomCharacterMapping { get; }
        Dictionary<string, char> RoomDisplayCharacters => RoomCharacterMapping;
        Dictionary<string, char> RoomTypeCharacters => RoomCharacterMapping;

        /// <summary>
        /// Room name to number mapping
        /// </summary>
        Dictionary<string, int> RoomNameToNumberMapping { get; }

        /// <summary>
        /// Level-related mappings
        /// </summary>
        Dictionary<MapLevel, string> LevelDisplayNames { get; }
        Dictionary<int, MapLevel> RoomToLevelMapping { get; }
        Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes { get; }

        /// <summary>
        /// Room positioning for map layout
        /// </summary>
        Dictionary<int, (int X, int Y)> RoomPositionMapping { get; }
        Dictionary<int, (int X, int Y)> RoomPositions => RoomPositionMapping;

        /// <summary>
        /// Map legend and display
        /// </summary>
        string MapLegend { get; }
        string MapLegendContent => MapLegend;
        string MapLegendFooter => "";

        /// <summary>
        /// Game configuration values
        /// </summary>
        int StartingRoom => 20;
        int MaxHealth => 100;
        int HealthStep => 10;
        int StartingPoints => 0;
        List<string> InitialPointsCheckList => new();
        int InventoryLocation => 9999;
        int PetFollowLocation => 9998;
        char DefaultRoomCharacter => '.';

        /// <summary>
        /// Game text
        /// </summary>
        string GetAdventureHelpText() => "Adventure help text goes here.";
        string GetAdventureThankYouText() => "Thank you for playing!";
        string GetWelcomeMessage(string playerName) => $"Welcome, {playerName}!";

        /// <summary>
        /// Method contracts
        /// </summary>
        char GetRoomDisplayChar(string roomName);
        string GetLevelDisplayName(MapLevel level);
        MapLevel GetLevelForRoom(int roomNumber);
        (int X, int Y) GetRoomPosition(int roomNumber);
        int GetRoomNumberFromName(string roomName);
        string GetCompleteMapLegend();
        int NoConnectionValue { get; }
    }
}