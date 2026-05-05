using System.Runtime.InteropServices;
using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class MonitorFallbackTests
	{
		private static readonly RuntimeContext UnknownRuntime = new(
			OperatingSystemFamily.Unknown,
			"test",
			Architecture.X64
		);

		[Fact]
		public void CpuMonitor_ShouldReturnNeutralInfoForUnknownPlatform()
		{
			var cpu = CpuMonitor.GetInfo(UnknownRuntime);

			Assert.Equal("Unknown CPU", cpu.Model);
			Assert.True(cpu.PhysicalCores > 0);
			Assert.True(cpu.LogicalCores > 0);
			Assert.Equal(0, cpu.LoadPercent);
			Assert.Equal(0, cpu.BaseClockGHz);
			Assert.Equal(0, cpu.CurrentClockGHz);
		}

		[Fact]
		public void GpuMonitor_ShouldReturnNeutralInfoForUnknownPlatform()
		{
			var gpu = GpuMonitor.GetInfo(UnknownRuntime);

			Assert.Equal("Unknown", gpu.Vendor);
			Assert.Equal("Unknown", gpu.Model);
			Assert.Equal(0, gpu.VramGB);
			Assert.Equal(0, gpu.LoadPercent);
		}

		[Fact]
		public void MemoryMonitor_ShouldReturnNeutralInfoForUnknownPlatform()
		{
			var memory = MemoryMonitor.GetInfo(UnknownRuntime);

			Assert.Equal(0, memory.TotalGB);
			Assert.Equal(0, memory.AvailableGB);
			Assert.Equal(0, memory.UsedBySystemGB);
			Assert.Equal(0, memory.UsedByCurrentProcessGB);
		}

		[Fact]
		public void ThermalMonitor_ShouldReturnNeutralInfoForUnknownPlatform()
		{
			var thermal = ThermalMonitor.GetInfo(UnknownRuntime);

			Assert.Equal(0, thermal.CpuTempCelsius);
			Assert.Equal(0, thermal.GpuTempCelsius);
			Assert.Equal(0, thermal.SystemTempCelsius);
			Assert.False(thermal.IsThrottling);
		}

		[Theory]
		[InlineData(OperatingSystemFamily.Windows)]
		[InlineData(OperatingSystemFamily.MacOS)]
		public void ThermalMonitor_ShouldReturnNeutralInfoForPlatformsWithoutNativeThermals(
			OperatingSystemFamily family
		)
		{
			var thermal = ThermalMonitor.GetInfo(new RuntimeContext(family, "test", Architecture.X64));

			Assert.Equal(0, thermal.CpuTempCelsius);
			Assert.Equal(0, thermal.GpuTempCelsius);
			Assert.Equal(0, thermal.SystemTempCelsius);
			Assert.False(thermal.IsThrottling);
		}

		[Fact]
		public void PowerMonitor_ShouldReturnNeutralInfoForUnknownPlatform()
		{
			var power = PowerMonitor.GetInfo(UnknownRuntime);

			Assert.True(power.IsPluggedIn);
			Assert.Equal(100, power.BatteryPercent);
			Assert.False(power.IsBatterySaverEnabled);
		}
	}
}
