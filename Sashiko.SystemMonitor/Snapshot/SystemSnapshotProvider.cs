using Sashiko.SystemMonitor.Models;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Snapshot
{
	public static class SystemSnapshotProvider
	{
		public static SystemSnapshot Capture()
		{
			return new SystemSnapshot(
				OsMonitor.GetInfo(),
				CpuMonitor.GetInfo(),
				GpuMonitor.GetInfo(),
				MemoryMonitor.GetInfo(),
				DiskMonitor.GetInfo(),
				ThermalMonitor.GetInfo(),
				PowerMonitor.GetInfo()
			);
		}
	}
}