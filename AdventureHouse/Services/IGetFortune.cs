using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface IGetFortune
    {
        public Fortune ReturnRandomFortune();

        public Fortune ReturnTimeBasedFortune();

        public Fortune ReturnFortuneById(int id);
    }
}