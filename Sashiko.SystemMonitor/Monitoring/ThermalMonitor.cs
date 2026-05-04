using System.Diagnostics;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class ThermalMonitor
	{
		public static ThermalInfo GetInfo()
		{
			return GetInfo(SystemPlatform.Current);
		}

		internal static ThermalInfo GetInfo(SystemPlatform platform)
		{
			if (platform.IsLinux)
				return GetLinuxThermals();

			if (platform.IsWindows)
				return GetWindowsThermals();

			if (platform.IsMacOS)
				return GetMacThermals();

			return new ThermalInfo(0, 0, 0, false);
		}

		// ------------------------------------------------------------
		// WINDOWS (no native API)
		// ------------------------------------------------------------

		private static ThermalInfo GetWindowsThermals()
		{
			// Windows does not expose temps natively.
			// Future: integrate LibreHardwareMonitor or NVML.
			return new ThermalInfo(0, 0, 0, false);
		}

		// ------------------------------------------------------------
		// LINUX (/sys/class/thermal + hwmon)
		// ------------------------------------------------------------

		private static ThermalInfo GetLinuxThermals()
		{
			double cpuTemp = ReadLinuxCpuTemp();
			double gpuTemp = ReadLinuxGpuTemp();
			double systemTemp = ReadLinuxSystemTemp();

			bool throttling = cpuTemp > 90 || gpuTemp > 90 || systemTemp > 80;

			return new ThermalInfo(cpuTemp, gpuTemp, systemTemp, throttling);
		}

		private static double ReadLinuxCpuTemp()
		{
			try
			{
				// Try hwmon first (more accurate)
				var hwmonDirs = Directory.GetDirectories("/sys/class/hwmon");

				foreach (var dir in hwmonDirs)
				{
					var tempFiles = Directory.GetFiles(dir, "temp*_input");
					foreach (var file in tempFiles)
					{
						var content = File.ReadAllText(file).Trim();
						if (double.TryParse(content, out var milli))
							return milli / 1000.0;
					}
				}

				// Fallback: thermal_zone
				var zones = Directory.GetDirectories("/sys/class/thermal");
				foreach (var zone in zones)
				{
					var tempFile = Path.Combine(zone, "temp");
					if (File.Exists(tempFile))
					{
						var content = File.ReadAllText(tempFile).Trim();
						if (double.TryParse(content, out var milli))
							return milli / 1000.0;
					}
				}
			}
			catch
			{
				// Thermal probes may be missing or permission-restricted on many Linux systems.
				return 0;
			}

			return 0;
		}

		private static double ReadLinuxGpuTemp()
		{
			var nvidiaTemp = ReadNvidiaGpuTemp();
			if (nvidiaTemp > 0)
				return nvidiaTemp;

			return ReadAmdGpuTemp();
		}

		private static double ReadNvidiaGpuTemp()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "nvidia-smi",
					Arguments = "--query-gpu=temperature.gpu --format=csv,noheader",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?.Trim();

				if (double.TryParse(output, out var temp))
					return temp;
			}
			catch
			{
				// nvidia-smi is optional and may not be installed even on systems with a GPU.
				return 0;
			}

			return 0;
		}

		private static double ReadAmdGpuTemp()
		{
			try
			{
				var hwmonDirs = Directory.GetDirectories("/sys/class/hwmon");

				foreach (var dir in hwmonDirs)
				{
					if (!IsAmdGpuHwmon(dir))
						continue;

					var temp = ReadFirstHwmonTemperature(dir);
					if (temp > 0)
						return temp;
				}
			}
			catch
			{
				// AMD hwmon probes are optional and may be hidden by the active driver.
				return 0;
			}

			return 0;
		}

		private static bool IsAmdGpuHwmon(string directory)
		{
			var nameFile = Path.Combine(directory, "name");
			if (!File.Exists(nameFile))
				return false;

			var name = File.ReadAllText(nameFile).Trim();
			return name.Contains("amdgpu", StringComparison.OrdinalIgnoreCase);
		}

		private static double ReadFirstHwmonTemperature(string directory)
		{
			var tempFile = Directory.GetFiles(directory, "temp*_input").FirstOrDefault();
			if (tempFile == null)
				return 0;

			var content = File.ReadAllText(tempFile).Trim();
			return double.TryParse(content, out var milli) ? milli / 1000.0 : 0;
		}

		private static double ReadLinuxSystemTemp()
		{
			try
			{
				var zones = Directory.GetDirectories("/sys/class/thermal");

				foreach (var zone in zones)
				{
					var typeFile = Path.Combine(zone, "type");
					var tempFile = Path.Combine(zone, "temp");

					if (!File.Exists(typeFile) || !File.Exists(tempFile))
						continue;

					var type = File.ReadAllText(typeFile).Trim();

					// Look for general system sensors
					if (IsSystemThermalZone(type))
					{
						var content = File.ReadAllText(tempFile).Trim();
						if (double.TryParse(content, out var milli))
							return milli / 1000.0;
					}
				}
			}
			catch
			{
				// System thermal zones are not guaranteed to exist on all Linux machines.
				return 0;
			}

			return 0;
		}

		private static bool IsSystemThermalZone(string type)
		{
			return type.Contains("x86_pkg_temp", StringComparison.OrdinalIgnoreCase) ||
				type.Contains("acpitz", StringComparison.OrdinalIgnoreCase) ||
				type.Contains("pch", StringComparison.OrdinalIgnoreCase) ||
				type.Contains("soc", StringComparison.OrdinalIgnoreCase);
		}

		// ------------------------------------------------------------
		// MACOS (no native API)
		// ------------------------------------------------------------

		private static ThermalInfo GetMacThermals()
		{
			// macOS does not expose temps without SMC bindings.
			// Future: integrate macOS SMC library.
			return new ThermalInfo(0, 0, 0, false);
		}
	}
}
