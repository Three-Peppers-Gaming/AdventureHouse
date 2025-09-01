using AdventureHouse.Services.Shared.FortuneService;
using AdventureHouse.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.AdventureServer.GameManagement
{
    public interface IItemManagementService
    {
        (PlayAdventureModel, CommandState) ProcessItemCommand(PlayAdventureModel playAdventure, CommandState commandState);
        (PlayAdventureModel, CommandState) ProcessUseItemCommand(PlayAdventureModel playAdventure, CommandState commandState, IGetFortune getFortune);
        Item GetItemDetails(string name, List<Item> items);
        List<Item> MoveItem(List<Item> items, string name, int newRoom);
    }
}