using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class LastNameGeneratorTests
	{
		[Fact]
		public void Generate_ShouldUseGeneralSurnamePool()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(usesGenderedLastNames: false));
			var generator = new LastNameGenerator(registry, new DeterministicRandomPicker());

			var names = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(new[] { "Rossi" }, names);
		}

		[Fact]
		public void Generate_ShouldUseGenderedSurnamePool()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(usesGenderedLastNames: true));
			var generator = new LastNameGenerator(registry, new DeterministicRandomPicker());

			var male = generator.Generate(LanguageId.Ita, Sex.Male);
			var female = generator.Generate(LanguageId.Ita, Sex.Female);

			Assert.Equal(new[] { "Ivanov" }, male);
			Assert.Equal(new[] { "Ivanova" }, female);
		}

		[Fact]
		public void Generate_ShouldGenerateDoubleSurnameWhenEnabled()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesDoubleLastName: true,
					doubleLastNameProbability: 1));
			var picker = new DeterministicRandomPicker("Rossi", "Bianchi");
			var generator = new LastNameGenerator(registry, picker);

			var names = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(new[] { "Rossi", "Bianchi" }, names);
		}

		[Fact]
		public void Generate_ShouldReturnSingleSurnameWhenDoubleProbabilityFails()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesDoubleLastName: true,
					doubleLastNameProbability: 1));
			var generator = new LastNameGenerator(
				registry,
				new DeterministicRandomPicker(chanceResults: new[] { false }));

			var names = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(new[] { "Rossi" }, names);
		}

		[Fact]
		public void Generate_ShouldAvoidDuplicateDoubleSurnameWhenAlternativesExist()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					usesDoubleLastName: true,
					doubleLastNameProbability: 1));
			var picker = new DeterministicRandomPicker("Rossi", "Rossi", "Bianchi");
			var generator = new LastNameGenerator(registry, picker);

			var names = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(new[] { "Rossi", "Bianchi" }, names);
		}
	}
}
