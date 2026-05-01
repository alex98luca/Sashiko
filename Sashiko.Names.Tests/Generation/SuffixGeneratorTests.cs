using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class SuffixGeneratorTests
	{
		[Fact]
		public void Generate_ShouldReturnSuffixWhenAllowed()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					allowSuffixes: true,
					suffixProbability: 1));
			var generator = new SuffixGenerator(registry, new DeterministicRandomPicker());

			var suffix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal("Jr.", suffix);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenDisabled()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(allowSuffixes: false));
			var generator = new SuffixGenerator(registry, new DeterministicRandomPicker());

			var suffix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Null(suffix);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenPoolIsEmpty()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				pool: NameGeneratorTestSupport.CreatePool(suffixes: Array.Empty<string>()),
				rules: NameGeneratorTestSupport.CreateRules(allowSuffixes: true, suffixProbability: 1));
			var generator = new SuffixGenerator(registry, new DeterministicRandomPicker());

			var suffix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Null(suffix);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenProbabilityFails()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					allowSuffixes: true,
					suffixProbability: 1));
			var generator = new SuffixGenerator(
				registry,
				new DeterministicRandomPicker(chanceResults: new[] { false }));

			var suffix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Null(suffix);
		}
	}
}
