using System.Text.Json;
using Sashiko.Core.Json;

namespace Sashiko.Core.Tests.Json
{
	public sealed class JsonFileReaderTests
	{
		private sealed class TestModel
		{
			public string? Name { get; set; }
			public int Value { get; set; }
		}

		// ------------------------------------------------------------
		// Successful deserialization
		// ------------------------------------------------------------

		[Fact]
		public void Deserialize_ShouldDeserializeValidJson()
		{
			var json = """
            { "Name": "Alex", "Value": 42 }
            """;

			var result = JsonFileReader.Deserialize<TestModel>(json, "test.json");

			Assert.Equal("Alex", result.Name);
			Assert.Equal(42, result.Value);
		}

		[Fact]
		public void Deserialize_ShouldUseDefaultOptions_WhenNoneProvided()
		{
			var json = """
            { "name": "Alex", "value": 42 }
            """;

			// Default System.Text.Json is case-sensitive
			var result = JsonFileReader.Deserialize<TestModel>(json, "test.json");

			Assert.Null(result.Name);
			Assert.Equal(0, result.Value);
		}

		[Fact]
		public void Deserialize_ShouldUseCustomOptions_WhenProvided()
		{
			var json = """
            { "NAME": "Alex", "VALUE": 42 }
            """;

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var result = JsonFileReader.Deserialize<TestModel>(json, "test.json", options);

			Assert.Equal("Alex", result.Name);
			Assert.Equal(42, result.Value);
		}

		// ------------------------------------------------------------
		// Error handling
		// ------------------------------------------------------------

		[Fact]
		public void Deserialize_ShouldThrow_WhenJsonIsInvalid()
		{
			var json = "{ invalid json }";

			Assert.Throws<InvalidOperationException>(() =>
				JsonFileReader.Deserialize<TestModel>(json, "test.json"));
		}

		[Fact]
		public void Deserialize_ShouldWrapJsonException()
		{
			var json = "{ invalid json }";

			var ex = Assert.Throws<InvalidOperationException>(() =>
				JsonFileReader.Deserialize<TestModel>(json, "test.json"));

			Assert.IsType<JsonException>(ex.InnerException);
		}

		[Fact]
		public void Deserialize_ShouldThrow_WhenJsonIsNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				JsonFileReader.Deserialize<TestModel>(null!, "test.json"));
		}
	}
}
