using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface IPlayerManagementService
    {
        Player SetPlayerPoints(bool isGeneric, string itemOrRoomName, PlayAdventure playAdventure);
        bool IsPlayerDead(PlayAdventure playAdventure);
        int CalculateNewHealth(PlayAdventure playAdventure);
        string GetHealthReport(int current, int max);
        string GetPlayerPoints(PlayAdventure playAdventure);
        string GetPlayerPointsMessage(PlayAdventure playAdventure);
    }
}