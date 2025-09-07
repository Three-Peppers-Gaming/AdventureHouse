using AdventureRealms.Services.AdventureServer.Models;

namespace AdventureRealms.Services.Shared.FortuneService
{
    public interface IGetFortune
    {
        public Fortune ReturnRandomFortune();

        public Fortune ReturnTimeBasedFortune();

        public Fortune ReturnFortuneById(int id);
    }
}