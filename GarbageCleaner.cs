using HardwareInfoDLL;
using LogGeneratorDLL;
using OOBEOfflineDriverInstaller.Properties;
using System;
using System.IO;

namespace OOBEOfflineDriverInstaller
{
    internal static class GarbageCleaner
    {
        public static void CleanDirectories(string path, LogGenerator log)
        {
            string model = HardwareInfo.GetModel(); //Checks for hardware model
            string type = HardwareInfo.GetFirmwareType(); //Checks for firmware type
            string osVersion = HardwareInfo.GetWinVersion(); //Checks for OS version
            string osArch = HardwareInfo.GetOSArchAlt(); //Checks for OS architecture
            if (model == ConstantsDLL.Properties.GenericResources.TO_BE_FILLED_BY_OEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt(); //Checks for hardware model (alt method)
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

            string pathExt = path + type + "\\" + osVersion + "\\" + osArch + "\\";
            DirectoryInfo directory = new DirectoryInfo(pathExt);

            try
            {
                //Check if driver directory exists
                if (!Directory.Exists(pathExt))
                {
                    throw new DirectoryNotFoundException(OodiStrings.DIRECTORY_DO_NOT_EXIST);
                }

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.ERASING_GARBAGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));

                //Deletes directories other than the model's one
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    if (dir.Name != model)
                    {
                        dir.Delete(true);
                    }
                }
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), OodiStrings.ERASING_SUCCESSFUL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            }
            catch (DirectoryNotFoundException e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), pathExt, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.GenericResources.CONSOLE_OUT_CLI));
            }
        }
    }
}
