using AdventureHouse.Services.AdventureServer.Models;

namespace AdventureHouse.Services.Shared.FortuneService
{
    public interface IGetFortune
    {
        public Fortune ReturnRandomFortune();

        public Fortune ReturnTimeBasedFortune();

        public Fortune ReturnFortuneById(int id);
    }
}