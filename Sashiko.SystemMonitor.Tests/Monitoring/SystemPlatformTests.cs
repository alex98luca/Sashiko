using System.Runtime.InteropServices;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class SystemPlatformTests
	{
		[Theory]
		[InlineData((int)SystemPlatformKind.Windows, "Windows", false)]
		[InlineData((int)SystemPlatformKind.Linux, "Linux", false)]
		[InlineData((int)SystemPlatformKind.MacOS, "macOS", false)]
		[InlineData((int)SystemPlatformKind.Android, "Android", true)]
		[InlineData((int)SystemPlatformKind.Ios, "iOS", true)]
		[InlineData((int)SystemPlatformKind.Unknown, "Unknown", false)]
		public void Platform_ShouldExposeFamilyAndMobileFlag(
			int kindValue,
			string expectedFamily,
			bool expectedMobile
		)
		{
			var kind = (SystemPlatformKind)kindValue;
			var platform = new SystemPlatform(kind, "test", Architecture.X64);

			Assert.Equal(expectedFamily, platform.Family);
			Assert.Equal(expectedMobile, platform.IsMobile);
		}

		[Theory]
		[InlineData(Architecture.X64, "x64")]
		[InlineData(Architecture.X86, "x86")]
		[InlineData(Architecture.Arm, "ARM")]
		[InlineData(Architecture.Arm64, "ARM64")]
		public void Platform_ShouldExposeArchitectureName(Architecture architecture, string expectedName)
		{
			var platform = new SystemPlatform(SystemPlatformKind.Linux, "test", architecture);

			Assert.Equal(expectedName, platform.ArchitectureName);
		}

		[Fact]
		public void Platform_ShouldExposeUnknownArchitectureName()
		{
			var platform = new SystemPlatform(SystemPlatformKind.Linux, "test", (Architecture)999);

			Assert.Equal("Unknown", platform.ArchitectureName);
		}

		[Fact]
		public void Current_ShouldReuseDetectedPlatform()
		{
			var first = SystemPlatform.Current;
			var second = SystemPlatform.Current;

			Assert.Same(first, second);
		}

		[Fact]
		public void OsMonitor_ShouldUseDetectedPlatform()
		{
			var platform = SystemPlatform.Current;
			var os = OsMonitor.GetInfo(platform);

			Assert.Equal(platform.Family, os.Family);
			Assert.Equal(platform.Version, os.Version);
			Assert.Equal(platform.ArchitectureName, os.Architecture);
			Assert.Equal(platform.IsMobile, os.IsMobile);
		}

		[Fact]
		public void OsMonitor_ShouldUseCurrentPlatform()
		{
			var os = OsMonitor.GetInfo();

			Assert.Equal(SystemPlatform.Current.Family, os.Family);
			Assert.Equal(SystemPlatform.Current.Version, os.Version);
			Assert.Equal(SystemPlatform.Current.ArchitectureName, os.Architecture);
			Assert.Equal(SystemPlatform.Current.IsMobile, os.IsMobile);
		}

		[Theory]
		[InlineData(true, false, false, "generic", (int)SystemPlatformKind.Windows)]
		[InlineData(false, true, false, "generic", (int)SystemPlatformKind.Linux)]
		[InlineData(false, true, false, "android linux", (int)SystemPlatformKind.Android)]
		[InlineData(false, false, true, "generic", (int)SystemPlatformKind.MacOS)]
		[InlineData(false, false, false, "android runtime", (int)SystemPlatformKind.Android)]
		[InlineData(false, false, false, "ios runtime", (int)SystemPlatformKind.Ios)]
		[InlineData(false, false, false, "generic", (int)SystemPlatformKind.Unknown)]
		public void Detect_ShouldMapRuntimeInformation(
			bool isWindows,
			bool isLinux,
			bool isMacOS,
			string description,
			int expectedKindValue
		)
		{
			var expectedKind = (SystemPlatformKind)expectedKindValue;
			var platform = SystemPlatform.Detect(
				description,
				Architecture.X64,
				isWindows,
				isLinux,
				isMacOS
			);

			Assert.Equal(expectedKind, platform.Kind);
			Assert.Equal(description, platform.Version);
			Assert.Equal("x64", platform.ArchitectureName);
		}

		[Theory]
		[InlineData("android linux", (int)SystemPlatformKind.Android)]
		[InlineData("ubuntu linux", (int)SystemPlatformKind.Linux)]
		public void DetectLinuxKind_ShouldMapLinuxRuntime(string description, int expectedKindValue)
		{
			var expectedKind = (SystemPlatformKind)expectedKindValue;

			Assert.Equal(expectedKind, SystemPlatform.DetectLinuxKind(description));
		}
	}
}
