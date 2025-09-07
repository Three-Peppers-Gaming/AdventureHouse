using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureServer.Models;

namespace AdventureRealms.Services.Shared.CommandProcessing
{
    public interface ICommandProcessingService
    {
        CommandState ParseCommand(GameMove gameMove);
        string FindCommandSynonym(string command);
        CommandState ConvertShortMove(string direction);
    }
}