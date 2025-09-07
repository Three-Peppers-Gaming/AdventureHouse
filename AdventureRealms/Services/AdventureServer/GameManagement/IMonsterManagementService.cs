using AdventureRealms.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public interface IMonsterManagementService
    {
        string CheckForMonsters(PlayAdventureModel playAdventure);
        (PlayAdventureModel, CommandState) AttackMonster(PlayAdventureModel playAdventure, CommandState commandState);
        string GetMonsterRoomDescription(PlayAdventureModel playAdventure);
    }
}