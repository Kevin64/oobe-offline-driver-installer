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
            bool instDrv, clnGrb, resChg, rebootAfter, verboseConsole;
            var parser = new FileIniDataParser();
            IniData def = parser.ReadFile(StringsAndConstants.defFile);
            var installDrv = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_1];
            var cleanGarb = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_2];
            var resChange = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_3];
            var resW = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_4];
            var resH = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_5];
            var defPath = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_6];
            var reboot = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_7];
            var verbose = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_8];

            try
            {
                instDrv = bool.Parse(installDrv);
                clnGrb = bool.Parse(cleanGarb);
                resChg = bool.Parse(resChange);
                rebootAfter = bool.Parse(reboot);
                verboseConsole = bool.Parse(verbose);
                if (instDrv)
                    PnpUtilCaller.installer(defPath, verboseConsole);
                else if (verboseConsole)
                    Console.WriteLine(StringsAndConstants.NOT_INSTALLING_DRIVERS);
                if (clnGrb)
                    GarbageCleaner.cleanDirectories(defPath, verboseConsole);
                else if (verboseConsole)
                    Console.WriteLine(StringsAndConstants.NOT_ERASING_GARBAGE);
                if (resChg)
                    PrmaryScreenResolution.ChangeResolution(Convert.ToInt32(resW), Convert.ToInt32(resH), verboseConsole);
                else if (verboseConsole)
                    Console.WriteLine(StringsAndConstants.NOT_CHANGING_RESOLUTION);
                if (rebootAfter)
                    Process.Start(StringsAndConstants.SHUTDOWN_CMD_1, StringsAndConstants.SHUTDOWN_CMD_2);
                else if (verboseConsole)
                    Console.WriteLine(StringsAndConstants.NOT_REBOOTING);
                if (verboseConsole)
                {
                    Console.WriteLine();
                    Console.WriteLine(StringsAndConstants.KEY_FINISH);
                    Console.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine(StringsAndConstants.PARAMETER_ERROR);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
            }
        }
    }
}