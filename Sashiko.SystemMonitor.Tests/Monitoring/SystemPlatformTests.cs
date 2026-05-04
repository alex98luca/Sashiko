using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class SystemPlatformTests
	{
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
	}
}
