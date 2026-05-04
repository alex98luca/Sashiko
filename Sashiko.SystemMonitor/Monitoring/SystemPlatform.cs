using System.Runtime.InteropServices;

namespace Sashiko.SystemMonitor.Monitoring
{
	internal enum SystemPlatformKind
	{
		Windows,
		Linux,
		MacOS,
		Android,
		Ios,
		Unknown
	}

	internal sealed record SystemPlatform(
		SystemPlatformKind Kind,
		string Version,
		Architecture Architecture
	)
	{
		public static SystemPlatform Current { get; } = Detect();

		public bool IsWindows => Kind == SystemPlatformKind.Windows;
		public bool IsLinux => Kind == SystemPlatformKind.Linux;
		public bool IsMacOS => Kind == SystemPlatformKind.MacOS;
		public bool IsMobile => Kind is SystemPlatformKind.Android or SystemPlatformKind.Ios;

		public string Family => Kind switch
		{
			SystemPlatformKind.Windows => "Windows",
			SystemPlatformKind.Linux => "Linux",
			SystemPlatformKind.MacOS => "macOS",
			SystemPlatformKind.Android => "Android",
			SystemPlatformKind.Ios => "iOS",
			_ => "Unknown"
		};

		public string ArchitectureName => Architecture switch
		{
			Architecture.X64 => "x64",
			Architecture.X86 => "x86",
			Architecture.Arm => "ARM",
			Architecture.Arm64 => "ARM64",
			_ => "Unknown"
		};

		private static SystemPlatform Detect()
		{
			return Detect(
				RuntimeInformation.OSDescription.Trim(),
				RuntimeInformation.OSArchitecture,
				RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
				RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
				RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
			);
		}

		internal static SystemPlatform Detect(
			string version,
			Architecture architecture,
			bool isWindows,
			bool isLinux,
			bool isMacOS
		)
		{
			if (isWindows)
				return new SystemPlatform(SystemPlatformKind.Windows, version, architecture);

			if (isLinux)
				return new SystemPlatform(DetectLinuxKind(version), version, architecture);

			if (isMacOS)
				return new SystemPlatform(SystemPlatformKind.MacOS, version, architecture);

			var description = version.ToLowerInvariant();

			if (description.Contains("android"))
				return new SystemPlatform(SystemPlatformKind.Android, version, architecture);

			if (description.Contains("ios"))
				return new SystemPlatform(SystemPlatformKind.Ios, version, architecture);

			return new SystemPlatform(SystemPlatformKind.Unknown, version, architecture);
		}

		internal static SystemPlatformKind DetectLinuxKind(string version)
		{
			var description = version.ToLowerInvariant();

			if (description.Contains("android"))
				return SystemPlatformKind.Android;

			return SystemPlatformKind.Linux;
		}
	}
}
