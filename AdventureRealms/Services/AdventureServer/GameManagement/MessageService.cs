using AdventureRealms.Services.AdventureServer.Models;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public class MessageService : IMessageService
    {
        public string GetFunMessage(List<Message> messages, string action, string commandOrObject)
        {
            if (action == null) action = "any";
            else action = action.ToLower();

            var random = new Random();
            var queryMessages = messages.FindAll(t => t.MessageTag.ToLower() == action.ToLower()).ToList();

            if (queryMessages.Count == 0)
            {
                queryMessages = messages.FindAll(t => t.MessageTag.ToLower() == "any").ToList();
            }

            if (queryMessages.Count > 0)
            {
                var msgId = random.Next(0, queryMessages.Count);
                var message = queryMessages[msgId].Messsage;
                
                if (message.Contains("@"))
                {
                    return message.Replace("@", commandOrObject);
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    return message;
                }
            }

            return "You can't do that here.";
        }

        public string GetRoomPath(Room room)
        {
            var result = "";
            var positionCount = 0;
            var commaSeparator = 0;
            var andCounter = 99;

            if (room.N != 99) positionCount++;
            if (room.S != 99) positionCount++;
            if (room.E != 99) positionCount++;
            if (room.W != 99) positionCount++;
            if (room.U != 99) positionCount++;
            if (room.D != 99) positionCount++;

            if (positionCount == 0) return "";
            if (positionCount > 1) andCounter = positionCount - 1;

            if (room.N != 99)
            {
                commaSeparator++;
                result += "north";
                if (andCounter == commaSeparator) { andCounter = 99; result += ", and "; }
                else if (andCounter != 99 && positionCount > 2 && commaSeparator > 0) result += ", ";
            }

            if (room.S != 99)
            {
                commaSeparator++;
                result += "south";
                if (andCounter == commaSeparator) { andCounter = 99; result += ", and "; }
                else if (andCounter != 99 && positionCount > 2 && commaSeparator > 0) result += ", ";
            }

            if (room.E != 99)
            {
                commaSeparator++;
                result += "east";
                if (andCounter == commaSeparator) { andCounter = 99; result += ", and "; }
                else if (andCounter != 99 && positionCount > 2 && commaSeparator > 0) result += ", ";
            }

            if (room.W != 99)
            {
                commaSeparator++;
                result += "west";
                if (andCounter == commaSeparator) { andCounter = 99; result += ", and "; }
                else if (andCounter != 99 && positionCount > 2 && commaSeparator > 0) result += ", ";
            }

            if (room.U != 99)
            {
                commaSeparator++;
                result += "up";
                if (andCounter == commaSeparator) { andCounter = 99; result += ", and "; }
                else if (andCounter != 99 && positionCount > 2 && commaSeparator > 0) result += ", ";
            }

            if (room.D != 99)
            {
                result += "down";
            }

            return "You can go " + result + " from here.\r\n";
        }

        public string GetRoomItemsList(int roomNumber, List<Item> items, bool verbose)
        {
            var result = GetRoomInventory(roomNumber, items);

            if (verbose)
            {
                return result;
            }
            else
            {
                return result == "No Items" ? "" : result;
            }
        }

        private string GetRoomInventory(int room, List<Item> items)
        {
            var itemString = "";
            var count = 0;

            foreach (var item in items)
            {
                if (item.Location == room)
                {
                    if (count > 0) itemString += ", ";
                    itemString += item.Name;
                    count++;
                }
            }

            if (count == 0) itemString = "No Items";
            return itemString;
        }

        public string GetHasPetMessage(List<Item> items, List<Message> messages, string eol)
        {
            var result = items.FirstOrDefault(t => t.Location == 9998);
            if (result is null) return "";
            return GetFunMessage(messages, "petfollow", result.Name) + eol;
        }
    }
}