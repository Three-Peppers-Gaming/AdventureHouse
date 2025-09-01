using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.Shared.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer.GameManagement
{
    public interface IRoomManagementService
    {
        Room GetRoom(List<Room> rooms, int roomNumber);
        (bool PlayerMoved, GameMoveResult GameResult, PlayAdventureModel Adventure, CommandState CommandState) ProcessPlayerMovement(
            PlayAdventureModel playAdventure, GameMoveResult gameResult, CommandState commandState);
        bool IsMoveDirectionValid(Room room, string direction);
    }
}