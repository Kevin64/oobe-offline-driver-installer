namespace OfflineDriverInstallerOOBE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PnpUtilCaller.installer();
            GarbageCleaner.cleanDiretories();
            PrmaryScreenResolution.ChangeResolution(StringsAndConstants.width, StringsAndConstants.height);
        }
    }
}