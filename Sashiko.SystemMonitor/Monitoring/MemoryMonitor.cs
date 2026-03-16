using System.Diagnostics;
using System.Runtime.InteropServices;
using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class MemoryMonitor
	{
		public static MemoryInfo GetInfo()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return GetWindowsMemory();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return GetLinuxMemory();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return GetMacMemory();

			return new MemoryInfo(0, 0, 0, 0);
		}

		// ------------------------------------------------------------
		// WINDOWS (GlobalMemoryStatusEx)
		// ------------------------------------------------------------

		private static MemoryInfo GetWindowsMemory()
		{
			MEMORYSTATUSEX mem = new MEMORYSTATUSEX();
			if (!GlobalMemoryStatusEx(ref mem))
				return new MemoryInfo(0, 0, 0, 0);

			double total = MemoryConverter.Convert(mem.ullTotalPhys, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
			double available = MemoryConverter.Convert(mem.ullAvailPhys, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
			double usedBySystem = total - available;

			double usedByCurrentProcess = MemoryConverter.Convert(Process.GetCurrentProcess().WorkingSet64, MemoryUnit.Bytes, MemoryUnit.Gigabytes);

			return new MemoryInfo(total, available, usedBySystem, usedByCurrentProcess);
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct MEMORYSTATUSEX
		{
			public uint dwLength;
			public uint dwMemoryLoad;
			public ulong ullTotalPhys;
			public ulong ullAvailPhys;
			public ulong ullTotalPageFile;
			public ulong ullAvailPageFile;
			public ulong ullTotalVirtual;
			public ulong ullAvailVirtual;
			public ulong ullAvailExtendedVirtual;

			public MEMORYSTATUSEX()
			{
				dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
				dwMemoryLoad = 0;
				ullTotalPhys = 0;
				ullAvailPhys = 0;
				ullTotalPageFile = 0;
				ullAvailPageFile = 0;
				ullTotalVirtual = 0;
				ullAvailVirtual = 0;
				ullAvailExtendedVirtual = 0;
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

		// ------------------------------------------------------------
		// LINUX (/proc/meminfo)
		// ------------------------------------------------------------

		private static MemoryInfo GetLinuxMemory()
		{
			try
			{
				var memInfo = File.ReadAllLines("/proc/meminfo");

				// Values in kB → convert to bytes → convert to GB
				double total = MemoryConverter.Convert(
					ParseMeminfo(memInfo, "MemTotal") * 1024,
					MemoryUnit.Bytes,
					MemoryUnit.Gigabytes
				);

				double available = MemoryConverter.Convert(
					ParseMeminfo(memInfo, "MemAvailable") * 1024,
					MemoryUnit.Bytes,
					MemoryUnit.Gigabytes
				);

				double usedBySystem = total - available;

				double usedByCurrentProcess = MemoryConverter.Convert(
					Process.GetCurrentProcess().WorkingSet64,
					MemoryUnit.Bytes,
					MemoryUnit.Gigabytes
				);

				return new MemoryInfo(total, available, usedBySystem, usedByCurrentProcess);
			}
			catch
			{
				return new MemoryInfo(0, 0, 0, 0);
			}
		}

		private static double ParseMeminfo(string[] lines, string key)
		{
			var line = lines.FirstOrDefault(l => l.StartsWith(key));
			if (line == null) return 0;

			var parts = line.Split(':')[1].Trim().Split(' ')[0];
			return double.TryParse(parts, out var kb) ? kb : 0;
		}

		// ------------------------------------------------------------
		// MACOS (sysctl + vm_stat)
		// ------------------------------------------------------------

		private static MemoryInfo GetMacMemory()
		{
			try
			{
				double total = GetMacTotalMemory();
				double free = GetMacFreeMemory();
				double usedBySystem = total - free;

				double usedByCurrentProcess = MemoryConverter.Convert(
					Process.GetCurrentProcess().WorkingSet64,
					MemoryUnit.Bytes,
					MemoryUnit.Gigabytes
				);

				return new MemoryInfo(total, free, usedBySystem, usedByCurrentProcess);
			}
			catch
			{
				return new MemoryInfo(0, 0, 0, 0);
			}
		}

		private static double GetMacTotalMemory()
		{
			var process = Process.Start(new ProcessStartInfo
			{
				FileName = "sysctl",
				Arguments = "-n hw.memsize",
				RedirectStandardOutput = true,
				UseShellExecute = false
			});

			var output = process?.StandardOutput.ReadToEnd()?.Trim();

			if (ulong.TryParse(output, out var bytes))
			{
				return MemoryConverter.Convert(bytes, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
			}

			return 0;
		}

		private static double GetMacFreeMemory()
		{
			var process = Process.Start(new ProcessStartInfo
			{
				FileName = "vm_stat",
				RedirectStandardOutput = true,
				UseShellExecute = false
			});

			var output = process?.StandardOutput.ReadToEnd();
			if (string.IsNullOrWhiteSpace(output))
				return 0;

			var lines = output.Split('\n');

			long pageSize = 4096;
			long freePages = 0;
			long inactivePages = 0;

			foreach (var line in lines)
			{
				if (line.StartsWith("Pages free"))
					freePages = ParseVmStat(line);

				if (line.StartsWith("Pages inactive"))
					inactivePages = ParseVmStat(line);

				if (line.StartsWith("page size of"))
				{
					var parts = line.Split(' ');
					long.TryParse(parts[3], out pageSize);
				}
			}

			long freeBytes = (freePages + inactivePages) * pageSize;

			return MemoryConverter.Convert(freeBytes, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
		}

		private static long ParseVmStat(string line)
		{
			var parts = line.Split(':')[1].Trim().Trim('.').Split(' ')[0];
			return long.TryParse(parts, out var value) ? value : 0;
		}
	}
}
