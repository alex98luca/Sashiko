namespace Sashiko.SystemMonitor.Models
{
	public sealed record SystemSnapshot(
		OsInfo Os,
		CpuInfo Cpu,
		GpuInfo Gpu,
		MemoryInfo Memory,
		DiskInfo Disk,
		ThermalInfo Thermal,
		PowerInfo Power
	);
}
