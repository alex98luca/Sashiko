using System.Reflection;

namespace Sashiko.Validation.Schema.Inspectors.CSharp
{
	internal static class RequiredPropertyDetector
	{
		public static bool IsRequired(PropertyInfo prop)
		{
			var type = prop.PropertyType;

			// ------------------------------------------------------------
			// Nullable value type → optional
			// ------------------------------------------------------------
			if (Nullable.GetUnderlyingType(type) != null)
				return false;

			// ------------------------------------------------------------
			// Nullable reference type → optional
			// ------------------------------------------------------------
			if (NullableReferenceDetector.IsNullableReference(prop))
				return false;

			// ------------------------------------------------------------
			// Collections → optional by default
			// ------------------------------------------------------------
			if (CollectionTypeDetector.TryGetElementType(type, out _))
				return false;

			// ------------------------------------------------------------
			// Otherwise → required
			// ------------------------------------------------------------
			return true;
		}
	}
}
