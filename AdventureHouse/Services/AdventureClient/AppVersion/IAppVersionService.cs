using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventureHouse.Services.AdventureClient.AppVersion
{
    public interface IAppVersionService
    {
        public string Version { get; }
    }
}