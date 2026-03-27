using System.Reflection;
using Sashiko.Validation.Schema.Attributes;

namespace Sashiko.Validation.Schema.Inspectors.CSharp
{
	internal static class RequiredPropertyDetector
	{
		public static bool IsRequired(PropertyInfo prop)
		{
			var type = prop.PropertyType;
			var declaringType = prop.DeclaringType!;

			// ------------------------------------------------------------
			// 1. Property-level overrides
			// ------------------------------------------------------------
			if (prop.IsDefined(typeof(RequiredAttribute), inherit: true))
				return true;

			if (prop.IsDefined(typeof(OptionalAttribute), inherit: true))
				return false;

			// ------------------------------------------------------------
			// 2. Class-level explicit lists
			// ------------------------------------------------------------
			var requiredList = declaringType.GetCustomAttribute<RequiredPropertiesAttribute>();
			if (requiredList?.Properties.Contains(prop.Name) == true)
				return true;

			var optionalList = declaringType.GetCustomAttribute<OptionalPropertiesAttribute>();
			if (optionalList?.Properties.Contains(prop.Name) == true)
				return false;

			// ------------------------------------------------------------
			// 3. Class-level global rules
			// ------------------------------------------------------------
			if (declaringType.IsDefined(typeof(AllRequiredAttribute), inherit: true))
				return true;

			if (declaringType.IsDefined(typeof(AllOptionalAttribute), inherit: true))
				return false;

			// ------------------------------------------------------------
			// 4. Default rules
			// ------------------------------------------------------------

			// Nullable value type → optional
			if (Nullable.GetUnderlyingType(type) != null)
				return false;

			// Collections → optional
			if (CollectionTypeDetector.TryGetElementType(type, out _))
				return false;

			// Reference types → optional
			if (!type.IsValueType)
				return false;

			// Non-nullable value types → required
			return true;
		}
	}
}
