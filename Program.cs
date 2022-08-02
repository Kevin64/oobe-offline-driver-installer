using System.Diagnostics;

namespace OfflineDriverInstallerOOBE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PnpUtilCaller.installer();
            GarbageCleaner.cleanDirectories();
            PrmaryScreenResolution.ChangeResolution(StringsAndConstants.width, StringsAndConstants.height);
            Process.Start("shutdown", "/r /f");
        }
    }
}