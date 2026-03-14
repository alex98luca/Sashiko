using System.Diagnostics;
using System.Runtime.InteropServices;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class GpuMonitor
	{
		public static GpuInfo GetInfo()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return GetWindowsGpu();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return GetLinuxGpu();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return GetMacGpu();

			return new GpuInfo("Unknown", "Unknown", 0, 0);
		}

		// ------------------------------------------------------------
		// WINDOWS (DXGI via dxdiag)
		// ------------------------------------------------------------

		private static GpuInfo GetWindowsGpu()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "dxdiag",
					Arguments = "/t dxdiag_output.txt",
					UseShellExecute = false,
					CreateNoWindow = true
				});

				process?.WaitForExit(2000);

				if (!File.Exists("dxdiag_output.txt"))
					return new GpuInfo("Unknown", "Unknown", 0, 0);

				var lines = File.ReadAllLines("dxdiag_output.txt");

				string model = lines.FirstOrDefault(l => l.Contains("Card name"))?
					.Split(':')[1].Trim() ?? "Unknown";

				string vendor = lines.FirstOrDefault(l => l.Contains("Manufacturer"))?
					.Split(':')[1].Trim() ?? "Unknown";

				string vramLine = lines.FirstOrDefault(l => l.Contains("Display Memory"));
				double vram = 0;

				if (vramLine != null)
				{
					var parts = vramLine.Split(':')[1].Trim().Split(' ')[0];
					if (double.TryParse(parts, out var mb))
						vram = mb / 1024.0;
				}

				File.Delete("dxdiag_output.txt");

				return new GpuInfo(vendor, model, vram, 0);
			}
			catch
			{
				return new GpuInfo("Unknown", "Unknown", 0, 0);
			}
		}

		// ------------------------------------------------------------
		// LINUX (lspci + sysfs)
		// ------------------------------------------------------------

		private static GpuInfo GetLinuxGpu()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "sh",
					Arguments = "-c \"lspci -mm | grep -i 'vga'\"",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?.Trim();

				if (string.IsNullOrWhiteSpace(output))
					return new GpuInfo("Unknown", "Unknown", 0, 0);

				// Example: "VGA compatible controller: NVIDIA Corporation GP107 [GeForce GTX 1050 Ti]"
				string model = output;
				string vendor = DetectLinuxVendor(output);

				double vram = DetectLinuxVram();

				return new GpuInfo(vendor, model, vram, 0);
			}
			catch
			{
				return new GpuInfo("Unknown", "Unknown", 0, 0);
			}
		}

		private static string DetectLinuxVendor(string line)
		{
			if (line.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase)) return "NVIDIA";
			if (line.Contains("AMD", StringComparison.OrdinalIgnoreCase) ||
				line.Contains("ATI", StringComparison.OrdinalIgnoreCase)) return "AMD";
			if (line.Contains("Intel", StringComparison.OrdinalIgnoreCase)) return "Intel";
			return "Unknown";
		}

		private static double DetectLinuxVram()
		{
			try
			{
				var card = Directory.GetDirectories("/sys/class/drm")
					.FirstOrDefault(d => d.Contains("card"));

				if (card == null)
					return 0;

				var vramPath = Path.Combine(card, "device", "mem_info_vram_total");

				if (File.Exists(vramPath))
				{
					var bytes = File.ReadAllText(vramPath).Trim();
					if (ulong.TryParse(bytes, out var b))
						return b / 1024.0 / 1024.0 / 1024.0;
				}
			}
			catch { }

			return 0;
		}

		// ------------------------------------------------------------
		// MACOS (system_profiler)
		// ------------------------------------------------------------

		private static GpuInfo GetMacGpu()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "system_profiler",
					Arguments = "SPDisplaysDataType",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd();

				if (string.IsNullOrWhiteSpace(output))
					return new GpuInfo("Unknown", "Unknown", 0, 0);

				string model = ExtractMacField(output, "Chipset Model");
				string vendor = ExtractMacField(output, "Vendor");
				double vram = ParseMacVram(ExtractMacField(output, "VRAM"));

				return new GpuInfo(vendor, model, vram, 0);
			}
			catch
			{
				return new GpuInfo("Unknown", "Unknown", 0, 0);
			}
		}

		private static string ExtractMacField(string text, string field)
		{
			var line = text.Split('\n')
				.FirstOrDefault(l => l.Trim().StartsWith(field));

			return line?.Split(':')[1].Trim() ?? "Unknown";
		}

		private static double ParseMacVram(string vram)
		{
			if (string.IsNullOrWhiteSpace(vram))
				return 0;

			if (vram.EndsWith("GB") && double.TryParse(vram[..^2], out var gb))
				return gb;

			if (vram.EndsWith("MB") && double.TryParse(vram[..^2], out var mb))
				return mb / 1024.0;

			return 0;
		}
	}
}