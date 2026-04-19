using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class MatronymicGenerator : IMatronymicGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		public MatronymicGenerator(INameRegistry registry, IRandomPicker picker)
		{
			_registry = registry;
			_picker = picker;
		}

		public string? Generate(LanguageId language, Sex sex, string motherName)
		{
			var entry = _registry.Get(language);
			var rules = entry.Rules;

			// Matronymics disabled for this culture
			if (!rules.UsesMatronymic)
				return null;

			// Probability check
			if (!_picker.Chance(rules.MatronymicProbability))
				return null;

			// No base name provided
			if (string.IsNullOrWhiteSpace(motherName))
				return null;

			return sex switch
			{
				Sex.Male => ApplyPattern(motherName, rules.MatronymicPatternMale),
				Sex.Female => ApplyPattern(motherName, rules.MatronymicPatternFemale),
				_ => null
			};
		}

		private static string? ApplyPattern(string baseName, string? pattern)
		{
			if (pattern is null)
				return null;

			return pattern.Replace("{name}", baseName);
		}
	}
}
