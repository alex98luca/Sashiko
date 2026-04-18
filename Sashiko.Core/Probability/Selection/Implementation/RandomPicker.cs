namespace Sashiko.Core.Probability.Selection.Implementation
{
	public sealed class RandomPicker : IRandomPicker
	{
		private readonly Random _rng;

		public RandomPicker(Random? rng = null)
		{
			_rng = rng ?? Random.Shared;
		}

		public T Pick<T>(IReadOnlyList<T> list)
		{
			if (list.Count == 0)
				throw new InvalidOperationException("Cannot pick from an empty list.");

			return list[_rng.Next(list.Count)];
		}

		public T Pick<T>(IEnumerable<T> sequence)
		{
			var list = sequence as IReadOnlyList<T> ?? sequence.ToList();
			return Pick(list);
		}

		public KeyValuePair<TKey, TValue> Pick<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dict)
		{
			if (dict.Count == 0)
				throw new InvalidOperationException("Cannot pick from an empty dictionary.");

			int index = _rng.Next(dict.Count);
			return dict.ElementAt(index);
		}

		public bool Chance(double probability)
			=> ChanceUtility.Of(probability, _rng);
	}
}
