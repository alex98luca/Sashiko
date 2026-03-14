using Sashiko.Core.Models.Enums;

namespace Sashiko.Core.Conversions
{
	public static class BandwidthConverter
	{
		private const double Kb = 1000.0;
		private const double Mb = Kb * 1000.0;
		private const double Gb = Mb * 1000.0;

		public static double Convert(double value, BandwidthUnit from, BandwidthUnit to)
		{
			double bits = from switch
			{
				BandwidthUnit.Bits => value,
				BandwidthUnit.Kilobits => value * Kb,
				BandwidthUnit.Megabits => value * Mb,
				BandwidthUnit.Gigabits => value * Gb,
				_ => throw new ArgumentOutOfRangeException(nameof(from))
			};

			return to switch
			{
				BandwidthUnit.Bits => bits,
				BandwidthUnit.Kilobits => bits / Kb,
				BandwidthUnit.Megabits => bits / Mb,
				BandwidthUnit.Gigabits => bits / Gb,
				_ => throw new ArgumentOutOfRangeException(nameof(to))
			};
		}
	}
}
