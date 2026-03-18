using System.Text.Json;
using Sashiko.Core.Data.Json;
using Xunit;

namespace Sashiko.Core.Tests.Data.Json
{
	public sealed class JsonListLoaderTests
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
		public void Load_ShouldDeserializeValidJsonList()
		{
			var json = """
            [
                { "Name": "Alex", "Value": 10 },
                { "Name": "Mira", "Value": 20 }
            ]
            """;

			var loader = new JsonListLoader<TestModel>();

			var result = loader.Load(json);

			Assert.Equal(2, result.Count);
			Assert.Equal("Alex", result[0].Name);
			Assert.Equal(10, result[0].Value);
		}

		[Fact]
		public void Load_ShouldRespectDefaultCaseSensitiveBehavior()
		{
			var json = """
            [
                { "name": "Alex", "value": 10 }
            ]
            """;

			var loader = new JsonListLoader<TestModel>();

			var result = loader.Load(json);

			Assert.Single(result);

			// Because default System.Text.Json is case-sensitive:
			Assert.Null(result[0].Name);
			Assert.Equal(0, result[0].Value);
		}

		[Fact]
		public void Load_ShouldUseCustomOptions_WhenProvided()
		{
			var json = """
            [
                { "NAME": "Alex", "VALUE": 10 }
            ]
            """;

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var loader = new JsonListLoader<TestModel>(options);

			var result = loader.Load(json);

			Assert.Single(result);
			Assert.Equal("Alex", result[0].Name);
			Assert.Equal(10, result[0].Value);
		}

		// ------------------------------------------------------------
		// Error handling
		// ------------------------------------------------------------

		[Fact]
		public void Load_ShouldThrow_WhenJsonIsInvalid()
		{
			var json = "{ invalid json }";
			var loader = new JsonListLoader<TestModel>();

			Assert.Throws<JsonException>(() => loader.Load(json));
		}

		[Fact]
		public void Load_ShouldThrow_WhenJsonIsNotAList()
		{
			var json = """ { "Name": "Alex" } """;
			var loader = new JsonListLoader<TestModel>();

			Assert.Throws<JsonException>(() => loader.Load(json));
		}

		[Fact]
		public void Load_ShouldThrow_WhenJsonIsNull()
		{
			var loader = new JsonListLoader<TestModel>();

			Assert.Throws<ArgumentNullException>(() => loader.Load(null!));
		}
	}
}
