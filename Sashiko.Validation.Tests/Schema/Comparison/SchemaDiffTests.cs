using Sashiko.Validation.Schema.Comparison;

namespace Sashiko.Validation.Tests.Schema.Comparison
{
	public sealed class SchemaDiffTests
	{
		[Fact]
		public void IsMatch_WhenMissingAndExtraAreEmpty_ReturnsTrue()
		{
			var diff = new SchemaDiff(
				Missing: new List<string>(),
				Extra: new List<string>()
			);

			Assert.True(diff.IsMatch);
		}

		[Fact]
		public void IsMatch_WhenMissingIsNotEmpty_ReturnsFalse()
		{
			var diff = new SchemaDiff(
				Missing: new List<string> { "id" },
				Extra: new List<string>()
			);

			Assert.False(diff.IsMatch);
		}

		[Fact]
		public void IsMatch_WhenExtraIsNotEmpty_ReturnsFalse()
		{
			var diff = new SchemaDiff(
				Missing: new List<string>(),
				Extra: new List<string> { "unexpected" }
			);

			Assert.False(diff.IsMatch);
		}

		[Fact]
		public void Constructor_StoresListsCorrectly()
		{
			var missing = new List<string> { "a" };
			var extra = new List<string> { "b" };

			var diff = new SchemaDiff(missing, extra);

			Assert.Equal(missing, diff.Missing);
			Assert.Equal(extra, diff.Extra);
		}
	}
}
