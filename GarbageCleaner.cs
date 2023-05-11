using HardwareInfoDLL;
using LogGeneratorDLL;
using OfflineDriverInstallerOOBE.Properties;
using System;
using System.IO;

namespace OfflineDriverInstallerOOBE
{
    internal static class GarbageCleaner
    {
        public static void CleanDirectories(string path, LogGenerator log)
        {
            string model = HardwareInfo.GetModel(); //Checks for hardware model
            if (model == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt(); //Checks for hardware model (alt method)
            }

            string pathExt = path + HardwareInfo.GetFwType() + "\\" + HardwareInfo.GetOSVersion() + "\\" + HardwareInfo.GetOSArchAlt() + "\\";
            DirectoryInfo directory = new DirectoryInfo(pathExt);

            try
            {
                //Check if driver directory exists
                if (!Directory.Exists(pathExt))
                {
                    throw new DirectoryNotFoundException(Strings.DIRECTORY_DO_NOT_EXIST);
                }

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.ERASING_GARBAGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                //Deletes directories other than the model's one
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    if (dir.Name != model)
                    {
                        dir.Delete(true);
                    }
                }
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.ERASING_SUCCESSFUL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
            catch (DirectoryNotFoundException e)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), pathExt, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
        }
    }
}
