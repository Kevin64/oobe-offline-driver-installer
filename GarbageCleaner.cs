using System.IO;
using ConstantsDLL;
using HardwareInfoDLL;
using LogGeneratorDLL;

namespace OfflineDriverInstallerOOBE
{
    internal static class GarbageCleaner
    {
        public static void cleanDirectories(string path, LogGenerator log)
        {
            string model = HardwareInfo.GetModel();
            if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
                model = HardwareInfo.GetModelAlt();
            string pathExt = path + HardwareInfo.GetBIOSType() + "\\" + HardwareInfo.getOSVersion() + "\\" + HardwareInfo.getOSArchAlt() + "\\";
            DirectoryInfo directory = new DirectoryInfo(pathExt);

            try
            {
                if (!Directory.Exists(pathExt))
                    throw new DirectoryNotFoundException(StringsAndConstants.DIRECTORY_DO_NOT_EXIST);

                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.ERASING_GARBAGE, string.Empty, StringsAndConstants.consoleOutCLI);

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    if (dir.Name != model)
                        dir.Delete(true);
                }
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.ERASING_SUCCESSFUL, string.Empty, StringsAndConstants.consoleOutCLI);
            }
            catch(DirectoryNotFoundException e)
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, pathExt, e.Message, StringsAndConstants.consoleOutCLI);
            }
        }
    }
}
