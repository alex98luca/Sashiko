using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static partial class MemoryMonitor
	{
		public static MemoryInfo GetInfo()
		{
			return GetInfo(SystemPlatform.Current);
		}

		internal static MemoryInfo GetInfo(SystemPlatform platform)
		{
			if (platform.IsWindows)
				return GetWindowsMemory();

			if (platform.IsLinux)
				return GetLinuxMemory();

			if (platform.IsMacOS)
				return GetMacMemory();

			return new MemoryInfo(0, 0, 0, 0);
		}

		// ------------------------------------------------------------
		// WINDOWS (GlobalMemoryStatusEx)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires Windows native memory APIs.")]
		private static MemoryInfo GetWindowsMemory()
		{
			var mem = new MemoryStatusEx();
			if (!GlobalMemoryStatusEx(ref mem))
				return new MemoryInfo(0, 0, 0, 0);

			double total = MemoryConverter.Convert(mem.TotalPhysicalMemory, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
			double available = MemoryConverter.Convert(mem.AvailablePhysicalMemory, MemoryUnit.Bytes, MemoryUnit.Gigabytes);
			double usedBySystem = total - available;

			double usedByCurrentProcess = MemoryConverter.Convert(Process.GetCurrentProcess().WorkingSet64, MemoryUnit.Bytes, MemoryUnit.Gigabytes);

			return new MemoryInfo(total, available, usedBySystem, usedByCurrentProcess);
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct MemoryStatusEx
		{
			public uint Length;
			public uint MemoryLoad;
			public ulong TotalPhysicalMemory;
			public ulong AvailablePhysicalMemory;
			public ulong TotalPageFile;
			public ulong AvailablePageFile;
			public ulong TotalVirtual;
			public ulong AvailableVirtual;
			public ulong AvailableExtendedVirtual;

			public MemoryStatusEx()
			{
				Length = (uint)Marshal.SizeOf<MemoryStatusEx>();
				MemoryLoad = 0;
				TotalPhysicalMemory = 0;
				AvailablePhysicalMemory = 0;
				TotalPageFile = 0;
				AvailablePageFile = 0;
				TotalVirtual = 0;
				AvailableVirtual = 0;
				AvailableExtendedVirtual = 0;
			}
		}

		[LibraryImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[ExcludeFromCodeCoverage(Justification = "Source-generated Windows native interop.")]
		private static partial bool GlobalMemoryStatusEx(ref MemoryStatusEx buffer);

		// ------------------------------------------------------------
		// LINUX (/proc/meminfo)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires Linux /proc/meminfo.")]
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

		[ExcludeFromCodeCoverage(Justification = "Requires macOS memory tooling.")]
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

		[ExcludeFromCodeCoverage(Justification = "Requires macOS sysctl.")]
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

		[ExcludeFromCodeCoverage(Justification = "Requires macOS vm_stat.")]
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
					if (!long.TryParse(parts[3], out pageSize))
						pageSize = 4096;
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
