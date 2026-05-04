using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class PrefixGeneratorTests
	{
		[Fact]
		public void Generate_ShouldReturnPrefixWhenAllowed()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					allowPrefixes: true,
					prefixProbability: 1));
			var generator = new PrefixGenerator(registry, new DeterministicRandomPicker());

			var prefix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal("Dr.", prefix);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenDisabled()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(allowPrefixes: false));
			var generator = new PrefixGenerator(registry, new DeterministicRandomPicker());

			var prefix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Null(prefix);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenPoolIsEmpty()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				pool: NameGeneratorTestSupport.CreatePool(prefixes: Array.Empty<string>()),
				rules: NameGeneratorTestSupport.CreateRules(allowPrefixes: true, prefixProbability: 1));
			var generator = new PrefixGenerator(registry, new DeterministicRandomPicker());

			var prefix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Null(prefix);
		}

		[Fact]
		public void Generate_ShouldReturnNullWhenProbabilityFails()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(
					allowPrefixes: true,
					prefixProbability: 1));
			var generator = new PrefixGenerator(
				registry,
				new DeterministicRandomPicker(chanceResults: NameGeneratorTestSupport.FailedChance));

			var prefix = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Null(prefix);
		}
	}
}
