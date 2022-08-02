using System;
using System.Diagnostics;

namespace OfflineDriverInstallerOOBE
{
    internal static class PnpUtilCaller
    {
		public static void installer()
        {
			string model = MiscMethods.GetModel();
			if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
				model = MiscMethods.GetModelAlt();
			string args = "/add-driver " + "\"" + StringsAndConstants.path + "\\" + MiscMethods.getOSVersion() + "\\" + MiscMethods.getOSArch() + "\\" + model + "\\" + "*" + "\"" + " /subdirs /install";

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
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
    }
}
