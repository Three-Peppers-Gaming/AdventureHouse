using AdventureHouse.Services.Models;
using AdventureHouse.Services.Data.AdventureData;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Services
{
    public class GameInstanceService : IGameInstanceService
    {
        private readonly IMemoryCache _gameCache;
        private readonly AdventureHouseData _adventureHouse = new();
        private readonly SpaceStationData _spaceStation = new();

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
                    var adventureHouse = _adventureHouse.SetupAdventure(tempId);
                    AddToCache(adventureHouse);
                    break;
                    
                case 2:
                    var spaceStation = _spaceStation.SetupAdventure(tempId);
                    AddToCache(spaceStation);
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

        public PlayAdventure GetGameInstance(string instanceId)
        {
            var playAdventure = GetFromCache(instanceId);
            if (playAdventure?.InstanceID == null)
            {
                playAdventure = new PlayAdventure
                {
                    StartRoom = -1,
                    WelcomeMessage = "Error: No Instance Found!"
                };
            }
            return playAdventure;
        }

        public bool UpdateGameInstance(PlayAdventure playAdventure)
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

        private void AddToCache(PlayAdventure p)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(8 * 60)); // 8 hours
            
            _gameCache.Set(p.InstanceID, p, cacheEntryOptions);
        }

        private void ReplaceInCache(PlayAdventure p)
        {
            _gameCache.Remove(p.InstanceID);
            AddToCache(p);
        }

        private PlayAdventure GetFromCache(string key)
        {
            return _gameCache.Get<PlayAdventure>(key);
        }

        private void RemoveFromCache(string key)
        {
            _gameCache.Remove(key);
        }
    }
}