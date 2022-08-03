using System.IO;

namespace OfflineDriverInstallerOOBE
{
    internal static class GarbageCleaner
    {
        public static void cleanDirectories(string path)
        {
            string model = MiscMethods.GetModel();
            if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
                model = MiscMethods.GetModelAlt();
            string p = path + MiscMethods.getOSVersion() + "\\" + MiscMethods.getOSArch() + "\\";
            DirectoryInfo directory = new DirectoryInfo(p);
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if(dir.Name != model)
                dir.Delete(true);
            }
        }
    }
}
