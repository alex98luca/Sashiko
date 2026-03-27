using Sashiko.Validation.Schema.Inspectors.CSharp;

namespace Sashiko.Validation.Tests.Schema.Inspectors.CSharp
{
	public sealed class LeafTypeDetectorTests
	{
		// ------------------------------------------------------------
		// Primitive CLR types
		// ------------------------------------------------------------

		[Fact]
		public void IsLeaf_WhenTypeIsInt_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(int)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsBool_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(bool)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsDouble_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(double)));
		}

		// ------------------------------------------------------------
		// Enums
		// ------------------------------------------------------------

		private enum TestEnum { A, B }

		[Fact]
		public void IsLeaf_WhenTypeIsEnum_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(TestEnum)));
		}

		// ------------------------------------------------------------
		// Nullable<T>
		// ------------------------------------------------------------

		[Fact]
		public void IsLeaf_WhenTypeIsNullableInt_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(int?)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsNullableEnum_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(TestEnum?)));
		}

		// ------------------------------------------------------------
		// JSON-compatible leaf types
		// ------------------------------------------------------------

		[Fact]
		public void IsLeaf_WhenTypeIsString_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(string)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsDecimal_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(decimal)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsDateTime_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(DateTime)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsGuid_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(Guid)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsTimeSpan_ReturnsTrue()
		{
			Assert.True(LeafTypeDetector.IsLeaf(typeof(TimeSpan)));
		}

		// ------------------------------------------------------------
		// Collections (should NOT be leaf)
		// ------------------------------------------------------------

		[Fact]
		public void IsLeaf_WhenTypeIsList_ReturnsFalse()
		{
			Assert.False(LeafTypeDetector.IsLeaf(typeof(List<int>)));
		}

		[Fact]
		public void IsLeaf_WhenTypeIsArray_ReturnsFalse()
		{
			Assert.False(LeafTypeDetector.IsLeaf(typeof(string[])));
		}

		// ------------------------------------------------------------
		// Custom reference types
		// ------------------------------------------------------------

		private sealed class CustomClass
		{
			public int Value { get; set; }
		}

		[Fact]
		public void IsLeaf_WhenTypeIsCustomClass_ReturnsFalse()
		{
			Assert.False(LeafTypeDetector.IsLeaf(typeof(CustomClass)));
		}

		// ------------------------------------------------------------
		// Edge cases
		// ------------------------------------------------------------

		[Fact]
		public void IsLeaf_WhenTypeIsObject_ReturnsFalse()
		{
			Assert.False(LeafTypeDetector.IsLeaf(typeof(object)));
		}
	}
}
