using System.Runtime.InteropServices;

namespace Sashiko.Core.Environment
{
	public sealed record RuntimeContext(
		OperatingSystemFamily Family,
		string Version,
		Architecture Architecture
	)
	{
		public bool IsWindows => Family == OperatingSystemFamily.Windows;
		public bool IsLinux => Family == OperatingSystemFamily.Linux;
		public bool IsMacOS => Family == OperatingSystemFamily.MacOS;
		public bool IsMobile => Family is OperatingSystemFamily.Android or OperatingSystemFamily.Ios;

		public string FamilyName => Family switch
		{
			OperatingSystemFamily.Windows => "Windows",
			OperatingSystemFamily.Linux => "Linux",
			OperatingSystemFamily.MacOS => "macOS",
			OperatingSystemFamily.Android => "Android",
			OperatingSystemFamily.Ios => "iOS",
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
	}
}
