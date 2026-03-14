namespace Sashiko.SystemMonitor.Models
{
	public sealed record DiskInfo(
		double TotalGB,
		double FreeGB,
		double UsedGB
	);
}
