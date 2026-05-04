using System.Reflection;
using Sashiko.SystemMonitor.Monitoring;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class MonitorParsingTests
	{
		[Theory]
		[InlineData("NVIDIA Corporation GP107", "NVIDIA")]
		[InlineData("Advanced Micro Devices AMD Radeon", "AMD")]
		[InlineData("ATI Radeon", "AMD")]
		[InlineData("Intel UHD Graphics", "Intel")]
		[InlineData("Virtual Display Adapter", "Unknown")]
		public void GpuMonitor_ShouldDetectLinuxVendor(string line, string expectedVendor)
		{
			Assert.Equal(expectedVendor, InvokePrivate<string>(
				typeof(GpuMonitor),
				"DetectLinuxVendor",
				line
			));
		}

		[Theory]
		[InlineData("VRAM: 2 GB", "VRAM", "2 GB")]
		[InlineData("Chipset Model: Apple M3", "Chipset Model", "Apple M3")]
		[InlineData("Vendor: Apple", "Vendor", "Apple")]
		[InlineData("No matching field", "VRAM", "Unknown")]
		public void GpuMonitor_ShouldExtractMacField(string text, string field, string expectedValue)
		{
			Assert.Equal(expectedValue, InvokePrivate<string>(
				typeof(GpuMonitor),
				"ExtractMacField",
				text,
				field
			));
		}

		[Theory]
		[InlineData("4 GB", 4)]
		[InlineData("512 MB", 0.5)]
		[InlineData("", 0)]
		[InlineData("shared", 0)]
		public void GpuMonitor_ShouldParseMacVram(string value, double expectedVram)
		{
			Assert.Equal(expectedVram, InvokePrivate<double>(
				typeof(GpuMonitor),
				"ParseMacVram",
				value
			));
		}

		[Theory]
		[InlineData("Now drawing from 'Battery Power'\\n -InternalBattery-0 75%; discharging;", 75)]
		[InlineData("Now drawing from 'AC Power'\\n -InternalBattery-0 100%; charged;", 100)]
		[InlineData("No battery information", 100)]
		public void PowerMonitor_ShouldExtractMacBatteryPercent(string output, int expectedPercent)
		{
			Assert.Equal(expectedPercent, InvokePrivate<int>(
				typeof(PowerMonitor),
				"ExtractMacBatteryPercent",
				output
			));
		}

		[Fact]
		public void MemoryMonitor_ShouldParseLinuxMeminfo()
		{
			string[] lines =
			[
				"MemTotal:       16384000 kB",
				"MemAvailable:    8192000 kB"
			];

			Assert.Equal(16384000, InvokePrivate<double>(
				typeof(MemoryMonitor),
				"ParseMeminfo",
				lines,
				"MemTotal"
			));
			Assert.Equal(0, InvokePrivate<double>(
				typeof(MemoryMonitor),
				"ParseMeminfo",
				lines,
				"SwapTotal"
			));
		}

		[Theory]
		[InlineData("Pages free:                               1000.", 1000)]
		[InlineData("Pages inactive:                            invalid.", 0)]
		public void MemoryMonitor_ShouldParseVmStat(string line, long expectedValue)
		{
			Assert.Equal(expectedValue, InvokePrivate<long>(
				typeof(MemoryMonitor),
				"ParseVmStat",
				line
			));
		}

		[Theory]
		[InlineData("x86_pkg_temp", true)]
		[InlineData("acpitz", true)]
		[InlineData("pch_skylake", true)]
		[InlineData("soc_thermal", true)]
		[InlineData("battery", false)]
		public void ThermalMonitor_ShouldDetectSystemThermalZones(string type, bool expectedResult)
		{
			Assert.Equal(expectedResult, InvokePrivate<bool>(
				typeof(ThermalMonitor),
				"IsSystemThermalZone",
				type
			));
		}

		private static T InvokePrivate<T>(Type type, string methodName, params object[] arguments)
		{
			var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

			Assert.NotNull(method);

			return (T)method.Invoke(null, arguments)!;
		}
	}
}
