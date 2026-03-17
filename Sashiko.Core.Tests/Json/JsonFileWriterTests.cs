using System.Text.Json;
using Sashiko.Core.Json;

namespace Sashiko.Core.Tests.Json
{
	public sealed class JsonFileWriterTests
	{
		private sealed class TestModel
		{
			public string? Name { get; set; }
			public int Value { get; set; }
		}

		// ------------------------------------------------------------
		// Successful writes
		// ------------------------------------------------------------

		[Fact]
		public void Write_ShouldCreateFileWithSerializedContent()
		{
			var temp = Path.GetTempFileName();
			var model = new TestModel { Name = "Alex", Value = 42 };

			JsonFileWriter.Write(temp, model);

			var json = File.ReadAllText(temp);
			var result = JsonSerializer.Deserialize<TestModel>(json);

			Assert.Equal("Alex", result!.Name);
			Assert.Equal(42, result.Value);
		}

		[Fact]
		public void Write_ShouldUseDefaultOptions_WhenNoneProvided()
		{
			var temp = Path.GetTempFileName();
			var model = new TestModel { Name = "Alex", Value = 42 };

			JsonFileWriter.Write(temp, model);

			var json = File.ReadAllText(temp);

			// Default System.Text.Json is compact and case-sensitive
			Assert.Contains("\"Name\":\"Alex\"", json);
			Assert.DoesNotContain("\n", json); // no indentation
		}

		[Fact]
		public void Write_ShouldUseCustomOptions_WhenProvided()
		{
			var temp = Path.GetTempFileName();
			var model = new TestModel { Name = "Alex", Value = 42 };

			var options = new JsonSerializerOptions
			{
				WriteIndented = true
			};

			JsonFileWriter.Write(temp, model, options);

			var json = File.ReadAllText(temp);

			Assert.Contains("\n", json); // indented
		}

		// ------------------------------------------------------------
		// Error handling
		// ------------------------------------------------------------

		[Fact]
		public void Write_ShouldThrow_WhenDirectoryDoesNotExist()
		{
			var path = Path.Combine("Z:\\nonexistent", "file.json");

			Assert.Throws<DirectoryNotFoundException>(() =>
				JsonFileWriter.Write(path, new TestModel()));
		}
	}
}
