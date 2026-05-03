namespace Sashiko.Validation.Tests
{
	public sealed class ValidationExceptionTests
	{
		[Fact]
		public void Constructor_StoresMessage()
		{
			var exception = new ValidationException("Invalid schema.");

			Assert.Equal("Invalid schema.", exception.Message);
		}

		[Fact]
		public void Constructor_StoresMessageAndInnerException()
		{
			var inner = new FormatException("Malformed value.");
			var exception = new ValidationException("Invalid schema.", inner);

			Assert.Equal("Invalid schema.", exception.Message);
			Assert.Same(inner, exception.InnerException);
		}
	}
}
