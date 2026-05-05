using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class OsMonitor
	{
		public static OsInfo GetInfo()
		{
			return GetInfo(RuntimeInfo.Current);
		}

		internal static OsInfo GetInfo(RuntimeContext runtime)
			=> new OsInfo(runtime.FamilyName, runtime.Version, runtime.ArchitectureName, runtime.IsMobile);
	}
}
