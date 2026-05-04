using Sashiko.Languages.Model;
using Sashiko.Languages.Registry;
using Sashiko.Validation;
using Sashiko.Validation.Validators.Json;

namespace Sashiko.Languages.Tests.Data
{
	public sealed class EmbeddedDataTests
	{
		private const string ResourceName =
			"Sashiko.Languages.Data.Languages.languages.json";

		private readonly JsonSchemaValidator _validator = new();

		private static string LoadEmbeddedJson()
		{
			var asm = typeof(LanguageRegistry).Assembly;

			using var stream = asm.GetManifestResourceStream(ResourceName)
				?? throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found.");

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		// ------------------------------------------------------------
		// 1. Basic JSON Validity
		// ------------------------------------------------------------

		[Fact]
		public void EmbeddedJson_ShouldBeValidUtf8()
		{
			var json = LoadEmbeddedJson();
			Assert.False(string.IsNullOrWhiteSpace(json));

			var doc = System.Text.Json.JsonDocument.Parse(json);
			Assert.Equal(System.Text.Json.JsonValueKind.Array, doc.RootElement.ValueKind);
		}

		// ------------------------------------------------------------
		// 2. Schema Validation
		// ------------------------------------------------------------

		[Fact]
		public void EmbeddedJson_ShouldMatchLanguageSchema()
		{
			var json = LoadEmbeddedJson();

			var context = new ValidationContext
			{
				Source = ResourceName,
				IgnoreCase = false
			};

			_validator.Validate<Language[]>(json, context);
		}

		// ------------------------------------------------------------
		// 3. Registry ShouldLoadSuccessfully
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldLoadEmbeddedData()
		{
			var registry = new LanguageRegistry();
			Assert.NotEmpty(registry.Languages);
		}
	}
}
