using AdventureHouse.Services.Models;
using AdventurHouse.Services;
using AdventureHouse.Services.Data.AdventureData;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Services
{
    public class AdventureFrameworkService : IPlayAdventure
    {
        private readonly AdventureHouseData _adventureHouse = new();
        private readonly IGetFortune _getfortune;
        private readonly IGameConfiguration _gameConfig;
        private readonly IGameInstanceService _gameInstanceService;
        private readonly ICommandProcessingService _commandProcessingService;
        private readonly IPlayerManagementService _playerManagementService;
        private readonly IMonsterManagementService _monsterManagementService;
        private readonly IItemManagementService _itemManagementService;
        private readonly IRoomManagementService _roomManagementService;
        private readonly IMessageService _messageService;

        public AdventureFrameworkService(
            IMemoryCache gameCache,
            IGetFortune getfortune,
            IGameInstanceService gameInstanceService,
            ICommandProcessingService commandProcessingService,
            IPlayerManagementService playerManagementService,
            IMonsterManagementService monsterManagementService,
            IItemManagementService itemManagementService,
            IRoomManagementService roomManagementService,
            IMessageService messageService)
        {
            _getfortune = getfortune;
            _gameConfig = _adventureHouse.GetGameConfiguration();
            _gameInstanceService = gameInstanceService;
            _commandProcessingService = commandProcessingService;
            _playerManagementService = playerManagementService;
            _monsterManagementService = monsterManagementService;
            _itemManagementService = itemManagementService;
            _roomManagementService = roomManagementService;
            _messageService = messageService;
        }

        public List<Game> FrameWork_GetGames()
        {
            List<Game> games = new()
            {
                new Game {Id = 1, Name = _gameConfig.GameName, Ver = _gameConfig.GameVersion, Desc = _gameConfig.GameDescription},
                new Game {Id = 1, Name = "Adventure House Part 2!", Ver = "00", Desc = "Exact same game as API Adventure house but using a different name"}
            };

            return games;
        }

        public GameMoveResult FrameWork_NewGame(int GameID)
        {
            if (!FrameWork_GetGames().Exists(g => g.Id == GameID))
            {
                GameID = 1;
            }

            var instanceId = _gameInstanceService.CreateNewGameInstance(GameID);
            var playAdventure = _gameInstanceService.GetGameInstance(instanceId);
            var room = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room);

            return new GameMoveResult
            {
                InstanceID = playAdventure.InstanceID,
                RoomName = room.Name,
                RoomMessage = playAdventure.WelcomeMessage + room.Desc + " " + _messageService.GetRoomPath(room),
                ItemsMessage = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, playAdventure.Player.Verbose),
                PlayerName = playAdventure.Player.Name,
                HealthReport = _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax),
            };
        }

        public GameMoveResult FrameWork_GameMove(GameMove move)
        {
            if (_gameInstanceService.GameInstanceExists(move.InstanceID))
            {
                var gmr = ProcessGameMove(move);
                return gmr;
            }
            else 
            {
                return new GameMoveResult
                {
                    InstanceID = "-1",
                    ItemsMessage = "",
                    RoomMessage = "Game does not exist. Please begin again."
                };
            }
        }

        private GameMoveResult ProcessGameMove(GameMove move)
        {
            var playAdventure = _gameInstanceService.GetGameInstance(move.InstanceID);
            var commandState = _commandProcessingService.ParseCommand(move);
            commandState.Command = _commandProcessingService.FindCommandSynonym(commandState.Command);

            var gameResult = new GameMoveResult
            {
                InstanceID = playAdventure.InstanceID,
                RoomName = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room).Name,
                RoomMessage = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room).Desc,
                PlayerName = playAdventure.Player.Name,
                ItemsMessage = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, true),
                HealthReport = _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax)
            };

            var (playerMoved, updatedGameResult, updatedAdventure, updatedCommandState) = 
                _roomManagementService.ProcessPlayerMovement(playAdventure, gameResult, commandState);

            if (playerMoved)
            {
                return PostActionUpdate(updatedGameResult, updatedAdventure, updatedCommandState.Message);
            }
            else
            {
                (updatedAdventure, updatedCommandState) = ProcessNonMovementActions(updatedAdventure, updatedCommandState);
            }

            return PostActionUpdate(gameResult, updatedAdventure, updatedCommandState.Message);
        }

        private (PlayAdventure, CommandState) ProcessNonMovementActions(PlayAdventure playAdventure, CommandState commandState)
        {
            // Item management commands
            if (new[] { "get", "drop", "pet", "shoo", "inv", "look" }.Contains(commandState.Command))
            {
                return _itemManagementService.ProcessItemCommand(playAdventure, commandState);
            }

            // Item usage commands
            if (new[] { "eat", "use", "read", "wave", "throw" }.Contains(commandState.Command))
            {
                return _itemManagementService.ProcessUseItemCommand(playAdventure, commandState, _getfortune);
            }

            // Monster combat commands
            if (commandState.Command == "attack")
            {
                return _monsterManagementService.AttackMonster(playAdventure, commandState);
            }

            // Control commands
            if (commandState.Command == "health")
            {
                commandState.Message = "You feel " + _playerManagementService.GetHealthReport(
                    playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax) + ".";
            }
            else if (commandState.Command == "points")
            {
                commandState.Message = _playerManagementService.GetPlayerPointsMessage(playAdventure);
            }
            else if (commandState.Command == "quit")
            {
                if (!_playerManagementService.IsPlayerDead(playAdventure))
                {
                    playAdventure.Player.HealthCurrent = 0;
                }
                else
                {
                    commandState.Message = "You are dead - try the command \"newgame\".";
                }
            }
            else if (commandState.Command == "help")
            {
                commandState.Message = playAdventure.GameHelp;
            }
            else if (commandState.Command == "newgame")
            {
                playAdventure = _adventureHouse.SetupAdventure(playAdventure.InstanceID);
                commandState.Message = "Game has been reset. Enjoy!";
            }

            return (playAdventure, commandState);
        }

        private GameMoveResult PostActionUpdate(GameMoveResult gameResult, PlayAdventure playAdventure, string actionMessage)
        {
            // Update player health
            playAdventure.Player.HealthCurrent = _playerManagementService.CalculateNewHealth(playAdventure);
            gameResult.HealthReport = _playerManagementService.GetHealthReport(
                playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax);

            // Handle health status messages
            string healthActionMessage = "";
            if (gameResult.HealthReport.ToLower() == "dead")
            {
                healthActionMessage = _messageService.GetFunMessage(playAdventure.Messages, "dead", "") + "\r\n";
                playAdventure.Player.PlayerDead = true;
            }
            else if (gameResult.HealthReport.ToLower() == "bad")
            {
                healthActionMessage = _messageService.GetFunMessage(playAdventure.Messages, "bad", "") + "\r\n";
            }

            // Update game result
            gameResult.InstanceID = playAdventure.InstanceID;
            gameResult.PlayerName = playAdventure.Player.Name;
            gameResult.RoomName = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room).Name;
            
            // Set points for room
            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, gameResult.RoomName, playAdventure);

            // Build room message
            var currentRoom = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room);
            gameResult.RoomMessage = currentRoom.Desc + " ";
            gameResult.RoomMessage += _messageService.GetRoomPath(currentRoom) + "\r\n" + actionMessage + "\r\n";

            // Add monster descriptions
            var monsterDesc = _monsterManagementService.GetMonsterRoomDescription(playAdventure);
            if (!string.IsNullOrEmpty(monsterDesc))
            {
                gameResult.RoomMessage += monsterDesc + "\r\n";
            }

            // Add special room messages
            if (playAdventure.Player.Room == 0) 
                gameResult.RoomMessage += playAdventure.GameThanks;

            gameResult.RoomMessage += _messageService.GetHasPetMessage(playAdventure.Items, playAdventure.Messages, "\r\n");
            gameResult.ItemsMessage = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, true);

            if (!string.IsNullOrEmpty(healthActionMessage))
            {
                gameResult.RoomMessage += "\r\n" + healthActionMessage + "\r\n";
            }

            _gameInstanceService.UpdateGameInstance(playAdventure);
            return gameResult;
        }
    }
}