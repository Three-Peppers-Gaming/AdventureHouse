using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Data.AdventureData;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using AdventureHouse.Services.AdventureServer.GameManagement;
using Microsoft.Extensions.Caching.Memory;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer
{
    /// <summary>
    /// The main Adventure Server service that implements all game logic
    /// This is the server-side component that handles all adventure game operations
    /// </summary>
    public class AdventureFrameworkService : IPlayAdventure
    {
        private readonly AdventureHouseData _adventureHouse = new();
        private readonly SpaceStationData _spaceStation = new();
        private readonly IGetFortune _getfortune;
        private readonly IGameInstanceService _gameInstanceService;
        private readonly ICommandProcessingService _commandProcessingService;
        private readonly IPlayerManagementService _playerManagementService;
        private readonly IMonsterManagementService _monsterManagementService;
        private readonly IItemManagementService _itemManagementService;
        private readonly IRoomManagementService _roomManagementService;
        private readonly IMessageService _messageService;
        private readonly IGameStateService _gameStateService;

        // Client state tracking for session management
        private readonly Dictionary<string, ClientSessionState> _clientSessions = new();

        public AdventureFrameworkService(
            IMemoryCache gameCache,
            IGetFortune getfortune,
            ICommandProcessingService commandProcessingService)
        {
            _getfortune = getfortune;
            _commandProcessingService = commandProcessingService;
            
            // Initialize all server services
            _gameInstanceService = new GameInstanceService(gameCache);
            _playerManagementService = new PlayerManagementService();
            _messageService = new MessageService();
            _monsterManagementService = new MonsterManagementService(_playerManagementService, _messageService);
            _itemManagementService = new ItemManagementService(_playerManagementService, null, _messageService);
            _roomManagementService = new RoomManagementService(_commandProcessingService, _monsterManagementService, _messageService);
            _gameStateService = new GameStateService();
            
            // Set up circular dependency
            ((ItemManagementService)_itemManagementService).SetRoomManagementService(_roomManagementService);
        }

        public List<Game> FrameWork_GetGames()
        {
            List<Game> games = new()
            {
                new Game {Id = 1, Name = _adventureHouse.GetGameConfiguration().GameName,
                         Ver = _adventureHouse.GetGameConfiguration().GameVersion,
                         Desc = _adventureHouse.GetGameConfiguration().GameDescription},

                new Game {Id = 2, Name = _spaceStation.GetGameConfiguration().GameName,
                         Ver = _spaceStation.GetGameConfiguration().GameVersion,
                         Desc = _spaceStation.GetGameConfiguration().GameDescription}
            };

            return games;
        }

        public GameMoveResult FrameWork_StartGameSession(int gameId, bool useClassicMode = false, bool useScrollMode = false)
        {
            // Create new game
            var gameResult = FrameWork_NewGame(gameId);

            // Store client session state
            var sessionState = new ClientSessionState
            {
                UseClassicMode = useClassicMode,
                UseScrollMode = useScrollMode,
                GameId = gameId
            };
            _clientSessions[gameResult.InstanceID] = sessionState;

            // Initialize game state service for this session
            _gameStateService.InitializeGame(this, gameId);

            // Add intro and available games to result
            gameResult.AvailableGames = FrameWork_GetGames();
            gameResult.ConsoleOutput = "Welcome to Adventure House!\n\nAvailable commands:\n" +
                "- Game commands: look, go north, get item, etc.\n" +
                "- Console commands: /help, /map, /time, /classic, /clear, /history\n" +
                "- Type 'help' for game help or '/help' for console help\n\n";

            return gameResult;
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

            var gameResult = new GameMoveResult
            {
                InstanceID = playAdventure.InstanceID,
                RoomName = room.Name,
                RoomMessage = playAdventure.WelcomeMessage + room.Desc + " " + _messageService.GetRoomPath(room),
                ItemsMessage = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, playAdventure.Player.Verbose),
                PlayerName = playAdventure.Player.Name,
                HealthReport = _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax),
            };

            return gameResult;
        }

        public GameMoveResult FrameWork_GameMove(GameMove move)
        {
            if (!_gameInstanceService.GameInstanceExists(move.InstanceID))
            {
                return new GameMoveResult
                {
                    InstanceID = "-1",
                    ItemsMessage = "",
                    RoomMessage = "Game session expired. Please start a new game."
                };
            }

            // Get or create session state
            if (!_clientSessions.TryGetValue(move.InstanceID, out var sessionState))
            {
                sessionState = new ClientSessionState
                {
                    UseClassicMode = move.UseClassicMode,
                    UseScrollMode = move.UseScrollMode,
                    GameId = 1 // Default
                };
                _clientSessions[move.InstanceID] = sessionState;
            }

            return ProcessGameMove(move);
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

        private (PlayAdventureModel, CommandState) ProcessNonMovementActions(PlayAdventureModel playAdventure, CommandState commandState)
        {
            // Item management commands
            if (new[] { "get", "drop", "pet", "shoo", "inv", "look" }.Contains(commandState.Command))
            {
                return _itemManagementService.ProcessItemCommand(playAdventure, commandState);
            }

            // Item usage commands
            if (new[] { "eat", "use", "read", "wave", "throw", "activate" }.Contains(commandState.Command))
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
                // Restart the same game type
                var gameId = playAdventure.GameID;
                switch (gameId)
                {
                    case 1:
                        playAdventure = _adventureHouse.SetupAdventure(playAdventure.InstanceID);
                        break;
                    case 2:
                        playAdventure = _spaceStation.SetupAdventure(playAdventure.InstanceID);
                        break;
                    default:
                        playAdventure = _adventureHouse.SetupAdventure(playAdventure.InstanceID);
                        break;
                }
                commandState.Message = "Game has been reset. Enjoy!";
            }

            return (playAdventure, commandState);
        }

        private GameMoveResult PostActionUpdate(GameMoveResult gameResult, PlayAdventureModel playAdventure, string actionMessage)
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

    /// <summary>
    /// Tracks client session state for console command processing
    /// </summary>
    internal class ClientSessionState
    {
        public bool UseClassicMode { get; set; }
        public bool UseScrollMode { get; set; }
        public int GameId { get; set; }
    }
}