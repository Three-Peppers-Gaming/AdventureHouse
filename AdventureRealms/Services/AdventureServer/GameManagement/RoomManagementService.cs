using AdventureRealms.Services.Shared.CommandProcessing;
using AdventureRealms.Services.AdventureServer.Models;
using AdventureRealms.Services.Shared.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public class RoomManagementService : IRoomManagementService
    {
        private readonly ICommandProcessingService _commandProcessingService;
        private readonly IMonsterManagementService _monsterManagementService;
        private readonly IMessageService _messageService;

        public RoomManagementService(
            ICommandProcessingService commandProcessingService,
            IMonsterManagementService monsterManagementService,
            IMessageService messageService)
        {
            _commandProcessingService = commandProcessingService;
            _monsterManagementService = monsterManagementService;
            _messageService = messageService;
        }

        public Room GetRoom(List<Room> rooms, int roomNumber)
        {
            return rooms.FirstOrDefault(t => t.Number == roomNumber);
        }

        public (bool PlayerMoved, GameMoveResult GameResult, PlayAdventureModel Adventure, CommandState CommandState) ProcessPlayerMovement(
            PlayAdventureModel playAdventure, GameMoveResult gameResult, CommandState commandState)
        {
            commandState.Message = "";
            
            if (playAdventure.Player.PlayerDead)
            {
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "DeadMove".Trim(), commandState.Modifier);
                return (false, gameResult, playAdventure, commandState);
            }

            var moveCommands = new List<string> { "go", "nor", "sou", "eas", "wes", "up", "down", "n", "s", "e", "w", "u", "d" };
            if (!moveCommands.Contains(commandState.Command))
            {
                return (false, gameResult, playAdventure, commandState);
            }

            if (commandState.Command == "go")
            {
                var validDirections = new List<string> { "north", "south", "east", "west", "up", "down" };
                commandState.Valid = validDirections.Contains(commandState.Modifier);
            }

            // Convert short move commands
            var shortMoveCommands = new List<string> { "nor", "sou", "eas", "wes", "up", "down", "n", "s", "e", "w", "u", "d" };
            if (shortMoveCommands.Contains(commandState.Command))
            {
                commandState = _commandProcessingService.ConvertShortMove(commandState.Command);
            }

            if (commandState.Valid)
            {
                (playAdventure, commandState) = MovePlayer(playAdventure, commandState);

                // Check for monsters in new room
                var monsterMessage = _monsterManagementService.CheckForMonsters(playAdventure);
                if (!string.IsNullOrEmpty(monsterMessage))
                {
                    commandState.Message += monsterMessage + "\r\n";
                }

                // Update game result with new room details
                gameResult.RoomName = GetRoom(playAdventure.Rooms, playAdventure.Player.Room).Name;
                gameResult.RoomMessage = GetRoom(playAdventure.Rooms, playAdventure.Player.Room).Desc;
                gameResult.PlayerName = playAdventure.Player.Name;
                gameResult.ItemsMessage = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, true);

                return (true, gameResult, playAdventure, commandState);
            }
            else
            {
                commandState.Message = "Wrong Way!";
            }

            return (false, gameResult, playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) MovePlayer(PlayAdventureModel playAdventure, CommandState commandState)
        {
            var room = GetRoom(playAdventure.Rooms, playAdventure.Player.Room);
            var direction = commandState.Modifier;

            if (IsMoveDirectionValid(room, direction))
            {
                playAdventure.Player.Room = direction switch
                {
                    "north" => room.N,
                    "south" => room.S,
                    "east" => room.E,
                    "west" => room.W,
                    "up" => room.U,
                    "down" => room.D,
                    _ => playAdventure.Player.Room
                };
            }
            else
            {
                commandState.Valid = false;
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, commandState.Modifier, commandState.Modifier) + "\r\n";
            }

            return (playAdventure, commandState);
        }

        public bool IsMoveDirectionValid(Room room, string direction)
        {
            return direction.ToLower() switch
            {
                "north" => room.N < 99,
                "south" => room.S < 99,
                "east" => room.E < 99,
                "west" => room.W < 99,
                "up" => room.U < 99,
                "down" => room.D < 99,
                _ => false
            };
        }
    }
}