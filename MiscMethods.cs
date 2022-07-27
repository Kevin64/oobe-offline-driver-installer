using System.Management;

namespace OfflineDriverInstallerOOBE
{
    internal static class MiscMethods
    {
		//Fetches the computer's manufacturer
		public static string GetBoardMaker()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_ComputerSystem");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					return queryObj.GetPropertyValue("Manufacturer").ToString();
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
		}

		//Fetches the computer's manufacturer (alternative method)
		public static string GetBoardMakerAlt()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_BaseBoard");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					return queryObj.GetPropertyValue("Manufacturer").ToString();
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
		}

		//Fetches the computer's model
		public static string GetModel()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_ComputerSystem");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					return queryObj.GetPropertyValue("Model").ToString();
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
		}

		//Fetches the computer's model (alternative method)
		public static string GetModelAlt()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_BaseBoard");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					return queryObj.GetPropertyValue("Product").ToString();
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
		}

		//Fetches the OS buidl number
		public static string getOSVersion()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_OperatingSystem");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					return queryObj.GetPropertyValue("Version").ToString();
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
		}

		//Fetches the OS architecture
		public static string getOSArch()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_OperatingSystem");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					return queryObj.GetPropertyValue("OSArchitecture").ToString();
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
		}
	}
}
