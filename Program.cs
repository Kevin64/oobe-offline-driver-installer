using ConstantsDLL;
using ConstantsDLL.Properties;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Newtonsoft.Json;
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
        private static string jsonFile;
        private static LogGenerator log;
        private static Definitions definitions;
        private static StreamReader fileC;

        public class ConfigurationOptions
        {
            public Definitions Definitions { get; set; }
        }

        public class Definitions
        {
            public string LogLocation { get; set; }
            public bool AddDrivers { get; set; }
            public bool InstallDrivers { get; set; }
            public bool CleanGarbage { get; set; }
            public bool ChangeResolution { get; set; }
            public int ResolutionWidth { get; set; }
            public int ResolutionHeight { get; set; }
            public string DriverPath { get; set; }
            public bool RebootAfterFinished { get; set; }
            public bool PauseAfterFinished { get; set; }
        }

        private static void Main(string[] args)
        {
            //Checks of OS is W10 or W11
            string osVer = HardwareInfo.GetWinVersion();
            if (osVer == ConstantsDLL.Properties.GenericResources.WIN_7_NAMENUM || osVer == ConstantsDLL.Properties.GenericResources.WIN_8_NAMENUM || osVer == ConstantsDLL.Properties.GenericResources.WIN_8_1_NAMENUM)
            {
                MessageBox.Show(OodiStrings.UNSUPPORTED_OS, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }

            //Checks if application is running and kills any additional instance
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show(ConstantsDLL.Properties.UIStrings.ALREADY_RUNNING, ConstantsDLL.Properties.UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            try
            {
                fileC = new StreamReader(GenericResources.CONFIG_FILE);
                jsonFile = fileC.ReadToEnd();
                ConfigurationOptions jsonParse = JsonConvert.DeserializeObject<ConfigurationOptions>(@jsonFile);
                fileC.Close();

                //Creates 'Definitions' JSON section object
                definitions = new Definitions()
                {
                    LogLocation = jsonParse.Definitions.LogLocation,
                    AddDrivers = jsonParse.Definitions.AddDrivers,
                    InstallDrivers = jsonParse.Definitions.InstallDrivers,
                    CleanGarbage = jsonParse.Definitions.CleanGarbage,
                    ChangeResolution = jsonParse.Definitions.ChangeResolution,
                    ResolutionWidth = jsonParse.Definitions.ResolutionWidth,
                    ResolutionHeight = jsonParse.Definitions.ResolutionHeight,
                    DriverPath = jsonParse.Definitions.DriverPath,
                    RebootAfterFinished = jsonParse.Definitions.RebootAfterFinished,
                    PauseAfterFinished = jsonParse.Definitions.PauseAfterFinished
                };

                if (definitions.DriverPath == null || definitions.LogLocation == null)
                {
                    throw new FormatException();
                }

                bool fileExists = bool.Parse(MiscMethods.CheckIfLogExists(definitions.LogLocation));
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + GenericResources.DEV_STATUS_BETA, definitions.LogLocation, ConstantsDLL.Properties.GenericResources.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + GenericResources.DEV_STATUS_BETA + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_DEBUG_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, definitions.LogLocation, ConstantsDLL.Properties.GenericResources.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_RELEASE_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
#endif
                //Checks if log file exists
                if (!fileExists)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.UIStrings.LOGFILE_NOTEXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.UIStrings.LOGFILE_EXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.LogStrings.LOG_PARAMETER_FILE_FOUND, Directory.GetCurrentDirectory() + "\\" + ConstantsDLL.Properties.GenericResources.CONFIG_FILE, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.ADD_DRIVERS, definitions.AddDrivers.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.INSTALL_DRIVERS, definitions.InstallDrivers.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.CHANGE_RESOLUTION, definitions.ChangeResolution.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.RESOLUTION_WIDTH, definitions.ResolutionWidth.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.RESOLUTION_HEIGHT, definitions.ResolutionHeight.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.DRIVER_PATH, definitions.DriverPath, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.REBOOT_AFTER_FINISH, definitions.RebootAfterFinished.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.PAUSE_AFTER_FINISH, definitions.PauseAfterFinished.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.LOG_LOCATION, definitions.LogLocation, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));

                if (definitions.AddDrivers) //Adds/Installs drivers
                {
                    if (definitions.InstallDrivers) //Adds and installs drivers
                    {
                        PnpUtilCaller.Installer(definitions.DriverPath, true, log);
                    }
                    else //Just adds drivers
                    {
                        PnpUtilCaller.Installer(definitions.DriverPath, false, log);
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.NOT_INSTALLING_DRIVERS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }

                if (definitions.CleanGarbage) //Cleans drivers that are not used
                {
                    GarbageCleaner.CleanDirectories(definitions.DriverPath, log);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.NOT_ERASING_GARBAGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }

                if (definitions.ChangeResolution) //Changes screen resolution
                {
                    PrmaryScreenResolution.ChangeResolution(definitions.ResolutionWidth, definitions.ResolutionHeight, log);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.NOT_CHANGING_RESOLUTION, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }

                if (definitions.RebootAfterFinished) //Reboots the computer
                {
                    Process.Start(ConstantsDLL.Properties.GenericResources.SHUTDOWN_CMD_1, ConstantsDLL.Properties.GenericResources.SHUTDOWN_CMD_2);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.NOT_REBOOTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                }

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.GenericResources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                if (definitions.PauseAfterFinished) //Pauses the program at the end
                {
                    Console.WriteLine();
                    Console.WriteLine(ConstantsDLL.Properties.UIStrings.KEY_FINISH);
                    Console.ReadLine();
                    Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS));
                }
            }
            catch (FileNotFoundException e) //If definition file was not found
            {
                Console.WriteLine(ConstantsDLL.Properties.LogStrings.LOG_PARAMETER_FILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.UIStrings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException || e is FormatException) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(ConstantsDLL.Properties.UIStrings.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.UIStrings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
        }
    }
}