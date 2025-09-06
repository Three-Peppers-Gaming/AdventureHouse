using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureHouse.Services.AdventureServer.Models;
using AdventureHouse.Services.AdventureClient.Models;
using PlayAdventureModel = AdventureHouse.Services.AdventureServer.Models.PlayAdventure;

namespace AdventureHouse.Services.Data.AdventureData
{
    /// <summary>
    /// Future Family Space Apartment escape adventure - all rooms, items, messages, and monsters
    /// A satirical take on 1960s-style space age family life in the year 2087
    /// </summary>
    public class FutureFamilyData
    {
        private readonly FutureFamilyConfiguration _config = new();

        public string GetAdventureHelpText()
        {
            return _config.GetAdventureHelpText();
        }

        public string GetAdventureThankYouText()
        {
            return _config.GetAdventureThankYouText();
        }

        public FutureFamilyConfiguration GetGameConfiguration()
        {
            return _config;
        }

        public PlayAdventureModel SetupAdventure(string instanceID)
        {
            var gamerTag = new GamerTags().RandomTag();

            var player = new Player
            {
                Name = "George Spacely",
                Room = _config.StartingRoom, // Master Bedroom - overslept!
                HealthCurrent = _config.MaxHealth,
                HealthMax = _config.MaxHealth,
                PlayerID = Guid.NewGuid().ToString(),
                Verbose = true,
                Points = _config.StartingPoints
            };

            var adventure = new PlayAdventureModel
            {
                GameID = 3, // Unique game ID
                GameName = _config.GameName,
                GameHelp = _config.GetAdventureHelpText(),
                GameThanks = _config.GetAdventureThankYouText(),
                InstanceID = instanceID,
                WelcomeMessage = _config.GetWelcomeMessage(gamerTag),
                StartRoom = _config.StartingRoom,
                MaxHealth = _config.MaxHealth,
                HealthStep = _config.HealthStep,
                Player = player,
                Items = Items(),
                Rooms = Rooms(),
                Messages = Messages(),
                Monsters = Monsters(),
                GameActive = true,
                PointsCheckList = new List<string>(_config.InitialPointsCheckList)
            };

            return adventure;
        }

        private static List<Room> Rooms()
        {
            return new List<Room>
            {
                // EXIT ROOM - Transport Tube to Galaxy Widgets Corp
                new() { Number = 0, Name = "Transport Tube", Desc = "Success! You've made it to the express transport tube to Galaxy Widgets Corp! The pneumatic capsule whooshes you through the skyways of Neo Angeles at 200 mph. Mr. Cogswell will have to wait another day to fire you. The space age commute is complete!", N = 99, S = 99, E = 99, W = 99, U = 99, D = 99, RoomPoints = 1000 },

                // UPPER LEVEL - Family and Entertainment Areas
                new() { Number = 1, Name = "Landing Pad", Desc = "The apartment's private landing pad extends into the sky above Neo Angeles. Your family's flying car 'The Cosmic Cruiser' sits here, but it's in the shop AGAIN. A transport tube entrance gleams invitingly to the north - that's your ticket to Galaxy Widgets Corp!", N = 0, S = 5, E = 99, W = 99, U = 99, D = 99, RoomPoints = 50 },
                
                new() { Number = 2, Name = "Family Room", Desc = "The main living area with a three-dimensional television (3D-TV) showing 'The Space Bachelors.' Atomic-age furniture fills the room in eye-popping colors. The robot maid Robotina's charging station sits in the corner, but she's nowhere to be seen. That's probably bad news.", N = 99, S = 99, E = 3, W = 99, U = 99, D = 7, RoomPoints = 25 },
                
                new() { Number = 3, Name = "Kitchen", Desc = "The ultra-modern space kitchen with an Atomic Food-O-Matic that creates meals from basic atoms. The breakfast nook has a view of the floating city below. Food pills are scattered about - apparently even robot maids can make a mess.", N = 99, S = 6, E = 4, W = 2, U = 99, D = 14, RoomPoints = 25 },
                
                new() { Number = 4, Name = "Dining Room", Desc = "The formal dining area with a hover-table that floats three feet off the ground. Place settings for the whole Spacely family are still out from last night's dinner. A beautiful view of the asteroid belt is visible through the dome window.", N = 99, S = 99, E = 99, W = 3, U = 99, D = 15, RoomPoints = 25 },
                
                new() { Number = 5, Name = "Upper Hallway", Desc = "The main corridor connecting the upper level family areas. Family photos float in anti-gravity frames, showing happier times when the robots worked properly. The air recycler hums loudly - maybe too loudly.", N = 1, S = 99, E = 6, W = 99, U = 17, D = 16, RoomPoints = 15 },
                
                new() { Number = 6, Name = "Bathroom", Desc = "The family wash room with sonic showers and automated hygiene systems. The mirror announces 'Good morning, George!' in a cheerful but slightly glitchy voice. Your space toothbrush hovers ominously in its charging dock.", N = 99, S = 99, E = 99, W = 5, U = 99, D = 12, RoomPoints = 20 },

                // MAIN LEVEL - Private Areas and Home Office
                new() { Number = 7, Name = "Master Bedroom", Desc = "Your space-age bedroom where you just woke up in a panic! The anti-gravity bed is still floating sideways - no wonder you overslept. Jane's side of the bed is empty (she's visiting her mother on Mars). Your work clothes should be in the closet to the west.", N = 99, S = 14, E = 9, W = 8, U = 2, D = 99, RoomPoints = 0 }, // Starting room, no points
                
                new() { Number = 8, Name = "Master Closet", Desc = "Your walk-in closet with an automated dressing system that's currently broken. Clothes float randomly in zero-gravity storage pods. Your Galaxy Widgets Corp uniform hangs in its special protection field, but you'll need to figure out how to get it.", N = 99, S = 99, E = 7, W = 99, U = 99, D = 99, RoomPoints = 50 },
                
                new() { Number = 9, Name = "Balcony", Desc = "The master bedroom's private balcony with a stunning view of Neo Angeles 2,000 feet below. Flying cars zip between the tower buildings like metallic insects. Your robot dog Nutso likes to sit here and contemplate the meaning of artificial life.", N = 99, S = 11, E = 99, W = 7, U = 99, D = 99, RoomPoints = 25 },
                
                new() { Number = 10, Name = "Air Car Garage", Desc = "The family's private air car garage where 'The Cosmic Cruiser' is normally parked. It's empty since the car is being repaired for the 47th time this year. A maintenance robot works on the landing platform, sparking and smoking ominously.", N = 11, S = 99, E = 99, W = 99, U = 99, D = 99, RoomPoints = 30 },
                
                new() { Number = 11, Name = "Storage Room", Desc = "A cluttered storage area filled with broken future gadgets and obsolete space appliances. Boxes labeled 'Atomic Lawn Mower' and 'Self-Walking Space Dog' gather cosmic dust. This is where good intentions go to die in the space age.", N = 9, S = 10, E = 12, W = 99, U = 99, D = 18, RoomPoints = 20 },
                
                new() { Number = 12, Name = "Laundry Room", Desc = "The automated laundry facility with a Sonic Wash-O-Matic and Atomic Dryer. Clean clothes orbit the room in neat formations while dirty ones huddle in the corner like guilty electrons. The machines hum with barely contained atomic energy.", N = 99, S = 13, E = 99, W = 11, U = 6, D = 99, RoomPoints = 20 },
                
                new() { Number = 13, Name = "Utility Room", Desc = "The apartment's mechanical heart where all the space-age systems converge. Pipes, wires, and atomic conduits snake everywhere like technological spaghetti. A control panel marked 'DANGER: ATOMIC SYSTEMS' flashes warning lights.", N = 12, S = 99, E = 16, W = 99, U = 99, D = 19, RoomPoints = 40 },
                
                new() { Number = 14, Name = "Home Office", Desc = "George's work-from-space office with a Video-Phone connection to Galaxy Widgets Corp. The desk computer shows 47 urgent messages from Mr. Cogswell, each one angrier than the last. Your briefcase sits open, but something important is missing from it.", N = 7, S = 99, E = 15, W = 99, U = 3, D = 99, RoomPoints = 75 },
                
                new() { Number = 15, Name = "Nursery", Desc = "Baby's space nursery with toy rockets and stuffed space creatures floating in the anti-gravity play area. A mobile of planets spins lazily overhead.", N = 99, S = 99, E = 16, W = 14, U = 4, D = 20, RoomPoints = 25 },
                
                new() { Number = 16, Name = "Elevator Shaft", Desc = "The main elevator connecting all apartment levels. The atomic-powered lift plays space-age muzak while it operates. Emergency exits lead to the basement level, and service access goes up to the roof systems.", N = 99, S = 99, E = 99, W = 15, U = 5, D = 13, RoomPoints = 10 },

                // SERVICE LEVEL - Roof Access
                new() { Number = 17, Name = "Roof Access", Desc = "The apartment building's roof level with access to communication arrays and atmospheric processors. The view from here shows the entire floating city of Neo Angeles stretching to the horizon. Solar collectors power the building's atomic systems.", N = 99, S = 99, E = 99, W = 99, U = 99, D = 5, RoomPoints = 100 },

                // BASEMENT LEVEL - Underground Systems
                new() { Number = 18, Name = "Basement", Desc = "The building's basement level where the main atomic generators hum with barely contained power. This is where the building's central computer brain processes the needs of 500 space-age families. Service tunnels lead deeper underground.", N = 19, S = 99, E = 20, W = 99, U = 11, D = 99, RoomPoints = 50 },
                
                new() { Number = 19, Name = "Mechanical Room", Desc = "The central mechanical systems for the entire apartment building. Massive atomic engines provide power, artificial gravity, and atmosphere control. Warning signs in 12 galactic languages advise extreme caution.", N = 99, S = 18, E = 99, W = 99, U = 13, D = 99, RoomPoints = 75 },
                
                new() { Number = 20, Name = "Emergency Shelter", Desc = "A secret emergency shelter built for atomic accidents or alien invasion. Supplies for 30 days are stored here, along with emergency communication equipment. A red button labeled 'PANIC' sits under a protective cover.", N = 99, S = 99, E = 99, W = 18, U = 15, D = 99, RoomPoints = 200 },

#if DEBUG
                new() { Number = 88, Name = "Debug Room", Desc = "Developer's space-age debugging facility. All the future technology that actually works is stored here. Connections to every room in the apartment for testing purposes.", N = 99, S = 99, E = 99, W = 99, U = 0, D = 7, RoomPoints = 0 },
#endif
            };
        }

        private static List<Item> Items()
        {
            return new List<Item>
            {
                // FOOD ITEMS (Space Age Nutrition)
                new() { Name = "SPACEPILL", Description = "A complete breakfast in pill form! Contains all essential nutrients, vitamins, and space-age flavor crystals. The label promises 'Atomic Goodness in Every Bite!'", Location = 3, Action = "The space pill tastes like... everything and nothing. But you feel great! The atomic nutrients surge through your space-age metabolism.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "50", ActionPoints = 15 },
                
                new() { Name = "COSMOBURGER", Description = "A delicious atomic-age burger created by the Food-O-Matic. Made from genuine synthetic beef and asteroid cheese, it's the taste of tomorrow!", Location = 3, Action = "The cosmic burger is surprisingly tasty! The atomic proteins restore your energy for the space-age workday ahead.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "75", ActionPoints = 20 },
                
                new() { Name = "ENERGYDRINK", Description = "Galaxy Cola - the refreshing space drink with real meteor water! 'Puts the fizz in your atomic buzz!' The label glows with radioactive carbonation.", Location = 4, Action = "The energy drink gives you a fantastic atomic boost! You feel ready to face any space-age challenge, even Mr. Cogswell's bad mood.", ActionVerb = "DRINK", ActionResult = "HEALTH", ActionValue = "100", ActionPoints = 25 },

                // KEY STORY ITEMS (Essential for Escape)
                new() { Name = "UNIFORM", Description = "Your official Galaxy Widgets Corp work uniform with built-in climate control and atomic radiation protection. The name tag reads 'George Spacely - Widget Inspector Level 3'.", Location = 8, Action = "You put on your Galaxy Widgets Corp uniform and immediately feel more professional. Now you look like a productive member of space society!", ActionVerb = "WEAR", ActionResult = "HEALTH", ActionValue = "25", ActionPoints = 100 },
                
                new() { Name = "BRIEFCASE", Description = "Your atomic-powered briefcase containing important Galaxy Widgets Corp documents. It has a special compartment for your transport pass, but the compartment appears to be empty. Uh oh.", Location = 14, Action = "You organize your briefcase for another exciting day of widget inspection. The atomic locks click satisfyingly into place.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "10", ActionPoints = 50 },
                
                new() { Name = "TRANSPORTPASS", Description = "Your Galaxy Widgets Corp transport pass that grants access to the executive transport tube. Without this, you'll have to take the slow public tubes with all the other space commuters. The horror!", Location = 20, Action = "The transport pass unlocks the executive tube to Galaxy Widgets Corp! You can almost hear Mr. Cogswell's disappointed sigh that you made it to work on time.", ActionVerb = "USE", ActionResult = "UNLOCK", ActionValue = "1|N|0|The executive transport tube is now open! Board it to escape to Galaxy Widgets Corp!|Transport pass accepted! Welcome aboard the executive express!", ActionPoints = 500 },

                // PET COMPANION (Robot Dog)
                new() { Name = "NUTSO", Description = "Your faithful robot dog companion. Unlike most space-age technology, Nutso actually works properly (most of the time). He's programmed with advanced artificial emotions and genuine robo-loyalty. His favorite activities include chasing atomic fire hydrants and barking at flying cars.", Location = 9, Action = "Nutso's circuits warm up with electronic joy! His tail wags at exactly 47.3 oscillations per minute. He will now follow you with devoted robo-loyalty through any space-age adventure.", ActionVerb = "PET", ActionResult = "FOLLOW", ActionValue = "9", ActionPoints = 100 },

                // TOOL/WEAPON ITEMS (For Robot Problems)
                new() { Name = "WRENCH", Description = "An atomic-powered socket wrench designed for space-age maintenance. It glows with contained atomic energy and can fix (or break) almost any futuristic gadget.", Location = 13, Action = "You brandish the atomic wrench like a space-age hero! Its atomic resonance frequency is perfectly tuned to disrupt malfunctioning robot circuits.", ActionVerb = "USE", ActionResult = "WEAPON", ActionValue = "ROBOTINA", ActionPoints = 75 },
                
                new() { Name = "REMOTE", Description = "The master remote control for all apartment systems. It has 247 buttons, most of which you don't understand. One button is labeled 'EMERGENCY ROBOT SHUTDOWN' in friendly atomic-age letters.", Location = 2, Action = "The remote control activates with a satisfying electronic beep! All your space-age appliances await your commands.", ActionVerb = "USE", ActionResult = "WEAPON", ActionValue = "MAINTENANCEBOT", ActionPoints = 50 },

                // SPECIAL ITEMS (Teleport and Story)
                new() { Name = "JETPACK", Description = "A personal atomic jetpack for quick apartment navigation. The label warns 'Do Not Use Indoors' but also promises 'Atomic-Powered Convenience!' The atomic thrust vectors look properly calibrated.", Location = 17, Action = "The jetpack activates with a mighty atomic whoosh! You zoom through the apartment at fantastic space-age speeds, arriving instantly at the family room!", ActionVerb = "USE", ActionResult = "TELEPORT", ActionValue = "2", ActionPoints = 100 },
                
                new() { Name = "VIDEOPHONE", Description = "The apartment's main video communication device. Currently showing a recorded message from your wife Jane on Mars: 'Don't forget to feed Nutso and try not to get fired today, dear!'", Location = 14, Action = "You dial Galaxy Widgets Corp and get Mr. Cogswell's secretary. 'Mr. Spacely is en route,' you announce confidently. 'He'll be there promptly!' (You hope.)", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "15", ActionPoints = 25 },

                // DISCOVERY ITEMS (Story and Points)
                new() { Name = "DIARY", Description = "Jane's personal space diary, floating near the bed. The latest entry reads: 'George is such a dreamer. He thinks robots will make life easier, but they just make everything more complicated!'", Location = 7, Action = "You read Jane's diary (sorry, Jane!). Her observations about space-age life are surprisingly insightful. She also mentions hiding the emergency transport pass in the family shelter. Clever wife!", ActionVerb = "READ", ActionResult = "HEALTH", ActionValue = "5", ActionPoints = 150 },
                
                new() { Name = "NEWSPAPER", Description = "The Neo Angeles Space Times with today's headline: 'ROBOT UPRISING FEARS UNFOUNDED, SAYS ROBOT COUNCIL.' The weather report predicts 'Cosmic radiation with a chance of meteor showers.'", Location = 1, Action = "The space news is full of the usual future problems: traffic jams in the skyways, protests against the Robot Labor Union, and a sale on atomic pet food. Nothing about your tardiness problem, fortunately.", ActionVerb = "READ", ActionResult = "HEALTH", ActionValue = "5", ActionPoints = 25 },

                // UTILITY ITEMS
                new() { Name = "MEDICINE", Description = "Space-age vitamins and atomic health supplements. The bottle promises to 'Boost Your Atomic Vitality!' and 'Provide Essential Space Nutrients!' The pills glow with healthy atomic energy.", Location = 6, Action = "The space medicine works wonders! Your atomic metabolism processes the vitamins efficiently, boosting your health for the challenges ahead.", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "125", ActionPoints = 30 },
                
                new() { Name = "TOOLS", Description = "A complete set of space-age tools for maintaining atomic appliances. Includes an atomic screwdriver, cosmic pliers, and a quantum-level adjustment wrench. 'For all your space-age fix-it needs!'", Location = 11, Action = "You organize the space tools efficiently. Having the right equipment makes you feel more confident about handling any atomic-age emergency that might arise.", ActionVerb = "USE", ActionResult = "HEALTH", ActionValue = "20", ActionPoints = 40 },

#if DEBUG
                // DEBUG ITEMS
                new() { Name = "DEBUGBEAM", Description = "Developer's instant transport device for testing space-age adventures.", Location = 88, Action = "The debug beam teleports you instantly through space-age technology!", ActionVerb = "USE", ActionResult = "TELEPORT", ActionValue = "1", ActionPoints = 0 },
                
                new() { Name = "SUPERFOOD", Description = "Developer's atomic super nutrition that restores all health instantly.", Location = 88, Action = "The atomic super food completely restores your space-age vitality!", ActionVerb = "EAT", ActionResult = "HEALTH", ActionValue = "1000", ActionPoints = 0 },
                
                new() { Name = "MEGAWRENCH", Description = "Developer's ultimate robot-fixing tool that works on any malfunctioning space-age technology.", Location = 88, Action = "The mega wrench can fix any robot problem with atomic-powered efficiency!", ActionVerb = "USE", ActionResult = "WEAPON", ActionValue = "ROBOTINA", ActionPoints = 0 },
#endif
            };
        }

        private static List<Message> Messages()
        {
            return new List<Message>
            {
                // Generic action responses with space-age flair
                new() { MessageTag = "any", Messsage = "That's not possible in this atomic age, space cadet!" },
                new() { MessageTag = "any", Messsage = "Your space-age intuition says that won't work here, George!" },
                new() { MessageTag = "any", Messsage = "The laws of atomic physics prevent that action, cosmic citizen!" },
                
                // Get item success messages
                new() { MessageTag = "getsuccess", Messsage = "Atomic success! You acquire the @ for your space-age adventure!" },
                new() { MessageTag = "getsuccess", Messsage = "Cosmic achievement! The @ is now in your possession!" },
                new() { MessageTag = "getsuccess", Messsage = "Space-age efficiency! You smoothly collect the @!" },
                new() { MessageTag = "getsuccess", Messsage = "Fantastic! The @ fits perfectly in your atomic-powered inventory!" },
                
                // Get item failed messages
                new() { MessageTag = "getfailed", Messsage = "Atomic disappointment! The @ is not available here!" },
                new() { MessageTag = "getfailed", Messsage = "Space-age sensors detect no @ in this location!" },
                new() { MessageTag = "getfailed", Messsage = "Cosmic negative! That @ cannot be acquired right now!" },
                
                // Drop item success messages
                new() { MessageTag = "dropsuccess", Messsage = "You place the @ down with space-age precision!" },
                new() { MessageTag = "dropsuccess", Messsage = "The @ settles into place with atomic-powered grace!" },
                new() { MessageTag = "dropsuccess", Messsage = "Space-age organization! The @ is now properly positioned!" },
                
                // Drop item failed messages
                new() { MessageTag = "dropfailed", Messsage = "Atomic error! You don't possess a @ to drop!" },
                new() { MessageTag = "dropfailed", Messsage = "Space-age inventory check: No @ detected in your possession!" },
                
                // Pet success messages (Robot Dog)
                new() { MessageTag = "petsuccess", Messsage = "Atomic loyalty activated! Nutso's circuits buzz with electronic happiness!" },
                new() { MessageTag = "petsuccess", Messsage = "Space-age companionship! Nutso's tail wags at optimal frequency!" },
                new() { MessageTag = "petsuccess", Messsage = "Cosmic bonding! Nutso will follow you with robotic devotion!" },
                
                // Pet failed messages
                new() { MessageTag = "petfailed", Messsage = "That's not your space-age companion, George!" },
                new() { MessageTag = "petfailed", Messsage = "Atomic confusion! You can't pet that!" },
                
                // Pet following messages (Robot Dog)
                new() { MessageTag = "petfollow", Messsage = "Nutso follows with electronic enthusiasm, his optical sensors scanning for adventure!" },
                new() { MessageTag = "petfollow", Messsage = "Your faithful robot dog trots behind you, circuits humming with contentment!" },
                new() { MessageTag = "petfollow", Messsage = "Nutso maintains perfect formation, his atomic-powered loyalty subroutines active!" },
                new() { MessageTag = "petfollow", Messsage = "The robo-dog's tail wags precisely as he follows your space-age journey!" },
                new() { MessageTag = "petfollow", Messsage = "Nutso's optical sensors track your movement with devoted digital attention!" },
                
                // Shoo success messages
                new() { MessageTag = "shoosuccess", Messsage = "Nutso's circuits process your dismissal and he returns to his charging station!" },
                new() { MessageTag = "shoosuccess", Messsage = "The robot dog acknowledges your command and powers down to standby mode!" },
                
                // Use item failed messages
                new() { MessageTag = "UseFailed", Messsage = "Atomic impossibility! The @ won't function here!" },
                new() { MessageTag = "UseFailed", Messsage = "Space-age technology failure! That won't work in this location!" },
                
                // Look command messages
                new() { MessageTag = "LookEmpty", Messsage = "Your space-age sensors detect nothing of interest!" },
                new() { MessageTag = "LookEmpty", Messsage = "Cosmic scanning reveals no significant items here!" },
                
                new() { MessageTag = "LookFailed", Messsage = "Atomic sensors fail to locate that item, space citizen!" },
                new() { MessageTag = "LookFailed", Messsage = "No @ detected in this cosmic location!" },
                
                // Death and health messages
                new() { MessageTag = "dead", Messsage = "Atomic failure! Your space-age life support has ceased functioning!" },
                new() { MessageTag = "dead", Messsage = "Cosmic catastrophe! George Spacely's adventure has ended!" },
                new() { MessageTag = "dead", Messsage = "Space-age tragedy! Mr. Cogswell will definitely fire you now!" },
                
                new() { MessageTag = "bad", Messsage = "Atomic warning! Your space-age health is critically low!" },
                new() { MessageTag = "bad", Messsage = "Cosmic alert! You need space-age nutrition immediately!" },
                new() { MessageTag = "bad", Messsage = "Emergency! Your atomic vitality is dangerously depleted!" },
                
                // Movement blocked messages with space-age references
                new() { MessageTag = "DeadMove", Messsage = "Space-age physics prevent movement when life support has failed!" },
                
                new() { MessageTag = "north", Messsage = "Atomic barriers prevent northward movement!" },
                new() { MessageTag = "north", Messsage = "Space-age architecture blocks the northern path!" },
                new() { MessageTag = "north", Messsage = "Cosmic obstructions prevent access to the north!" },
                
                new() { MessageTag = "south", Messsage = "Southern passage blocked by space-age safety systems!" },
                new() { MessageTag = "south", Messsage = "Atomic security prevents southward movement!" },
                new() { MessageTag = "south", Messsage = "No cosmic pathway leads south from here!" },
                
                new() { MessageTag = "east", Messsage = "Eastern access denied by space-age protocols!" },
                new() { MessageTag = "east", Messsage = "Atomic barriers block the eastward route!" },
                new() { MessageTag = "east", Messsage = "Cosmic regulations prevent eastern movement!" },
                
                new() { MessageTag = "west", Messsage = "Western path secured by space-age safety systems!" },
                new() { MessageTag = "west", Messsage = "Atomic obstacles prevent westward travel!" },
                new() { MessageTag = "west", Messsage = "No cosmic corridor extends to the west!" },
                
                new() { MessageTag = "up", Messsage = "Upward access restricted by space-age engineering!" },
                new() { MessageTag = "up", Messsage = "Atomic elevators not available from this location!" },
                new() { MessageTag = "up", Messsage = "Cosmic architecture prevents vertical ascent!" },
                
                new() { MessageTag = "down", Messsage = "Downward passage sealed by space-age design!" },
                new() { MessageTag = "down", Messsage = "Atomic flooring prevents descent from this room!" },
                new() { MessageTag = "down", Messsage = "No cosmic pathway leads downward!" },
                
                // Monster encounter messages (Malfunctioning Robots)
                new() { MessageTag = "MonsterAppear", Messsage = "Atomic alert! A malfunctioning @ appears with sparking circuits!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Space-age danger! A glitchy @ blocks your path with erratic movements!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Cosmic malfunction! A broken @ emerges with hostile programming!" },
                new() { MessageTag = "MonsterAppear", Messsage = "Emergency! A rogue @ activates with threatening electronic sounds!" },
                
                new() { MessageTag = "MonsterAttack", Messsage = "The @ attacks with malfunctioning space-age fury!" },
                new() { MessageTag = "MonsterAttack", Messsage = "Atomic aggression! The @ strikes with robotic hostility!" },
                new() { MessageTag = "MonsterAttack", Messsage = "The glitchy @ lashes out with corrupted programming!" },
                
                new() { MessageTag = "MonsterHit", Messsage = "Ouch! The @'s atomic attack damages your space-age health!" },
                new() { MessageTag = "MonsterHit", Messsage = "Cosmic impact! The @ strikes you with robotic precision!" },
                new() { MessageTag = "MonsterHit", Messsage = "Atomic pain! The @'s malfunction causes you injury!" },
                
                new() { MessageTag = "MonsterMiss", Messsage = "Space-age reflexes! You dodge the @'s clumsy attack!" },
                new() { MessageTag = "MonsterMiss", Messsage = "Atomic evasion! The @ misses due to faulty targeting!" },
                new() { MessageTag = "MonsterMiss", Messsage = "Cosmic luck! The @'s circuits malfunction during its attack!" },
                
                new() { MessageTag = "MonsterDefeated", Messsage = "Atomic victory! The @ shuts down with electronic whimpers!" },
                new() { MessageTag = "MonsterDefeated", Messsage = "Space-age triumph! You've fixed the malfunctioning @!" },
                new() { MessageTag = "MonsterDefeated", Messsage = "Cosmic success! The @ returns to normal operation!" },
                
                // Combat success/failure messages
                new() { MessageTag = "AttackSuccess", Messsage = "Atomic effectiveness! Your @ successfully repairs the @!" },
                new() { MessageTag = "AttackSuccess", Messsage = "Space-age precision! The @ responds perfectly to your @!" },
                new() { MessageTag = "AttackSuccess", Messsage = "Cosmic mastery! Your @ fixes the broken @ immediately!" },
                
                new() { MessageTag = "AttackFailed", Messsage = "Atomic incompatibility! You need the right tool to fix the @!" },
                new() { MessageTag = "AttackFailed", Messsage = "Space-age error! Your equipment can't repair the @ properly!" },
                new() { MessageTag = "AttackFailed", Messsage = "Cosmic mismatch! The @ requires specialized tools!" },
                
                new() { MessageTag = "NoMonster", Messsage = "No malfunctioning robots detected here, space citizen!" },
                new() { MessageTag = "NoMonster", Messsage = "Atomic sensors show no hostile technology in this area!" },
                new() { MessageTag = "NoMonster", Messsage = "All space-age systems operating normally here!" },
                
                // Pet combat assistance messages (Robot Dog vs Robot)
                new() { MessageTag = "PetAttackSuccess", Messsage = "Atomic teamwork! Nutso assists with his advanced diagnostic circuits!" },
                new() { MessageTag = "PetAttackSuccess", Messsage = "Space-age cooperation! Your robot dog provides technical support!" },
                new() { MessageTag = "PetAttackSuccess", Messsage = "Cosmic partnership! Nutso's circuits interface with the malfunctioning unit!" },
                
                new() { MessageTag = "PetAttackFailed", Messsage = "Nutso's circuits are confused by the malfunction and he whimpers electronically!" },
                new() { MessageTag = "PetAttackFailed", Messsage = "The robot dog's programming conflicts with the hostile unit!" },
                new() { MessageTag = "PetAttackFailed", Messsage = "Nutso's peaceful circuits resist attacking another robot!" },
                
                new() { MessageTag = "PetDefeated", Messsage = "Atomic triumph! Nutso's loyal assistance proves invaluable!" },
                new() { MessageTag = "PetDefeated", Messsage = "Space-age victory! Your robot dog's help saves the day!" },
                new() { MessageTag = "PetDefeated", Messsage = "Cosmic success! Nutso deserves extra atomic dog treats!" }
            };
        }

        private static List<Monster> Monsters()
        {
            return new List<Monster>
            {
                // Robotina - The Malfunctioning Maid
                new() 
                { 
                    Key = "ROBOTINA", 
                    Name = "Robotina", 
                    Description = "Your family's robot maid has gone haywire! Her programming is stuck in 'AGGRESSIVE CLEANING MODE' and she's attacking everything with scrub brushes and vacuum attachments. Her optical sensors glow red with fury!", 
                    RoomNumber = 2, // Family Room - where she should be cleaning
                    ObjectNameThatCanAttackThem = "WRENCH", 
                    AttacksToKill = 3, 
                    CanHitPlayer = true, 
                    HitOdds = 40, 
                    HealthDamage = 20, 
                    AppearanceChance = 80, 
                    IsPresent = false, 
                    CurrentHealth = 3, 
                    PetAttackChance = 50 
                },

                // Maintenance Bot - In the Air Car Garage
                new() 
                { 
                    Key = "MAINTENANCEBOT", 
                    Name = "Maintenance Bot", 
                    Description = "The building's maintenance robot has suffered a circuit overload! Sparks fly from its atomic repair systems as it waves welding torches and plasma cutters dangerously. It broadcasts 'DANGER! EVACUATE!' in a loop.", 
                    RoomNumber = 10, // Air Car Garage
                    ObjectNameThatCanAttackThem = "REMOTE", 
                    AttacksToKill = 2, 
                    CanHitPlayer = true, 
                    HitOdds = 35, 
                    HealthDamage = 25, 
                    AppearanceChance = 70, 
                    IsPresent = false, 
                    CurrentHealth = 2, 
                    PetAttackChance = 40 
                },

                // Security Bot - In the Utility Room
                new() 
                { 
                    Key = "SECURITYBOT", 
                    Name = "Security Bot", 
                    Description = "The apartment's security robot has activated emergency protocols! Its alarm systems blare warnings while defensive force fields crackle around its chassis. Red lasers sweep the room!", 
                    RoomNumber = 13, // Utility Room
                    ObjectNameThatCanAttackThem = "WRENCH", 
                    AttacksToKill = 2, 
                    CanHitPlayer = true, 
                    HitOdds = 45, 
                    HealthDamage = 30, 
                    AppearanceChance = 75, 
                    IsPresent = false, 
                    CurrentHealth = 2, 
                    PetAttackChance = 30 
                }
            };
        }
    }
}