namespace Sashiko.SystemMonitor.Models
{
	public sealed record MemoryInfo(
		double TotalGB,
		double AvailableGB,
		double UsedBySystemGB,
		double UsedByCurrentProcessGB
	);
}
