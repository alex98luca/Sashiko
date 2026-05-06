using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;
using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Monitoring.Native;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class CpuMonitor
	{
		private const string UnknownCpuModel = "Unknown CPU";
		private const double UnknownClockGHz = 0;

		private static ulong _prevIdle;
		private static ulong _prevTotal;

		public static CpuInfo GetInfo()
		{
			return GetInfo(RuntimeInfo.Current);
		}

		internal static CpuInfo GetInfo(RuntimeContext runtime)
		{
			var model = DetectModel(runtime);
			var physical = DetectPhysicalCores(runtime);
			var logical = System.Environment.ProcessorCount;
			var load = DetectCpuLoad(runtime);

			return new CpuInfo(
				model,
				physical,
				logical,
				load,
				UnknownClockGHz,
				UnknownClockGHz
			);
		}

		private static string DetectModel(RuntimeContext runtime)
		{
			if (runtime.IsWindows)
				return WindowsCpuModel();

			if (runtime.IsLinux)
				return LinuxCpuModel();

			if (runtime.IsMacOS)
				return MacCpuModel();

			return UnknownCpuModel;
		}

		[ExcludeFromCodeCoverage(Justification = "Requires Windows registry access.")]
		private static string WindowsCpuModel()
		{
			try
			{
				if (!OperatingSystem.IsWindows())
					return UnknownCpuModel;

				using var key = Registry.LocalMachine.OpenSubKey(
					@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");

				return key?.GetValue("ProcessorNameString")?.ToString()?.Trim()
					   ?? UnknownCpuModel;
			}
			catch
			{
				return UnknownCpuModel;
			}
		}

		[ExcludeFromCodeCoverage(Justification = "Requires Linux /proc/cpuinfo.")]
		private static string LinuxCpuModel()
		{
			try
			{
				var line = File.ReadLines("/proc/cpuinfo")
					.FirstOrDefault(l => l.StartsWith("model name"));

				return line?.Split(':')[1].Trim() ?? UnknownCpuModel;
			}
			catch
			{
				return UnknownCpuModel;
			}
		}

		[ExcludeFromCodeCoverage(Justification = "Requires macOS sysctl.")]
		private static string MacCpuModel()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = SystemCommandPaths.MacSysctl,
					Arguments = "-n machdep.cpu.brand_string",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?.Trim();
				return string.IsNullOrWhiteSpace(output) ? UnknownCpuModel : output;
			}
			catch
			{
				return UnknownCpuModel;
			}
		}

		private static int DetectPhysicalCores(RuntimeContext runtime)
		{
			if (runtime.IsWindows)
				return System.Environment.ProcessorCount;

			if (runtime.IsLinux)
				return LinuxPhysicalCores();

			if (runtime.IsMacOS)
				return MacPhysicalCores();

			return System.Environment.ProcessorCount;
		}

		[ExcludeFromCodeCoverage(Justification = "Requires Linux /proc/cpuinfo.")]
		private static int LinuxPhysicalCores()
		{
			try
			{
				return File.ReadLines("/proc/cpuinfo")
					.Count(l => l.StartsWith("processor"));
			}
			catch
			{
				return System.Environment.ProcessorCount;
			}
		}

		[ExcludeFromCodeCoverage(Justification = "Requires macOS sysctl.")]
		private static int MacPhysicalCores()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = SystemCommandPaths.MacSysctl,
					Arguments = "-n hw.physicalcpu",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?.Trim();
				return int.TryParse(output, out var cores) ? cores : System.Environment.ProcessorCount;
			}
			catch
			{
				return System.Environment.ProcessorCount;
			}
		}

		private static double DetectCpuLoad(RuntimeContext runtime)
		{
			if (runtime.IsWindows)
				return WindowsCpuLoad();

			if (runtime.IsLinux)
				return LinuxCpuLoad();

			if (runtime.IsMacOS)
				return MacCpuLoad();

			return 0;
		}

		[ExcludeFromCodeCoverage(Justification = "Requires Windows native system timing APIs.")]
		private static double WindowsCpuLoad()
		{
			if (!WindowsNativeMethods.GetSystemTimes(out var idle, out var kernel, out var user))
				return 0;

			ulong idleTicks = idle.ToUInt64();
			ulong totalTicks = kernel.ToUInt64() + user.ToUInt64();

			ulong idleDelta = idleTicks - _prevIdle;
			ulong totalDelta = totalTicks - _prevTotal;

			_prevIdle = idleTicks;
			_prevTotal = totalTicks;

			if (totalDelta == 0)
				return 0;

			double usage = 100.0 * (1.0 - (double)idleDelta / totalDelta);
			return Math.Round(usage, 1);
		}

		[ExcludeFromCodeCoverage(Justification = "Requires Linux /proc/stat.")]
		private static double LinuxCpuLoad()
		{
			try
			{
				var parts = File.ReadLines("/proc/stat")
					.First(l => l.StartsWith("cpu "))
					.Split(' ', StringSplitOptions.RemoveEmptyEntries)
					.Skip(1)
					.Select(ulong.Parse)
					.ToArray();

				ulong idle = parts[3];
				ulong total = parts.Aggregate(0UL, (a, b) => a + b);

				ulong idleDelta = idle - _prevIdle;
				ulong totalDelta = total - _prevTotal;

				_prevIdle = idle;
				_prevTotal = total;

				if (totalDelta == 0)
					return 0;

				double usage = 100.0 * (1.0 - (double)idleDelta / totalDelta);
				return Math.Round(usage, 1);
			}
			catch
			{
				return 0;
			}
		}

		[ExcludeFromCodeCoverage(Justification = "Requires macOS process sampling.")]
		private static double MacCpuLoad()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = SystemCommandPaths.MacPs,
					Arguments = "-A -o %cpu",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd();
				var lines = output?.Split('\n')
					.Skip(1)
					.Where(l => double.TryParse(l.Trim(), out _))
					.Select(l => double.Parse(l.Trim()));

				return Math.Round(lines?.Sum() ?? 0, 1);
			}
			catch
			{
				return 0;
			}
		}
	}
}
