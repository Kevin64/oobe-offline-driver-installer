using System;
using System.Diagnostics;
using ConstantsDLL;
using HardwareInfoDLL;

namespace OfflineDriverInstallerOOBE
{
    internal static class PnpUtilCaller
    {
		public static void installer(string path)
        {
			string model = HardwareInfo.GetModel();
			if (model == StringsAndConstants.ToBeFilledByOEM || model == "")
				model = HardwareInfo.GetModelAlt();
			string args = "/add-driver " + "\"" + path + HardwareInfo.GetBIOSType() + "\\" + HardwareInfo.getOSVersion() + "\\" + HardwareInfo.getOSArchAlt() + "\\" + model + "\\" + "*" + "\"" + " /subdirs /install";

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
