using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface IMonsterManagementService
    {
        string CheckForMonsters(PlayAdventure playAdventure);
        (PlayAdventure, CommandState) AttackMonster(PlayAdventure playAdventure, CommandState commandState);
        string GetMonsterRoomDescription(PlayAdventure playAdventure);
    }
}