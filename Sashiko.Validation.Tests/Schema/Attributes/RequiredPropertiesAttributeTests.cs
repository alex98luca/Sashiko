using Sashiko.Validation.Schema.Attributes;

namespace Sashiko.Validation.Tests.Schema.Attributes
{
	public sealed class RequiredPropertiesAttributeTests
	{
		[Fact]
		public void Constructor_StoresProvidedPropertyNames()
		{
			var attr = new RequiredPropertiesAttribute("A", "B", "C");

			Assert.Equal(new[] { "A", "B", "C" }, attr.Properties);
		}

		[Fact]
		public void Properties_AreReadOnly()
		{
			var attr = new RequiredPropertiesAttribute("A");

			Assert.IsAssignableFrom<IReadOnlyList<string>>(attr.Properties);
		}

		[Fact]
		public void Constructor_AllowsEmptyList()
		{
			var attr = new RequiredPropertiesAttribute();

			Assert.Empty(attr.Properties);
		}

		[Fact]
		public void Constructor_PreservesOrder()
		{
			var attr = new RequiredPropertiesAttribute("X", "Y", "Z");

			Assert.Equal("X", attr.Properties[0]);
			Assert.Equal("Y", attr.Properties[1]);
			Assert.Equal("Z", attr.Properties[2]);
		}

		[Fact]
		public void Constructor_AllowsDuplicateNames()
		{
			var attr = new RequiredPropertiesAttribute("A", "A");

			Assert.Equal(new[] { "A", "A" }, attr.Properties);
		}
	}
}
