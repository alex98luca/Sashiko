using Sashiko.Core.Models.Enums;

namespace Sashiko.Core.Conversions
{
	public static class MemoryConverter
	{
		private const double KB = 1024.0;
		private const double MB = KB * 1024.0;
		private const double GB = MB * 1024.0;
		private const double TB = GB * 1024.0;

		public static double Convert(double value, MemoryUnit from, MemoryUnit to)
		{
			double bytes = from switch
			{
				MemoryUnit.Bytes => value,
				MemoryUnit.Kilobytes => value * KB,
				MemoryUnit.Megabytes => value * MB,
				MemoryUnit.Gigabytes => value * GB,
				MemoryUnit.Terabytes => value * TB,
				_ => throw new ArgumentOutOfRangeException(nameof(from))
			};

			return to switch
			{
				MemoryUnit.Bytes => bytes,
				MemoryUnit.Kilobytes => bytes / KB,
				MemoryUnit.Megabytes => bytes / MB,
				MemoryUnit.Gigabytes => bytes / GB,
				MemoryUnit.Terabytes => bytes / TB,
				_ => throw new ArgumentOutOfRangeException(nameof(to))
			};
		}
	}
}
