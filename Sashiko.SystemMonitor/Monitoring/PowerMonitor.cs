using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Monitoring.Native;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class PowerMonitor
	{
		public static PowerInfo GetInfo()
		{
			return GetInfo(RuntimeInfo.Current);
		}

		internal static PowerInfo GetInfo(RuntimeContext runtime)
		{
			if (runtime.IsWindows)
				return GetWindowsPower();

			if (runtime.IsLinux)
				return GetLinuxPower();

			if (runtime.IsMacOS)
				return GetMacPower();

			return new PowerInfo(true, 100, false);
		}

		// ------------------------------------------------------------
		// WINDOWS (GetSystemPowerStatus)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires Windows native power APIs.")]
		private static PowerInfo GetWindowsPower()
		{
			try
			{
				if (!WindowsNativeMethods.GetSystemPowerStatus(out var status))
					return new PowerInfo(true, 100, false);

				bool plugged = status.ACLineStatus == 1;
				int percent = status.BatteryLifePercent;
				bool saver = status.SystemStatusFlag == 1;

				return new PowerInfo(plugged, percent, saver);
			}
			catch
			{
				return new PowerInfo(true, 100, false);
			}
		}

		// ------------------------------------------------------------
		// LINUX (/sys/class/power_supply)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires Linux /sys/class/power_supply.")]
		private static PowerInfo GetLinuxPower()
		{
			try
			{
				string powerPath = "/sys/class/power_supply";

				if (!Directory.Exists(powerPath))
					return new PowerInfo(true, 100, false);

				var batteryDir = Directory.GetDirectories(powerPath)
					.FirstOrDefault(d => d.Contains("BAT"));

				int percent = 100;
				bool plugged = true;

				if (batteryDir != null)
				{
					string capFile = Path.Combine(batteryDir, "capacity");
					string statusFile = Path.Combine(batteryDir, "status");

					if (File.Exists(capFile))
					{
						var content = File.ReadAllText(capFile).Trim();
						if (int.TryParse(content, out var p))
							percent = p;
					}

					if (File.Exists(statusFile))
					{
						var status = File.ReadAllText(statusFile).Trim().ToLowerInvariant();
						plugged = status.Contains("charging") || status.Contains("full");
					}
				}

				bool saver = percent < 20 && !plugged;

				return new PowerInfo(plugged, percent, saver);
			}
			catch
			{
				return new PowerInfo(true, 100, false);
			}
		}

		// ------------------------------------------------------------
		// MACOS (pmset -g batt)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires macOS pmset.")]
		private static PowerInfo GetMacPower()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "pmset",
					Arguments = "-g batt",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd();

				if (string.IsNullOrWhiteSpace(output))
					return new PowerInfo(true, 100, false);

				bool plugged = output.Contains("AC Power");
				int percent = ExtractMacBatteryPercent(output);

				return new PowerInfo(plugged, percent, false);
			}
			catch
			{
				return new PowerInfo(true, 100, false);
			}
		}

		private static int ExtractMacBatteryPercent(string output)
		{
			var line = output.Split('\n')
				.FirstOrDefault(l => l.Contains('%'));

			if (line == null)
				return 100;

			var percentParts = line.Split('%')[0].Split(' ');
			var percentStr = percentParts[^1].Trim();

			return int.TryParse(percentStr, out var p) ? p : 100;
		}
	}
}
