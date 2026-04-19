using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Data;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class GivenNameGenerator : IGivenNameGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		public GivenNameGenerator(
			INameRegistry registry,
			IRandomPicker picker)
		{
			_registry = registry;
			_picker = picker;
		}

		public IReadOnlyList<string> Generate(LanguageId language, Sex sex)
		{
			var entry = _registry.Get(language);
			var pool = entry.Pool;
			var rules = entry.Rules;

			int count = PickGivenNameCount(rules);

			var result = new List<string>(capacity: count);

			for (int i = 0; i < count; i++)
				result.Add(PickGivenName(pool, rules, sex));

			return result;
		}

		private int PickGivenNameCount(NameRules rules)
		{
			int min = rules.GivenNameCountMin;
			int max = rules.GivenNameCountMax;

			if (min == max)
				return min;

			// Inclusive range [min, max]
			var range = Enumerable.Range(min, max - min + 1);
			return _picker.Pick(range);
		}

		private string PickGivenName(NamePool pool, NameRules rules, Sex sex)
		{
			// Unisex name?
			if (pool.UnisexFirstNames.Count > 0 &&
				_picker.Chance(rules.UnisexFirstNameProbability))
			{
				return _picker.Pick(pool.UnisexFirstNames);
			}

			// Gendered name
			return sex switch
			{
				Sex.Male => _picker.Pick(pool.MaleFirstNames),
				Sex.Female => _picker.Pick(pool.FemaleFirstNames),
				_ => _picker.Pick(pool.UnisexFirstNames)
			};
		}
	}
}
