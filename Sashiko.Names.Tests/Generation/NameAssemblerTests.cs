using Sashiko.Names.Generation.Implementation;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Generation
{
	public sealed class NameAssemblerTests
	{
		[Fact]
		public void Assemble_ShouldRespectLastFirstOrder()
		{
			var registry = NameGeneratorTestSupport.CreateRegistry(
				rules: NameGeneratorTestSupport.CreateRules(order: NameOrder.LastFirst));
			var assembler = new NameAssembler(registry);

			var name = assembler.Assemble(
				LanguageId.Ita,
				Sex.Male,
				new[] { "Marco" },
				patronymic: "Ivanovich",
				matronymic: null,
				new[] { "Rossi" },
				prefix: "Dr.",
				suffix: "Jr.");

			Assert.Equal("Dr. Rossi Marco Ivanovich Jr.", name.FullName);
		}
	}
}
