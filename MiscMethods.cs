using ConstantsDLL;
using OfflineDriverInstallerOOBE.Properties;
using System;
using System.IO;

using System.Windows.Forms;

namespace OfflineDriverInstallerOOBE
{
    internal class MiscMethods
    {
        public static string checkIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
#else
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT);
#endif
                if (!b)
                {
                    Directory.CreateDirectory(path);
                    return "false";
                }
                return "true";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
    }
}
