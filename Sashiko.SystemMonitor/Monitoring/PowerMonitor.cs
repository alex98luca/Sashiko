using System.Diagnostics;
using System.Runtime.InteropServices;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static partial class PowerMonitor
	{
		public static PowerInfo GetInfo()
		{
			return GetInfo(SystemPlatform.Current);
		}

		internal static PowerInfo GetInfo(SystemPlatform platform)
		{
			if (platform.IsWindows)
				return GetWindowsPower();

			if (platform.IsLinux)
				return GetLinuxPower();

			if (platform.IsMacOS)
				return GetMacPower();

			return new PowerInfo(true, 100, false);
		}

		// ------------------------------------------------------------
		// WINDOWS (GetSystemPowerStatus)
		// ------------------------------------------------------------

		private static PowerInfo GetWindowsPower()
		{
			try
			{
				if (!GetSystemPowerStatus(out var status))
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

		[StructLayout(LayoutKind.Sequential)]
		private struct SystemPowerStatus
		{
			public byte ACLineStatus;
			public byte BatteryFlag;
			public byte BatteryLifePercent;
			public byte SystemStatusFlag;
			public uint BatteryLifeTime;
			public uint BatteryFullLifeTime;
		}

		[LibraryImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool GetSystemPowerStatus(out SystemPowerStatus status);

		// ------------------------------------------------------------
		// LINUX (/sys/class/power_supply)
		// ------------------------------------------------------------

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
