using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class NameGeneratorTests
	{
		[Fact]
		public void Generate_ShouldGenerateCompletePersonName()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules());
			var picker = new DeterministicRandomPicker();
			var generator = new NameGenerator(
				registry,
				picker,
				new GivenNameGenerator(registry, picker),
				new PatronymicGenerator(registry, picker),
				new MatronymicGenerator(registry, picker),
				new LastNameGenerator(registry, picker),
				new PrefixGenerator(registry, picker),
				new SuffixGenerator(registry, picker),
				new NameAssembler(registry));

			var name = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(new[] { "Marco" }, name.GivenNames);
			Assert.Equal(new[] { "Rossi" }, name.LastNames);
			Assert.Equal("Marco Rossi", name.FullName);
			Assert.Equal(name.FullName, name.DisplayName);
		}
	}
}
