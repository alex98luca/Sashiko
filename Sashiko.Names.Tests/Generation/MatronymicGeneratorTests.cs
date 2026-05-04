using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class MatronymicGeneratorTests
	{
		[Fact]
		public void Generate_ShouldApplySexSpecificPattern()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesMatronymic: true,
					matronymicPatternMale: "{mother}son",
					matronymicPatternFemale: "{mother}dotter",
					matronymicProbability: 1));
			var generator = new MatronymicGenerator(registry, new DeterministicRandomPicker());

			Assert.Equal("Annason", generator.Generate(LanguageId.Ita, Sex.Male, "Anna"));
			Assert.Equal("Annadotter", generator.Generate(LanguageId.Ita, Sex.Female, "Anna"));
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenDisabled()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(usesMatronymic: false));
			var generator = new MatronymicGenerator(registry, new DeterministicRandomPicker());

			var matronymic = generator.Generate(LanguageId.Ita, Sex.Female, "Anna");

			Assert.Null(matronymic);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenProbabilityFails()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesMatronymic: true,
					matronymicPatternFemale: "{mother}dotter",
					matronymicProbability: 1));
			var generator = new MatronymicGenerator(
				registry,
				new DeterministicRandomPicker(chanceResults: NameGeneratorTestSupport.FailedChance));

			var matronymic = generator.Generate(LanguageId.Ita, Sex.Female, "Anna");

			Assert.Null(matronymic);
		}

		[Fact]
		public void Generate_ShouldReturnNullForBlankMotherName()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesMatronymic: true,
					matronymicPatternFemale: "{mother}dotter",
					matronymicProbability: 1));
			var generator = new MatronymicGenerator(registry, new DeterministicRandomPicker());

			var matronymic = generator.Generate(LanguageId.Ita, Sex.Female, " ");

			Assert.Null(matronymic);
		}
	}
}
