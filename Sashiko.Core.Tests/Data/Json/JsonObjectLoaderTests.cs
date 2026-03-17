using System.Text.Json;
using Sashiko.Core.Data.Json;

namespace Sashiko.Core.Tests.Data.Json
{
	public sealed class JsonObjectLoaderTests
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
		public void Load_ShouldDeserializeValidJsonObject()
		{
			var json = """
            {
                "Name": "Alex",
                "Value": 42
            }
            """;

			var loader = new JsonObjectLoader<TestModel>();

			var result = loader.Load(json);

			Assert.Equal("Alex", result.Name);
			Assert.Equal(42, result.Value);
		}

		[Fact]
		public void Load_ShouldRespectDefaultCaseSensitiveBehavior()
		{
			var json = """
            {
                "name": "Alex",
                "value": 42
            }
            """;

			var loader = new JsonObjectLoader<TestModel>();

			var result = loader.Load(json);

			// Default System.Text.Json is case-sensitive:
			Assert.Null(result.Name);
			Assert.Equal(0, result.Value);
		}

		[Fact]
		public void Load_ShouldUseCustomOptions_WhenProvided()
		{
			var json = """
            {
                "NAME": "Alex",
                "VALUE": 42
            }
            """;

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var loader = new JsonObjectLoader<TestModel>(options);

			var result = loader.Load(json);

			Assert.Equal("Alex", result.Name);
			Assert.Equal(42, result.Value);
		}

		// ------------------------------------------------------------
		// Error handling
		// ------------------------------------------------------------

		[Fact]
		public void Load_ShouldThrow_WhenJsonIsInvalid()
		{
			var json = "{ invalid json }";
			var loader = new JsonObjectLoader<TestModel>();

			Assert.Throws<JsonException>(() => loader.Load(json));
		}

		[Fact]
		public void Load_ShouldThrow_WhenJsonIsArrayInsteadOfObject()
		{
			var json = """
            [
                { "Name": "Alex" }
            ]
            """;

			var loader = new JsonObjectLoader<TestModel>();

			Assert.Throws<JsonException>(() => loader.Load(json));
		}

		[Fact]
		public void Load_ShouldThrow_WhenJsonIsNull()
		{
			var loader = new JsonObjectLoader<TestModel>();

			Assert.Throws<ArgumentNullException>(() => loader.Load(null!));
		}
	}
}
