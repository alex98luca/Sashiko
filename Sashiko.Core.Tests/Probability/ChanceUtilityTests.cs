using Sashiko.Core.Probability;

namespace Sashiko.Core.Tests.Probability
{
	public sealed class ChanceUtilityTests
	{
		// ------------------------------------------------------------
		// Boundary conditions
		// ------------------------------------------------------------

		[Fact]
		public void Of_ReturnsFalse_WhenProbabilityIsZero()
		{
			var rng = new Random(123);
			Assert.False(ChanceUtility.Of(0.0, rng));
		}

		[Fact]
		public void Of_ReturnsFalse_WhenProbabilityIsNegative()
		{
			var rng = new Random(123);
			Assert.False(ChanceUtility.Of(-0.5, rng));
		}

		[Fact]
		public void Of_ReturnsTrue_WhenProbabilityIsOne()
		{
			var rng = new Random(123);
			Assert.True(ChanceUtility.Of(1.0, rng));
		}

		[Fact]
		public void Of_ReturnsTrue_WhenProbabilityIsGreaterThanOne()
		{
			var rng = new Random(123);
			Assert.True(ChanceUtility.Of(2.0, rng));
		}

		// ------------------------------------------------------------
		// Deterministic behavior with seeded RNG
		// ------------------------------------------------------------

		[Fact]
		public void Of_UsesRngDeterministically()
		{
			var rng1 = new Random(42);
			var rng2 = new Random(42);

			bool a = ChanceUtility.Of(0.5, rng1);
			bool b = ChanceUtility.Of(0.5, rng2);

			Assert.Equal(a, b);
		}

		// ------------------------------------------------------------
		// Correct probability semantics (< not <=)
		// ------------------------------------------------------------

		[Fact]
		public void Of_UsesStrictLessThanComparison()
		{
			// We force NextDouble() to return exactly 0.5 by using a custom RNG.
			var rng = new FixedRandom(0.5);

			// Probability = 0.5 → NextDouble() == 0.5 → 0.5 < 0.5 is false
			Assert.False(ChanceUtility.Of(0.5, rng));
		}

		// ------------------------------------------------------------
		// Helper RNG for deterministic edge-case testing
		// ------------------------------------------------------------

		private sealed class FixedRandom : Random
		{
			private readonly double _value;

			public FixedRandom(double value)
			{
				_value = value;
			}

			public override double NextDouble() => _value;
		}
	}
}
