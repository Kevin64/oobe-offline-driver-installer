using System;
using System.Diagnostics;
using System.IO;
using ConstantsDLL;
using HardwareInfoDLL;
using LogGeneratorDLL;

namespace OfflineDriverInstallerOOBE
{
    internal static class PnpUtilCaller
    {
		public static void installer(string path, LogGenerator log)
        {
			string model = HardwareInfo.GetModel();
			if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
				model = HardwareInfo.GetModelAlt();
			string type = HardwareInfo.GetBIOSType();
			string osVersion = HardwareInfo.getOSVersion();
			string osArch = HardwareInfo.getOSArchAlt();
			string pathExt = path + type + "\\" + osVersion + "\\" + osArch + "\\" + model + "\\";
            string args = "/add-driver " + "\"" + pathExt + "*" + "\"" + " /subdirs /install";

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.HW_MODEL, model, StringsAndConstants.consoleOutCLI);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.OS_VERSION, osVersion, StringsAndConstants.consoleOutCLI);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.OS_ARCH, osArch, StringsAndConstants.consoleOutCLI);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.FIRMWARE_TYPE, type, StringsAndConstants.consoleOutCLI);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INSTALLING, string.Empty, StringsAndConstants.consoleOutCLI);

            try
			{
                if (!Directory.Exists(pathExt))
                    throw new DirectoryNotFoundException(StringsAndConstants.DIRECTORY_DO_NOT_EXIST);

                Process process = new Process();
				process.StartInfo.FileName = "pnputil.exe";
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.Arguments = args;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				process.Close();

				log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.INSTALL_FINISHED, string.Empty, StringsAndConstants.consoleOutCLI);
            }
            catch (DirectoryNotFoundException e)
			{
                log.LogWrite(StringsAndConstants.LOG_ERROR, pathExt, e.Message, StringsAndConstants.consoleOutCLI);
			}
		}
    }
}
