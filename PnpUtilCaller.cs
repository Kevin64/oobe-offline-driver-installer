using HardwareInfoDLL;
using LogGeneratorDLL;
using OOBEOfflineDriverInstaller.Properties;
using System;
using System.Diagnostics;
using System.IO;

namespace OOBEOfflineDriverInstaller
{
    internal static class PnpUtilCaller
    {
        public static void Installer(string path, bool install, LogGenerator log)
        {
            string inst = string.Empty;
            string model = HardwareInfo.GetModel(); //Checks for hardware model
            string type = HardwareInfo.GetFirmwareType(); //Checks for firmware type
            string osVersion = HardwareInfo.GetOSBuildAndRevision(); //Checks for OS version
            osVersion = osVersion.Substring(0, osVersion.LastIndexOf("."));
            string osArch = HardwareInfo.GetOSArchAlt(); //Checks for OS architecture

            if (model == ConstantsDLL.Properties.GenericResources.TO_BE_FILLED_BY_OEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt(); //Checks for hardware model (alt method)
            }
            if (install)
            {
                inst = " /install";
            }
            if (type == "0")
            {
                type = ConstantsDLL.Properties.GenericResources.FW_TYPE_BIOS;
            }
            else if (type == "1")
            {
                type = ConstantsDLL.Properties.GenericResources.FW_TYPE_UEFI;
            }
            if (osArch.Contains("64"))
            {
                osArch = "x64";
            }

            string pathExt = path + type + "\\" + osVersion + "\\" + osArch + "\\" + model + "\\";
            string args = "/add-driver " + "\"" + pathExt + "*" + "\"" + " /subdirs" + inst;

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.HW_MODEL, model, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.OS_VERSION, osVersion, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.OS_ARCH, osArch, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.FIRMWARE_TYPE, type, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            if (install)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.ADDING_INSTALLING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            }
            else
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.ADDING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            }

            try
            {
                //Check if driver directory exists
                if (!Directory.Exists(pathExt))
                {
                    throw new DirectoryNotFoundException(OodiStrings.DIRECTORY_DO_NOT_EXIST);
                }

                //Create pnputil process with args
                Process process = new Process();
                process.StartInfo.FileName = "pnputil.exe";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                _ = process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    string output = process.StandardOutput.ReadLine();
                    if (output != string.Empty)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), output, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
                    }
                }
                process.WaitForExit();
                process.Close();

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.INSTALL_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            }
            catch (DirectoryNotFoundException e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), pathExt, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            }
        }
    }
}
