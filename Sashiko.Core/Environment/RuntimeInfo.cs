using System.Runtime.InteropServices;

namespace Sashiko.Core.Environment
{
	public static class RuntimeInfo
	{
		public static RuntimeContext Current { get; } = Detect();

		public static bool IsDebug =>
#if DEBUG
			true;
#else
            false;
#endif

		public static bool IsRelease => !IsDebug;

		public static bool IsWindows => Current.IsWindows;
		public static bool IsLinux => Current.IsLinux;
		public static bool IsMacOS => Current.IsMacOS;
		public static bool IsMobile => Current.IsMobile;

		private static RuntimeContext Detect()
		{
			var version = RuntimeInformation.OSDescription.Trim();
			var architecture = RuntimeInformation.OSArchitecture;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return new RuntimeContext(OperatingSystemFamily.Windows, version, architecture);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return new RuntimeContext(DetectLinuxFamily(version), version, architecture);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return new RuntimeContext(OperatingSystemFamily.MacOS, version, architecture);

			var description = version.ToLowerInvariant();

			if (description.Contains("android"))
				return new RuntimeContext(OperatingSystemFamily.Android, version, architecture);

			if (description.Contains("ios"))
				return new RuntimeContext(OperatingSystemFamily.Ios, version, architecture);

			return new RuntimeContext(OperatingSystemFamily.Unknown, version, architecture);
		}

		private static OperatingSystemFamily DetectLinuxFamily(string version)
		{
			return version.Contains("android", StringComparison.OrdinalIgnoreCase)
				? OperatingSystemFamily.Android
				: OperatingSystemFamily.Linux;
		}
	}
}
