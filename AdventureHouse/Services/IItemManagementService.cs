using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface IItemManagementService
    {
        (PlayAdventure, CommandState) ProcessItemCommand(PlayAdventure playAdventure, CommandState commandState);
        (PlayAdventure, CommandState) ProcessUseItemCommand(PlayAdventure playAdventure, CommandState commandState, IGetFortune getFortune);
        Item GetItemDetails(string name, List<Item> items);
        List<Item> MoveItem(List<Item> items, string name, int newRoom);
    }
}