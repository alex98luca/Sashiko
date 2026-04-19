using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class NameGenerator : INameGenerator
	{
		private readonly INameRegistry _registry;
		private readonly IRandomPicker _picker;

		private readonly IGivenNameGenerator _given;
		private readonly IPatronymicGenerator _patronymic;
		private readonly IMatronymicGenerator _matronymic;
		private readonly ILastNameGenerator _last;
		private readonly IPrefixGenerator _prefix;
		private readonly ISuffixGenerator _suffix;
		private readonly INameAssembler _assembler;

		public NameGenerator(
			INameRegistry registry,
			IRandomPicker picker,
			IGivenNameGenerator given,
			IPatronymicGenerator patronymic,
			IMatronymicGenerator matronymic,
			ILastNameGenerator last,
			IPrefixGenerator prefix,
			ISuffixGenerator suffix,
			INameAssembler assembler)
		{
			_registry = registry;
			_picker = picker;

			_given = given;
			_patronymic = patronymic;
			_matronymic = matronymic;
			_last = last;
			_prefix = prefix;
			_suffix = suffix;
			_assembler = assembler;
		}

		public PersonName Generate(
			LanguageId language,
			Sex sex,
			string? fatherName = null,
			string? motherName = null)
		{
			// ------------------------------------------------------------
			// Resolve LanguageId.Random → actual supported language
			// ------------------------------------------------------------
			if (language == LanguageId.Random)
				language = ResolveRandomLanguage();

			var entry = _registry.Get(language);
			var pool = entry.Pool;

			// ------------------------------------------------------------
			// Resolve parent names (fallback to random)
			// ------------------------------------------------------------
			fatherName ??= _picker.Pick(pool.MaleFirstNames);
			motherName ??= _picker.Pick(pool.FemaleFirstNames);

			// ------------------------------------------------------------
			// Generate components
			// ------------------------------------------------------------
			var given = _given.Generate(language, sex);
			var patronymic = _patronymic.Generate(language, sex, fatherName);
			var matronymic = _matronymic.Generate(language, sex, motherName);
			var last = _last.Generate(language, sex);
			var prefix = _prefix.Generate(language, sex);
			var suffix = _suffix.Generate(language, sex);

			// ------------------------------------------------------------
			// Assemble final name
			// ------------------------------------------------------------
			return _assembler.Assemble(
				language,
				sex,
				given,
				patronymic,
				matronymic,
				last,
				prefix,
				suffix
			);
		}

		// ------------------------------------------------------------
		// INTERNAL HELPERS
		// ------------------------------------------------------------
		private LanguageId ResolveRandomLanguage()
		{
			var languages = _registry.All
				.Select(e => e.Language)
				.ToList();

			return _picker.Pick(languages);
		}
	}
}
