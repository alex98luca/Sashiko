using Sashiko.Validation.Schema.Inspectors.CSharp;

namespace Sashiko.Validation.Tests.Schema.Inspectors.CSharp
{
	public sealed class NullableReferenceDetectorTests
	{
		// ------------------------------------------------------------
		// Test classes compiled with nullable reference types enabled
		// ------------------------------------------------------------

#nullable enable
		private sealed class NullableReferenceTestClass
		{
			public string? NullableString { get; set; } = null;
			public string NonNullableString { get; set; } = "";
			public object? NullableObject { get; set; }
			public object NonNullableObject { get; set; } = new();
		}
#nullable restore

		private sealed class ValueTypeTestClass
		{
			public int Value { get; set; }
			public int? NullableValue { get; set; }
		}

#nullable disable
		private sealed class LegacyReferenceTestClass
		{
			public string Text { get; set; } = "";
			public object Value { get; set; } = new();
		}
#nullable restore

		// ------------------------------------------------------------
		// Nullable reference types
		// ------------------------------------------------------------

		[Fact]
		public void IsNullableReference_WhenPropertyIsNullableString_ReturnsTrue()
		{
			// Arrange
			var prop = typeof(NullableReferenceTestClass).GetProperty(nameof(NullableReferenceTestClass.NullableString))!;

			// Act
			var result = NullableReferenceDetector.IsNullableReference(prop);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsNullableReference_WhenPropertyIsNullableObject_ReturnsTrue()
		{
			var prop = typeof(NullableReferenceTestClass).GetProperty(nameof(NullableReferenceTestClass.NullableObject))!;
			Assert.True(NullableReferenceDetector.IsNullableReference(prop));
		}

		// ------------------------------------------------------------
		// Non-nullable reference types
		// ------------------------------------------------------------

		[Fact]
		public void IsNullableReference_WhenPropertyIsNonNullableString_ReturnsFalse()
		{
			var prop = typeof(NullableReferenceTestClass).GetProperty(nameof(NullableReferenceTestClass.NonNullableString))!;
			Assert.False(NullableReferenceDetector.IsNullableReference(prop));
		}

		[Fact]
		public void IsNullableReference_WhenPropertyIsNonNullableObject_ReturnsFalse()
		{
			var prop = typeof(NullableReferenceTestClass).GetProperty(nameof(NullableReferenceTestClass.NonNullableObject))!;
			Assert.False(NullableReferenceDetector.IsNullableReference(prop));
		}

		// ------------------------------------------------------------
		// Value types (never nullable reference types)
		// ------------------------------------------------------------

		[Fact]
		public void IsNullableReference_WhenPropertyIsValueType_ReturnsFalse()
		{
			var prop = typeof(ValueTypeTestClass).GetProperty(nameof(ValueTypeTestClass.Value))!;
			Assert.False(NullableReferenceDetector.IsNullableReference(prop));
		}

		[Fact]
		public void IsNullableReference_WhenPropertyIsNullableValueType_ReturnsFalse()
		{
			var prop = typeof(ValueTypeTestClass).GetProperty(nameof(ValueTypeTestClass.NullableValue))!;
			Assert.False(NullableReferenceDetector.IsNullableReference(prop));
		}

		// ------------------------------------------------------------
		// String special-case behavior
		// ------------------------------------------------------------

#nullable enable
		private sealed class StringSpecialCaseTestClass
		{
			public string? NullableString { get; set; }
			public string NonNullableString { get; set; } = "";
		}
#nullable restore

		[Fact]
		public void IsNullableReference_WhenStringIsNullable_ReturnsTrue()
		{
			var prop = typeof(StringSpecialCaseTestClass).GetProperty(nameof(StringSpecialCaseTestClass.NullableString))!;
			Assert.True(NullableReferenceDetector.IsNullableReference(prop));
		}

		[Fact]
		public void IsNullableReference_WhenStringIsNonNullable_ReturnsFalse()
		{
			var prop = typeof(StringSpecialCaseTestClass).GetProperty(nameof(StringSpecialCaseTestClass.NonNullableString))!;
			Assert.False(NullableReferenceDetector.IsNullableReference(prop));
		}

		[Fact]
		public void IsNullableReference_WhenStringHasNoNullableMetadata_ReturnsTrue()
		{
			var prop = typeof(LegacyReferenceTestClass).GetProperty(nameof(LegacyReferenceTestClass.Text))!;

			Assert.True(NullableReferenceDetector.IsNullableReference(prop));
		}

		[Fact]
		public void IsNullableReference_WhenObjectHasNoNullableMetadata_ReturnsFalse()
		{
			var prop = typeof(LegacyReferenceTestClass).GetProperty(nameof(LegacyReferenceTestClass.Value))!;

			Assert.False(NullableReferenceDetector.IsNullableReference(prop));
		}
	}
}
