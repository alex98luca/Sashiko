namespace Sashiko.SystemMonitor.Models
{
	public sealed record ThermalInfo(
		double CpuTempCelsius,
		double GpuTempCelsius,
		double SystemTempCelsius,
		bool IsThrottling
	);
}
