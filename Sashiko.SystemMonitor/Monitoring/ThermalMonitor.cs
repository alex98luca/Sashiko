using System.Diagnostics;
using System.Runtime.InteropServices;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class ThermalMonitor
	{
		public static ThermalInfo GetInfo()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return GetLinuxThermals();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return GetWindowsThermals();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
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
			catch { }

			return 0;
		}

		private static double ReadLinuxGpuTemp()
		{
			// NVIDIA GPUs (nvidia-smi)
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
			catch { }

			// AMD GPUs (hwmon)
			try
			{
				var hwmonDirs = Directory.GetDirectories("/sys/class/hwmon");

				foreach (var dir in hwmonDirs)
				{
					var nameFile = Path.Combine(dir, "name");
					if (File.Exists(nameFile))
					{
						var name = File.ReadAllText(nameFile).Trim().ToLower();
						if (name.Contains("amdgpu"))
						{
							var tempFile = Directory.GetFiles(dir, "temp*_input").FirstOrDefault();
							if (tempFile != null)
							{
								var content = File.ReadAllText(tempFile).Trim();
								if (double.TryParse(content, out var milli))
									return milli / 1000.0;
							}
						}
					}
				}
			}
			catch { }

			return 0;
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

					var type = File.ReadAllText(typeFile).Trim().ToLower();

					// Look for general system sensors
					if (type.Contains("x86_pkg_temp") ||
						type.Contains("acpitz") ||
						type.Contains("pch") ||
						type.Contains("soc"))
					{
						var content = File.ReadAllText(tempFile).Trim();
						if (double.TryParse(content, out var milli))
							return milli / 1000.0;
					}
				}
			}
			catch { }

			return 0;
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