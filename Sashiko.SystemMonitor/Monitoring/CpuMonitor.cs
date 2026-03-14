using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class CpuMonitor
	{
		// Cached values for CPU load calculation
		private static ulong _prevIdle;
		private static ulong _prevTotal;

		public static CpuInfo GetInfo()
		{
			var model = DetectModel();
			var physical = DetectPhysicalCores();
			var logical = Environment.ProcessorCount;
			var load = DetectCpuLoad();
			var baseClock = DetectBaseClock();
			var currentClock = DetectCurrentClock();

			return new CpuInfo(
				model,
				physical,
				logical,
				load,
				baseClock,
				currentClock
			);
		}

		// ------------------------------------------------------------
		// CPU Model
		// ------------------------------------------------------------

		private static string DetectModel()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return WindowsCpuModel();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return LinuxCpuModel();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return MacCpuModel();

			return "Unknown CPU";
		}

		private static string WindowsCpuModel()
		{
			try
			{
				using var key = Registry.LocalMachine.OpenSubKey(
					@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");

				return key?.GetValue("ProcessorNameString")?.ToString()?.Trim()
					   ?? "Unknown CPU";
			}
			catch
			{
				return "Unknown CPU";
			}
		}

		private static string LinuxCpuModel()
		{
			try
			{
				var line = File.ReadLines("/proc/cpuinfo")
					.FirstOrDefault(l => l.StartsWith("model name"));

				return line?.Split(':')[1].Trim() ?? "Unknown CPU";
			}
			catch
			{
				return "Unknown CPU";
			}
		}

		private static string MacCpuModel()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "sysctl",
					Arguments = "-n machdep.cpu.brand_string",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?.Trim();
				return string.IsNullOrWhiteSpace(output) ? "Unknown CPU" : output;
			}
			catch
			{
				return "Unknown CPU";
			}
		}

		// ------------------------------------------------------------
		// Physical Cores
		// ------------------------------------------------------------

		private static int DetectPhysicalCores()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return Environment.ProcessorCount; // refined later

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return LinuxPhysicalCores();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return MacPhysicalCores();

			return Environment.ProcessorCount;
		}

		private static int LinuxPhysicalCores()
		{
			try
			{
				return File.ReadLines("/proc/cpuinfo")
					.Count(l => l.StartsWith("processor"));
			}
			catch
			{
				return Environment.ProcessorCount;
			}
		}

		private static int MacPhysicalCores()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "sysctl",
					Arguments = "-n hw.physicalcpu",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?.Trim();
				return int.TryParse(output, out var cores) ? cores : Environment.ProcessorCount;
			}
			catch
			{
				return Environment.ProcessorCount;
			}
		}

		// ------------------------------------------------------------
		// CPU Load (Non-blocking, No Sleep)
		// ------------------------------------------------------------

		private static double DetectCpuLoad()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return WindowsCpuLoad();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return LinuxCpuLoad();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return MacCpuLoad();

			return 0;
		}

		// WINDOWS: Use GetSystemTimes (fast, no sleep)
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetSystemTimes(
			out FILETIME idleTime,
			out FILETIME kernelTime,
			out FILETIME userTime);

		private static double WindowsCpuLoad()
		{
			if (!GetSystemTimes(out var idle, out var kernel, out var user))
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

		private static double MacCpuLoad()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "ps",
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

		// ------------------------------------------------------------
		// Clock Speeds (future)
		// ------------------------------------------------------------

		private static double DetectBaseClock() => 0;
		private static double DetectCurrentClock() => 0;
	}

	// Helper for Windows FILETIME
	internal static class FileTimeExtensions
	{
		public static ulong ToUInt64(this FILETIME ft)
			=> ((ulong)ft.dwHighDateTime << 32) | (uint)ft.dwLowDateTime;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct FILETIME
	{
		public uint dwLowDateTime;
		public uint dwHighDateTime;
	}
}