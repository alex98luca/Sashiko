using Sashiko.SystemMonitor.Models;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Snapshot
{
	public static class SystemSnapshotProvider
	{
		public static SystemSnapshot Capture()
		{
			var platform = SystemPlatform.Current;

			return new SystemSnapshot(
				OsMonitor.GetInfo(platform),
				CpuMonitor.GetInfo(platform),
				GpuMonitor.GetInfo(platform),
				MemoryMonitor.GetInfo(platform),
				DiskMonitor.GetInfo(),
				ThermalMonitor.GetInfo(platform),
				PowerMonitor.GetInfo(platform)
			);
		}
	}
}
