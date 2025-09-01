using AdventureHouse.Services.Models;
using AdventurHouse.Services;
using AdventureHouse.Services.Data.AdventureData;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace AdventureHouse.Services
{
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
                new Game {Id = 1, Name = _adventureHouse.GetGameConfiguration().GameName, 
                         Ver = _adventureHouse.GetGameConfiguration().GameVersion, 
                         Desc = _adventureHouse.GetGameConfiguration().GameDescription},
                         
                new Game {Id = 2, Name = _spaceStation.GetGameConfiguration().GameName, 
                         Ver = _spaceStation.GetGameConfiguration().GameVersion, 
                         Desc = _spaceStation.GetGameConfiguration().GameDescription}
            };

            return games;
        }

        // Get the appropriate game configuration based on the adventure
        private IGameConfiguration GetGameConfiguration(PlayAdventure playAdventure)
        {
            return playAdventure.GameID switch
            {
                1 => _adventureHouse.GetGameConfiguration(),
                2 => _spaceStation.GetGameConfiguration(),
                _ => _adventureHouse.GetGameConfiguration() // Default fallback
            };
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
            if (new[] { "eat", "use", "read", "wave", "throw", "activate" }.Contains(commandState.Command))
            {
                return _itemManagementService.ProcessUseItemCommand(playAdventure, commandState, _getfortune);
            }

            // Monster combat commands
            if (commandState.Command == "attack")
            {
                return _monsterManagementService.AttackMonster(playAdventure, commandState);
            }

            // Debug commands
#if DEBUG
            if (new[] { "validateadventure", "validate", "check", "verify" }.Contains(commandState.Command))
            {
                var gameConfig = GetGameConfiguration(playAdventure);
                commandState.Message = ValidateAdventureConfiguration(playAdventure, gameConfig);
                return (playAdventure, commandState);
            }
#endif

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
#if DEBUG
                commandState.Message += "\r\n\r\n=== DEBUG COMMANDS ===\r\n" +
                    "validateadventure (or 'validate', 'check', 'verify') - Validates the adventure configuration for errors and warnings.";
#endif
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

#if DEBUG
        private string ValidateAdventureConfiguration(PlayAdventure playAdventure, IGameConfiguration gameConfig)
        {
            var validation = new StringBuilder();
            var issues = new List<string>();
            var warnings = new List<string>();

            validation.AppendLine("=== ADVENTURE VALIDATION REPORT ===");
            validation.AppendLine($"Game: {playAdventure.GameName}");
            validation.AppendLine($"Instance: {playAdventure.InstanceID}");
            validation.AppendLine($"Configuration: {gameConfig.GameName} v{gameConfig.GameVersion}");
            validation.AppendLine();

            // Validate basic game configuration
            ValidateBasicConfiguration(playAdventure, issues, warnings);

            // Validate rooms
            ValidateRooms(playAdventure, issues, warnings, gameConfig);

            // Validate items
            ValidateItems(playAdventure, issues, warnings, gameConfig);

            // Validate messages
            ValidateMessages(playAdventure, issues, warnings);

            // Validate monsters
            ValidateMonsters(playAdventure, issues, warnings);

            // Validate player configuration
            ValidatePlayerConfiguration(playAdventure, issues, warnings);

            // Calculate connection statistics
            var totalConnections = playAdventure.Rooms.SelectMany(r => new[] { r.N, r.S, r.E, r.W, r.U, r.D })
                .Count(c => c > 0 && c != gameConfig.NoConnectionValue);
            
            var itemsInRooms = playAdventure.Items.Count(i => i.Location > 0 && 
                i.Location != gameConfig.InventoryLocation && 
                i.Location != gameConfig.PetFollowLocation);

            // Build summary
            validation.AppendLine($"CONFIGURATION SUMMARY:");
            validation.AppendLine($"- Rooms: {playAdventure.Rooms.Count} (with {totalConnections} valid connections)");
            validation.AppendLine($"- Items: {playAdventure.Items.Count} ({itemsInRooms} placed in rooms)");
            validation.AppendLine($"- Messages: {playAdventure.Messages.Count} (for game variety)");
            validation.AppendLine($"- Monsters: {playAdventure.Monsters.Count}");
            validation.AppendLine($"- Starting Room: {playAdventure.StartRoom} ({playAdventure.Rooms.FirstOrDefault(r => r.Number == playAdventure.StartRoom)?.Name ?? "Unknown"})");
            validation.AppendLine($"- Max Health: {playAdventure.MaxHealth}, Health Step: {playAdventure.HealthStep}");
            validation.AppendLine();

            validation.AppendLine($"VALIDATION RESULTS:");
            validation.AppendLine($"- Critical Issues: {issues.Count}");
            validation.AppendLine($"- Warnings/Notes: {warnings.Count}");
            validation.AppendLine();

            if (issues.Any())
            {
                validation.AppendLine("(X) CRITICAL ISSUES (Need fixing):");
                foreach (var issue in issues)
                {
                    validation.AppendLine($"  • {issue}");
                }
                validation.AppendLine();
            }

            if (warnings.Any())
            {
                validation.AppendLine("⚠️  WARNINGS & NOTES:");
                foreach (var warning in warnings)
                {
                    validation.AppendLine($"  • {warning}");
                }
                validation.AppendLine();
            }

            if (!issues.Any() && !warnings.Any())
            {
                validation.AppendLine("✅ Adventure configuration appears to be perfectly valid!");
            }
            else if (!issues.Any())
            {
                validation.AppendLine("✅ No critical issues found! Adventure should work correctly.");
                validation.AppendLine("   Warnings above are informational or suggest improvements.");
            }

            validation.AppendLine();
            validation.AppendLine($"Validation completed at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            return validation.ToString();
        }

        private void ValidateBasicConfiguration(PlayAdventure playAdventure, List<string> issues, List<string> warnings)
        {
            if (string.IsNullOrEmpty(playAdventure.GameName))
                issues.Add("Game name is empty");

            if (string.IsNullOrEmpty(playAdventure.WelcomeMessage))
                warnings.Add("Welcome message is empty");

            if (string.IsNullOrEmpty(playAdventure.GameHelp))
                warnings.Add("Game help is empty");

            if (playAdventure.MaxHealth <= 0)
                issues.Add("Max health must be greater than 0");

            if (playAdventure.HealthStep <= 0)
                warnings.Add("Health step should be greater than 0");
        }

        private void ValidateRooms(PlayAdventure playAdventure, List<string> issues, List<string> warnings, IGameConfiguration gameConfig)
        {
            if (!playAdventure.Rooms.Any())
            {
                issues.Add("No rooms defined");
                return;
            }

            var roomNumbers = playAdventure.Rooms.Select(r => r.Number).ToList();
            var duplicateRooms = roomNumbers.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);

            foreach (var duplicate in duplicateRooms)
            {
                issues.Add($"Duplicate room number: {duplicate}");
            }

            // Check if starting room exists
            if (!roomNumbers.Contains(playAdventure.StartRoom))
            {
                issues.Add($"Starting room {playAdventure.StartRoom} does not exist");
            }

            foreach (var room in playAdventure.Rooms)
            {
                if (string.IsNullOrEmpty(room.Name))
                    warnings.Add($"Room {room.Number} has no name");

                if (string.IsNullOrEmpty(room.Desc))
                    warnings.Add($"Room {room.Number} has no description");

                // Validate room connections (skip the NoConnectionValue which is 99)
                var connections = new[] { room.N, room.S, room.E, room.W, room.U, room.D };
                foreach (var connection in connections.Where(c => c > 0 && c != gameConfig.NoConnectionValue))
                {
                    if (!roomNumbers.Contains(connection))
                    {
                        issues.Add($"Room {room.Number} connects to non-existent room {connection}");
                    }
                }

                // Check for isolated rooms (no connections in or out, excluding NoConnectionValue)
                bool hasOutgoing = connections.Any(c => c > 0 && c != gameConfig.NoConnectionValue);
                bool hasIncoming = playAdventure.Rooms.Any(r => 
                    new[] { r.N, r.S, r.E, r.W, r.U, r.D }.Any(dir => dir == room.Number));

                if (!hasOutgoing && !hasIncoming && room.Number != playAdventure.StartRoom)
                {
                    warnings.Add($"Room {room.Number} ({room.Name}) appears to be isolated");
                }
            }
        }

        private void ValidateItems(PlayAdventure playAdventure, List<string> issues, List<string> warnings, IGameConfiguration gameConfig)
        {
            var roomNumbers = playAdventure.Rooms.Select(r => r.Number).ToList();
            
            foreach (var item in playAdventure.Items)
            {
                if (string.IsNullOrEmpty(item.Name))
                    warnings.Add("Item with empty name found");

                if (string.IsNullOrEmpty(item.Description))
                    warnings.Add($"Item '{item.Name}' has no description");

                // Validate item location (including special locations)
                if (item.Location > 0 && 
                    item.Location != gameConfig.InventoryLocation && 
                    item.Location != gameConfig.PetFollowLocation &&
                    item.Location != gameConfig.NoConnectionValue)
                {
                    if (!roomNumbers.Contains(item.Location))
                    {
                        issues.Add($"Item '{item.Name}' is in non-existent room {item.Location}");
                    }
                }

                // Validate action configuration
                if (!string.IsNullOrEmpty(item.Action))
                {
                    if (string.IsNullOrEmpty(item.ActionVerb))
                        warnings.Add($"Item '{item.Name}' has action but no action verb");
                    
                    if (string.IsNullOrEmpty(item.ActionResult))
                        warnings.Add($"Item '{item.Name}' has action but no action result");
                }
            }

            // Check for duplicate item names (but allow duplicates as they may be intentional for variety)
            var duplicateItems = playAdventure.Items.GroupBy(i => i.Name.ToLower())
                .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key))
                .Select(g => g.Key);

            foreach (var duplicate in duplicateItems)
            {
                warnings.Add($"Duplicate item name: {duplicate} (may be intentional for variety)");
            }
        }

        private void ValidateMessages(PlayAdventure playAdventure, List<string> issues, List<string> warnings)
        {
            foreach (var message in playAdventure.Messages)
            {
                if (string.IsNullOrEmpty(message.MessageTag))
                    warnings.Add("Message with empty tag found");

                if (string.IsNullOrEmpty(message.Messsage))
                    warnings.Add($"Message '{message.MessageTag}' has empty content");
            }

            // Check for duplicate message tags (but note they may be intentional for variety)
            var duplicateTags = playAdventure.Messages.GroupBy(m => m.MessageTag.ToLower())
                .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key))
                .Select(g => g.Key);

            foreach (var duplicate in duplicateTags)
            {
                warnings.Add($"Multiple messages for tag '{duplicate}' (adds variety to game text)");
            }

            // Check for essential message tags
            var essentialTags = new[] { "dead", "bad" };
            var existingTags = playAdventure.Messages.Select(m => m.MessageTag.ToLower()).ToList();
            
            foreach (var tag in essentialTags)
            {
                if (!existingTags.Contains(tag))
                {
                    warnings.Add($"Missing recommended message tag: {tag}");
                }
            }

            // Check for "pet" messages (using petfollow instead of pet)
            if (!existingTags.Contains("petfollow"))
            {
                warnings.Add("Missing recommended message tag: petfollow (for pet following messages)");
            }
        }

        private void ValidateMonsters(PlayAdventure playAdventure, List<string> issues, List<string> warnings)
        {
            var roomNumbers = playAdventure.Rooms.Select(r => r.Number).ToList();
            var itemNames = playAdventure.Items.Select(i => i.Name.ToLower()).ToList();

            foreach (var monster in playAdventure.Monsters)
            {
                if (string.IsNullOrEmpty(monster.Name))
                    warnings.Add("Monster with empty name found");

                if (string.IsNullOrEmpty(monster.Description))
                    warnings.Add($"Monster '{monster.Name}' has no description");

                if (monster.RoomNumber > 0 && !roomNumbers.Contains(monster.RoomNumber))
                {
                    issues.Add($"Monster '{monster.Name}' is in non-existent room {monster.RoomNumber}");
                }

                if (monster.AttacksToKill <= 0)
                    warnings.Add($"Monster '{monster.Name}' has invalid attacks to kill value");

                if (!string.IsNullOrEmpty(monster.ObjectNameThatCanAttackThem))
                {
                    if (!itemNames.Contains(monster.ObjectNameThatCanAttackThem.ToLower()))
                    {
                        warnings.Add($"Monster '{monster.Name}' references non-existent attack item '{monster.ObjectNameThatCanAttackThem}'");
                    }
                }

                if (monster.HitOdds < 0 || monster.HitOdds > 100)
                    warnings.Add($"Monster '{monster.Name}' has invalid hit odds: {monster.HitOdds}");

                if (monster.AppearanceChance < 0 || monster.AppearanceChance > 100)
                    warnings.Add($"Monster '{monster.Name}' has invalid appearance chance: {monster.AppearanceChance}");
            }

            // Check for duplicate monster keys
            var duplicateKeys = playAdventure.Monsters.GroupBy(m => m.Key.ToLower())
                .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key))
                .Select(g => g.Key);

            foreach (var duplicate in duplicateKeys)
            {
                warnings.Add($"Duplicate monster key: {duplicate}");
            }
        }

        private void ValidatePlayerConfiguration(PlayAdventure playAdventure, List<string> issues, List<string> warnings)
        {
            if (playAdventure.Player == null)
            {
                issues.Add("Player object is null");
                return;
            }

            if (string.IsNullOrEmpty(playAdventure.Player.Name))
                warnings.Add("Player has no name");

            if (playAdventure.Player.HealthMax != playAdventure.MaxHealth)
                warnings.Add($"Player max health ({playAdventure.Player.HealthMax}) doesn't match game max health ({playAdventure.MaxHealth})");

            if (playAdventure.Player.Room != playAdventure.StartRoom)
                warnings.Add($"Player room ({playAdventure.Player.Room}) doesn't match start room ({playAdventure.StartRoom})");

            var roomNumbers = playAdventure.Rooms.Select(r => r.Number).ToList();
            if (!roomNumbers.Contains(playAdventure.Player.Room))
            {
                issues.Add($"Player is in non-existent room {playAdventure.Player.Room}");
            }
        }
#endif

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