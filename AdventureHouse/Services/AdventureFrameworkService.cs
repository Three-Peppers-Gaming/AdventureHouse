using AdventureHouse.Services.Models;
using AdventurHouse.Services;
using AdventureHouse.Services.Data.AdventureData;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureHouse.Services
{
    public class AdventureFrameworkService : IPlayAdventure
    {
        // Game 1 
        private readonly Data.AdventureData.AdventureHouseData adventureHouse = new();

        // storage for the adventures
        private readonly IMemoryCache _gameCache;

        // Fortune Service for Reading the Book 
        private readonly IGetFortune _getfortune;

        // Game configuration for current game
        private readonly IGameConfiguration _gameConfig;

        public AdventureFrameworkService(IMemoryCache GameCache, IGetFortune getfortune)
        {
            _gameCache = GameCache;
            _getfortune = getfortune;
            _gameConfig = adventureHouse.GetGameConfiguration();
        }

        #region Game Cache Management

        private void Cache_AddPlayAdventure(PlayAdventure p)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions()
              // Keep in cache for this time, reset time if accessed.
              .SetSlidingExpiration(TimeSpan.FromMinutes(8 * 60)); //  8 hours

            _ = _gameCache.Set(p.InstanceID, p, cacheEntryOptions); 

        }

        private void Cache_ReplacePlayAdventure(PlayAdventure p)
        {
            _gameCache.Remove(p.InstanceID);
            Cache_AddPlayAdventure(p);
        }

        private PlayAdventure Cache_GetPlayAdventure(string key)
        {
            var cacheEntry = _gameCache.Get<PlayAdventure>(key);

            return cacheEntry;
        }

        private void Cache_RemovePlayAdventure(string key)
        {
            _gameCache.Remove(key);

        }

        #endregion Game Cache Management 

        #region Instance Management 

        private string GameInstance_New(int gamechoice)
        {
            var tempId = Guid.NewGuid().ToString();
            if (gamechoice == 1)
            {
                var p = adventureHouse.SetupAdventure(tempId);
                Cache_AddPlayAdventure(p);
            }

            return tempId;
        }

        public Boolean GameInstance_Exists(string id)
        {
            var adventure = Cache_GetPlayAdventure(id);

            if (adventure.InstanceID == null) return false;
            return true;
        }

        public PlayAdventure GameInstance_GetObject(string InstanceId)
        {

            PlayAdventure playAdventure = Cache_GetPlayAdventure(InstanceId);
            if (playAdventure.InstanceID == null)
            {
                playAdventure.StartRoom = -1;
                playAdventure.WelcomeMessage = "Error: No Instance Found!";
            }
            return playAdventure;
        }

        public Boolean GameInstance_Update(PlayAdventure p)
        {
            if (GameInstance_Exists(p.InstanceID))
            {
                Cache_ReplacePlayAdventure(p);
                return true;
            }

            return false;
        }

        public Boolean GameInstance_Delete(string InstanceID)
        {
            if (GameInstance_Exists(InstanceID))
            {
                Cache_RemovePlayAdventure(InstanceID);
                return true;
            }

            return false;
        }


        #endregion Instance Management

        #region Game Management - Interface Public Entry

        public List<Game> FrameWork_GetGames()
        {
            List<Game> _games = new()
            {
                new Game {Id =1, Name=_gameConfig.GameName, Ver=_gameConfig.GameVersion, Desc=_gameConfig.GameDescription  },
                new Game {Id =1, Name="Adventure House Part 2!", Ver="00", Desc="Exact same game as API Adventure house but using a different name"}
            };

            return _games;
        }

        public GameMoveResult FrameWork_NewGame(int GameID)
        {
            // If the game ID is not in the games list default to 1
            if (!FrameWork_GetGames().Exists(g => g.Id == GameID))
            {
                GameID = 1;
            }

            // setup the game instance 
            var p = GameInstance_GetObject(GameInstance_New(GameID).ToString());

            //get the room details for the first room 
            var _room = Object_GetRoom(p.Rooms, p.Player.Room);

            return new GameMoveResult
            {
                InstanceID = p.InstanceID,
                RoomName = _room.Name,
                RoomMessage = p.WelcomeMessage + _room.Desc + " " + GetRoomPath(_room),
                ItemsMessage = GetRoomItemsList(p.Player.Room, p.Items, p.Player.Verbose),
                PlayerName = p.Player.Name,
                HealthReport = GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax),
            };

        }

        public GameMoveResult FrameWork_GameMove(GameMove move)
        {

            if (GameInstance_Exists(move.InstanceID))
            {
                var gmr = Main_ProcessGameMove(move);
                return gmr;
            }
            else return new GameMoveResult
            {
                InstanceID = "-1",
                ItemsMessage = "",
                RoomMessage = "Game does not exist. Please begin again."
            };

        }
        #endregion Game Management 

        #region Set Player Points and Health

        private static Player Helper_SetPlayerPoints(bool isgeneric, string ItemorRoomName, PlayAdventure p)
        {
            Room rm;
            Item item;

            string c;
            c = p.PointsCheckList.Find(t => t.ToString().ToLower() == ItemorRoomName.ToLower());


            if ((c == null) && (isgeneric == true))
            {
                p.Player.Points += 5;
                p.PointsCheckList.Add(ItemorRoomName);
            }
            else
            {

                if (c == null)
                {
                    // If the item or room has not been scored then add the points 

                    rm = p.Rooms.Find(t => t.Name.ToLower() == ItemorRoomName.ToLower());
                    if (rm != null)
                    {
                        p.Player.Points += rm.RoomPoints;
                        p.PointsCheckList.Add(rm.Name);

                    }
                    else
                    {
                        item = p.Items.Find(t => t.Name.ToLower() == ItemorRoomName.ToLower());
                        if (item != null)
                        {
                            p.Player.Points += item.ActionPoints;
                            p.PointsCheckList.Add(item.Name);

                        }
                    }

                }
            }

            return p.Player;
        }

        private static bool Helper_IsPlayerDead(PlayAdventure p)
        {
            if (p.Player.HealthCurrent < 1) return true;
            return false;
        }
        private static int Helper_SetPlayerNewHealth(PlayAdventure p)
        {
            return p.Player.HealthCurrent - p.HealthStep;
        }


        private static Tuple<bool, GameMoveResult, PlayAdventure, CommandState> Helper_DidPlayerMove(PlayAdventure p, GameMoveResult gmr, CommandState cs)
        {
            cs.Message = "";
            if (p.Player.PlayerDead == false)
            {
                var movecommands = new List<string> { "go", "nor", "sou", "eas", "wes", "up", "down", "n", "s", "e", "w", "u", "d" };
                if (movecommands.Contains(cs.Command))
                {

                    List<string> ml;

                    if (cs.Command == "go")
                    {
                        ml = new List<string> { "north", "south", "east", "west", "up", "down" };
                        if (ml.Contains(cs.Modifier))
                        {
                            cs.Valid = true;
                        }
                        else
                        {
                            cs.Valid = false;
                        }
                    }

                    // if the command is a short move convert to word
                    // shortcut for moves
                    ml = new List<string> { "nor", "sou", "eas", "wes", "up", "down", "n", "s", "e", "w", "u", "d" };
                    if (ml.Contains(cs.Command))
                    {
                        cs = Parse_ConvertShortMove(cs.Command);
                    }

                    if (cs.Valid == true)
                    {
                        (p, cs) = Action_MovePlayer(p, cs);

                        // update the gmr with the new room details

                        gmr.RoomName = Object_GetRoom(p.Rooms, p.Player.Room).Name;
                        gmr.RoomMessage = Object_GetRoom(p.Rooms, p.Player.Room).Desc;
                        gmr.PlayerName = p.Player.Name;
                        gmr.ItemsMessage = GetRoomItemsList(p.Player.Room, p.Items, true);

                        return new Tuple<bool, GameMoveResult, PlayAdventure, CommandState>(true, gmr, p, cs);
                    }
                    else if (cs.Valid == false)
                    {
                        cs.Message = "Wrong Way!";
                    }

                }

            }
            else { cs.Message = GetFunMessage(p.Messages, "DeadMove".Trim(), cs.Modifier); }

            return new Tuple<bool, GameMoveResult, PlayAdventure, CommandState>(false, gmr, p, cs);

        }

        private static Boolean Helper_IsMoveDirectionOK(Room room, string direction)
        {
            switch (direction.ToLower())
            {
                case "north":
                    if (room.N < 99) return true;
                    break;
                case "south":
                    if (room.S < 99) return true;
                    break;
                case "east":
                    if (room.E < 99) return true;
                    break;
                case "west":
                    if (room.W < 99) return true;
                    break;
                case "up":
                    if (room.U < 99) return true;
                    break;
                case "down":
                    if (room.D < 99) return true;
                    break;
            }
            return false;
        }

        #endregion Player Points

        #region Game Object Management 

        private static Room Object_GetRoom(List<Room> rooms, int roomnumber)
        {
            return (rooms.FirstOrDefault(t => t.Number == roomnumber));
        }

        private static List<Item> Object_MoveItem(List<Item> invitems, string Name, int NewRoom)
        {
            var itemIndex = invitems.FindIndex(t => t.Name.ToLower().Equals(Name.ToLower()));
            var invitem = invitems[itemIndex];
            invitem.Location = NewRoom;
            invitems[itemIndex] = invitem;
            return invitems;
        }

        #endregion Game Object Management

        #region Game Command Parse

        private static CommandState Parse_ConvertShortMove(string direction)
        {
            var cs = new CommandState // setup with assumed move details
            {
                Valid = true,
                RawCommand = direction,
                Command = "go",
                Modifier = "",
                Message = ""
            };

            switch (direction.ToLower())
            {
                case "n":
                case "nor":
                    cs.Modifier = "north";
                    break;
                case "s":
                case "sou":
                    cs.Modifier = "south";
                    break;
                case "e":
                case "eas":
                    cs.Modifier = "east";
                    break;
                case "w":
                case "wes":
                    cs.Modifier = "west";
                    break;
                case "u":
                case "up":
                    cs.Modifier = "up";
                    break;
                case "d":
                case "dow":
                    cs.Modifier = "down";
                    break;
                default:
                    cs.Message = "Wrong Way!";
                    cs.Valid = false;
                    break;
            }

            return cs;
        }

        private static CommandState Parse_Command(GameMove gm)
        {
            // Parse the command
            // take into account messy typing with extra spaces front, back and between
            // use the firt two words   wave  wand  and wave wand should parse the same 
            var c = gm.Move.Trim().ToLower();

            if (c == null) c = "";

            var cs = new CommandState();
            {
                cs.Valid = true;
                cs.Modifier = "";
                cs.Message = "";
            }

            if (c != "")
            {
                var cList = c.Split(" ").ToList<string>();
                var cmds = new List<string>();

                //take the first 2 that are not empty 
                int cnt = 0;
                foreach (string s in cList)
                {
                    if (s.Trim() != "") { cmds.Add(s.ToLower()); cnt++; }
                    if (cnt == 2) { break; }
                }

                if (cmds.Count > 1)
                {
                    cs.RawCommand = gm.Move;
                    cs.Command = cmds[0];
                    cs.Modifier = cmds[1];
                }
                else
                {
                    cs.RawCommand = gm.Move;
                    cs.Command = cmds[0];
                }
            }
            else
            {
                cs.Valid = false;
                cs.Modifier = "";
                cs.Message = "";
                cs.RawCommand = "";
                cs.Command = "";
            }

            return cs;
        }

        private static string Parse_FindCommandSynonym(string cmd)
        {
            // TODO: sometime make this list driven

            if (cmd == null) return "";
            if (cmd == "") return cmd;

            return (cmd.Trim()) switch
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
                "examine" => "look",
                "inventory" => "inv",
                "pack" => "inv",
                "scare" => "shoo",
                "kick" => "shoo",
                "kiss" => "pet",
                "hug" => "pet",
                "restart" => "quit",
                "score" => "points",
                "result" => "points",
                "study" => "read",
                "learn" => "read",
                _ => cmd,
            };

        }

        #endregion Game Command Parse

        #region Primary Game Command Processing


        private GameMoveResult Main_ProcessGameMove(GameMove move)
        {
            var cs = new CommandState();

            // Get the Instance we need to process 
            var p = GameInstance_GetObject(move.InstanceID);

            //setup the intial response message and result
            cs.Message = "";

            var gmr = new GameMoveResult
            {
                InstanceID = p.InstanceID,
                RoomName = Object_GetRoom(p.Rooms, p.Player.Room).Name,
                RoomMessage = Object_GetRoom(p.Rooms, p.Player.Room).Desc,
                PlayerName = p.Player.Name,
                ItemsMessage = GetRoomItemsList(p.Player.Room, p.Items, true),
                HealthReport = GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax)
            };

            //parse the command 
            cs = Parse_Command(move);
            cs.Command = Parse_FindCommandSynonym(cs.Command);

            bool playermoved;
            (playermoved, gmr, p, cs) = Helper_DidPlayerMove(p, gmr, cs);

            if (playermoved)
            {
                return Main_PostActionUpdate(gmr, p, cs.Message);
            }
            else
            {
                (p, cs) = Action_Processing(p, cs);
            }

            return Main_PostActionUpdate(gmr, p, cs.Message);
        }

        private GameMoveResult Main_PostActionUpdate(GameMoveResult gmr, PlayAdventure p, string actionMessage)
        {
            // Compute Player Health 
            p.Player.HealthCurrent = Helper_SetPlayerNewHealth(p);
            gmr.HealthReport = GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax);


            // This provides the output to the user after we process the action and resulting activity
            string healthActionMessage = "";

            if (gmr.HealthReport.ToLower() == "dead")
            {
                healthActionMessage = GetFunMessage(p.Messages, "dead", "") + "\r\n";
                p.Player.PlayerDead = true;
            }

            if (gmr.HealthReport.ToLower() == "bad")
            {
                healthActionMessage = GetFunMessage(p.Messages, "bad", "") + "\r\n";
            }

            gmr.InstanceID = p.InstanceID;
            gmr.PlayerName = p.Player.Name;

            // set room name and add to points if not already added
            gmr.RoomName = Object_GetRoom(p.Rooms, p.Player.Room).Name;
            p.Player = Helper_SetPlayerPoints(false, gmr.RoomName, p);

            // setup output message
            gmr.RoomMessage = Object_GetRoom(p.Rooms, p.Player.Room).Desc + " ";
            gmr.RoomMessage += GetRoomPath(Object_GetRoom(p.Rooms, p.Player.Room)) + "\r\n" + actionMessage + "\r\n";

            if (p.Player.Room == 0) gmr.RoomMessage += p.GameThanks;

            gmr.RoomMessage += GetHasPetMessage(p.Items, p.Messages, "\r\n");
            gmr.ItemsMessage = GetRoomItemsList(p.Player.Room, p.Items, true);

            if (healthActionMessage != "")
            {
                gmr.RoomMessage += "\r\n" + healthActionMessage + "\r\n";
            }

            GameInstance_Update(p);
            return gmr;
        }


        #endregion Primary Game Command Processing

        #region Game Validation Methods


        #endregion Game Validation Methods

        #region String Generation Methods

        private static string GetRoomInventory(int room, List<Item> Items)
        {
            string _itemstring = "";
            int count = 0;

            foreach (Item i in Items)
            {

                if (i.Location == room)
                {
                    if (count > 0) _itemstring += ", ";
                    _itemstring += i.Name;
                    count++;
                }

            }

            if (count == 0) _itemstring = "No Items";

            return _itemstring;

        }

        private static string GetRoomItemsList(int RoomNumber, List<Item> Items, bool verbose)
        {
            string _result = GetRoomInventory(RoomNumber, Items);

            if (verbose) { return _result; }
            else
            {
                if (_result == "No Items") { return ""; }
                else { return _result; }
            }
        }

        private static string GetRoomPath(Room rm)
        {
            // gets the available list of path out of a room and returns a string with the path. 

            string _result = "";

            int pc = 0;  // position count
            int cs = 0;  // comman seperator 
            int ac = 99; // if the and counter is not 99 then we won't add a comma

            // First figure out how many directions we can follow. Add +1 to each direction that is not 99
            // This is use to know when we want to add 'and' or just a comma

            if (rm.N != 99) { pc++; };
            if (rm.S != 99) { pc++; }
            if (rm.E != 99) { pc++; }
            if (rm.W != 99) { pc++; }
            if (rm.U != 99) { pc++; }
            if (rm.D != 99) { pc++; };

            // A room could have no exit. You can "teleport" in a room with no exits so return and empty string
            if (pc == 0) { return ""; }

            // Figure out which position in the sentance we want to add the "and". This is the total positions minus one 
            if (pc > 1) { ac = pc - 1; }

            // If we can move North then add "north" to the string
            if (rm.N != 99) { cs++; _result += "north"; }

            // if "and" coutner is equal to the comma seperator couter then add the comma which is 1 less than the number of item in the list
            if (ac == cs) { ac = 99; _result += ", and "; }

            // otherwise if we have more than 1 item remaining in our list add a comma 
            if ((ac != 99) && (pc > 2) && (cs > 0)) { _result += ", "; }

            if (rm.S != 99) { cs++; _result += "south"; }
            if (ac == cs) { ac = 99; _result += ", and "; }
            if ((ac != 99) && (pc > 2) && (cs > 0)) { _result += ", "; }

            if (rm.E != 99) { cs++; _result += "east"; }
            if (ac == cs) { ac = 99; _result += ", and "; }
            if ((ac != 99) && (pc > 2) && (cs > 0)) { _result += ", "; }

            if (rm.W != 99) { cs++; _result += "west"; }
            if (ac == cs) { ac = 99; _result += ", and "; }
            if ((ac != 99) && (pc > 2) && (cs > 0)) { _result += ", "; }

            if (rm.U != 99) { cs++; _result += "up"; }
            if (ac == cs) { ac = 99; _result += ", and "; }
            if ((ac != 99) && (pc > 2) && (cs > 0)) { _result += ", "; }

            if (rm.D != 99) { _result += "down"; }

            return "You can go " + _result + " from here.\r\n";

        }

        private static string GetHealthReport(int current, int max)
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

        private static Item GetItemDetails(string name, List<Item> Items)
        {
            var _result = Items.FirstOrDefault(t => t.Name.ToLower().Equals(name.ToLower()));
            return _result;
        }

        private static string GetHasPetMessage(List<Item> Items, List<Message> messages, string eol)
        { // checks for item in pet slot and returns message for room description

            var _result = Items.FirstOrDefault(t => t.Location == 9998);
            if (_result is null) return "";
            return GetFunMessage(messages, "petfollow", _result.Name) + eol;
        }

        private static string GetFunMessage(List<Message> messages, string action, string commandorobject)
        {
            string _message = "";

            if (action == null) { action = "any"; } else { action = action.ToLower(); }

            List<Message> _querymesssages;

            Random r = new(); // picks a random integer between the lower bound(inclusive) and the upper bound(exclusive).

            _querymesssages = _querymesssages = messages.FindAll(t => t.MessageTag.ToLower() == action.ToLower()).ToList();

            if (_querymesssages.Count == 0)
            {
                _querymesssages = _querymesssages = messages.FindAll(t => t.MessageTag.ToLower() == "any").ToList();
            }

            if (_querymesssages.Count > 0)
            {
                var msgid = r.Next(0, _querymesssages.Count);
                _message = _querymesssages[msgid].Messsage;
                if (_message.Contains("@")) { return _message.Replace("@", commandorobject); }
                else if (_message != "") { return _message; }
            }

            return "You can't do that here.";
        }

        private static string GetPlayerPoints(PlayAdventure p)
        {

            return p.Player.Points.ToString();

        }

        private static string GetPlayerPointsString(PlayAdventure p)
        {
            string _result = "";
            int cnt = 0;
            foreach (string i in p.PointsCheckList)
            {
                cnt++;
                _result += i.ToUpper();
                if (cnt < p.PointsCheckList.Count) _result += ", ";

            }

            return _result;
        }

        private static string GetPlayerPointsMessage(PlayAdventure p)
        {
            string _result = "";
            _result += "\r\nTotal Points : " + GetPlayerPoints(p);
            _result += "\r\nMilestones   : " + GetPlayerPointsString(p) + "\r\n";

            return _result;
        }

        #endregion String Generation Mthods

        #region Game Actions to Activities

        private static Tuple<PlayAdventure, CommandState> Action_MovePlayer(PlayAdventure p, CommandState cs)
        {
            // The command will have been parsed and we will expect the direction to be in command modifier
            var room = Object_GetRoom(p.Rooms, p.Player.Room);

            var direction = cs.Modifier;

            if (Helper_IsMoveDirectionOK(room, direction))
            {
                // move player
                switch (direction)
                {
                    case "north":
                        p.Player.Room = room.N;
                        break;
                    case "south":
                        p.Player.Room = room.S;
                        break;
                    case "east":
                        p.Player.Room = room.E;
                        break;
                    case "west":
                        p.Player.Room = room.W;
                        break;
                    case "up":
                        p.Player.Room = room.U;
                        break;
                    case "down":
                        p.Player.Room = room.D;
                        break;

                } // end moving the player 


            }
            else
            {
                // set command state to no valid
                cs.Valid = false;
                // set message about the wrong direction
                cs.Message = GetFunMessage(p.Messages, cs.Modifier, cs.Modifier) + "\r\n";
            }

            return new Tuple<PlayAdventure, CommandState>(p, cs);
        }

        private Tuple<PlayAdventure, CommandState> Action_Processing(PlayAdventure p, CommandState cs)
        {
            // The command will have been parsed and we will expect the item to be in command modifier

            //aquire and drops items and pet
            if (cs.Command == "get") { (p, cs) = Action_ItemManagemet(p, cs); }
            if (cs.Command == "drop") { (p, cs) = Action_ItemManagemet(p, cs); }
            if (cs.Command == "pet") { (p, cs) = Action_ItemManagemet(p, cs); }
            if (cs.Command == "shoo") { (p, cs) = Action_ItemManagemet(p, cs); }
            if (cs.Command == "inv") { (p, cs) = Action_ItemManagemet(p, cs); }
            if (cs.Command == "look") { (p, cs) = Action_ItemManagemet(p, cs); }

            // use item commands 
            if (cs.Command == "eat") { (p, cs) = Action_UseItem(p, cs); }
            if (cs.Command == "use") { (p, cs) = Action_UseItem(p, cs); }
            if (cs.Command == "read") { (p, cs) = Action_UseItem(p, cs); }
            if (cs.Command == "wave") { (p, cs) = Action_UseItem(p, cs); }
            if (cs.Command == "throw") 
                {
              
                    (p, cs) = Action_UseItem(p, cs); 
                }

            // control command 
            if (cs.Command == "health") { cs.Message = "You feel " + GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax) + "."; }
            if (cs.Command == "points") { cs.Message = GetPlayerPointsMessage(p); } 
            if (cs.Command == "quit") { if (!Helper_IsPlayerDead(p)) { p.Player.HealthCurrent = 0; } else { cs.Message = "You are dead - try the command \"newgame\"."; } }
            if (cs.Command == "help") { cs.Message = p.GameHelp; }
            if (cs.Command == "newgame") 
            {
                p = adventureHouse.SetupAdventure(p.InstanceID);
                cs.Message = "Game has been reset. Enjoy!";

            }

            return new Tuple<PlayAdventure, CommandState>(p, cs);
        }

        private static Tuple<PlayAdventure, CommandState> Action_ItemManagemet(PlayAdventure p, CommandState cs)
        {
            var requesteditem = cs.Modifier.ToLower();

            var dead = Helper_IsPlayerDead(p);


            if ((cs.Command == "get") & (!dead))
            {
                // var room = GetRoom(p.Rooms, p.Player.Room);
                var item = GetItemDetails(requesteditem, p.Items);

                if (item is not null)
                {
                    if (item.Location == p.Player.Room)
                    {
                        if (item.ActionVerb.ToLower() == "pet")
                        {
                            cs.Valid = false;
                            cs.Message = GetFunMessage(p.Messages, "petfailed", cs.Modifier) + "\r\n";
                        }
                        else
                        {
                            p.Items = Object_MoveItem(p.Items, requesteditem, 9999); // 9999 is backpack
                            cs.Message = GetFunMessage(p.Messages, "getsuccess", cs.Modifier);
                        }
                    }
                }
                else
                {
                    cs.Valid = false;
                    cs.Message = GetFunMessage(p.Messages, "getfailed", cs.Modifier) + "\r\n";
                }
            }
            else { if (dead) { cs.Message = "Dead people don't need stuff";  } }

            if ((cs.Command == "drop") & (!dead))
            {
                var room = Object_GetRoom(p.Rooms, p.Player.Room);
                var item = GetItemDetails(requesteditem, p.Items);

                if (item is not null)
                {
                    if (item.Location == 9999)
                    {
                        p.Items = Object_MoveItem(p.Items, requesteditem, room.Number);
                        cs.Message = GetFunMessage(p.Messages, "dropsuccess", cs.Modifier) + "\r\n";
                    }

                }
                else cs.Valid = false;

                if (cs.Valid == false)
                {
                    cs.Valid = false;
                    // set message about the wrong direction
                    cs.Message = GetFunMessage(p.Messages, "dropfailed", cs.Modifier) + "\r\n";
                }
            }
            else { if ((cs.Command == "drop") & (dead)) { cs.Message = "Dead people don't need stuff"; } }

            if (cs.Command == "pet")
            {
               
                var item = GetItemDetails(requesteditem, p.Items);

                if (item is not null)
                {
                    // You get "pet" the pet over and over to get the messages as long as its in the room on pet slot
                    if ((item.Location == p.Player.Room) | (item.Location == 9998))
                    {
                        p.Items = Object_MoveItem(p.Items, requesteditem, 9998);
                        cs.Message = GetFunMessage(p.Messages, "petsuccess", cs.Modifier) + "\r\n";
                        p.Player = Helper_SetPlayerPoints(false, item.Name, p);

                    }

                }
                else cs.Valid = false;

                if (cs.Valid == false)
                {
                    cs.Message = GetFunMessage(p.Messages, "any", cs.Modifier) + "\r\n";
                }

            }

            if ((cs.Command == "shoo") & (!dead))
            {
                
                var item = GetItemDetails(requesteditem, p.Items);

                if (item is not null)
                {
                    if (item.Location == 9998)
                    {
                        p.Items = Object_MoveItem(p.Items, requesteditem, Convert.ToInt16(item.ActionValue));
                        cs.Message = GetFunMessage(p.Messages, "shoosuccess", cs.Modifier) + "\r\n";
                    }

                }
                else cs.Valid = false;

                if (cs.Valid == false)
                {
                    cs.Valid = false;
                    cs.Message = GetFunMessage(p.Messages, "any", cs.Modifier) + "\r\n";
                }
            }
            else { if ((cs.Command == "shoo") & (dead)) { cs.Message = "Dead people cannot scare pets! "; } }

            if (cs.Command == "inv")
            {
                var _result = GetRoomInventory(9999, p.Items);

                cs.Message = "Your pack contains :";
                if (_result == "") _result = " [Empty]";
                cs.Message += _result + "\r\n";
                cs.Valid = true;
            }

            if (cs.Command == "look")
            {
                if (requesteditem == "")
                {
                    cs.Message = GetFunMessage(p.Messages, "LookEmpty", cs.Command);
                    cs.Valid = false;
                }
                else
                {
                    var item = GetItemDetails(requesteditem, p.Items);

                    if (item != null)
                    {
                        if ((item.Location == 9999) || (item.Location == p.Player.Room))
                        {
                            cs.Message = "You look at the " + cs.Modifier.ToLower() +" and see: "+ item.Description;
                        }
                        else
                        {
                            cs.Valid = false;
                            cs.Message = GetFunMessage(p.Messages, "LookFailed", cs.Modifier);
                        }

                    }
                    else
                    {
                        cs.Valid = false;
                        cs.Message = GetFunMessage(p.Messages, "LookEmpty", "");
                    }
                }

            }

            return new Tuple<PlayAdventure, CommandState>(p, cs);
        }

        private Tuple<PlayAdventure, CommandState> Action_UseItem(PlayAdventure p, CommandState cs)
        {
            var command = cs.Command.ToLower();
            var requesteditem = cs.Modifier.ToLower();
            var item = GetItemDetails(requesteditem, p.Items);

            if ((item != null) & (!Helper_IsPlayerDead(p)) && item.Location == 9999)
            {
                if (item.ActionVerb.ToLower() == cs.Command.ToLower())
                {

                    if (item.ActionResult.ToLower() == "health")
                    {
                        var currenthealth = p.Player.HealthCurrent;
                        var newhealth = p.Player.HealthCurrent + Convert.ToInt32(item.ActionValue);
                        p.Player.HealthCurrent = newhealth;

                        if (currenthealth > newhealth)
                        {
                            cs.Message = "That made you feel bad. Don't do that too much.\r\n\r\n" + "Your health is " + GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax);
                        }

                        if (newhealth > p.Player.HealthMax)
                        {
                            cs.Message = "That made you feel very full.\r\n\r\n" + "Currently you feel " + GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax);
                        }
                        else if (currenthealth < newhealth)
                        {
                            cs.Message = "That made you feel better.\r\n\r\n" + "You currently feel " + GetHealthReport(p.Player.HealthCurrent, p.Player.HealthMax);
                        }

                        p.Player = Helper_SetPlayerPoints(false, cs.Modifier, p); // set points for using item

                    }

                    if (item.ActionResult.ToLower() == "unlock")
                    {
                        List<string> unlockDetails = item.ActionValue.Split("|").ToList<string>();

                        var unlockfromroom = Convert.ToInt32(unlockDetails[0].ToString()); // Room # that this item works
                        var unlockdirection = unlockDetails[1].ToString(); // Direction we are unlocking 
                        var unlocktoroom = Convert.ToInt32(unlockDetails[2].ToString());  // Room Destination after the unlock
                        var unlockedroomdesc = unlockDetails[3].ToString(); // Room Desc after an unlock
                        var lockedroomdesc = unlockDetails[4].ToString(); // room Desc after a lock


                        if (Convert.ToInt32(unlockfromroom) == p.Player.Room)
                        {
                         
                            if (p.Rooms[unlockfromroom].Desc == unlockedroomdesc) // allows to toggle locked and unlocked
                            {
                                p.Rooms[unlockfromroom].Desc = lockedroomdesc;
                                if (unlockdirection.ToLower() == "n") { p.Rooms[unlockfromroom].N = 99; }
                                if (unlockdirection.ToLower() == "s") { p.Rooms[unlockfromroom].S = 99; }
                                if (unlockdirection.ToLower() == "e") { p.Rooms[unlockfromroom].E = 99; }
                                if (unlockdirection.ToLower() == "w") { p.Rooms[unlockfromroom].W = 99; }
                                if (unlockdirection.ToLower() == "u") { p.Rooms[unlockfromroom].U = 99; }
                                if (unlockdirection.ToLower() == "d") { p.Rooms[unlockfromroom].D = 99; }
                            }
                            else
                            {
                                p.Rooms[unlockfromroom].Desc = unlockedroomdesc;
                                if (unlockdirection.ToLower() == "n") { p.Rooms[unlockfromroom].N = unlocktoroom; }
                                if (unlockdirection.ToLower() == "s") { p.Rooms[unlockfromroom].S = unlocktoroom; }
                                if (unlockdirection.ToLower() == "e") { p.Rooms[unlockfromroom].E = unlocktoroom; }
                                if (unlockdirection.ToLower() == "w") { p.Rooms[unlockfromroom].W = unlocktoroom; }
                                if (unlockdirection.ToLower() == "u") { p.Rooms[unlockfromroom].U = unlocktoroom; }
                                if (unlockdirection.ToLower() == "d") { p.Rooms[unlockfromroom].D = unlocktoroom; }
                            }

                            p.Player = Helper_SetPlayerPoints(false, cs.Modifier, p); // set points for unlock item

                            
                        }
                        
                        else { cs.Valid = false; }
                    }

                    if (item.ActionResult.ToLower() == "teleport")
                    {
                        p.Items = Object_MoveItem(p.Items, cs.Modifier, p.Player.Room);

                        p.Player.Room = Convert.ToInt32(item.ActionValue);
                        p.Player = Helper_SetPlayerPoints(false, cs.Modifier, p);
                        cs.Message = "A magicial set of fingers has dropped you in this room..";

                        p.Player = Helper_SetPlayerPoints(false, cs.Modifier, p); // set points for teleport item

                    }


                    if (item.ActionResult.ToLower() == "fortune")
                    {
                        p.Player = Helper_SetPlayerPoints(false, cs.Modifier, p);
                        cs.Message = $"You look at the {item.Action} and read: \"{_getfortune.ReturnTimeBasedFortune().phrase}\", The text mysteriously fades and disappears.\r\n";
                    }



                }
                else { cs.Valid = false; }
            }
            else { cs.Valid = false; }

            if (cs.Valid == false) { if (!Helper_IsPlayerDead(p)) { cs.Message = GetFunMessage(p.Messages, command + "Failed", cs.Modifier); } else { cs.Message = "You are dead. You can't use the " + cs.Modifier.ToLower() + "."; } }

            return new Tuple<PlayAdventure, CommandState>(p, cs);
        }

        #endregion Game Actions to Activities


    }
}

































































































































































































