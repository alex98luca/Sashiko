using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class PatronymicGeneratorTests
	{
		[Fact]
		public void Generate_ShouldApplySexSpecificPattern()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesPatronymic: true,
					patronymicPatternMale: "{father}ovich",
					patronymicPatternFemale: "{father}ovna",
					patronymicProbability: 1));
			var generator = new PatronymicGenerator(registry, new DeterministicRandomPicker());

			Assert.Equal("Ivanovich", generator.Generate(LanguageId.Ita, Sex.Male, "Ivan"));
			Assert.Equal("Ivanovna", generator.Generate(LanguageId.Ita, Sex.Female, "Ivan"));
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenDisabled()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(usesPatronymic: false));
			var generator = new PatronymicGenerator(registry, new DeterministicRandomPicker());

			var patronymic = generator.Generate(LanguageId.Ita, Sex.Male, "Ivan");

			Assert.Null(patronymic);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenProbabilityFails()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesPatronymic: true,
					patronymicPatternMale: "{father}ovich",
					patronymicProbability: 1));
			var generator = new PatronymicGenerator(
				registry,
				new DeterministicRandomPicker(chanceResults: new[] { false }));

			var patronymic = generator.Generate(LanguageId.Ita, Sex.Male, "Ivan");

			Assert.Null(patronymic);
		}

		[Fact]
		public void Generate_ShouldReturnNullForBlankFatherName()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesPatronymic: true,
					patronymicPatternMale: "{father}ovich",
					patronymicProbability: 1));
			var generator = new PatronymicGenerator(registry, new DeterministicRandomPicker());

			var patronymic = generator.Generate(LanguageId.Ita, Sex.Male, " ");

			Assert.Null(patronymic);
		}
	}
}
