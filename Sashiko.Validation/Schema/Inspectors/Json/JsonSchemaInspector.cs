using System.Text.Json;
using Sashiko.Validation.Schema.Path;

namespace Sashiko.Validation.Schema.Inspectors.Json
{
	/// <summary>
	/// Extracts a flattened schema representation from a JSON element.
	/// Produces paths like "parent.child.name" for nested objects.
	/// Handles arrays, objects, and primitives consistently.
	/// </summary>
	public static class JsonSchemaInspector
	{
		public static HashSet<string> GetSchema(JsonElement element)
		{
			var fields = new HashSet<string>(StringComparer.Ordinal);
			var visited = new HashSet<JsonElement>(); // recursion guard
			InspectElement(element, prefix: "", fields, visited);
			return fields;
		}

		// ------------------------------------------------------------
		// Core dispatcher
		// ------------------------------------------------------------
		private static void InspectElement(
			JsonElement element,
			string prefix,
			HashSet<string> fields,
			HashSet<JsonElement> visited)
		{
			// Prevent infinite recursion (rare but possible)
			if (visited.Contains(element))
				return;

			visited.Add(element);

			switch (element.ValueKind)
			{
				case JsonValueKind.Object:
					InspectObject(element, prefix, fields, visited);
					break;

				case JsonValueKind.Array:
					InspectArray(element, prefix, fields, visited);
					break;

				default:
					// Primitive → leaf
					if (!string.IsNullOrEmpty(prefix))
						fields.Add(prefix);
					break;
			}
		}

		// ------------------------------------------------------------
		// Object inspector
		// ------------------------------------------------------------
		private static void InspectObject(
			JsonElement element,
			string prefix,
			HashSet<string> fields,
			HashSet<JsonElement> visited)
		{
			foreach (var prop in element.EnumerateObject())
			{
				var path = PropertyPathBuilder.Combine(prefix, prop.Name);
				InspectElement(prop.Value, path, fields, visited);
			}
		}

		// ------------------------------------------------------------
		// Array inspector
		// ------------------------------------------------------------
		private static void InspectArray(
			JsonElement element,
			string prefix,
			HashSet<string> fields,
			HashSet<JsonElement> visited)
		{
			bool containsComplex = false;

			foreach (var item in element.EnumerateArray())
			{
				if (item.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
				{
					containsComplex = true;
					InspectElement(item, prefix, fields, visited);
				}
			}

			// Array of primitives → treat as leaf
			if (!containsComplex && !string.IsNullOrEmpty(prefix))
				fields.Add(prefix);
		}
	}
}
