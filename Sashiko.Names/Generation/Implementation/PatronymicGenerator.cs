using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class PatronymicGenerator : IPatronymicGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		public PatronymicGenerator(INameRegistry registry, IRandomPicker picker)
		{
			_registry = registry;
			_picker = picker;
		}

		public string? Generate(LanguageId language, Sex sex, string fatherName)
		{
			var entry = _registry.Get(language);
			var rules = entry.Rules;

			// Patronymics disabled for this culture
			if (!rules.UsesPatronymic)
				return null;

			// Probability check
			if (!_picker.Chance(rules.PatronymicProbability))
				return null;

			// No base name provided
			if (string.IsNullOrWhiteSpace(fatherName))
				return null;

			return sex switch
			{
				Sex.Male => ApplyPattern(fatherName, rules.PatronymicPatternMale),
				Sex.Female => ApplyPattern(fatherName, rules.PatronymicPatternFemale),
				_ => null
			};
		}

		private static string? ApplyPattern(string fatherName, string? pattern)
		{
			if (pattern is null)
				return null;

			return pattern.Replace("{father}", fatherName);
		}
	}
}
