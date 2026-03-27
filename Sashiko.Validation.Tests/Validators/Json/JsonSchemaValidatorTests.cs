using System.Text.Json;
using Sashiko.Validation.Validators.Json;

namespace Sashiko.Validation.Tests.Validators.Json
{
	public sealed class JsonSchemaValidatorTests
	{
		private readonly JsonSchemaValidator _validator = new();

#nullable enable
		private sealed class SimpleModel
		{
			public int Id { get; set; }
			public string Name { get; set; } = "";
			public string? OptionalDescription { get; set; }
		}

		private sealed class NestedModel
		{
			public SimpleModel Info { get; set; } = new();
			public int Count { get; set; }
		}
#nullable restore

		private static JsonElement Parse(string json)
			=> JsonDocument.Parse(json).RootElement;

		// ------------------------------------------------------------
		// Matching schema
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenSchemaMatches_DoesNotThrow()
		{
			var json = """{ "Id": 1 }""";
			var element = Parse(json);

			_validator.Validate<SimpleModel>(element);
		}

		// ------------------------------------------------------------
		// Missing required fields
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenRequiredFieldMissing_Throws()
		{
			var json = """{ "Name": "abc" }""";
			var element = Parse(json);

			var ex = Assert.Throws<ValidationException>(() =>
				_validator.Validate<SimpleModel>(element));

			Assert.Contains("Missing required fields", ex.Message);
			Assert.Contains("Id", ex.Message);
		}

		// ------------------------------------------------------------
		// Optional fields do not cause errors
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenOptionalFieldMissing_DoesNotThrow()
		{
			var json = """{ "Id": 1 }""";
			var element = Parse(json);

			_validator.Validate<SimpleModel>(element);
		}

		// ------------------------------------------------------------
		// Extra fields
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenExtraFieldsPresent_Throws()
		{
			var json = """{ "Id": 1, "Extra": 123 }""";
			var element = Parse(json);

			var ex = Assert.Throws<ValidationException>(() =>
				_validator.Validate<SimpleModel>(element));

			Assert.Contains("Unexpected fields", ex.Message);
			Assert.Contains("Extra", ex.Message);
		}

		// ------------------------------------------------------------
		// Case sensitivity
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenIgnoreCaseIsFalse_RequiresExactCase()
		{
			var json = """{ "id": 1 }""";
			var element = Parse(json);

			var context = new ValidationContext { IgnoreCase = false };

			var ex = Assert.Throws<ValidationException>(() =>
				_validator.Validate<SimpleModel>(element, context));

			Assert.Contains("Id", ex.Message);
			Assert.DoesNotContain("Name", ex.Message); // Name is optional
		}

		[Fact]
		public void Validate_WhenIgnoreCaseIsTrue_IgnoresCase()
		{
			var json = """{ "id": 1 }""";
			var element = Parse(json);

			var context = new ValidationContext { IgnoreCase = true };

			_validator.Validate<SimpleModel>(element, context);
		}

		// ------------------------------------------------------------
		// Nested objects
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenNestedObjectMissingRequiredField_Throws()
		{
			var json = """{ "Info": { "Name": "abc" }, "Count": 5 }""";
			var element = Parse(json);

			var ex = Assert.Throws<ValidationException>(() =>
				_validator.Validate<NestedModel>(element));

			Assert.Contains("Info.Id", ex.Message);
		}

		// ------------------------------------------------------------
		// Arrays
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenArrayElementsMatchSchema_DoesNotThrow()
		{
			var json = """[ { "Id": 1 }, { "Id": 2 } ]""";
			var element = Parse(json);

			_validator.Validate<List<SimpleModel>>(element);
		}

		[Fact]
		public void Validate_WhenArrayElementMissingRequiredField_Throws()
		{
			var json = """[ { "Id": 1 }, { "Name": "abc" } ]""";
			var element = Parse(json);

			var ex = Assert.Throws<ValidationException>(() =>
				_validator.Validate<List<SimpleModel>>(element));

			Assert.Contains("index 1", ex.Message);
			Assert.Contains("Id", ex.Message);
		}

		// ------------------------------------------------------------
		// Invalid input type
		// ------------------------------------------------------------

		[Fact]
		public void Validate_WhenInputIsInvalidType_Throws()
		{
			var ex = Assert.Throws<ValidationException>(() =>
				_validator.Validate<SimpleModel>(12345));

			Assert.Contains("Unsupported input type", ex.Message);
		}
	}
}
