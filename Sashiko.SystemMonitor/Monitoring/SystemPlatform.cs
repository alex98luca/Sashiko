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
			var version = RuntimeInformation.OSDescription.Trim();
			var architecture = RuntimeInformation.OSArchitecture;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return new SystemPlatform(SystemPlatformKind.Windows, version, architecture);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return new SystemPlatform(DetectLinuxKind(version), version, architecture);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return new SystemPlatform(SystemPlatformKind.MacOS, version, architecture);

			var description = version.ToLowerInvariant();

			if (description.Contains("android"))
				return new SystemPlatform(SystemPlatformKind.Android, version, architecture);

			if (description.Contains("ios"))
				return new SystemPlatform(SystemPlatformKind.Ios, version, architecture);

			return new SystemPlatform(SystemPlatformKind.Unknown, version, architecture);
		}

		private static SystemPlatformKind DetectLinuxKind(string version)
		{
			var description = version.ToLowerInvariant();

			if (description.Contains("android"))
				return SystemPlatformKind.Android;

			return SystemPlatformKind.Linux;
		}
	}
}
