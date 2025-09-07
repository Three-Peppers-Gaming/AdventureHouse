using AdventureRealms.Services.Shared.Models;
using AdventureRealms.Services.AdventureServer.Models;

namespace AdventureRealms.Services.Shared.CommandProcessing
{
    public class CommandProcessingService : ICommandProcessingService
    {
        public CommandState ParseCommand(GameMove gameMove)
        {
            var command = gameMove.Move?.Trim().ToLower() ?? "";
            
            var cs = new CommandState
            {
                Valid = true,
                Modifier = "",
                Message = ""
            };

            if (!string.IsNullOrEmpty(command))
            {
                var commandList = command.Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).Take(2).ToList();

                if (commandList.Count > 1)
                {
                    cs.RawCommand = gameMove.Move;
                    cs.Command = commandList[0];
                    cs.Modifier = commandList[1];
                }
                else if (commandList.Count == 1)
                {
                    cs.RawCommand = gameMove.Move;
                    cs.Command = commandList[0];
                }
            }
            else
            {
                cs.Valid = false;
                cs.RawCommand = "";
                cs.Command = "";
            }

            return cs;
        }

        public string FindCommandSynonym(string cmd)
        {
            if (string.IsNullOrEmpty(cmd)) return "";

            return cmd.Trim() switch
            {
                "pick" => "get",
                "take" => "get",
                "pickup" => "get",
                "grab" => "get",
                "run" => "go",
                "move" => "go",
                "climb" => "go",
                "put" => "drop",
                "bite" => "use",
                "taste" => "use",
                "unlock" => "use",
                "lock" => "use",
                "start" => "activate",
                "power" => "activate",
                "boot" => "activate",
                "turn" => "activate",
                "examine" => "look",
                "inventory" => "inv",
                "pack" => "inv",
                "scare" => "shoo",
                "kick" => "shoo",
                "kiss" => "pet",
                "hug" => "pet",
                "activate" => "pet",
                "enable" => "pet",
                "wake" => "pet",
                "awaken" => "pet",
                "restart" => "quit",
                "score" => "points",
                "result" => "points",
                "study" => "read",
                "learn" => "read",
                "fight" => "attack",
                "hit" => "attack",
                "kill" => "attack",
                "slay" => "attack",
                "strike" => "attack",
#if DEBUG
                "validate" => "validateadventure",
                "check" => "validateadventure",
                "verify" => "validateadventure",
#endif
                _ => cmd,
            };
        }

        public CommandState ConvertShortMove(string direction)
        {
            var cs = new CommandState
            {
                Valid = true,
                RawCommand = direction,
                Command = "go",
                Modifier = "",
                Message = ""
            };

            cs.Modifier = direction.ToLower() switch
            {
                "n" or "nor" => "north",
                "s" or "sou" => "south",
                "e" or "eas" => "east",
                "w" or "wes" => "west",
                "u" or "up" => "up",
                "d" or "dow" => "down",
                _ => ""
            };

            if (string.IsNullOrEmpty(cs.Modifier))
            {
                cs.Message = "Wrong Way!";
                cs.Valid = false;
            }

            return cs;
        }
    }
}