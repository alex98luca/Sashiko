using System.Runtime.InteropServices;

namespace Sashiko.Core.Environment
{
	public static class RuntimeInfo
	{
		public static bool IsDebug =>
#if DEBUG
			true;
#else
            false;
#endif

		public static bool IsRelease => !IsDebug;

		public static bool IsWindows =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		public static bool IsLinux =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

		public static bool IsMacOS =>
			RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	}
}
