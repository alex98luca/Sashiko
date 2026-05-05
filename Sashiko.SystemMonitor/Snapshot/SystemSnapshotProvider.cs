using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Models;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Snapshot
{
	public static class SystemSnapshotProvider
	{
		public static SystemSnapshot Capture()
		{
			var runtime = RuntimeInfo.Current;

			return new SystemSnapshot(
				OsMonitor.GetInfo(runtime),
				CpuMonitor.GetInfo(runtime),
				GpuMonitor.GetInfo(runtime),
				MemoryMonitor.GetInfo(runtime),
				DiskMonitor.GetInfo(),
				ThermalMonitor.GetInfo(runtime),
				PowerMonitor.GetInfo(runtime)
			);
		}
	}
}
