using AdventureServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventureServer.Services
{
    public class AppVersionService : IAppVersionService
    {
        string IAppVersionService.Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    }
}
