using Sashiko.Names.Generation;
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
			var generator = new NameGenerator(new NameGeneratorDependencies
			{
				Registry = registry,
				Picker = picker,
				Given = new GivenNameGenerator(registry, picker),
				Patronymic = new PatronymicGenerator(registry, picker),
				Matronymic = new MatronymicGenerator(registry, picker),
				Last = new LastNameGenerator(registry, picker),
				Prefix = new PrefixGenerator(registry, picker),
				Suffix = new SuffixGenerator(registry, picker),
				Assembler = new NameAssembler(registry)
			});

			var name = generator.Generate(LanguageId.Ita, Sex.Male);

			Assert.Equal(NameGeneratorTestSupport.SingleMaleFirstName, name.GivenNames);
			Assert.Equal(NameGeneratorTestSupport.SingleLastName, name.LastNames);
			Assert.Equal("Marco Rossi", name.FullName);
			Assert.Equal(name.FullName, name.DisplayName);
		}
	}
}
