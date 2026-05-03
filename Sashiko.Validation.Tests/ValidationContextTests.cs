namespace Sashiko.Validation.Tests
{
	public sealed class ValidationContextTests
	{
		[Fact]
		public void Constructor_UsesNeutralDefaults()
		{
			var context = new ValidationContext();

			Assert.Null(context.Source);
			Assert.False(context.IgnoreCase);
			Assert.Null(context.Metadata);
		}

		[Fact]
		public void InitProperties_StoreProvidedValues()
		{
			var metadata = new Dictionary<string, object>
			{
				["request-id"] = "abc-123"
			};

			var context = new ValidationContext
			{
				Source = "schema.json",
				IgnoreCase = true,
				Metadata = metadata
			};

			Assert.Equal("schema.json", context.Source);
			Assert.True(context.IgnoreCase);
			Assert.Same(metadata, context.Metadata);
		}
	}
}
