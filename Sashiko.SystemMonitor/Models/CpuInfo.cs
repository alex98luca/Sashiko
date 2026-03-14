namespace Sashiko.SystemMonitor.Models
{
	public sealed record CpuInfo(
		string Model,
		int PhysicalCores,
		int LogicalCores,
		double LoadPercent,
		double BaseClockGHz,
		double CurrentClockGHz
	);
}
