using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantAgentPicker
{
    internal class Global
    {
        public static string appDataRoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "REclipsent");
        public static string roamingFolder = Path.Combine(appDataRoamingPath, "ValorantAgentPicker");
        public static string settingsFile = Path.Combine(roamingFolder, "settings.json");
    }
}
