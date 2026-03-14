namespace Sashiko.SystemMonitor.Models
{
	public sealed record PowerInfo(
		bool IsPluggedIn,
		int BatteryPercent,
		bool IsBatterySaverEnabled
	);
}
