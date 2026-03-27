using Sashiko.Validation.Schema.Attributes;
using Sashiko.Validation.Schema.Inspectors.CSharp;

namespace Sashiko.Validation.Tests.Schema.Inspectors.CSharp
{
	public sealed class RequiredPropertyDetectorTests
	{
		// ------------------------------------------------------------
		// A. Property-level attributes
		// ------------------------------------------------------------

		private sealed class PropertyLevelTestClass
		{
			[Required]
			public string RequiredProp { get; set; } = "";

			[Optional]
			public string OptionalProp { get; set; } = "";

			public string DefaultRef { get; set; } = "";

			public int DefaultValue { get; set; }
		}

		[Fact]
		public void RequiredAttribute_OverridesEverything()
		{
			var prop = typeof(PropertyLevelTestClass).GetProperty(nameof(PropertyLevelTestClass.RequiredProp))!;
			Assert.True(RequiredPropertyDetector.IsRequired(prop));
		}

		[Fact]
		public void OptionalAttribute_OverridesEverything()
		{
			var prop = typeof(PropertyLevelTestClass).GetProperty(nameof(PropertyLevelTestClass.OptionalProp))!;
			Assert.False(RequiredPropertyDetector.IsRequired(prop));
		}

		// ------------------------------------------------------------
		// B. Class-level explicit lists
		// ------------------------------------------------------------

		[RequiredProperties("A")]
		private sealed class RequiredListTestClass
		{
			public string A { get; set; } = "";
			public string B { get; set; } = "";
		}

		[Fact]
		public void RequiredPropertiesAttribute_MarksListedPropertiesAsRequired()
		{
			var a = typeof(RequiredListTestClass).GetProperty("A")!;
			var b = typeof(RequiredListTestClass).GetProperty("B")!;

			Assert.True(RequiredPropertyDetector.IsRequired(a));
			Assert.False(RequiredPropertyDetector.IsRequired(b));
		}

		[OptionalProperties("B")]
		private sealed class OptionalListTestClass
		{
			public string A { get; set; } = "";
			public string B { get; set; } = "";
		}

		[Fact]
		public void OptionalPropertiesAttribute_MarksListedPropertiesAsOptional()
		{
			var a = typeof(OptionalListTestClass).GetProperty("A")!;
			var b = typeof(OptionalListTestClass).GetProperty("B")!;

			Assert.False(RequiredPropertyDetector.IsRequired(b));
			Assert.False(RequiredPropertyDetector.IsRequired(a)); // default ref type
		}

		// ------------------------------------------------------------
		// C. Class-level global rules
		// ------------------------------------------------------------

		[AllRequired]
		private sealed class AllRequiredTestClass
		{
			public string A { get; set; } = "";
			[Optional] public string B { get; set; } = "";
		}

		[Fact]
		public void AllRequired_MakesEverythingRequired_UnlessOptional()
		{
			var a = typeof(AllRequiredTestClass).GetProperty("A")!;
			var b = typeof(AllRequiredTestClass).GetProperty("B")!;

			Assert.True(RequiredPropertyDetector.IsRequired(a));
			Assert.False(RequiredPropertyDetector.IsRequired(b));
		}

		[AllOptional]
		private sealed class AllOptionalTestClass
		{
			[Required] public string A { get; set; } = "";
			public string B { get; set; } = "";
		}

		[Fact]
		public void AllOptional_MakesEverythingOptional_UnlessRequired()
		{
			var a = typeof(AllOptionalTestClass).GetProperty("A")!;
			var b = typeof(AllOptionalTestClass).GetProperty("B")!;

			Assert.True(RequiredPropertyDetector.IsRequired(a));
			Assert.False(RequiredPropertyDetector.IsRequired(b));
		}

		// ------------------------------------------------------------
		// D. Default rules
		// ------------------------------------------------------------

		private sealed class DefaultRulesTestClass
		{
			public int NonNullableValue { get; set; }
			public int? NullableValue { get; set; }
			public string RefType { get; set; } = "";
			public List<int> Collection { get; set; } = new();
		}

		[Fact]
		public void DefaultRules_AreAppliedCorrectly()
		{
			var t = typeof(DefaultRulesTestClass);

			Assert.True(RequiredPropertyDetector.IsRequired(t.GetProperty("NonNullableValue")!));
			Assert.False(RequiredPropertyDetector.IsRequired(t.GetProperty("NullableValue")!));
			Assert.False(RequiredPropertyDetector.IsRequired(t.GetProperty("RefType")!));
			Assert.False(RequiredPropertyDetector.IsRequired(t.GetProperty("Collection")!));
		}
	}
}
