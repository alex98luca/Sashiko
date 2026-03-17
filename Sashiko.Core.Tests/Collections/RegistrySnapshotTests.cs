using System.Collections.Concurrent;
using Sashiko.Core.Collections;

namespace Sashiko.Core.Tests.Collections
{
	public sealed class RegistrySnapshotTests
	{
		// ------------------------------------------------------------
		// CreateEmpty<T>
		// ------------------------------------------------------------

		[Fact]
		public void CreateEmpty_ShouldReturnEmptyDictionary()
		{
			var dict = RegistrySnapshot.CreateEmpty<int>();

			Assert.NotNull(dict);
			Assert.Empty(dict);
		}

		[Fact]
		public void CreateEmpty_ShouldUseCaseInsensitiveComparer()
		{
			var dict = RegistrySnapshot.CreateEmpty<int>();

			dict["Hello"] = 1;

			Assert.True(dict.ContainsKey("hello"));
			Assert.Equal(1, dict["HELLO"]);
		}

		// ------------------------------------------------------------
		// Replace
		// ------------------------------------------------------------

		[Fact]
		public void Replace_ShouldClearTargetBeforeCopying()
		{
			var target = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["A"] = 10,
				["B"] = 20
			};

			var snapshot = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["C"] = 30
			};

			RegistrySnapshot.Replace(target, snapshot);

			Assert.Single(target);
			Assert.True(target.ContainsKey("c"));
			Assert.False(target.ContainsKey("a"));
		}

		[Fact]
		public void Replace_ShouldCopyAllEntriesFromSnapshot()
		{
			var target = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

			var snapshot = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["A"] = 1,
				["B"] = 2,
				["C"] = 3
			};

			RegistrySnapshot.Replace(target, snapshot);

			Assert.Equal(3, target.Count);
			Assert.Equal(1, target["a"]);
			Assert.Equal(2, target["b"]);
			Assert.Equal(3, target["c"]);
		}

		[Fact]
		public void Replace_ShouldOverwriteExistingKeys()
		{
			var target = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["A"] = 100
			};

			var snapshot = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["A"] = 999
			};

			RegistrySnapshot.Replace(target, snapshot);

			Assert.Single(target);
			Assert.Equal(999, target["a"]);
		}

		[Fact]
		public void Replace_ShouldHandleEmptySnapshot()
		{
			var target = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["A"] = 1
			};

			var snapshot = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

			RegistrySnapshot.Replace(target, snapshot);

			Assert.Empty(target);
		}
	}
}
