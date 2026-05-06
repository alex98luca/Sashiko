using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class SystemCommandPathsTests
	{
		[Fact]
		public void UnixCommandPaths_ShouldUseFixedExecutablePaths()
		{
			Assert.Equal("/usr/bin/lspci", SystemCommandPaths.LinuxLspci);
			Assert.Equal("/usr/bin/nvidia-smi", SystemCommandPaths.LinuxNvidiaSmi);
			Assert.Equal("/usr/bin/pmset", SystemCommandPaths.MacPmset);
			Assert.Equal("/bin/ps", SystemCommandPaths.MacPs);
			Assert.Equal("/usr/sbin/system_profiler", SystemCommandPaths.MacSystemProfiler);
			Assert.Equal("/usr/sbin/sysctl", SystemCommandPaths.MacSysctl);
			Assert.Equal("/usr/bin/vm_stat", SystemCommandPaths.MacVmStat);
		}

		[Fact]
		public void WindowsDxDiag_ShouldUseSystem32ExecutablePath()
		{
			var path = SystemCommandPaths.WindowsDxDiag;

			Assert.Equal("dxdiag.exe", Path.GetFileName(path));
			Assert.Equal("System32", Path.GetFileName(Path.GetDirectoryName(path)));
		}
	}
}
