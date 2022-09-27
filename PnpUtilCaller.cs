using System;
using System.Diagnostics;
using ConstantsDLL;
using HardwareInfoDLL;

namespace OfflineDriverInstallerOOBE
{
    internal static class PnpUtilCaller
    {
		public static void installer(string path, bool verbose)
        {
			string model = HardwareInfo.GetModel();
			if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
				model = HardwareInfo.GetModelAlt();
			string type = HardwareInfo.GetBIOSType();
			string osVersion = HardwareInfo.getOSVersion();
			string osArch = HardwareInfo.getOSArchAlt();
            string args = "/add-driver " + "\"" + path + type + "\\" + osVersion + "\\" + osArch + "\\" + model + "\\" + "*" + "\"" + " /subdirs /install";

			if(verbose)
			{
                Console.WriteLine(StringsAndConstants.HW_MODEL + model);
                Console.WriteLine(StringsAndConstants.OS_VERSION + osVersion);
                Console.WriteLine(StringsAndConstants.OS_ARCH + osArch);
                Console.WriteLine(StringsAndConstants.FIRMWARE_TYPE + type);
				Console.WriteLine();
                Console.WriteLine(StringsAndConstants.INSTALLING);
                Console.WriteLine();
            }

            try
			{
				Process process = new Process();
				process.StartInfo.FileName = "pnputil.exe";
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.Arguments = args;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				process.Close();
                
				if(verbose)
				{
                    Console.WriteLine(StringsAndConstants.INSTALL_FINISHED);
                    Console.WriteLine();
                }
            }
            catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
    }
}
