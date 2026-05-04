using Sashiko.Names.Generation;
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

			var name = assembler.Assemble(new NameAssemblyRequest
			{
				Language = LanguageId.Ita,
				GivenNames = NameGeneratorTestSupport.SingleMaleFirstName,
				Patronymic = "Ivanovich",
				LastNames = NameGeneratorTestSupport.SingleLastName,
				Prefix = "Dr.",
				Suffix = "Jr."
			});

			Assert.Equal("Dr. Rossi Marco Ivanovich Jr.", name.FullName);
		}
	}
}
