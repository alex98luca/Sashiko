using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static partial class CpuMonitor
	{
		private const string UnknownCpuModel = "Unknown CPU";
		private const double UnknownClockGHz = 0;

		// Cached values for CPU load calculation
		private static ulong _prevIdle;
		private static ulong _prevTotal;

		public static CpuInfo GetInfo()
		{
			return GetInfo(SystemPlatform.Current);
		}

		internal static CpuInfo GetInfo(SystemPlatform platform)
		{
			var model = DetectModel(platform);
			var physical = DetectPhysicalCores(platform);
			var logical = Environment.ProcessorCount;
			var load = DetectCpuLoad(platform);

			return new CpuInfo(
				model,
				physical,
				logical,
				load,
				UnknownClockGHz,
				UnknownClockGHz
			);
		}

		// ------------------------------------------------------------
		// CPU Model
		// ------------------------------------------------------------

		private static string DetectModel(SystemPlatform platform)
		{
			if (platform.IsWindows)
				return WindowsCpuModel();

			if (platform.IsLinux)
				return LinuxCpuModel();

			if (platform.IsMacOS)
				return MacCpuModel();

			return UnknownCpuModel;
		}

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
				return string.IsNullOrWhiteSpace(output) ? UnknownCpuModel : output;
			}
			catch
			{
				return UnknownCpuModel;
			}
		}

		// ------------------------------------------------------------
		// Physical Cores
		// ------------------------------------------------------------

		private static int DetectPhysicalCores(SystemPlatform platform)
		{
			if (platform.IsWindows)
				return Environment.ProcessorCount; // refined later

			if (platform.IsLinux)
				return LinuxPhysicalCores();

			if (platform.IsMacOS)
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

		private static double DetectCpuLoad(SystemPlatform platform)
		{
			if (platform.IsWindows)
				return WindowsCpuLoad();

			if (platform.IsLinux)
				return LinuxCpuLoad();

			if (platform.IsMacOS)
				return MacCpuLoad();

			return 0;
		}

		// WINDOWS: Use GetSystemTimes (fast, no sleep)
		[LibraryImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool GetSystemTimes(
			out FileTime idleTime,
			out FileTime kernelTime,
			out FileTime userTime);

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

	}

	internal static class FileTimeExtensions
	{
		public static ulong ToUInt64(this FileTime fileTime)
			=> ((ulong)fileTime.HighDateTime << 32) | fileTime.LowDateTime;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct FileTime
	{
		public uint LowDateTime;
		public uint HighDateTime;
	}
}
