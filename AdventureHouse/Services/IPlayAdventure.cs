using AdventureHouse.Services.Models;

namespace AdventurHouse.Services
{
    public interface IPlayAdventure 
    {
        public GameMoveResult FrameWork_NewGame(int GameID);

        public List<Game> FrameWork_GetGames();

        GameMoveResult FrameWork_GameMove(GameMove gameMove);

    }
}
