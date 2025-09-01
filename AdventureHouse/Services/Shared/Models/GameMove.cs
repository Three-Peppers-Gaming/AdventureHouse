using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureHouse.Services.Shared.Models
{
    public class GameMove
    {
        public string InstanceID { get; set; } = string.Empty;
        public string Move { get; set; } = string.Empty;
        
        /// <summary>
        /// Indicates if this is a console command (starts with / or is a known console command)
        /// </summary>
        public bool IsConsoleCommand { get; set; } = false;
        
        /// <summary>
        /// Client display mode preferences
        /// </summary>
        public bool UseClassicMode { get; set; } = false;
        public bool UseScrollMode { get; set; } = false;
    }
}