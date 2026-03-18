using System.Reflection;
using Sashiko.Validation.Schema.Comparison;
using Sashiko.Validation.Schema.Path;

namespace Sashiko.Validation.Schema.Inspectors.CSharp
{
	/// <summary>
	/// Extracts a flattened schema representation from a C# type.
	/// Produces paths like "Parent.Child.Name" for nested properties.
	/// Handles collections, nullable types, and recursion safely.
	/// </summary>
	public static class CSharpSchemaInspector
	{
		public static Dictionary<string, SchemaField> GetSchema(Type type)
		{
			var visited = new HashSet<Type>();
			return GetFields(type, prefix: "", visited);
		}

		private static Dictionary<string, SchemaField> GetFields(
			Type type,
			string prefix,
			HashSet<Type> visited)
		{
			var fields = new Dictionary<string, SchemaField>(StringComparer.Ordinal);

			// ------------------------------------------------------------
			// Prevent infinite recursion on self-referencing types
			// ------------------------------------------------------------
			if (visited.Contains(type))
				return fields;

			visited.Add(type);

			// ------------------------------------------------------------
			// Collections → inspect element type instead
			// ------------------------------------------------------------
			if (CollectionTypeDetector.TryGetElementType(type, out var elementType))
				return GetFields(elementType, prefix, visited);

			// ------------------------------------------------------------
			// Inspect public instance properties
			// ------------------------------------------------------------
			foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				// Skip indexers
				if (prop.GetIndexParameters().Length > 0)
					continue;

				// Skip unreadable properties
				if (prop.GetMethod == null)
					continue;

				var propertyType = prop.PropertyType;
				var name = PropertyPathBuilder.Combine(prefix, prop.Name);
				bool isRequired = RequiredPropertyDetector.IsRequired(prop);

				// --------------------------------------------------------
				// Leaf property → add directly
				// --------------------------------------------------------
				if (LeafTypeDetector.IsLeaf(propertyType))
				{
					fields[name] = new SchemaField(name, isRequired);
					continue;
				}

				// --------------------------------------------------------
				// Complex property → recurse
				// --------------------------------------------------------
				var nestedPrefix = PropertyPathBuilder.Combine(prefix, prop.Name);
				foreach (var nested in GetFields(propertyType, nestedPrefix, visited))
					fields[nested.Key] = nested.Value;
			}

			return fields;
		}
	}
}
