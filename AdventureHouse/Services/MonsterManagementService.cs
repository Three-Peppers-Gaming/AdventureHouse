using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public class MonsterManagementService : IMonsterManagementService
    {
        private readonly IPlayerManagementService _playerManagementService;
        private readonly IMessageService _messageService;

        public MonsterManagementService(
            IPlayerManagementService playerManagementService,
            IMessageService messageService)
        {
            _playerManagementService = playerManagementService;
            _messageService = messageService;
        }

        public string CheckForMonsters(PlayAdventure playAdventure)
        {
            var roomMonsters = playAdventure.Monsters
                .Where(m => m.RoomNumber == playAdventure.Player.Room).ToList();
            var messages = new List<string>();
            var random = new Random();

            foreach (var monster in roomMonsters)
            {
                if (!monster.IsPresent && monster.CurrentHealth > 0)
                {
                    if (random.Next(1, 101) <= monster.AppearanceChance)
                    {
                        monster.IsPresent = true;
                        monster.CurrentHealth = monster.AttacksToKill;
                        messages.Add(_messageService.GetFunMessage(playAdventure.Messages, 
                            "MonsterAppear", monster.Name));
                    }
                }

                if (monster.IsPresent && monster.CurrentHealth > 0)
                {
                    if (monster.CanHitPlayer && random.Next(1, 101) <= monster.HitOdds)
                    {
                        playAdventure.Player.HealthCurrent -= monster.HealthDamage;
                        messages.Add(_messageService.GetFunMessage(playAdventure.Messages, 
                            "MonsterHit", monster.Name) + $" (Lost {monster.HealthDamage} health)");
                    }
                    else if (monster.CanHitPlayer)
                    {
                        messages.Add(_messageService.GetFunMessage(playAdventure.Messages, 
                            "MonsterMiss", monster.Name));
                    }
                }
            }

            return string.Join("\r\n", messages);
        }

        public (PlayAdventure, CommandState) AttackMonster(PlayAdventure playAdventure, CommandState commandState)
        {
            var targetName = commandState.Modifier.ToUpper();
            var roomMonsters = playAdventure.Monsters.Where(m => 
                m.RoomNumber == playAdventure.Player.Room && 
                m.IsPresent && 
                m.CurrentHealth > 0).ToList();

            if (!roomMonsters.Any())
            {
                commandState.Message = _messageService.GetFunMessage(playAdventure.Messages, "NoMonster", "");
                commandState.Valid = false;
                return (playAdventure, commandState);
            }

            var targetMonster = roomMonsters.FirstOrDefault(m =>
                m.Name.ToUpper() == targetName ||
                m.Key.ToUpper() == targetName) ?? roomMonsters.First();

            var requiredWeapon = playAdventure.Items.FirstOrDefault(i =>
                i.Name.ToUpper() == targetMonster.ObjectNameThatCanAttackThem.ToUpper() &&
                i.Location == 9999);

            if (requiredWeapon != null)
            {
                return HandleWeaponAttack(playAdventure, commandState, targetMonster, requiredWeapon);
            }

            return HandlePetAttack(playAdventure, commandState, targetMonster);
        }

        private (PlayAdventure, CommandState) HandleWeaponAttack(
            PlayAdventure playAdventure, 
            CommandState commandState, 
            Monster targetMonster, 
            Item weapon)
        {
            targetMonster.CurrentHealth--;
            commandState.Message = $"You successfully attack the {targetMonster.Name} with your {weapon.Name}!";

            if (targetMonster.CurrentHealth <= 0)
            {
                targetMonster.IsPresent = false;
                targetMonster.CurrentHealth = targetMonster.AttacksToKill;
                commandState.Message += "\r\n" + _messageService.GetFunMessage(
                    playAdventure.Messages, "MonsterDefeated", targetMonster.Name);
                
                playAdventure.Player = _playerManagementService.SetPlayerPoints(
                    true, $"Defeated{targetMonster.Name}", playAdventure);
            }

            return (playAdventure, commandState);
        }

        private (PlayAdventure, CommandState) HandlePetAttack(
            PlayAdventure playAdventure, 
            CommandState commandState, 
            Monster targetMonster)
        {
            var followingPet = playAdventure.Items.FirstOrDefault(i => i.Location == 9998);

            if (followingPet != null)
            {
                var random = new Random();
                var petAttackRoll = random.Next(1, 21);

                if (petAttackRoll > 14)
                {
                    targetMonster.CurrentHealth--;
                    commandState.Message = $"Your {followingPet.Name} leaps into action and attacks the {targetMonster.Name}!";

                    if (targetMonster.CurrentHealth <= 0)
                    {
                        targetMonster.IsPresent = false;
                        targetMonster.CurrentHealth = targetMonster.AttacksToKill;
                        commandState.Message += $"\r\nYour brave {followingPet.Name} defeats the {targetMonster.Name}!";
                        commandState.Message += "\r\n" + _messageService.GetFunMessage(
                            playAdventure.Messages, "MonsterDefeated", targetMonster.Name);
                        
                        playAdventure.Player = _playerManagementService.SetPlayerPoints(
                            true, $"PetDefeated{targetMonster.Name}", playAdventure);
                    }
                    else
                    {
                        commandState.Message += $" The {targetMonster.Name} is wounded but still fighting!";
                    }

                    return (playAdventure, commandState);
                }
                else
                {
                    commandState.Message = $"You need a {targetMonster.ObjectNameThatCanAttackThem.ToLower()} to attack the {targetMonster.Name}. ";
                    commandState.Message += $"Your {followingPet.Name} looks ready to help but hesitates.";
                }
            }
            else
            {
                var playerWeapons = playAdventure.Items.Where(i =>
                    i.Location == 9999 &&
                    i.ActionResult?.ToUpper() == "WEAPON").ToList();

                if (playerWeapons.Any())
                {
                    var weaponNames = string.Join(", ", playerWeapons.Select(w => w.Name.ToLower()));
                    commandState.Message = $"Your {weaponNames} won't work against the {targetMonster.Name}. You need a {targetMonster.ObjectNameThatCanAttackThem.ToLower()} to defeat it.";
                }
                else
                {
                    commandState.Message = $"You need a {targetMonster.ObjectNameThatCanAttackThem.ToLower()} to attack the {targetMonster.Name}.";
                }
            }

            commandState.Valid = false;
            return (playAdventure, commandState);
        }

        public string GetMonsterRoomDescription(PlayAdventure playAdventure)
        {
            var roomMonsters = playAdventure.Monsters.Where(m => 
                m.RoomNumber == playAdventure.Player.Room && 
                m.IsPresent && 
                m.CurrentHealth > 0).ToList();
            
            if (roomMonsters.Any())
            {
                var descriptions = roomMonsters.Select(m => m.Description);
                return "\r\n" + string.Join("\r\n", descriptions);
            }
            return string.Empty;
        }
    }
}