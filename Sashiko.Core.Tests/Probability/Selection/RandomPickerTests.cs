using Sashiko.Core.Probability.Selection.Implementation;

namespace Sashiko.Core.Tests.Probability.Selection
{
	public sealed class RandomPickerTests
	{
		// ------------------------------------------------------------
		// Pick<T>(IReadOnlyList<T>)
		// ------------------------------------------------------------

		[Fact]
		public void Pick_List_Throws_WhenEmpty()
		{
			var picker = new RandomPicker(new Random(42));
			var list = Array.Empty<int>();

			Assert.Throws<InvalidOperationException>(() => picker.Pick(list));
		}

		[Fact]
		public void Pick_List_ReturnsDeterministicValue_WithSeededRng()
		{
			var picker1 = new RandomPicker(new Random(42));
			var picker2 = new RandomPicker(new Random(42));

			var list = new[] { "a", "b", "c", "d" };

			var a = picker1.Pick(list);
			var b = picker2.Pick(list);

			Assert.Equal(a, b);
		}

		// ------------------------------------------------------------
		// Pick<T>(IEnumerable<T>)
		// ------------------------------------------------------------

		[Fact]
		public void Pick_Enumerable_Throws_WhenEmpty()
		{
			var picker = new RandomPicker(new Random(42));
			IEnumerable<int> seq = Array.Empty<int>();

			Assert.Throws<InvalidOperationException>(() => picker.Pick(seq));
		}

		[Fact]
		public void Pick_Enumerable_ReturnsDeterministicValue_WithSeededRng()
		{
			var picker1 = new RandomPicker(new Random(42));
			var picker2 = new RandomPicker(new Random(42));

			IEnumerable<int> seq = new List<int> { 10, 20, 30, 40 };

			var a = picker1.Pick(seq);
			var b = picker2.Pick(seq);

			Assert.Equal(a, b);
		}

		// ------------------------------------------------------------
		// Pick<TKey, TValue>(IReadOnlyDictionary<TKey, TValue>)
		// ------------------------------------------------------------

		[Fact]
		public void Pick_Dictionary_Throws_WhenEmpty()
		{
			var picker = new RandomPicker(new Random(42));
			IReadOnlyDictionary<string, int> dict = new Dictionary<string, int>();

			Assert.Throws<InvalidOperationException>(() => picker.Pick(dict));
		}

		[Fact]
		public void Pick_Dictionary_ReturnsDeterministicValue_WithSeededRng()
		{
			var picker1 = new RandomPicker(new Random(42));
			var picker2 = new RandomPicker(new Random(42));

			IReadOnlyDictionary<string, int> dict = new Dictionary<string, int>
			{
				["one"] = 1,
				["two"] = 2,
				["three"] = 3
			};

			var a = picker1.Pick(dict);
			var b = picker2.Pick(dict);

			Assert.Equal(a.Key, b.Key);
			Assert.Equal(a.Value, b.Value);
		}

		// ------------------------------------------------------------
		// Chance(probability)
		// ------------------------------------------------------------

		[Fact]
		public void Chance_DelegatesToChanceUtility()
		{
			var picker = new RandomPicker(new FixedRandom(0.5));

			// Probability = 0.5 → NextDouble() = 0.5 → 0.5 < 0.5 is false
			Assert.False(picker.Chance(0.5));
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
