using Sashiko.Validation.Schema.Path;

namespace Sashiko.Validation.Tests.Schema.Path
{
	public sealed class PropertyPathBuilderTests
	{
		[Fact]
		public void Combine_WhenPrefixIsEmpty_ReturnsName()
		{
			// Arrange
			string prefix = "";
			string name = "Child";

			// Act
			var result = PropertyPathBuilder.Combine(prefix, name);

			// Assert
			Assert.Equal("Child", result);
		}

		[Fact]
		public void Combine_WhenPrefixIsNull_ReturnsName()
		{
			// Arrange
			string? prefix = null;
			string name = "Child";

			// Act
			var result = PropertyPathBuilder.Combine(prefix!, name);

			// Assert
			Assert.Equal("Child", result);
		}

		[Fact]
		public void Combine_WhenPrefixIsNonEmpty_ReturnsJoinedPath()
		{
			// Arrange
			string prefix = "Parent";
			string name = "Child";

			// Act
			var result = PropertyPathBuilder.Combine(prefix, name);

			// Assert
			Assert.Equal("Parent.Child", result);
		}

		[Fact]
		public void Combine_WhenPrefixAlreadyContainsDot_StillAppendsCorrectly()
		{
			// Arrange
			string prefix = "Parent.Sub";
			string name = "Child";

			// Act
			var result = PropertyPathBuilder.Combine(prefix, name);

			// Assert
			Assert.Equal("Parent.Sub.Child", result);
		}

		[Fact]
		public void Combine_WhenNameIsEmpty_ReturnsPrefixDot()
		{
			// Arrange
			string prefix = "Parent";
			string name = "";

			// Act
			var result = PropertyPathBuilder.Combine(prefix, name);

			// Assert
			Assert.Equal("Parent.", result);
		}
	}
}
