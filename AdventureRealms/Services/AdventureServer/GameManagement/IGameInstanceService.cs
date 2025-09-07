using AdventureRealms.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
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