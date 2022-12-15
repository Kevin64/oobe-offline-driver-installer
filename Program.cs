using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;
using ConstantsDLL;
using LogGeneratorDLL;
using System.Windows.Forms;
using OfflineDriverInstallerOOBE.Properties;
using System.IO;
using IniParser.Exceptions;
using System.Reflection;
using System.Linq;
using HardwareInfoDLL;

namespace OfflineDriverInstallerOOBE
{
    internal class Program
    {
        private static LogGenerator log;
        static void Main(string[] args)
        {
            //Checks of OS is W10 or W11
            string osVer = HardwareInfo.getOSInfoAux();
            if (osVer == StringsAndConstants.windows7 || osVer == StringsAndConstants.windows8 || osVer == StringsAndConstants.windows8_1)
            {
                MessageBox.Show(StringsAndConstants.UNSUPPORTED_OS, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }

            //Checks if application is running and kills any additional instance
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show(StringsAndConstants.ALREADY_RUNNING, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            IniData def = null;
            bool addDrvBool = false;
            bool installDrvBool = false;
            bool cleanGarbBool = false;
            bool resChangeBool = false;
            bool rebootBool = false;
            bool pauseBool = false;
            var parser = new FileIniDataParser();

            try
            {
                //Parses the INI file
                def = parser.ReadFile(StringsAndConstants.defFile);

                //Reads the INI file section
                var addDrvStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_10];
                var installDrvStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_1];
                var cleanGarbStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_2];
                var resChangeStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_3];
                var resWidthStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_4];
                var resHeightStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_5];
                var drvPathStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_6];
                var rebootStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_7];
                var pauseStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_8];
                var logLocationStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_9];

                if (drvPathStr == null || logLocationStr == null)
                    throw new FormatException();

                bool fileExists = bool.Parse(MiscMethods.checkIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.dev_status, logLocationStr, StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEBUG_MODE, string.Empty, StringsAndConstants.consoleOutCLI);
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, logLocationStr, StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RELEASE_MODE, string.Empty, StringsAndConstants.consoleOutCLI);
#endif
                //Checks if log file exists
                if (!fileExists)
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_NOTEXISTS, string.Empty, StringsAndConstants.consoleOutCLI);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_EXISTS, string.Empty, StringsAndConstants.consoleOutCLI);

                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEFFILE_FOUND, Directory.GetCurrentDirectory() + "\\" + StringsAndConstants.defFile, StringsAndConstants.consoleOutCLI);

                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_10, addDrvStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_1, installDrvStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_2, cleanGarbStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_3, resChangeStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_4, resWidthStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_5, resHeightStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_6, drvPathStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_7, rebootStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_8, pauseStr, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INI_SECTION_1_9, logLocationStr, StringsAndConstants.consoleOutCLI);

                try
                {
                    addDrvBool = bool.Parse(addDrvStr);
                    installDrvBool = bool.Parse(installDrvStr);
                    cleanGarbBool = bool.Parse(cleanGarbStr);
                    resChangeBool = bool.Parse(resChangeStr);
                    rebootBool = bool.Parse(rebootStr);
                    pauseBool = bool.Parse(pauseStr);
                }
                catch (Exception e) //If definition file was malformed, but the logfile is created (log path is defined)
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.PARAMETER_ERROR, e.Message, StringsAndConstants.consoleOutCLI);
                    Console.WriteLine(StringsAndConstants.KEY_FINISH);
                    Console.ReadLine();
                    log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutCLI);
                    Environment.Exit(StringsAndConstants.RETURN_ERROR);
                }

                if (addDrvBool) //Adds/Installs drivers
                {
                    if (installDrvBool) //Adds and installs drivers
                        PnpUtilCaller.installer(drvPathStr, true, log);
                    else //Just adds drivers
                        PnpUtilCaller.installer(drvPathStr, false, log);
                }
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.NOT_INSTALLING_DRIVERS, string.Empty, StringsAndConstants.consoleOutCLI);
                if (cleanGarbBool) //Cleans drivers that are not used
                    GarbageCleaner.cleanDirectories(drvPathStr, log);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.NOT_ERASING_GARBAGE, string.Empty, StringsAndConstants.consoleOutCLI);
                if (resChangeBool) //Changes screen resolution
                    PrmaryScreenResolution.ChangeResolution(Convert.ToInt32(resWidthStr), Convert.ToInt32(resHeightStr), log);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.NOT_CHANGING_RESOLUTION, string.Empty, StringsAndConstants.consoleOutCLI);
                if (rebootBool) //Reboots the computer
                    Process.Start(StringsAndConstants.SHUTDOWN_CMD_1, StringsAndConstants.SHUTDOWN_CMD_2);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.NOT_REBOOTING, string.Empty, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutCLI);
                if (pauseBool) //Pauses the program at the end
                {
                    Console.WriteLine();
                    Console.WriteLine(StringsAndConstants.KEY_FINISH);
                    Console.ReadLine();
                    Environment.Exit(StringsAndConstants.RETURN_SUCCESS);
                }
            }
            catch(ParsingException e) //If definition file was not found
            {
                Console.WriteLine(StringsAndConstants.LOG_DEFFILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
            catch (FormatException e) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(StringsAndConstants.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
        }
    }
}