using AdventureHouse.Services.Shared.Models;
using AdventureHouse.Services.AdventureServer.Models;

namespace AdventureHouse.Services.Shared.CommandProcessing
{
    public interface ICommandProcessingService
    {
        CommandState ParseCommand(GameMove gameMove);
        string FindCommandSynonym(string command);
        CommandState ConvertShortMove(string direction);
    }
}