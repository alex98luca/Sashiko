using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class GivenNameGeneratorTests
	{
		[Fact]
		public void Generate_ShouldUseConfiguredCount()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(givenNameCountMin: 2, givenNameCountMax: 2));
			var generator = new GivenNameGenerator(registry, new DeterministicRandomPicker());

			var names = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(new[] { "Marco", "Marco" }, names);
		}

		[Fact]
		public void Generate_ShouldUseUnisexPoolWhenProbabilityPasses()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(unisexFirstNameProbability: 1));
			var generator = new GivenNameGenerator(registry, new DeterministicRandomPicker());

			var names = generator.Generate(LanguageId.Ita, Sex.Female);

			Assert.Equal(new[] { "Alex" }, names);
		}

		[Fact]
		public void Generate_ShouldUseFemalePoolWhenUnisexProbabilityFails()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(unisexFirstNameProbability: 1));
			var generator = new GivenNameGenerator(
				registry,
				new DeterministicRandomPicker(chanceResults: new[] { false }));

			var names = generator.Generate(LanguageId.Ita, Sex.Female);

			Assert.Equal(new[] { "Anna" }, names);
		}

		[Fact]
		public void Generate_ShouldPickCountFromConfiguredRange()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(givenNameCountMin: 1, givenNameCountMax: 2));
			var generator = new GivenNameGenerator(registry, new DeterministicRandomPicker(2));

			var names = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(2, names.Count);
		}
	}
}
