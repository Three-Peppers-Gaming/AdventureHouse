using AdventureRealms.Services.Shared.FortuneService;
using AdventureRealms.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public class ItemManagementService : IItemManagementService
    {
        private readonly IPlayerManagementService _playerManagementService;
        private IRoomManagementService? _roomManagementService;
        private readonly IMessageService _messageService;

        public ItemManagementService(
            IPlayerManagementService playerManagementService,
            IRoomManagementService? roomManagementService,
            IMessageService messageService)
        {
            _playerManagementService = playerManagementService;
            _roomManagementService = roomManagementService;
            _messageService = messageService;
        }

        // Allow setting the room management service after construction to handle circular dependency
        public void SetRoomManagementService(IRoomManagementService roomManagementService)
        {
            _roomManagementService = roomManagementService;
        }

        public Item GetItemDetails(string name, List<Item> items)
        {
            return items.FirstOrDefault(t => t.Name.ToLower().Equals(name.ToLower()))!;
        }

        public List<Item> MoveItem(List<Item> items, string name, int newRoom)
        {
            var itemIndex = items.FindIndex(t => t.Name.ToLower().Equals(name.ToLower()));
            if (itemIndex >= 0)
            {
                var item = items[itemIndex];
                item.Location = newRoom;
                items[itemIndex] = item;
            }
            return items;
        }

        public (PlayAdventureModel, CommandState) ProcessItemCommand(PlayAdventureModel playAdventure, CommandState commandState)
        {
            var requestedItem = commandState.Modifier.ToLower();
            var isDead = _playerManagementService.IsPlayerDead(playAdventure);

            switch (commandState.Command)
            {
                case "get":
                    return ProcessGetCommand(playAdventure, commandState, requestedItem, isDead);
                case "drop":
                    return ProcessDropCommand(playAdventure, commandState, requestedItem, isDead);
                case "pet":
                    return ProcessPetCommand(playAdventure, commandState, requestedItem);
                case "shoo":
                    return ProcessShooCommand(playAdventure, commandState, requestedItem, isDead);
                case "inv":
                    return ProcessInventoryCommand(playAdventure, commandState);
                case "look":
                    return ProcessLookCommand(playAdventure, commandState, requestedItem);
                default:
                    return (playAdventure, commandState);
            }
        }

        private (PlayAdventureModel, CommandState) ProcessGetCommand(PlayAdventureModel playAdventure, CommandState commandState, string requestedItem, bool isDead)
        {
            if (isDead)
            {
                commandState.Message = "Dead people don't need stuff";
                return (playAdventure, commandState);
            }

            var item = GetItemDetails(requestedItem, playAdventure.Items);
            if (item != null && item.Location == playAdventure.Player.Room)
            {
                if (item.ActionVerb.ToLower() == "pet")
                {
                    commandState.Valid = false;
                    commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "petfailed", commandState.Modifier) + "\r\n";
                }
                else
                {
                    playAdventure.Items = MoveItem(playAdventure.Items, requestedItem, 9999); // 9999 is backpack
                    commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "getsuccess", commandState.Modifier);
                }
            }
            else
            {
                commandState.Valid = false;
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "getfailed", commandState.Modifier) + "\r\n";
            }

            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessDropCommand(PlayAdventureModel playAdventure, CommandState commandState, string requestedItem, bool isDead)
        {
            if (isDead)
            {
                commandState.Message = "Dead people don't need stuff";
                return (playAdventure, commandState);
            }

            var room = _roomManagementService!.GetRoom(playAdventure.Rooms, playAdventure.Player.Room);
            var item = GetItemDetails(requestedItem, playAdventure.Items);

            if (item != null && item.Location == 9999)
            {
                playAdventure.Items = MoveItem(playAdventure.Items, requestedItem, room.Number);
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "dropsuccess", commandState.Modifier) + "\r\n";
            }
            else
            {
                commandState.Valid = false;
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "dropfailed", commandState.Modifier) + "\r\n";
            }

            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessPetCommand(PlayAdventureModel playAdventure, CommandState commandState, string requestedItem)
        {
            var item = GetItemDetails(requestedItem, playAdventure.Items);

            if (item != null && (item.Location == playAdventure.Player.Room || item.Location == 9998))
            {
                playAdventure.Items = MoveItem(playAdventure.Items, requestedItem, 9998);
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "petsuccess", commandState.Modifier) + "\r\n";
                playAdventure.Player = _playerManagementService.SetPlayerPoints(false, item.Name, playAdventure);
            }
            else
            {
                commandState.Valid = false;
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "any", commandState.Modifier) + "\r\n";
            }

            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessShooCommand(PlayAdventureModel playAdventure, CommandState commandState, string requestedItem, bool isDead)
        {
            if (isDead)
            {
                commandState.Message = "Dead people cannot scare pets!";
                return (playAdventure, commandState);
            }

            var item = GetItemDetails(requestedItem, playAdventure.Items);

            if (item != null && item.Location == 9998)
            {
                playAdventure.Items = MoveItem(playAdventure.Items, requestedItem, Convert.ToInt16(item.ActionValue));
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "shoosuccess", commandState.Modifier) + "\r\n";
            }
            else
            {
                commandState.Valid = false;
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "any", commandState.Modifier) + "\r\n";
            }

            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessInventoryCommand(PlayAdventureModel playAdventure, CommandState commandState)
        {
            var result = _messageService.GetRoomItemsList(9999, playAdventure.Items, true);
            commandState.Message = "Your pack contains :" + (string.IsNullOrEmpty(result) ? " [Empty]" : result) + "\r\n";
            commandState.Valid = true;
            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessLookCommand(PlayAdventureModel playAdventure, CommandState commandState, string requestedItem)
        {
            if (string.IsNullOrEmpty(requestedItem))
            {
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "LookEmpty", commandState.Command);
                commandState.Valid = false;
            }
            else
            {
                var item = GetItemDetails(requestedItem, playAdventure.Items);

                if (item != null && (item.Location == 9999 || item.Location == playAdventure.Player.Room))
                {
                    commandState.Message = "You look at the " + commandState.Modifier.ToLower() + " and see: " + item.Description;
                }
                else
                {
                    commandState.Valid = false;
                    commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, 
                        item != null ? "LookFailed" : "LookEmpty", 
                        item != null ? commandState.Modifier : "");
                }
            }

            return (playAdventure, commandState);
        }

        public (PlayAdventureModel, CommandState) ProcessUseItemCommand(PlayAdventureModel playAdventure, CommandState commandState, IGetFortune getFortune)
        {
            var requestedItem = commandState.Modifier.ToLower();
            var item = GetItemDetails(requestedItem, playAdventure.Items);

            if (item == null || _playerManagementService.IsPlayerDead(playAdventure) || item.Location != 9999)
            {
                commandState.Valid = false;
                commandState.Message = item == null 
                    ? _messageService.GetFunMessage(playAdventure.Messages, "UseFailed", commandState.Modifier) + "\r\n"
                    : _playerManagementService.IsPlayerDead(playAdventure) 
                        ? "Dead people don't need stuff"
                        : _messageService.GetFunMessage(playAdventure.Messages, "UseFailed", commandState.Modifier) + "\r\n";
                return (playAdventure, commandState);
            }

            if (item.ActionVerb.ToLower() != commandState.Command.ToLower())
            {
                commandState.Valid = false;
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "UseFailed", commandState.Modifier) + "\r\n";
                return (playAdventure, commandState);
            }

            return ProcessItemAction(playAdventure, commandState, item, getFortune);
        }

        private (PlayAdventureModel, CommandState) ProcessItemAction(PlayAdventureModel playAdventure, CommandState commandState, Item item, IGetFortune getFortune)
        {
            switch (item.ActionResult.ToLower())
            {
                case "health":
                    return ProcessHealthAction(playAdventure, commandState, item);
                case "fortune":
                    return ProcessFortuneAction(playAdventure, commandState, item, getFortune);
                case "unlock":
                    return ProcessUnlockAction(playAdventure, commandState, item);
                default:
                    commandState.Message = "Nothing happens.";
                    return (playAdventure, commandState);
            }
        }

        private (PlayAdventureModel, CommandState) ProcessHealthAction(PlayAdventureModel playAdventure, CommandState commandState, Item item)
        {
            var currentHealth = playAdventure.Player.HealthCurrent;
            var newHealth = currentHealth + Convert.ToInt32(item.ActionValue);
            playAdventure.Player.HealthCurrent = newHealth;

            // Remove the consumed item from inventory (move to location 9998 = used)
            if (item.ActionVerb.ToLower() == "eat")
            {
                playAdventure.Items = MoveItem(playAdventure.Items, item.Name, 9998);
            }

            if (currentHealth > newHealth)
            {
                commandState.Message = "That made you feel bad. Don't do that too much.\r\n\r\n" + 
                    "Your health is " + _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax);
            }
            else if (newHealth > playAdventure.Player.HealthMax)
            {
                commandState.Message = "That made you feel very full.\r\n\r\n" + 
                    "Currently you feel " + _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax);
            }
            else if (currentHealth < newHealth)
            {
                commandState.Message = "That made you feel better.\r\n\r\n" + 
                    "You currently feel " + _playerManagementService.GetHealthReport(playAdventure.Player.HealthCurrent, playAdventure.Player.HealthMax);
            }

            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessFortuneAction(PlayAdventureModel playAdventure, CommandState commandState, Item item, IGetFortune getFortune)
        {
            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
            commandState.Message = $"You look at the {item.Action} and read: \"{getFortune.ReturnTimeBasedFortune().phrase}\", The text mysteriously fades and disappears.\r\n";
            return (playAdventure, commandState);
        }

        private (PlayAdventureModel, CommandState) ProcessUnlockAction(PlayAdventureModel playAdventure, CommandState commandState, Item item)
        {
            // ActionValue format: "roomNum|direction|newRoom|newDesc1|newDesc2"
            var actionParts = item.ActionValue.Split('|');
            if (actionParts.Length >= 3)
            {
                var roomNumber = Convert.ToInt32(actionParts[0]);
                var direction = actionParts[1].ToUpper();
                var newRoomConnection = Convert.ToInt32(actionParts[2]);

                // Find and update the room
                var room = playAdventure.Rooms.FirstOrDefault(r => r.Number == roomNumber);
                if (room != null)
                {
                    // Update the room connection based on direction
                    switch (direction)
                    {
                        case "N": room.N = newRoomConnection; break;
                        case "S": room.S = newRoomConnection; break;
                        case "E": room.E = newRoomConnection; break;
                        case "W": room.W = newRoomConnection; break;
                        case "U": room.U = newRoomConnection; break;
                        case "D": room.D = newRoomConnection; break;
                    }

                    // Update room description if provided
                    if (actionParts.Length >= 4 && !string.IsNullOrEmpty(actionParts[3]))
                    {
                        room.Desc = actionParts[3];
                    }

                    // Display the action message and success message
                    commandState.Message = item.Action + "\r\n";
                    if (actionParts.Length >= 5 && !string.IsNullOrEmpty(actionParts[4]))
                    {
                        commandState.Message += actionParts[4] + "\r\n";
                    }

                    // Award points and consume the key
                    playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
                    playAdventure.Items = MoveItem(playAdventure.Items, item.Name, 9998); // Move to used
                }
                else
                {
                    commandState.Message = "The key doesn't seem to work here.\r\n";
                }
            }
            else
            {
                commandState.Message = "The key is damaged and won't work.\r\n";
            }

            return (playAdventure, commandState);
        }
    }
}