namespace Sashiko.Core.Probability.Selection
{
	public interface IRandomPicker
	{
		T Pick<T>(IReadOnlyList<T> list);
		T Pick<T>(IEnumerable<T> sequence);
		KeyValuePair<TKey, TValue> Pick<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dict);

		bool Chance(double probability);
	}
}
