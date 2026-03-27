namespace Sashiko.Validation.Schema.Inspectors.CSharp
{
	/// <summary>
	/// Determines whether a type should be treated as a leaf node
	/// in schema inspection. Leaf nodes correspond to JSON primitives:
	/// string, number, boolean, null, and simple value types.
	/// </summary>
	internal static class LeafTypeDetector
	{
		public static bool IsLeaf(Type type)
		{
			// ------------------------------------------------------------
			// Nullable<T> → leaf if underlying type is leaf
			// ------------------------------------------------------------
			var underlying = Nullable.GetUnderlyingType(type);
			if (underlying != null)
				return IsLeaf(underlying);

			// ------------------------------------------------------------
			// Primitive CLR types
			// ------------------------------------------------------------
			if (type.IsPrimitive || type.IsEnum)
				return true;

			// ------------------------------------------------------------
			// Common JSON-compatible leaf types
			// ------------------------------------------------------------
			if (type == typeof(string) ||
				type == typeof(decimal) ||
				type == typeof(DateTime) ||
				type == typeof(Guid) ||
				type == typeof(TimeSpan))
				return true;

			// ------------------------------------------------------------
			// Numeric types not covered by IsPrimitive
			// (decimal, double, float, etc.)
			// ------------------------------------------------------------
			if (type == typeof(double) ||
				type == typeof(float) ||
				type == typeof(bool))
				return true;

			// ------------------------------------------------------------
			// Collections are NOT leaf types
			// (string is already handled above)
			// ------------------------------------------------------------
			if (typeof(IEnumerable<>).IsAssignableFrom(type))
				return false;

			// ------------------------------------------------------------
			// Default: not a leaf
			// ------------------------------------------------------------
			return false;
		}
	}
}
