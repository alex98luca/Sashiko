using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class RuntimeContextIntegrationTests
	{
		[Fact]
		public void OsMonitor_ShouldUseCoreRuntimeContext()
		{
			var runtime = RuntimeInfo.Current;
			var os = OsMonitor.GetInfo(runtime);

			Assert.Equal(runtime.FamilyName, os.Family);
			Assert.Equal(runtime.Version, os.Version);
			Assert.Equal(runtime.ArchitectureName, os.Architecture);
			Assert.Equal(runtime.IsMobile, os.IsMobile);
		}

		[Fact]
		public void OsMonitor_ShouldUseCurrentRuntimeContext()
		{
			var runtime = RuntimeInfo.Current;
			var os = OsMonitor.GetInfo();

			Assert.Equal(runtime.FamilyName, os.Family);
			Assert.Equal(runtime.Version, os.Version);
			Assert.Equal(runtime.ArchitectureName, os.Architecture);
			Assert.Equal(runtime.IsMobile, os.IsMobile);
		}
	}
}
