using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Sashiko.Core.Environment;
using Sashiko.SystemMonitor.Models;

namespace Sashiko.SystemMonitor.Monitoring
{
	public static class GpuMonitor
	{
		private const string UnknownGpuValue = "Unknown";

		public static GpuInfo GetInfo()
		{
			return GetInfo(RuntimeInfo.Current);
		}

		internal static GpuInfo GetInfo(RuntimeContext runtime)
		{
			if (runtime.IsWindows)
				return GetWindowsGpu();

			if (runtime.IsLinux)
				return GetLinuxGpu();

			if (runtime.IsMacOS)
				return GetMacGpu();

			return UnknownGpu();
		}

		// ------------------------------------------------------------
		// WINDOWS (DXGI via dxdiag)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires Windows dxdiag.")]
		private static GpuInfo GetWindowsGpu()
		{
			var outputPath = Path.Combine(Path.GetTempPath(), $"sashiko-dxdiag-{Guid.NewGuid():N}.txt");

			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = SystemCommandPaths.WindowsDxDiag,
					Arguments = $"/t \"{outputPath}\"",
					UseShellExecute = false,
					CreateNoWindow = true
				});

				process?.WaitForExit(2000);

				if (!File.Exists(outputPath))
					return UnknownGpu();

				var lines = File.ReadAllLines(outputPath);

				string model = lines.FirstOrDefault(l => l.Contains("Card name"))?
					.Split(':')[1].Trim() ?? UnknownGpuValue;

				string vendor = lines.FirstOrDefault(l => l.Contains("Manufacturer"))?
					.Split(':')[1].Trim() ?? UnknownGpuValue;

				string? vramLine = lines.FirstOrDefault(l => l.Contains("Display Memory"));
				double vram = 0;

				if (vramLine != null)
				{
					var parts = vramLine.Split(':')[1].Trim().Split(' ')[0];
					if (double.TryParse(parts, out var mb))
						vram = mb / 1024.0;
				}

				return new GpuInfo(vendor, model, vram, 0);
			}
			catch
			{
				return UnknownGpu();
			}
			finally
			{
				if (File.Exists(outputPath))
					File.Delete(outputPath);
			}
		}

		// ------------------------------------------------------------
		// LINUX (lspci + sysfs)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires host GPU tooling and sysfs driver support.")]
		private static GpuInfo GetLinuxGpu()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = SystemCommandPaths.LinuxLspci,
					Arguments = "-mm",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd()?
					.Split('\n')
					.FirstOrDefault(IsLinuxDisplayDevice)
					?.Trim();

				if (string.IsNullOrWhiteSpace(output))
					return UnknownGpu();

				// Example: "VGA compatible controller: NVIDIA Corporation GP107 [GeForce GTX 1050 Ti]"
				string model = output;
				string vendor = DetectLinuxVendor(output);

				double vram = DetectLinuxVram();

				return new GpuInfo(vendor, model, vram, 0);
			}
			catch
			{
				return UnknownGpu();
			}
		}

		private static string DetectLinuxVendor(string line)
		{
			if (line.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase)) return "NVIDIA";
			if (line.Contains("AMD", StringComparison.OrdinalIgnoreCase) ||
				line.Contains("ATI", StringComparison.OrdinalIgnoreCase)) return "AMD";
			if (line.Contains("Intel", StringComparison.OrdinalIgnoreCase)) return "Intel";
			return UnknownGpuValue;
		}

		private static bool IsLinuxDisplayDevice(string line)
		{
			return line.Contains("VGA", StringComparison.OrdinalIgnoreCase) ||
				line.Contains("Display", StringComparison.OrdinalIgnoreCase) ||
				line.Contains("3D", StringComparison.OrdinalIgnoreCase);
		}

		[ExcludeFromCodeCoverage(Justification = "Requires host GPU sysfs driver support.")]
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
			catch
			{
				// VRAM sysfs probes are optional and often unavailable without driver support.
				return 0;
			}

			return 0;
		}

		// ------------------------------------------------------------
		// MACOS (system_profiler)
		// ------------------------------------------------------------

		[ExcludeFromCodeCoverage(Justification = "Requires macOS system_profiler.")]
		private static GpuInfo GetMacGpu()
		{
			try
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = SystemCommandPaths.MacSystemProfiler,
					Arguments = "SPDisplaysDataType",
					RedirectStandardOutput = true,
					UseShellExecute = false
				});

				var output = process?.StandardOutput.ReadToEnd();

				if (string.IsNullOrWhiteSpace(output))
					return UnknownGpu();

				string model = ExtractMacField(output, "Chipset Model");
				string vendor = ExtractMacField(output, "Vendor");
				double vram = ParseMacVram(ExtractMacField(output, "VRAM"));

				return new GpuInfo(vendor, model, vram, 0);
			}
			catch
			{
				return UnknownGpu();
			}
		}

		private static string ExtractMacField(string text, string field)
		{
			var line = text.Split('\n')
				.FirstOrDefault(l => l.Trim().StartsWith(field));

			return line?.Split(':')[1].Trim() ?? UnknownGpuValue;
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

		private static GpuInfo UnknownGpu()
			=> new GpuInfo(UnknownGpuValue, UnknownGpuValue, 0, 0);
	}
}
