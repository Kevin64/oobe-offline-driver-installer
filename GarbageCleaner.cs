using System.IO;
using ConstantsDLL;
using HardwareInfoDLL;

namespace OfflineDriverInstallerOOBE
{
    internal static class GarbageCleaner
    {
        public static void cleanDirectories(string path)
        {
            string model = HardwareInfo.GetModel();
            if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
                model = HardwareInfo.GetModelAlt();
            string p = path + HardwareInfo.GetBIOSType() + "\\" + HardwareInfo.getOSVersion() + "\\" + HardwareInfo.getOSArchAlt() + "\\";
            DirectoryInfo directory = new DirectoryInfo(p);
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if(dir.Name != model)
                dir.Delete(true);
            }
        }
    }
}
