using Sashiko.Maintenance.Libraries.Languages.Updating;

namespace Sashiko.Maintenance.Tests.Libraries.Languages.Updating
{
	public sealed class IsoCodeNormalizerTests
	{
		[Theory]
		[InlineData(" en ", "en")]
		[InlineData("EN", "en")]
		[InlineData(" En ", "en")]
		public void Normalize_ShouldTrimAndLowercase(string input, string expected)
		{
			var result = IsoCodeNormalizer.Normalize(input);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Normalize_ShouldReturnNull_ForEmptyOrWhitespace(string? input)
		{
			var result = IsoCodeNormalizer.Normalize(input);
			Assert.Null(result);
		}
	}
}
