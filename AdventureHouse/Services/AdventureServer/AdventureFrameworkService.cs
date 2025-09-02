using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.Data.AdventureData;
using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.Shared.CommandProcessing;
using AdventureHouse.Services.AdventureServer.GameManagement;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer
{
    /// <summary>
    /// Clean Adventure Server - 100% client-server decoupling with 2 endpoints
    /// </summary>
    public class AdventureFrameworkService : IPlayAdventure
    {
        private readonly AdventureHouseData _adventureHouse = new();
        private readonly SpaceStationData _spaceStation = new();
        private readonly FutureFamilyData _futureFamily = new();
        private readonly IGetFortune _getfortune;
        private readonly IGameInstanceService _gameInstanceService;
        private readonly ICommandProcessingService _commandProcessingService;
        private readonly IPlayerManagementService _playerManagementService;
        private readonly IMonsterManagementService _monsterManagementService;
        private readonly IItemManagementService _itemManagementService;
        private readonly IRoomManagementService _roomManagementService;
        private readonly IMessageService _messageService;

        // Session state tracking for clean client-server separation
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

            // Set up circular dependency
            ((ItemManagementService)_itemManagementService).SetRoomManagementService(_roomManagementService);
        }

        /// <summary>
        /// Endpoint 1: Get list of available games
        /// </summary>
        public List<Game> GameList()
        {
            return new List<Game>
            {
                new Game {
                    Id = 1,
                    Name = _adventureHouse.GetGameConfiguration().GameName,
                    Ver = _adventureHouse.GetGameConfiguration().GameVersion,
                    Desc = _adventureHouse.GetGameConfiguration().GameDescription
                },
                new Game {
                    Id = 2,
                    Name = _spaceStation.GetGameConfiguration().GameName,
                    Ver = _spaceStation.GetGameConfiguration().GameVersion,
                    Desc = _spaceStation.GetGameConfiguration().GameDescription
                },
                new Game {
                    Id = 3,
                    Name = _futureFamily.GetGameConfiguration().GameName,
                    Ver = _futureFamily.GetGameConfiguration().GameVersion,
                    Desc = _futureFamily.GetGameConfiguration().GameDescription
                }
            };
        }

        /// <summary>
        /// Endpoint 2: Play game - handles all game interactions
        /// Single endpoint for new games, moves, commands, everything
        /// </summary>
        public AdventureHouse.Services.Shared.Models.GamePlayResponse PlayGame(AdventureHouse.Services.Shared.Models.GamePlayRequest request)
        {
            try
            {
                // Handle new game creation
                if (string.IsNullOrEmpty(request.SessionId))
                {
                    return CreateNewGameSession(request);
                }

                // Handle existing game interaction
                return ProcessGameCommand(request);
            }
            catch (Exception ex)
            {
                return new AdventureHouse.Services.Shared.Models.GamePlayResponse
                {
                    SessionId = "-1",
                    CommandResponse = $"Server error: {ex.Message}",
                    InvalidCommand = true
                };
            }
        }

        /// <summary>
        /// Create a new game session
        /// </summary>
        private AdventureHouse.Services.Shared.Models.GamePlayResponse CreateNewGameSession(AdventureHouse.Services.Shared.Models.GamePlayRequest request)
        {
            // Validate game ID
            var games = GameList();
            if (!games.Any(g => g.Id == request.GameId))
            {
                request.GameId = 1; // Default to Adventure House
            }

            // Create new game instance
            var instanceId = _gameInstanceService.CreateNewGameInstance(request.GameId);
            var playAdventure = _gameInstanceService.GetGameInstance(instanceId);
            var gameConfig = GetGameConfiguration(request.GameId);

            // Store client session state
            var sessionState = new ClientSessionState
            {
                UseClassicMode = request.UseClassicMode,
                UseScrollMode = request.UseScrollMode,
                GameId = request.GameId,
                GameConfig = gameConfig,
                MapState = new AdventureClient.Models.MapState(gameConfig, playAdventure.Rooms, playAdventure.Player.Room)
            };
            sessionState.MapState.UpdatePlayerPosition(playAdventure.Player.Room);
            _clientSessions[instanceId] = sessionState;

            // Build response
            var room = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room);
            var roomDescription = playAdventure.WelcomeMessage + "\n\n" + room.Desc;
            var roomPath = _messageService.GetRoomPath(room);
            if (!string.IsNullOrEmpty(roomPath))
            {
                roomDescription += " " + roomPath;
            }
            
            return new AdventureHouse.Services.Shared.Models.GamePlayResponse
            {
                SessionId = instanceId,
                GameName = playAdventure.GameName,
                RoomName = room.Name,
                RoomDescription = roomDescription,
                ItemsInRoom = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, playAdventure.Player.Verbose),
                PlayerName = playAdventure.Player.Name,
                PlayerHealth = _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax),
                CommandResponse = "", // Server provides no client instructions - pure game content only
                MapData = CreateMapDataForClient(sessionState.MapState, playAdventure.Player.Room, playAdventure),
                AvailableGames = games
            };
        }

        /// <summary>
        /// Process command for existing game session
        /// </summary>
        private AdventureHouse.Services.Shared.Models.GamePlayResponse ProcessGameCommand(AdventureHouse.Services.Shared.Models.GamePlayRequest request)
        {
            // Validate session
            if (!_gameInstanceService.GameInstanceExists(request.SessionId))
            {
                return new AdventureHouse.Services.Shared.Models.GamePlayResponse
                {
                    SessionId = "-1",
                    CommandResponse = "Game session expired. Please start a new game.",
                    InvalidCommand = true
                };
            }

            // Reject console commands - these should be handled by the client
            if (IsConsoleCommand(request.Command))
            {
                return new AdventureHouse.Services.Shared.Models.GamePlayResponse
                {
                    SessionId = request.SessionId,
                    CommandResponse = "Console commands are not supported by the server. Handle these in the client.",
                    InvalidCommand = true,
                    MapData = CreateMapDataForClient(_clientSessions[request.SessionId].MapState, 
                        _gameInstanceService.GetGameInstance(request.SessionId).Player.Room,
                        _gameInstanceService.GetGameInstance(request.SessionId))
                };
            }

            var playAdventure = _gameInstanceService.GetGameInstance(request.SessionId);
            var sessionState = _clientSessions[request.SessionId];

            // Parse and process command
            var commandState = _commandProcessingService.ParseCommand(new GameMove 
            { 
                InstanceID = request.SessionId, 
                Move = request.Command 
            });
            commandState.Command = _commandProcessingService.FindCommandSynonym(commandState.Command);

            // Process game command only
            return ProcessGameCommandInternal(request, commandState, playAdventure, sessionState);
        }

        /// <summary>
        /// Check if a command is a console command that should be handled by the client
        /// </summary>
        private bool IsConsoleCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;
            
            var cmd = command.Trim().ToLower();
            
            // Commands starting with / are console commands
            if (cmd.StartsWith("/")) return true;
            
            // Specific console commands that should be handled by client (but not game commands)
            return cmd is "map" or "chelp" or "clear" or "time" or 
                         "intro" or "history" or "resign" or "classic" or "scroll";
        }

        /// <summary>
        /// Process game command and return response
        /// </summary>
        private AdventureHouse.Services.Shared.Models.GamePlayResponse ProcessGameCommandInternal(AdventureHouse.Services.Shared.Models.GamePlayRequest request, CommandState commandState, PlayAdventureModel playAdventure, ClientSessionState sessionState)
        {
            var room = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room);
            
            var response = new AdventureHouse.Services.Shared.Models.GamePlayResponse
            {
                SessionId = request.SessionId,
                GameName = playAdventure.GameName,
                RoomName = room.Name,
                RoomDescription = room.Desc,
                PlayerName = playAdventure.Player.Name,
                ItemsInRoom = _messageService.GetRoomItemsList(playAdventure.Player.Room, playAdventure.Items, true),
                PlayerHealth = _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax)
            };

            // Process movement
            var (playerMoved, updatedGameResult, updatedAdventure, updatedCommandState) =
                _roomManagementService.ProcessPlayerMovement(playAdventure, new GameMoveResult 
                { 
                    InstanceID = request.SessionId,
                    RoomName = room.Name,
                    RoomMessage = room.Desc,
                    PlayerName = playAdventure.Player.Name,
                    ItemsMessage = response.ItemsInRoom,
                    HealthReport = response.PlayerHealth
                }, commandState);

            if (playerMoved)
            {
                // Player moved - update map and room info
                sessionState.MapState.UpdatePlayerPosition(updatedAdventure.Player.Room);
                var newRoom = _roomManagementService.GetRoom(updatedAdventure.Rooms, updatedAdventure.Player.Room);
                response.RoomName = newRoom.Name;
                response.RoomDescription = newRoom.Desc + " " + _messageService.GetRoomPath(newRoom);
                response.ItemsInRoom = _messageService.GetRoomItemsList(updatedAdventure.Player.Room, updatedAdventure.Items, true);
                response.CommandResponse = updatedCommandState.Message;
            }
            else
            {
                // Process non-movement actions (get, drop, use, etc.)
                (updatedAdventure, updatedCommandState) = ProcessNonMovementActions(updatedAdventure, updatedCommandState);
                response.CommandResponse = updatedCommandState.Message;
                
                // Update item state in current room
                var currentRoomNumber = updatedAdventure.Player.Room;
                bool hasItems = !string.IsNullOrEmpty(response.ItemsInRoom) && 
                               response.ItemsInRoom != "No Items" && 
                               !response.ItemsInRoom.Contains("nothing");
                sessionState.MapState.UpdateRoomItems(currentRoomNumber, hasItems);
                response.ItemsInRoom = _messageService.GetRoomItemsList(updatedAdventure.Player.Room, updatedAdventure.Items, true);
            }

            // Update final game state
            updatedAdventure.Player.HealthCurrent = _playerManagementService.CalculateNewHealth(updatedAdventure);
            response.PlayerHealth = _playerManagementService.GetHealthReport(updatedAdventure.Player.HealthCurrent, updatedAdventure.Player.HealthMax);
            
            // Check game status
            response.PlayerDead = _playerManagementService.IsPlayerDead(updatedAdventure);
            response.GameCompleted = (updatedAdventure.Player.Room == 0); // Room 0 is typically exit
            // InvalidCommand should only be true if SERVER rejects the command
            // Game logic validation (like missing parameters) should not set InvalidCommand
            response.InvalidCommand = false; // Server processed the command, even if game logic found it invalid

            // Include map data
            response.MapData = CreateMapDataForClient(sessionState.MapState, updatedAdventure.Player.Room, updatedAdventure);

            // Update cache
            _gameInstanceService.UpdateGameInstance(updatedAdventure);

            return response;
        }

        #region Helper Methods

        private IGameConfiguration GetGameConfiguration(int gameId)
        {
            return gameId switch
            {
                1 => new AdventureHouseConfiguration(),
                2 => new SpaceStationConfiguration(),
                3 => new FutureFamilyConfiguration(),
                _ => new AdventureHouseConfiguration() // Default fallback
            };
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
            switch (commandState.Command)
            {
                case "health":
                    commandState.Message = "You feel " + _playerManagementService.GetHealthReport(
                        playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax) + ".";
                    break;
                case "points":
                    commandState.Message = _playerManagementService.GetPlayerPointsMessage(playAdventure);
                    break;
                case "help":
                    commandState.Message = playAdventure.GameHelp;
                    break;
                case "quit":
                    playAdventure.Player.HealthCurrent = 0;
                    commandState.Message = "You have quit the game.";
                    break;
                case "newgame":
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
                        case 3:
                            playAdventure = _futureFamily.SetupAdventure(playAdventure.InstanceID);
                            break;
                        default:
                            playAdventure = _adventureHouse.SetupAdventure(playAdventure.InstanceID);
                            break;
                    }
                    commandState.Message = "Game has been reset. Enjoy!";
                    break;
                default:
                    commandState.Message = "I don't understand that command.";
                    commandState.Valid = false;
                    break;
            }

            return (playAdventure, commandState);
        }

        private PlayerMapData? CreateMapDataForClient(AdventureClient.Models.MapState mapState, int currentRoom, PlayAdventureModel gameInstance)
        {
            if (mapState == null || gameInstance == null)
            {
                return null;
            }

            var sessionState = _clientSessions[gameInstance.InstanceID];
            if (sessionState?.GameConfig == null)
            {
                return null;
            }

            var gameConfig = sessionState.GameConfig;

            // Build discovered rooms list - only include visited rooms
            var discoveredRooms = new List<DiscoveredRoom>();

            foreach (var room in gameInstance.Rooms)
            {
                if (!mapState.IsRoomVisited(room.Number)) continue;

                var discoveredRoom = new DiscoveredRoom
                {
                    RoomNumber = room.Number,
                    Name = room.Name,
                    Level = gameConfig.GetLevelForRoom(room.Number).ToString(),
                    Position = new MapPosition
                    {
                        X = gameConfig.GetRoomPosition(room.Number).X,
                        Y = gameConfig.GetRoomPosition(room.Number).Y
                    },
                    DisplayChar = gameConfig.GetRoomDisplayChar(room.Name),
                    HasItems = DoesRoomHaveItems(room.Number, gameInstance.Items),
                    IsCurrentLocation = (room.Number == currentRoom),
                    Connections = GetDiscoveredConnections(room, mapState)
                };

                discoveredRooms.Add(discoveredRoom);
            }

            return new PlayerMapData
            {
                CurrentRoom = currentRoom,
                CurrentLevel = mapState.CurrentLevel.ToString(),
                CurrentLevelDisplayName = gameConfig.GetLevelDisplayName(mapState.CurrentLevel),
                DiscoveredRooms = discoveredRooms,
                VisitedRoomCount = mapState.VisitedRoomCount,
                RenderingConfig = new MapRenderingConfig
                {
                    GameName = gameConfig.GameName,
                    RoomTypeChars = gameConfig.RoomDisplayCharacters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    LevelDisplayNames = gameConfig.LevelDisplayNames.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
                    DefaultRoomChar = gameConfig.DefaultRoomCharacter,
                    PlayerChar = '@',
                    ItemsChar = '+'
                }
            };
        }

        private bool DoesRoomHaveItems(int roomNumber, List<AdventureHouse.Services.AdventureServer.Models.Item> items)
        {
            return items.Any(item => item.Location == roomNumber);
        }

        private List<RoomConnection> GetDiscoveredConnections(AdventureHouse.Services.AdventureServer.Models.Room room, AdventureClient.Models.MapState mapState)
        {
            var connections = new List<RoomConnection>();

            var directions = new Dictionary<string, int>
            {
                { "north", room.N },
                { "south", room.S },
                { "east", room.E },
                { "west", room.W },
                { "up", room.U },
                { "down", room.D }
            };

            foreach (var direction in directions)
            {
                if (direction.Value != 99 && mapState.IsRoomVisited(direction.Value))
                {
                    connections.Add(new RoomConnection
                    {
                        Direction = direction.Key,
                        TargetRoom = direction.Value,
                        TargetRoomDiscovered = true
                    });
                }
            }

            return connections;
        }

        private string GenerateMapMarkdown(PlayAdventureModel playAdventure, ClientSessionState sessionState)
        {
            var mapState = sessionState?.MapState;
            var gameConfig = sessionState?.GameConfig;

            if (mapState == null || gameConfig == null)
            {
                return "**Map Error**\n\nMap data not available for this game.";
            }

            var currentLevel = mapState.CurrentLevel;
            var currentRoomName = mapState.GetCurrentRoomName();
            var levelDisplayName = gameConfig.GetLevelDisplayName(currentLevel);

            var markdown = new StringBuilder();

            markdown.AppendLine($"# {playAdventure.GameName} - Map Display");
            markdown.AppendLine();
            markdown.AppendLine($"**Current Location:** {currentRoomName}  ");
            markdown.AppendLine($"**Current Level:** {levelDisplayName}  ");
            markdown.AppendLine($"**Rooms Visited:** {mapState.VisitedRoomCount}");
            markdown.AppendLine();
            markdown.AppendLine("**ASCII Map:**");
            markdown.AppendLine("```");
            markdown.AppendLine(mapState.GenerateCurrentLevelMap());
            markdown.AppendLine("```");
            markdown.AppendLine();
            markdown.AppendLine("**Map Symbols:**");
            markdown.AppendLine("- `@` = Your current location");
            markdown.AppendLine("- `+` = Items available in room");
            markdown.AppendLine("- `.` = Horizontal connections");
            markdown.AppendLine("- `:` = Vertical connections");
            markdown.AppendLine("- `^` = Stairs/ladder going up");
            markdown.AppendLine("- `v` = Stairs/ladder going down");

            return markdown.ToString();
        }

        private string GenerateGameIntroMarkdown(IGameConfiguration? gameConfig, PlayAdventureModel playAdventure)
        {
            if (gameConfig == null) return "Game information not available.";

            var markdown = new StringBuilder();

            markdown.AppendLine($"# {gameConfig.GameName}");
            markdown.AppendLine($"**Version:** {gameConfig.GameVersion}");
            markdown.AppendLine();
            markdown.AppendLine($"**Description:** {gameConfig.GameDescription}");
            markdown.AppendLine();
            markdown.AppendLine("## Game Status");
            markdown.AppendLine($"- **Player:** {playAdventure.Player.Name}");
            markdown.AppendLine($"- **Health:** {playAdventure.Player.HealthCurrent}/{playAdventure.Player.HealthMax}");
            markdown.AppendLine($"- **Points:** {playAdventure.Player.Points}");
            markdown.AppendLine($"- **Current Room:** {_roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room).Name}");

            return markdown.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Tracks client session state for clean client-server separation
    /// </summary>
    internal class ClientSessionState
    {
        public bool UseClassicMode { get; set; }
        public bool UseScrollMode { get; set; }
        public int GameId { get; set; }
        public IGameConfiguration? GameConfig { get; set; }
        public AdventureClient.Models.MapState? MapState { get; set; }
    }
}