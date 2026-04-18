using Sashiko.Core.Text;

namespace Sashiko.Core.Tests.Text
{
	public sealed class StringNormalizerTests
	{
		// ------------------------------------------------------------
		// NormalizeOptional
		// ------------------------------------------------------------

		[Fact]
		public void NormalizeOptional_ReturnsNull_WhenValueIsNull()
		{
			Assert.Null(StringNormalizer.NormalizeOptional(null));
		}

		[Fact]
		public void NormalizeOptional_ReturnsNull_WhenValueIsWhitespace()
		{
			Assert.Null(StringNormalizer.NormalizeOptional("   "));
		}

		[Fact]
		public void NormalizeOptional_TrimsValue_WhenNonEmpty()
		{
			var result = StringNormalizer.NormalizeOptional("  hello  ");
			Assert.Equal("hello", result);
		}

		// ------------------------------------------------------------
		// NormalizeCollection
		// ------------------------------------------------------------

		[Fact]
		public void NormalizeCollection_Throws_WhenNull()
		{
			Assert.Throws<ArgumentException>(() => StringNormalizer.NormalizeCollection(null!));
		}

		[Fact]
		public void NormalizeCollection_Throws_WhenAllValuesAreWhitespace()
		{
			var input = new[] { "   ", "\t", "" };
			Assert.Throws<ArgumentException>(() => StringNormalizer.NormalizeCollection(input));
		}

		[Fact]
		public void NormalizeCollection_FiltersOutWhitespaceEntries()
		{
			var input = new[] { "a", " ", "b", "", "c" };
			var result = StringNormalizer.NormalizeCollection(input);

			Assert.Equal(new[] { "a", "b", "c" }, result);
		}

		[Fact]
		public void NormalizeCollection_TrimsAllEntries()
		{
			var input = new[] { "  a  ", "  b", "c  " };
			var result = StringNormalizer.NormalizeCollection(input);

			Assert.Equal(new[] { "a", "b", "c" }, result);
		}

		[Fact]
		public void NormalizeCollection_ReturnsImmutableList()
		{
			var input = new[] { "a", "b" };
			var result = StringNormalizer.NormalizeCollection(input);

			Assert.Throws<NotSupportedException>(() =>
			{
				// Cast to IList<T> to attempt modification
				var list = (IList<string>)result;
				list.Add("c");
			});
		}

		[Fact]
		public void NormalizeCollection_ReturnsDeterministicOutput()
		{
			var input = new List<string> { "  x  ", "y", "  z" };
			var result1 = StringNormalizer.NormalizeCollection(input);
			var result2 = StringNormalizer.NormalizeCollection(input);

			Assert.Equal(result1, result2);
		}
	}
}
