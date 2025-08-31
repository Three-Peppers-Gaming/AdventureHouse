using System;
using System.Collections.Generic;
using AdventureHouse.Services.Models;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Interface for game-specific configuration classes
    /// This allows different games to provide their own data mappings and text
    /// </summary>
    public interface IGameConfiguration
    {
        #region Game Identity
        string GameName { get; }
        string GameVersion { get; }
        string GameDescription { get; }
        #endregion

        #region Room Display Configuration
        Dictionary<string, char> RoomDisplayCharacters { get; }
        Dictionary<string, char> RoomTypeCharacters { get; }
        Dictionary<string, int> RoomNameToNumberMapping { get; }
        #endregion

        #region Map and Help Text
        string MapLegendContent { get; }
        string MapLegendFooter { get; }
        Dictionary<MapLevel, string> LevelDisplayNames { get; }
        string GetCompleteMapLegend();
        string GetAdventureHelpText();
        string GetAdventureThankYouText();
        string GetWelcomeMessage(string gamerTag);
        #endregion

        #region Game Settings
        int StartingRoom { get; }
        int MaxHealth { get; }
        int HealthStep { get; }
        int StartingPoints { get; }
        string InitialPointsCheckList { get; }
        int InventoryLocation { get; }
        int PetFollowLocation { get; }
        int NoConnectionValue { get; }
        char DefaultRoomCharacter { get; }
        #endregion

        #region Map Configuration
        Dictionary<int, MapLevel> RoomToLevelMapping { get; }
        Dictionary<MapLevel, (int GridWidth, int GridHeight)> LevelGridSizes { get; }
        Dictionary<int, (int X, int Y)> RoomPositions { get; }
        #endregion

        #region Helper Methods
        char GetRoomDisplayChar(string roomName);
        int GetRoomNumberFromName(string roomName);
        string GetLevelDisplayName(MapLevel level);
        MapLevel GetLevelForRoom(int roomNumber);
        (int X, int Y) GetRoomPosition(int roomNumber);
        #endregion
    }
}