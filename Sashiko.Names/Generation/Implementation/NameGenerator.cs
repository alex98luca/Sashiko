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

		public NameGenerator(NameGeneratorDependencies dependencies)
		{
			_registry = dependencies.Registry;
			_picker = dependencies.Picker;

			_given = dependencies.Given;
			_patronymic = dependencies.Patronymic;
			_matronymic = dependencies.Matronymic;
			_last = dependencies.Last;
			_prefix = dependencies.Prefix;
			_suffix = dependencies.Suffix;
			_assembler = dependencies.Assembler;
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
			return _assembler.Assemble(new NameAssemblyRequest
			{
				Language = language,
				GivenNames = given,
				Patronymic = patronymic,
				Matronymic = matronymic,
				LastNames = last,
				Prefix = prefix,
				Suffix = suffix
			});
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
