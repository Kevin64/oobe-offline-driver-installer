using ConstantsDLL.Properties;
using System;
using System.IO;

using System.Windows.Forms;

namespace OOBEOfflineDriverInstaller
{
    internal class MiscMethods
    {
        public static string CheckIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + ConstantsDLL.Properties.GenericResources.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + GenericResources.DEV_STATUS_BETA + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + ConstantsDLL.Properties.GenericResources.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + ConstantsDLL.Properties.GenericResources.LOG_FILE_EXT);
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
