using System.Runtime.InteropServices;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class OsMonitor
	{
		public static OsInfo GetInfo()
		{
			var family = DetectOsFamily();
			var version = DetectOsVersion();
			var arch = DetectArchitecture();
			var isMobile = DetectMobile(family);

			return new OsInfo(family, version, arch, isMobile);
		}

		private static string DetectOsFamily()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return "Windows";

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return "Linux";

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return "macOS";

			// Future expansion: Android, iOS
			var desc = RuntimeInformation.OSDescription.ToLowerInvariant();

			if (desc.Contains("android"))
				return "Android";

			if (desc.Contains("ios"))
				return "iOS";

			return "Unknown";
		}

		private static string DetectOsVersion()
		{
			// RuntimeInformation.OSDescription is the most reliable cross-platform source
			return RuntimeInformation.OSDescription.Trim();
		}

		private static string DetectArchitecture()
		{
			return RuntimeInformation.OSArchitecture switch
			{
				Architecture.X64 => "x64",
				Architecture.X86 => "x86",
				Architecture.Arm => "ARM",
				Architecture.Arm64 => "ARM64",
				_ => "Unknown"
			};
		}

		private static bool DetectMobile(string family)
		{
			return family is "Android" or "iOS";
		}
	}
}