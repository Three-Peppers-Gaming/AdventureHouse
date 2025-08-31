using AdventureHouse.Services.Models;

namespace AdventureHouse.Services
{
    public interface ICommandProcessingService
    {
        CommandState ParseCommand(GameMove gameMove);
        string FindCommandSynonym(string command);
        CommandState ConvertShortMove(string direction);
    }
}