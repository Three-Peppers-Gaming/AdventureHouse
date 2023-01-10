using AdventureServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventureServer.Interfaces
{
    public interface IAppVersionService
    {
        public string Version { get; }
    }

}


