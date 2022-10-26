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
                //Checks if log directory exists
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT);
#endif
                //If not, creates a new directory
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
