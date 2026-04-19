using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class SuffixGenerator : ISuffixGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		public SuffixGenerator(INameRegistry registry, IRandomPicker picker)
		{
			_registry = registry;
			_picker = picker;
		}

		public string? Generate(LanguageId language, Sex sex)
		{
			var entry = _registry.Get(language);
			var pool = entry.Pool;
			var rules = entry.Rules;

			// Suffixes disabled or none available
			if (!rules.AllowSuffixes || pool.Suffixes.Count == 0)
				return null;

			// Probability check
			if (!_picker.Chance(rules.SuffixProbability))
				return null;

			return _picker.Pick(pool.Suffixes);
		}
	}
}
