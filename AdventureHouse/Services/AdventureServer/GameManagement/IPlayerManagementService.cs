using AdventureHouse.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer.GameManagement
{
    public interface IPlayerManagementService
    {
        Player SetPlayerPoints(bool isGeneric, string itemOrRoomName, PlayAdventureModel playAdventure);
        bool IsPlayerDead(PlayAdventureModel playAdventure);
        int CalculateNewHealth(PlayAdventureModel playAdventure);
        string GetHealthReport(int current, int max);
        string GetPlayerPoints(PlayAdventureModel playAdventure);
        string GetPlayerPointsMessage(PlayAdventureModel playAdventure);
    }
}