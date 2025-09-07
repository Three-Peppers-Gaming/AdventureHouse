using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventureRealms.Services.AdventureClient.AppVersion
{
    public class AppVersionService : IAppVersionService
    {
        string IAppVersionService.Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}