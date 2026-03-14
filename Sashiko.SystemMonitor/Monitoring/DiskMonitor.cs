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
					.FirstOrDefault(d => d.IsReady && d.RootDirectory.FullName == Path.GetPathRoot(Environment.CurrentDirectory));

				if (drive == null)
					return new DiskInfo(0, 0, 0);

				double total = drive.TotalSize / 1024.0 / 1024.0 / 1024.0;
				double free = drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
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