namespace Sashiko.Registries.Tests
{
	public sealed class RegistryLoadExceptionTests
	{
		[Fact]
		public void Constructor_StoresRegistryContext()
		{
			var exception = new RegistryLoadException(
				"Registry load failed.",
				"languages.json",
				typeof(TestRegistry));

			Assert.Equal("Registry load failed.", exception.Message);
			Assert.Equal("languages.json", exception.SourceName);
			Assert.Equal(typeof(TestRegistry), exception.RegistryType);
		}

		[Fact]
		public void Constructor_StoresInnerException()
		{
			var inner = new InvalidOperationException("Invalid registry.");
			var exception = new RegistryLoadException(
				"Registry load failed.",
				"languages.json",
				typeof(TestRegistry),
				inner);

			Assert.Same(inner, exception.InnerException);
		}

		private sealed class TestRegistry
		{
		}
	}
}
