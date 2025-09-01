using AdventureHouse.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer.GameManagement
{
    public interface IGameInstanceService
    {
        string CreateNewGameInstance(int gameChoice);
        bool GameInstanceExists(string instanceId);
        PlayAdventureModel GetGameInstance(string instanceId);
        bool UpdateGameInstance(PlayAdventureModel playAdventure);
        bool DeleteGameInstance(string instanceId);
    }
}