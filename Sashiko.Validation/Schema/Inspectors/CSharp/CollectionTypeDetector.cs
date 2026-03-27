namespace Sashiko.Validation.Schema.Inspectors.CSharp
{
	internal static class CollectionTypeDetector
	{
		/// <summary>
		/// Determines whether the given type represents a collection
		/// and extracts its element type if so.
		/// </summary>
		public static bool TryGetElementType(Type type, out Type elementType)
		{
			elementType = null!;

			// ------------------------------------------------------------
			// Exclude string (it implements IEnumerable<char> but is not a collection)
			// ------------------------------------------------------------
			if (type == typeof(string))
				return false;

			// ------------------------------------------------------------
			// Arrays
			// ------------------------------------------------------------
			if (type.IsArray)
			{
				elementType = type.GetElementType()!;
				return true;
			}

			// ------------------------------------------------------------
			// Direct IEnumerable<T> implementation
			// ------------------------------------------------------------
			if (type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				elementType = type.GetGenericArguments()[0];
				return true;
			}

			// ------------------------------------------------------------
			// Any implemented IEnumerable<T> interface
			// (covers List<T>, HashSet<T>, ICollection<T>, IReadOnlyList<T>, etc.)
			// ------------------------------------------------------------
			var iface = type.GetInterfaces()
				.FirstOrDefault(i =>
					i.IsGenericType &&
					i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

			if (iface != null)
			{
				elementType = iface.GetGenericArguments()[0];
				return true;
			}

			return false;
		}
	}
}
