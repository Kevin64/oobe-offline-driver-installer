using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;

namespace OfflineDriverInstallerOOBE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new FileIniDataParser();
            IniData def = parser.ReadFile(StringsAndConstants.defFile);
            var resW = def["Definitions"]["ResolutionWidth"];
            var resH = def["Definitions"]["ResolutionHeight"];
            var defPath = def["Definitions"]["DriverPath"];
            var reboot = def["Definitions"]["RebootAfterFinished"];
            bool rebootAfter = bool.Parse(reboot);
            PnpUtilCaller.installer(defPath);
            GarbageCleaner.cleanDirectories(defPath);
            PrmaryScreenResolution.ChangeResolution(Convert.ToInt32(resW), Convert.ToInt32(resH));
            if(rebootAfter == true)
                Process.Start("shutdown", "/r /f /t 0");
        }
    }
}