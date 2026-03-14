using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;

namespace Sashiko.Core.Tests.Conversions
{
	public class MemoryConverterTests
	{
		private const double Tolerance = 0.0000001;

		[Fact]
		public void BytesToKilobytes_ShouldConvertCorrectly()
		{
			double result = MemoryConverter.Convert(1024, MemoryUnit.Bytes, MemoryUnit.Kilobytes);
			Assert.Equal(1.0, result, Tolerance);
		}

		[Fact]
		public void KilobytesToBytes_ShouldConvertCorrectly()
		{
			double result = MemoryConverter.Convert(1, MemoryUnit.Kilobytes, MemoryUnit.Bytes);
			Assert.Equal(1024, result, Tolerance);
		}

		[Fact]
		public void MegabytesToGigabytes_ShouldConvertCorrectly()
		{
			double result = MemoryConverter.Convert(1024, MemoryUnit.Megabytes, MemoryUnit.Gigabytes);
			Assert.Equal(1.0, result, Tolerance);
		}

		[Fact]
		public void GigabytesToMegabytes_ShouldConvertCorrectly()
		{
			double result = MemoryConverter.Convert(1, MemoryUnit.Gigabytes, MemoryUnit.Megabytes);
			Assert.Equal(1024, result, Tolerance);
		}

		[Fact]
		public void Convert_IdentityConversion_ShouldReturnSameValue()
		{
			double result = MemoryConverter.Convert(123.456, MemoryUnit.Gigabytes, MemoryUnit.Gigabytes);
			Assert.Equal(123.456, result, Tolerance);
		}

		[Fact]
		public void Convert_BytesToTerabytes_ShouldConvertCorrectly()
		{
			double bytes = 1024.0 * 1024 * 1024 * 1024;
			double result = MemoryConverter.Convert(bytes, MemoryUnit.Bytes, MemoryUnit.Terabytes);
			Assert.Equal(1.0, result, Tolerance);
		}

		[Fact]
		public void Convert_TerabytesToBytes_ShouldConvertCorrectly()
		{
			double result = MemoryConverter.Convert(1, MemoryUnit.Terabytes, MemoryUnit.Bytes);
			Assert.Equal(1024.0 * 1024 * 1024 * 1024, result, Tolerance);
		}

		[Fact]
		public void Convert_RoundTrip_ShouldReturnOriginalValue()
		{
			double original = 42.75;
			double bytes = MemoryConverter.Convert(original, MemoryUnit.Gigabytes, MemoryUnit.Bytes);
			double roundTrip = MemoryConverter.Convert(bytes, MemoryUnit.Bytes, MemoryUnit.Gigabytes);

			Assert.Equal(original, roundTrip, Tolerance);
		}
	}
}
