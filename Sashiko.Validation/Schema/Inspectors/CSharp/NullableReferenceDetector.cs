using System.Reflection;

namespace Sashiko.Validation.Schema.Inspectors.CSharp
{
	/// <summary>
	/// Detects whether a property is a nullable reference type (C# 8+).
	/// This relies on compiler-generated metadata:
	/// - NullableAttribute
	/// - NullableContextAttribute
	/// </summary>
	internal static class NullableReferenceDetector
	{
		private const string NullableAttributeFullName =
			"System.Runtime.CompilerServices.NullableAttribute";

		private const string NullableContextAttributeFullName =
			"System.Runtime.CompilerServices.NullableContextAttribute";

		public static bool IsNullableReference(PropertyInfo prop)
		{
			var type = prop.PropertyType;

			// ------------------------------------------------------------
			// Only reference types can be nullable reference types
			// ------------------------------------------------------------
			if (!type.IsClass)
				return false;

			// ------------------------------------------------------------
			// string is implicitly nullable unless metadata says otherwise
			// ------------------------------------------------------------
			if (type == typeof(string))
			{
				if (TryGetNullableFlag(prop, out var flag))
					return flag == 2;

				return true; // default: nullable
			}

			// ------------------------------------------------------------
			// For all other reference types, use metadata
			// ------------------------------------------------------------
			if (TryGetNullableFlag(prop, out var resultFlag))
				return resultFlag == 2;

			return false;
		}

		// ------------------------------------------------------------
		// Metadata extraction helpers
		// ------------------------------------------------------------

		private static bool TryGetNullableFlag(PropertyInfo prop, out byte flag)
		{
			// 1. Property-level [Nullable]
			var attr = prop.CustomAttributes
				.FirstOrDefault(a => a.AttributeType.FullName == NullableAttributeFullName);

			if (attr != null && TryReadFlag(attr, out flag))
				return true;

			// 2. Declaring type [NullableContext]
			var typeContext = prop.DeclaringType?
				.CustomAttributes
				.FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeFullName);

			if (typeContext != null && TryReadContext(typeContext, out flag))
				return true;

			// 3. Assembly-level [NullableContext]
			var asmContext = prop.DeclaringType?.Assembly
				.CustomAttributes
				.FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeFullName);

			if (asmContext != null && TryReadContext(asmContext, out flag))
				return true;

			flag = 0;
			return false;
		}

		private static bool TryReadFlag(CustomAttributeData attr, out byte flag)
		{
			var arg = attr.ConstructorArguments[0].Value;

			if (arg is byte b)
			{
				flag = b;
				return true;
			}

			if (arg is IReadOnlyCollection<CustomAttributeTypedArgument> arrayArgs)
			{
				var first = arrayArgs.FirstOrDefault().Value;
				if (first is byte b2)
				{
					flag = b2;
					return true;
				}
			}

			if (arg is byte[] bytes && bytes.Length > 0)
			{
				flag = bytes[0];
				return true;
			}

			flag = 0;
			return false;
		}

		private static bool TryReadContext(CustomAttributeData attr, out byte flag)
		{
			var arg = attr.ConstructorArguments[0].Value;

			if (arg is byte b)
			{
				flag = b;
				return true;
			}

			flag = 0;
			return false;
		}
	}
}
