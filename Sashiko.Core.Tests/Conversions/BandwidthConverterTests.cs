using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;

namespace Sashiko.Core.Tests.Conversions
{
	public class BandwidthConverterTests
	{
		private const double Tolerance = 0.0000001;

		[Fact]
		public void BitsToKilobits_ShouldConvertCorrectly()
		{
			double result = BandwidthConverter.Convert(1000, BandwidthUnit.Bits, BandwidthUnit.Kilobits);
			Assert.Equal(1.0, result, Tolerance);
		}

		[Fact]
		public void KilobitsToBits_ShouldConvertCorrectly()
		{
			double result = BandwidthConverter.Convert(1, BandwidthUnit.Kilobits, BandwidthUnit.Bits);
			Assert.Equal(1000, result, Tolerance);
		}

		[Fact]
		public void MegabitsToGigabits_ShouldConvertCorrectly()
		{
			double result = BandwidthConverter.Convert(1000, BandwidthUnit.Megabits, BandwidthUnit.Gigabits);
			Assert.Equal(1.0, result, Tolerance);
		}

		[Fact]
		public void GigabitsToMegabits_ShouldConvertCorrectly()
		{
			double result = BandwidthConverter.Convert(1, BandwidthUnit.Gigabits, BandwidthUnit.Megabits);
			Assert.Equal(1000, result, Tolerance);
		}

		[Fact]
		public void Convert_IdentityConversion_ShouldReturnSameValue()
		{
			double result = BandwidthConverter.Convert(123.456, BandwidthUnit.Megabits, BandwidthUnit.Megabits);
			Assert.Equal(123.456, result, Tolerance);
		}

		[Fact]
		public void Convert_RoundTrip_ShouldReturnOriginalValue()
		{
			double original = 512.25;
			double bits = BandwidthConverter.Convert(original, BandwidthUnit.Gigabits, BandwidthUnit.Bits);
			double roundTrip = BandwidthConverter.Convert(bits, BandwidthUnit.Bits, BandwidthUnit.Gigabits);

			Assert.Equal(original, roundTrip, Tolerance);
		}
	}
}
