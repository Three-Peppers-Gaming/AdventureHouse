using AdventureRealms.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
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