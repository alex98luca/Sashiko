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

		[Theory]
		[InlineData(OperatingSystemFamily.Windows, "Windows", false)]
		[InlineData(OperatingSystemFamily.Linux, "Linux", false)]
		[InlineData(OperatingSystemFamily.MacOS, "macOS", false)]
		[InlineData(OperatingSystemFamily.Android, "Android", true)]
		[InlineData(OperatingSystemFamily.Ios, "iOS", true)]
		[InlineData(OperatingSystemFamily.Unknown, "Unknown", false)]
		public void RuntimeContext_ShouldExposeFriendlyFamilyName(
			OperatingSystemFamily family,
			string expectedName,
			bool expectedMobile
		)
		{
			var environment = new RuntimeContext(family, "test", Architecture.X64);

			Assert.Equal(expectedName, environment.FamilyName);
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
	}
}
