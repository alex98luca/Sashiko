using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class OsMonitor
	{
		public static OsInfo GetInfo()
		{
			return GetInfo(SystemPlatform.Current);
		}

		internal static OsInfo GetInfo(SystemPlatform platform)
			=> new OsInfo(platform.Family, platform.Version, platform.ArchitectureName, platform.IsMobile);
	}
}
