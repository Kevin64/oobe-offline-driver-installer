using System.IO;

namespace OfflineDriverInstallerOOBE
{
    internal static class GarbageCleaner
    {
        public static void cleanDirectories()
        {
            string model = MiscMethods.GetModel();
            if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
                model = MiscMethods.GetModelAlt();
            string path = StringsAndConstants.path + "\\" + MiscMethods.getOSVersion() + "\\" + MiscMethods.getOSArch() + "\\";
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if(dir.Name != model)
                dir.Delete(true);
            }
        }
    }
}
