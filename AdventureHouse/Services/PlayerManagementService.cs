using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public class PlayerManagementService : IPlayerManagementService
    {
        public Player SetPlayerPoints(bool isGeneric, string itemOrRoomName, PlayAdventure playAdventure)
        {
            var existingPoint = playAdventure.PointsCheckList.Find(t => 
                t.ToString().ToLower() == itemOrRoomName.ToLower());

            if (existingPoint == null && isGeneric)
            {
                playAdventure.Player.Points += 5;
                playAdventure.PointsCheckList.Add(itemOrRoomName);
            }
            else if (existingPoint == null)
            {
                var room = playAdventure.Rooms.Find(t => 
                    t.Name.ToLower() == itemOrRoomName.ToLower());
                
                if (room != null)
                {
                    playAdventure.Player.Points += room.RoomPoints;
                    playAdventure.PointsCheckList.Add(room.Name);
                }
                else
                {
                    var item = playAdventure.Items.Find(t => 
                        t.Name.ToLower() == itemOrRoomName.ToLower());
                    
                    if (item != null)
                    {
                        playAdventure.Player.Points += item.ActionPoints;
                        playAdventure.PointsCheckList.Add(item.Name);
                    }
                }
            }

            return playAdventure.Player;
        }

        public bool IsPlayerDead(PlayAdventure playAdventure)
        {
            return playAdventure.Player.HealthCurrent < 1;
        }

        public int CalculateNewHealth(PlayAdventure playAdventure)
        {
            return playAdventure.Player.HealthCurrent - playAdventure.HealthStep;
        }

        public string GetHealthReport(int current, int max)
        {
            double hp = (double)current / max;

            return hp switch
            {
                >= .7 => "Great",
                >= .5 => "Okay",
                >= .3 => "Bad",
                >= .1 => "Horriable",
                _ => "Dead"
            };
        }

        public string GetPlayerPoints(PlayAdventure playAdventure)
        {
            return playAdventure.Player.Points.ToString();
        }

        public string GetPlayerPointsMessage(PlayAdventure playAdventure)
        {
            var result = "";
            result += "\r\nTotal Points : " + GetPlayerPoints(playAdventure);
            result += "\r\nMilestones   : " + GetPlayerPointsString(playAdventure) + "\r\n";
            return result;
        }

        private string GetPlayerPointsString(PlayAdventure playAdventure)
        {
            var result = "";
            var count = 0;
            foreach (var item in playAdventure.PointsCheckList)
            {
                count++;
                result += item.ToUpper();
                if (count < playAdventure.PointsCheckList.Count) 
                    result += ", ";
            }
            return result;
        }
    }
}