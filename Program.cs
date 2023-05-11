using HardwareInfoDLL;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using LogGeneratorDLL;
using OOBEOfflineDriverInstaller.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OOBEOfflineDriverInstaller
{
    internal class Program
    {
        private static LogGenerator log;

        private static void Main(string[] args)
        {
            //Checks of OS is W10 or W11
            string osVer = HardwareInfo.GetWinVersion();
            if (osVer == ConstantsDLL.Properties.Resources.windows7 || osVer == ConstantsDLL.Properties.Resources.windows8 || osVer == ConstantsDLL.Properties.Resources.windows8_1)
            {
                MessageBox.Show(Strings.UNSUPPORTED_OS, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }

            //Checks if application is running and kills any additional instance
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show(ConstantsDLL.Properties.Strings.ALREADY_RUNNING, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            IniData def = null;
            bool addDrvBool = false;
            bool installDrvBool = false;
            bool cleanGarbBool = false;
            bool resChangeBool = false;
            bool rebootBool = false;
            bool pauseBool = false;
            FileIniDataParser parser = new FileIniDataParser();

            try
            {
                //Parses the INI file
                def = parser.ReadFile(ConstantsDLL.Properties.Resources.defFile);

                //Reads the INI file section
                string addDrvStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_10];
                string installDrvStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_1];
                string cleanGarbStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_2];
                string resChangeStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_3];
                string resWidthStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_4];
                string resHeightStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_5];
                string drvPathStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_6];
                string rebootStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_7];
                string pauseStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_8];
                string logLocationStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_9];

                if (drvPathStr == null || logLocationStr == null)
                {
                    throw new FormatException();
                }

                bool fileExists = bool.Parse(MiscMethods.CheckIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.dev_status, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_DEBUG_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_RELEASE_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
#endif
                //Checks if log file exists
                if (!fileExists)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_NOTEXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_EXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_DEFFILE_FOUND, Directory.GetCurrentDirectory() + "\\" + ConstantsDLL.Properties.Resources.defFile, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_10, addDrvStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_1, installDrvStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_2, cleanGarbStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_3, resChangeStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_4, resWidthStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_5, resHeightStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_6, drvPathStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_7, rebootStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_8, pauseStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Resources.INI_SECTION_1_9, logLocationStr, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

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
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.PARAMETER_ERROR, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                    Console.ReadLine();
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                }

                if (addDrvBool) //Adds/Installs drivers
                {
                    if (installDrvBool) //Adds and installs drivers
                    {
                        PnpUtilCaller.Installer(drvPathStr, true, log);
                    }
                    else //Just adds drivers
                    {
                        PnpUtilCaller.Installer(drvPathStr, false, log);
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.NOT_INSTALLING_DRIVERS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                if (cleanGarbBool) //Cleans drivers that are not used
                {
                    GarbageCleaner.CleanDirectories(drvPathStr, log);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.NOT_ERASING_GARBAGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                if (resChangeBool) //Changes screen resolution
                {
                    PrmaryScreenResolution.ChangeResolution(Convert.ToInt32(resWidthStr), Convert.ToInt32(resHeightStr), log);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.NOT_CHANGING_RESOLUTION, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                if (rebootBool) //Reboots the computer
                {
                    Process.Start(ConstantsDLL.Properties.Resources.SHUTDOWN_CMD_1, ConstantsDLL.Properties.Resources.SHUTDOWN_CMD_2);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.NOT_REBOOTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                if (pauseBool) //Pauses the program at the end
                {
                    Console.WriteLine();
                    Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                    Console.ReadLine();
                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS));
                }
            }
            catch (ParsingException e) //If definition file was not found
            {
                Console.WriteLine(ConstantsDLL.Properties.Strings.LOG_DEFFILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
            catch (FormatException e) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(ConstantsDLL.Properties.Strings.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
        }
    }
}