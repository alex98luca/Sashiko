using Sashiko.Validation.Schema.Inspectors.CSharp;

namespace Sashiko.Validation.Tests.Schema.Inspectors.CSharp
{
	public sealed class CollectionTypeDetectorTests
	{
		// ------------------------------------------------------------
		// Non-collections
		// ------------------------------------------------------------

		[Fact]
		public void TryGetElementType_WhenTypeIsString_ReturnsFalse()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(string), out var element);

			// Assert
			Assert.False(result);
			Assert.Null(element);
		}

		[Fact]
		public void TryGetElementType_WhenTypeIsNotCollection_ReturnsFalse()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(int), out var element);

			// Assert
			Assert.False(result);
			Assert.Null(element);
		}

		// ------------------------------------------------------------
		// Arrays
		// ------------------------------------------------------------

		[Fact]
		public void TryGetElementType_WhenTypeIsArray_ReturnsElementType()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(int[]), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(int), element);
		}

		// ------------------------------------------------------------
		// Generic collections
		// ------------------------------------------------------------

		[Fact]
		public void TryGetElementType_WhenTypeIsList_ReturnsElementType()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(List<string>), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(string), element);
		}

		[Fact]
		public void TryGetElementType_WhenTypeIsIEnumerableT_ReturnsElementType()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(IEnumerable<double>), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(double), element);
		}

		[Fact]
		public void TryGetElementType_WhenTypeIsICollectionT_ReturnsElementType()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(ICollection<Guid>), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(Guid), element);
		}

		[Fact]
		public void TryGetElementType_WhenTypeIsIReadOnlyListT_ReturnsElementType()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(IReadOnlyList<decimal>), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(decimal), element);
		}

		// ------------------------------------------------------------
		// Custom collection types
		// ------------------------------------------------------------

		private sealed class CustomCollection : List<DateTime> { }

		[Fact]
		public void TryGetElementType_WhenTypeIsCustomCollection_ReturnsElementType()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(CustomCollection), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(DateTime), element);
		}

		// ------------------------------------------------------------
		// Types implementing multiple IEnumerable<T> interfaces
		// ------------------------------------------------------------

		private sealed class MultiInterfaceCollection : IEnumerable<int>, IEnumerable<long>
		{
			IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
			IEnumerator<long> IEnumerable<long>.GetEnumerator() => throw new NotImplementedException();
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotImplementedException();
		}

		[Fact]
		public void TryGetElementType_WhenTypeImplementsMultipleIEnumerableInterfaces_PicksFirstMatch()
		{
			// Act
			var result = CollectionTypeDetector.TryGetElementType(typeof(MultiInterfaceCollection), out var element);

			// Assert
			Assert.True(result);
			Assert.Equal(typeof(int), element); // first matching interface
		}
	}
}
