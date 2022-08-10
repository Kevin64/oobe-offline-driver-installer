using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;
using ConstantsDLL;

namespace OfflineDriverInstallerOOBE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new FileIniDataParser();
            IniData def = parser.ReadFile(StringsAndConstants.defFile);
            var installDrv = def["Definitions"]["InstallDrivers"];
            var cleanGarb = def["Definitions"]["CleanGarbage"];
            var resChange = def["Definitions"]["ChangeResolution"];
            var resW = def["Definitions"]["ResolutionWidth"];
            var resH = def["Definitions"]["ResolutionHeight"];
            var defPath = def["Definitions"]["DriverPath"];            
            var reboot = def["Definitions"]["RebootAfterFinished"];
            bool instDrv = bool.Parse(installDrv);
            bool clnGrb = bool.Parse(cleanGarb);
            bool resChg = bool.Parse(resChange);
            bool rebootAfter = bool.Parse(reboot);
            if(instDrv)
                PnpUtilCaller.installer(defPath);
            if(clnGrb)
                GarbageCleaner.cleanDirectories(defPath);
            if(resChg)
                PrmaryScreenResolution.ChangeResolution(Convert.ToInt32(resW), Convert.ToInt32(resH));
            if(rebootAfter)
                Process.Start("shutdown", "/r /f /t 0");
        }
    }
}