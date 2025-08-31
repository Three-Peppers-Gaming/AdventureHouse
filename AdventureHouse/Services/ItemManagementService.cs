using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public class ItemManagementService : IItemManagementService
    {
        private readonly IPlayerManagementService _playerManagementService;
        private readonly IRoomManagementService _roomManagementService;
        private readonly IMessageService _messageService;

        public ItemManagementService(
            IPlayerManagementService playerManagementService,
            IRoomManagementService roomManagementService,
            IMessageService messageService)
        {
            _playerManagementService = playerManagementService;
            _roomManagementService = roomManagementService;
            _messageService = messageService;
        }

        public Item GetItemDetails(string name, List<Item> items)
        {
            return items.FirstOrDefault(t => t.Name.ToLower().Equals(name.ToLower()));
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

        public (PlayAdventure, CommandState) ProcessItemCommand(PlayAdventure playAdventure, CommandState commandState)
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

        private (PlayAdventure, CommandState) ProcessGetCommand(PlayAdventure playAdventure, CommandState commandState, string requestedItem, bool isDead)
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

        private (PlayAdventure, CommandState) ProcessDropCommand(PlayAdventure playAdventure, CommandState commandState, string requestedItem, bool isDead)
        {
            if (isDead)
            {
                commandState.Message = "Dead people don't need stuff";
                return (playAdventure, commandState);
            }

            var room = _roomManagementService.GetRoom(playAdventure.Rooms, playAdventure.Player.Room);
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

        private (PlayAdventure, CommandState) ProcessPetCommand(PlayAdventure playAdventure, CommandState commandState, string requestedItem)
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

        private (PlayAdventure, CommandState) ProcessShooCommand(PlayAdventure playAdventure, CommandState commandState, string requestedItem, bool isDead)
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

        private (PlayAdventure, CommandState) ProcessInventoryCommand(PlayAdventure playAdventure, CommandState commandState)
        {
            var result = _messageService.GetRoomItemsList(9999, playAdventure.Items, true);
            commandState.Message = "Your pack contains :" + (string.IsNullOrEmpty(result) ? " [Empty]" : result) + "\r\n";
            commandState.Valid = true;
            return (playAdventure, commandState);
        }

        private (PlayAdventure, CommandState) ProcessLookCommand(PlayAdventure playAdventure, CommandState commandState, string requestedItem)
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

        public (PlayAdventure, CommandState) ProcessUseItemCommand(PlayAdventure playAdventure, CommandState commandState, IGetFortune getFortune)
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

        private (PlayAdventure, CommandState) ProcessItemAction(PlayAdventure playAdventure, CommandState commandState, Item item, IGetFortune getFortune)
        {
            switch (item.ActionResult.ToLower())
            {
                case "health":
                    return ProcessHealthAction(playAdventure, commandState, item);
                case "unlock":
                    return ProcessUnlockAction(playAdventure, commandState, item);
                case "teleport":
                    return ProcessTeleportAction(playAdventure, commandState, item);
                case "fortune":
                    return ProcessFortuneAction(playAdventure, commandState, item, getFortune);
                case "weapon":
                    return ProcessWeaponAction(playAdventure, commandState, item);
                default:
                    return (playAdventure, commandState);
            }
        }

        private (PlayAdventure, CommandState) ProcessHealthAction(PlayAdventure playAdventure, CommandState commandState, Item item)
        {
            var currentHealth = playAdventure.Player.HealthCurrent;
            var newHealth = currentHealth + Convert.ToInt32(item.ActionValue);
            playAdventure.Player.HealthCurrent = newHealth;

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

        private (PlayAdventure, CommandState) ProcessUnlockAction(PlayAdventure playAdventure, CommandState commandState, Item item)
        {
            var unlockDetails = item.ActionValue.Split("|").ToList();
            var unlockFromRoom = Convert.ToInt32(unlockDetails[0]);
            var unlockDirection = unlockDetails[1];
            var unlockToRoom = Convert.ToInt32(unlockDetails[2]);
            var unlockedRoomDesc = unlockDetails[3];
            var lockedRoomDesc = unlockDetails[4];

            if (unlockFromRoom != playAdventure.Player.Room)
            {
                commandState.Valid = false;
                return (playAdventure, commandState);
            }

            var room = playAdventure.Rooms[unlockFromRoom];
            var isCurrentlyUnlocked = room.Desc == unlockedRoomDesc;

            // Toggle lock state
            if (isCurrentlyUnlocked)
            {
                room.Desc = lockedRoomDesc;
                SetRoomDirection(room, unlockDirection, 99); // Lock
            }
            else
            {
                room.Desc = unlockedRoomDesc;
                SetRoomDirection(room, unlockDirection, unlockToRoom); // Unlock
            }

            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
            return (playAdventure, commandState);
        }

        private void SetRoomDirection(Room room, string direction, int roomNumber)
        {
            switch (direction.ToLower())
            {
                case "n": room.N = roomNumber; break;
                case "s": room.S = roomNumber; break;
                case "e": room.E = roomNumber; break;
                case "w": room.W = roomNumber; break;
                case "u": room.U = roomNumber; break;
                case "d": room.D = roomNumber; break;
            }
        }

        private (PlayAdventure, CommandState) ProcessTeleportAction(PlayAdventure playAdventure, CommandState commandState, Item item)
        {
            playAdventure.Items = MoveItem(playAdventure.Items, commandState.Modifier, playAdventure.Player.Room);
            playAdventure.Player.Room = Convert.ToInt32(item.ActionValue);
            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
            commandState.Message = "A magical set of fingers has dropped you in this room..";
            return (playAdventure, commandState);
        }

        private (PlayAdventure, CommandState) ProcessFortuneAction(PlayAdventure playAdventure, CommandState commandState, Item item, IGetFortune getFortune)
        {
            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
            commandState.Message = $"You look at the {item.Action} and read: \"{getFortune.ReturnTimeBasedFortune().phrase}\", The text mysteriously fades and disappears.\r\n";
            return (playAdventure, commandState);
        }

        private (PlayAdventure, CommandState) ProcessWeaponAction(PlayAdventure playAdventure, CommandState commandState, Item item)
        {
            var roomMonsters = playAdventure.Monsters.Where(m => 
                m.RoomNumber == playAdventure.Player.Room && 
                m.IsPresent && 
                m.CurrentHealth > 0).ToList();
            
            var targetMonster = roomMonsters.FirstOrDefault(m => 
                m.ObjectNameThatCanAttackThem.ToUpper() == item.Name.ToUpper());

            if (targetMonster != null)
            {
                targetMonster.CurrentHealth--;
                commandState.Message = $"You successfully attack the {targetMonster.Name} with your {item.Name}!";

                if (targetMonster.CurrentHealth <= 0)
                {
                    targetMonster.IsPresent = false;
                    targetMonster.CurrentHealth = targetMonster.AttacksToKill;
                    commandState.Message += "\r\n" + _messageService.GetFunMessage(playAdventure.Messages, "MonsterDefeated", targetMonster.Name);
                    playAdventure.Player = _playerManagementService.SetPlayerPoints(true, $"Defeated{targetMonster.Name}", playAdventure);
                }
            }
            else
            {
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "AttackFailed", "monster");
            }

            playAdventure.Player = _playerManagementService.SetPlayerPoints(false, commandState.Modifier, playAdventure);
            return (playAdventure, commandState);
        }
    }
}