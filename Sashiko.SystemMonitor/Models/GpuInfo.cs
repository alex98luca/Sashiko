namespace Sashiko.SystemMonitor.Models
{
	public sealed record GpuInfo(
		string Vendor,        // NVIDIA, AMD, Intel, Apple
		string Model,
		double VramGB,
		double LoadPercent
	);
}
