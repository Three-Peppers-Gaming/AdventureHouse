using AdventureRealms.Services.AdventureServer.Models;
using AdventureRealms.Services.Shared.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public interface IRoomManagementService
    {
        Room GetRoom(List<Room> rooms, int roomNumber);
        (bool PlayerMoved, GameMoveResult GameResult, PlayAdventureModel Adventure, CommandState CommandState) ProcessPlayerMovement(
            PlayAdventureModel playAdventure, GameMoveResult gameResult, CommandState commandState);
        bool IsMoveDirectionValid(Room room, string direction);
    }
}