using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class PrefixGenerator : IPrefixGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		public PrefixGenerator(INameRegistry registry, IRandomPicker picker)
		{
			_registry = registry;
			_picker = picker;
		}

		public string? Generate(LanguageId language, Sex sex)
		{
			var entry = _registry.Get(language);
			var pool = entry.Pool;
			var rules = entry.Rules;

			// Prefixes disabled or none available
			if (!rules.AllowPrefixes || pool.Prefixes.Count == 0)
				return null;

			// Probability check
			if (!_picker.Chance(rules.PrefixProbability))
				return null;

			return _picker.Pick(pool.Prefixes);
		}
	}
}
