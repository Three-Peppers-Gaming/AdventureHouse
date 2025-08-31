using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface IRoomManagementService
    {
        Room GetRoom(List<Room> rooms, int roomNumber);
        (bool PlayerMoved, GameMoveResult GameResult, PlayAdventure Adventure, CommandState CommandState) ProcessPlayerMovement(
            PlayAdventure playAdventure, GameMoveResult gameResult, CommandState commandState);
        bool IsMoveDirectionValid(Room room, string direction);
    }
}