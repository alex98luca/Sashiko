using System.Text.Json;
using Sashiko.Validation.Schema.Comparison;
using Sashiko.Validation.Schema.Inspectors.CSharp;
using Sashiko.Validation.Schema.Inspectors.Json;
using Sashiko.Validation.Schema.Model;

namespace Sashiko.Validation.Validators.Json
{
	public sealed class JsonSchemaValidator : ISchemaValidator
	{
		public void Validate<T>(object input, ValidationContext? context = null)
		{
			context ??= new ValidationContext();

			var root = ToJsonElement(input);
			var expected = CSharpSchemaInspector.GetSchema(typeof(T));

			if (expected.Kind == SchemaNodeKind.Array)
			{
				ValidateArray(expected, root, context);
				return;
			}

			var actual = JsonSchemaInspector.GetSchema(root);
			var diff = SchemaComparer.Compare(expected, actual, context.IgnoreCase);

			if (!diff.IsMatch)
				ThrowSchemaError(context.Source, diff);
		}

		private void ValidateArray(SchemaNode expected, JsonElement root, ValidationContext context)
		{
			if (root.ValueKind != JsonValueKind.Array)
				throw new ValidationException("Expected JSON array.");

			int index = 0;
			foreach (var item in root.EnumerateArray())
			{
				var actualElement = JsonSchemaInspector.GetSchema(item);
				var diff = SchemaComparer.Compare(expected.Element!, actualElement, context.IgnoreCase);

				if (!diff.IsMatch)
					ThrowSchemaErrorForArrayElement(context.Source, diff, index);

				index++;
			}
		}

		private static JsonElement ToJsonElement(object input)
		{
			return input switch
			{
				JsonElement el => el,
				string json => JsonDocument.Parse(json).RootElement,
				_ => throw new ValidationException(
					$"Unsupported input type '{input.GetType().FullName}' for JsonSchemaValidator. " +
					"Expected string or JsonElement.")
			};
		}

		private static void ThrowSchemaError(string? source, SchemaDiff diff)
		{
			var location = string.IsNullOrEmpty(source) ? "input" : $"'{source}'";

			var message =
				$"Schema mismatch in {location}.\n" +
				(diff.Missing.Count > 0
					? $"Missing required fields:\n  {string.Join("\n  ", diff.Missing)}\n"
					: "") +
				(diff.Extra.Count > 0
					? $"Unexpected fields:\n  {string.Join("\n  ", diff.Extra)}\n"
					: "");

			throw new ValidationException(message);
		}

		private static void ThrowSchemaErrorForArrayElement(string? source, SchemaDiff diff, int index)
		{
			var location = string.IsNullOrEmpty(source) ? "input" : $"'{source}'";

			var message =
				$"Schema mismatch in {location} for array element at index {index}.\n" +
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
