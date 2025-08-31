using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface IGameInstanceService
    {
        string CreateNewGameInstance(int gameChoice);
        bool GameInstanceExists(string instanceId);
        PlayAdventure GetGameInstance(string instanceId);
        bool UpdateGameInstance(PlayAdventure playAdventure);
        bool DeleteGameInstance(string instanceId);
    }
}