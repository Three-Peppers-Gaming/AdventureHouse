using AdventureRealms.Services.AdventureServer.Models;
using AdventureRealms.Services.Data.AdventureData;
using Microsoft.Extensions.Caching.Memory;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public class GameInstanceService : IGameInstanceService
    {
        private readonly IMemoryCache _gameCache;
        private readonly AdventureHouseData _adventureHouse = new();
        private readonly SpaceStationData _spaceStation = new();
        private readonly FutureFamilyData _futureFamilyData = new();

        public GameInstanceService(IMemoryCache gameCache)
        {
            _gameCache = gameCache;
        }

        public string CreateNewGameInstance(int gameChoice)
        {
            var tempId = Guid.NewGuid().ToString();
            
            switch (gameChoice)
            {
                case 1:
                    var AdventureRealms = _adventureHouse.SetupAdventure(tempId);
                    AddToCache(AdventureRealms);
                    break;
                    
                case 2:
                    var spaceStation = _spaceStation.SetupAdventure(tempId);
                    AddToCache(spaceStation);
                    break;

                case 3:
                    var futureFamily = _futureFamilyData.SetupAdventure(tempId);    
                    AddToCache(futureFamily);
                    break;

                default:
                    // Default to Adventure House
                    var defaultGame = _adventureHouse.SetupAdventure(tempId);
                    AddToCache(defaultGame);
                    break;
            }
            
            return tempId;
        }

        public bool GameInstanceExists(string instanceId)
        {
            var adventure = GetFromCache(instanceId);
            return adventure?.InstanceID != null;
        }

        public PlayAdventureModel GetGameInstance(string instanceId)
        {
            var playAdventure = GetFromCache(instanceId);
            if (playAdventure?.InstanceID == null)
            {
                playAdventure = new PlayAdventureModel
                {
                    StartRoom = -1,
                    WelcomeMessage = "Error: No Instance Found!"
                };
            }
            return playAdventure;
        }

        public bool UpdateGameInstance(PlayAdventureModel playAdventure)
        {
            if (GameInstanceExists(playAdventure.InstanceID))
            {
                ReplaceInCache(playAdventure);
                return true;
            }
            return false;
        }

        public bool DeleteGameInstance(string instanceId)
        {
            if (GameInstanceExists(instanceId))
            {
                RemoveFromCache(instanceId);
                return true;
            }
            return false;
        }

        private void AddToCache(PlayAdventureModel p)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(8 * 60)); // 8 hours
            
            _gameCache.Set(p.InstanceID, p, cacheEntryOptions);
        }

        private void ReplaceInCache(PlayAdventureModel p)
        {
            _gameCache.Remove(p.InstanceID);
            AddToCache(p);
        }

        private PlayAdventureModel GetFromCache(string key)
        {
            return _gameCache.Get<PlayAdventureModel>(key);
        }

        private void RemoveFromCache(string key)
        {
            _gameCache.Remove(key);
        }
    }
}
