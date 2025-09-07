using AdventureRealms.Services.Shared.FortuneService;
using AdventureRealms.Services.AdventureServer.Models;
using PlayAdventureModel = AdventureRealms.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureRealms.Services.AdventureServer.GameManagement
{
    public interface IItemManagementService
    {
        (PlayAdventureModel, CommandState) ProcessItemCommand(PlayAdventureModel playAdventure, CommandState commandState);
        (PlayAdventureModel, CommandState) ProcessUseItemCommand(PlayAdventureModel playAdventure, CommandState commandState, IGetFortune getFortune);
        Item GetItemDetails(string name, List<Item> items);
        List<Item> MoveItem(List<Item> items, string name, int newRoom);
    }
}