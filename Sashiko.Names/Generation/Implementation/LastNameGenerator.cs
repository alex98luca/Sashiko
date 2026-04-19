using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Data;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class LastNameGenerator : ILastNameGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		public LastNameGenerator(INameRegistry registry, IRandomPicker picker)
		{
			_registry = registry;
			_picker = picker;
		}

		public IReadOnlyList<string> Generate(LanguageId language, Sex sex)
		{
			var entry = _registry.Get(language);
			var pool = entry.Pool;
			var rules = entry.Rules;

			// ------------------------------------------------------------
			// Pick the primary surname
			// ------------------------------------------------------------
			var first = PickSurname(pool, rules, sex);

			// ------------------------------------------------------------
			// Double surname?
			// ------------------------------------------------------------
			if (rules.UsesDoubleLastName &&
				_picker.Chance(rules.DoubleLastNameProbability))
			{
				string second;

				// Ensure second surname is not identical (unless only one exists)
				do
				{
					second = PickSurname(pool, rules, sex);
				}
				while (second == first && pool.LastNames.Count > 1);

				return new[] { first, second };
			}

			return new[] { first };
		}

		// ------------------------------------------------------------
		// INTERNAL HELPERS
		// ------------------------------------------------------------
		private string PickSurname(NamePool pool, NameRules rules, Sex sex)
		{
			// If the culture uses gendered surnames, pick from the appropriate list
			if (rules.UsesGenderedLastNames)
			{
				return sex switch
				{
					Sex.Male => _picker.Pick(pool.MaleLastNames),
					Sex.Female => _picker.Pick(pool.FemaleLastNames),
					_ => _picker.Pick(pool.LastNames)
				};
			}

			// Otherwise, pick from the general surname list
			return _picker.Pick(pool.LastNames);
		}
	}
}
