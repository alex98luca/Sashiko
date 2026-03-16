using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class DiskMonitor
	{
		public static DiskInfo GetInfo()
		{
			try
			{
				// We assume the simulator runs from the primary drive
				var drive = DriveInfo.GetDrives()
					.FirstOrDefault(d => d.IsReady &&
						d.RootDirectory.FullName == Path.GetPathRoot(Environment.CurrentDirectory));

				if (drive == null)
					return new DiskInfo(0, 0, 0);

				double total = MemoryConverter.Convert(drive.TotalSize, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
				double free = MemoryConverter.Convert(drive.AvailableFreeSpace, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
				double used = total - free;

				return new DiskInfo(total, free, used);
			}
			catch
			{
				return new DiskInfo(0, 0, 0);
			}
		}
	}
}
