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
			return Detect(
				RuntimeInformation.OSDescription.Trim(),
				RuntimeInformation.OSArchitecture,
				RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
				RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
				RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
			);
		}

		internal static RuntimeContext Detect(
			string version,
			Architecture architecture,
			bool isWindows,
			bool isLinux,
			bool isMacOS
		)
		{
			if (isWindows)
				return new RuntimeContext(OperatingSystemFamily.Windows, version, architecture);

			if (isLinux)
				return new RuntimeContext(DetectLinuxFamily(version), version, architecture);

			if (isMacOS)
				return new RuntimeContext(OperatingSystemFamily.MacOS, version, architecture);

			var description = version.ToLowerInvariant();

			if (description.Contains("android"))
				return new RuntimeContext(OperatingSystemFamily.Android, version, architecture);

			if (description.Contains("ios"))
				return new RuntimeContext(OperatingSystemFamily.Ios, version, architecture);

			return new RuntimeContext(OperatingSystemFamily.Unknown, version, architecture);
		}

		internal static OperatingSystemFamily DetectLinuxFamily(string version)
		{
			return version.Contains("android", StringComparison.OrdinalIgnoreCase)
				? OperatingSystemFamily.Android
				: OperatingSystemFamily.Linux;
		}
	}
}
