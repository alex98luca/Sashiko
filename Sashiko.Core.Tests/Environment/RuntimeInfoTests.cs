using System.Runtime.InteropServices;
using Sashiko.Core.Environment;

namespace Sashiko.Core.Tests.Environment
{
	public sealed class RuntimeInfoTests
	{
		[Fact]
		public void Current_ShouldReuseDetectedRuntimeContext()
		{
			var first = RuntimeInfo.Current;
			var second = RuntimeInfo.Current;

			Assert.Same(first, second);
		}

		[Fact]
		public void LegacyOsFlags_ShouldMirrorCurrentRuntimeContext()
		{
			Assert.Equal(RuntimeInfo.Current.IsWindows, RuntimeInfo.IsWindows);
			Assert.Equal(RuntimeInfo.Current.IsLinux, RuntimeInfo.IsLinux);
			Assert.Equal(RuntimeInfo.Current.IsMacOS, RuntimeInfo.IsMacOS);
			Assert.Equal(RuntimeInfo.Current.IsMobile, RuntimeInfo.IsMobile);
		}

		[Fact]
		public void ReleaseFlag_ShouldMirrorDebugFlag()
		{
			Assert.Equal(!RuntimeInfo.IsDebug, RuntimeInfo.IsRelease);
		}

		[Theory]
		[InlineData(OperatingSystemFamily.Windows, "Windows", true, false, false, false)]
		[InlineData(OperatingSystemFamily.Linux, "Linux", false, true, false, false)]
		[InlineData(OperatingSystemFamily.MacOS, "macOS", false, false, true, false)]
		[InlineData(OperatingSystemFamily.Android, "Android", false, false, false, true)]
		[InlineData(OperatingSystemFamily.Ios, "iOS", false, false, false, true)]
		[InlineData(OperatingSystemFamily.Unknown, "Unknown", false, false, false, false)]
		public void RuntimeContext_ShouldExposeFriendlyFamilyName(
			OperatingSystemFamily family,
			string expectedName,
			bool expectedWindows,
			bool expectedLinux,
			bool expectedMacOS,
			bool expectedMobile
		)
		{
			var environment = new RuntimeContext(family, "test", Architecture.X64);

			Assert.Equal(expectedName, environment.FamilyName);
			Assert.Equal(expectedWindows, environment.IsWindows);
			Assert.Equal(expectedLinux, environment.IsLinux);
			Assert.Equal(expectedMacOS, environment.IsMacOS);
			Assert.Equal(expectedMobile, environment.IsMobile);
		}

		[Theory]
		[InlineData(Architecture.X64, "x64")]
		[InlineData(Architecture.X86, "x86")]
		[InlineData(Architecture.Arm, "ARM")]
		[InlineData(Architecture.Arm64, "ARM64")]
		public void RuntimeContext_ShouldExposeFriendlyArchitectureName(
			Architecture architecture,
			string expectedName
		)
		{
			var environment = new RuntimeContext(OperatingSystemFamily.Linux, "test", architecture);

			Assert.Equal(expectedName, environment.ArchitectureName);
		}

		[Fact]
		public void RuntimeContext_ShouldExposeUnknownArchitectureName()
		{
			var environment = new RuntimeContext(
				OperatingSystemFamily.Unknown,
				"test",
				(Architecture)999
			);

			Assert.Equal("Unknown", environment.ArchitectureName);
		}

		[Theory]
		[InlineData(true, false, false, "generic", OperatingSystemFamily.Windows)]
		[InlineData(false, true, false, "generic", OperatingSystemFamily.Linux)]
		[InlineData(false, true, false, "android linux", OperatingSystemFamily.Android)]
		[InlineData(false, false, true, "generic", OperatingSystemFamily.MacOS)]
		[InlineData(false, false, false, "android runtime", OperatingSystemFamily.Android)]
		[InlineData(false, false, false, "ios runtime", OperatingSystemFamily.Ios)]
		[InlineData(false, false, false, "generic", OperatingSystemFamily.Unknown)]
		public void Detect_ShouldMapRuntimeInformation(
			bool isWindows,
			bool isLinux,
			bool isMacOS,
			string description,
			OperatingSystemFamily expectedFamily
		)
		{
			var environment = RuntimeInfo.Detect(
				description,
				Architecture.X64,
				isWindows,
				isLinux,
				isMacOS
			);

			Assert.Equal(expectedFamily, environment.Family);
			Assert.Equal(description, environment.Version);
			Assert.Equal("x64", environment.ArchitectureName);
		}

		[Theory]
		[InlineData("android linux", OperatingSystemFamily.Android)]
		[InlineData("ubuntu linux", OperatingSystemFamily.Linux)]
		public void DetectLinuxFamily_ShouldMapLinuxRuntime(
			string description,
			OperatingSystemFamily expectedFamily
		)
		{
			Assert.Equal(expectedFamily, RuntimeInfo.DetectLinuxFamily(description));
		}
	}
}
