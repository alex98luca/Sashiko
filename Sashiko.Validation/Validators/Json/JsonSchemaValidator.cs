using System.Text.Json;
using Sashiko.Validation.Schema.Comparison;
using Sashiko.Validation.Schema.Inspectors.CSharp;
using Sashiko.Validation.Schema.Inspectors.Json;

namespace Sashiko.Validation.Validators.Json
{
	/// <summary>
	/// Validates that JSON input matches the schema of T.
	/// Does NOT validate JSON syntax or deserialization.
	/// </summary>
	public sealed class JsonSchemaValidator : ISchemaValidator
	{
		public void Validate<T>(object input, ValidationContext? context = null)
		{
			context ??= new ValidationContext();

			var root = ToJsonElement(input);
			var expected = CSharpSchemaInspector.GetSchema(typeof(T));
			var ignoreCase = context.IgnoreCase;

			// ------------------------------------------------------------
			// Handle collections
			// ------------------------------------------------------------
			if (CollectionTypeDetector.TryGetElementType(typeof(T), out var elementType) &&
				root.ValueKind == JsonValueKind.Array)
			{
				var expectedElement = CSharpSchemaInspector.GetSchema(elementType);

				int index = 0;
				foreach (var item in root.EnumerateArray())
				{
					var actualElement = JsonSchemaInspector.GetSchema(item);
					var diff = SchemaComparer.Compare(expectedElement, actualElement, ignoreCase);

					if (!diff.IsMatch)
						ThrowSchemaError(context.Source, diff, index);

					index++;
				}

				return;
			}

			// ------------------------------------------------------------
			// Handle single object
			// ------------------------------------------------------------
			var actual = JsonSchemaInspector.GetSchema(root);
			var diffSingle = SchemaComparer.Compare(expected, actual, ignoreCase);

			if (!diffSingle.IsMatch)
				ThrowSchemaError(context.Source, diffSingle);
		}

		// ------------------------------------------------------------
		// Input normalization
		// ------------------------------------------------------------
		private static JsonElement ToJsonElement(object input)
		{
			switch (input)
			{
				case JsonElement element:
					return element;

				case string json:
					// Parse once and keep the document alive
					var doc = JsonDocument.Parse(json);
					return doc.RootElement;

				default:
					throw new ValidationException(
						$"Unsupported input type '{input.GetType().FullName}' for JsonSchemaValidator. " +
						"Expected string or JsonElement.");
			}
		}

		// ------------------------------------------------------------
		// Error formatting
		// ------------------------------------------------------------
		private static void ThrowSchemaError(string? source, SchemaDiff diff, int? index = null)
		{
			var location = string.IsNullOrEmpty(source) ? "input" : $"'{source}'";

			var prefix = index is null
				? $"Schema mismatch in {location}."
				: $"Schema mismatch in {location} for array element at index {index}.";

			var message =
				prefix + "\n" +
				(diff.Missing.Count > 0
					? $"Missing required fields:\n  {string.Join("\n  ", diff.Missing)}\n"
					: "") +
				(diff.Extra.Count > 0
					? $"Unexpected fields:\n  {string.Join("\n  ", diff.Extra)}\n"
					: "");

			throw new ValidationException(message);
		}
	}
}
