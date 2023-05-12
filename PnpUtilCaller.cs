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
            string type = HardwareInfo.GetFwType(); //Checks for firmware type
            string osVersion = HardwareInfo.GetOSVersion(); //Checks for OS version
            string osArch = HardwareInfo.GetOSArchAlt(); //Checks for OS architecture

            if (model == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt(); //Checks for hardware model (alt method)
            }
            if (install)
            {
                inst = " /install";
            }
            if (type == "0")
            {
                type = ConstantsDLL.Properties.Resources.fwTypeBIOS;
            }
            else if (type == "1")
            {
                type = ConstantsDLL.Properties.Resources.fwTypeUEFI;
            }
            if (osArch.Contains("64"))
            {
                osArch = "x64";
            }

            string pathExt = path + type + "\\" + osVersion + "\\" + osArch + "\\" + model + "\\";
            string args = "/add-driver " + "\"" + pathExt + "*" + "\"" + " /subdirs" + inst;

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.HW_MODEL, model, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.OS_VERSION, osVersion, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.OS_ARCH, osArch, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.FIRMWARE_TYPE, type, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            if (install)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.ADDING_INSTALLING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
            else
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.ADDING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }

            try
            {
                //Check if driver directory exists
                if (!Directory.Exists(pathExt))
                {
                    throw new DirectoryNotFoundException(Strings.DIRECTORY_DO_NOT_EXIST);
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
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), output, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    }
                }
                process.WaitForExit();
                process.Close();

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.INSTALL_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
            catch (DirectoryNotFoundException e)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), pathExt, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
        }
    }
}
