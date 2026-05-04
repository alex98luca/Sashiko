using Sashiko.Core.Probability.Selection;
using Sashiko.Core.Probability.Selection.Implementation;
using Sashiko.Names.Generation;
using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Api
{
	public sealed class NameService
	{
		private readonly NameGenerator _generator;

		public NameService()
		{
			var registry = new NameRegistry();
			var picker = new RandomPicker();

			_generator = CreateGenerator(registry, picker);
		}

		// For testing or advanced scenarios
		internal NameService(INameRegistry registry, IRandomPicker picker)
		{
			_generator = CreateGenerator(registry, picker);
		}

		public PersonName Generate(
			Sex sex,
			LanguageId language = LanguageId.Random,
			string? fatherName = null,
			string? motherName = null)
			=> _generator.Generate(language, sex, fatherName, motherName);

		private static NameGenerator CreateGenerator(
			INameRegistry registry,
			IRandomPicker picker)
		{
			var given = new GivenNameGenerator(registry, picker);
			var patronymic = new PatronymicGenerator(registry, picker);
			var matronymic = new MatronymicGenerator(registry, picker);
			var last = new LastNameGenerator(registry, picker);
			var prefix = new PrefixGenerator(registry, picker);
			var suffix = new SuffixGenerator(registry, picker);
			var assembler = new NameAssembler(registry);

			return new NameGenerator(new NameGeneratorDependencies
			{
				Registry = registry,
				Picker = picker,
				Given = given,
				Patronymic = patronymic,
				Matronymic = matronymic,
				Last = last,
				Prefix = prefix,
				Suffix = suffix,
				Assembler = assembler
			});
		}
	}
}
