using AdventureHouse.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer.GameManagement
{
    public interface IMonsterManagementService
    {
        string CheckForMonsters(PlayAdventureModel playAdventure);
        (PlayAdventureModel, CommandState) AttackMonster(PlayAdventureModel playAdventure, CommandState commandState);
        string GetMonsterRoomDescription(PlayAdventureModel playAdventure);
    }
}