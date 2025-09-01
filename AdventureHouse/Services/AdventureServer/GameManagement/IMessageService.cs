using AdventureHouse.Services.AdventureServer.Models;

namespace AdventureHouse.Services.AdventureServer.GameManagement
{
    public interface IMessageService
    {
        string GetFunMessage(List<Message> messages, string action, string commandOrObject);
        string GetRoomPath(Room room);
        string GetRoomItemsList(int roomNumber, List<Item> items, bool verbose);
        string GetHasPetMessage(List<Item> items, List<Message> messages, string eol);
    }
}