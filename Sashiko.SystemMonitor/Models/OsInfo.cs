namespace Sashiko.SystemMonitor.Models
{
	public sealed record OsInfo(
		string Family,        // Windows, Linux, macOS, Android, iOS
		string Version,
		string Architecture,  // x64, ARM64
		bool IsMobile
	);
}
