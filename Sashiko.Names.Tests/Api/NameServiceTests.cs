using Sashiko.Names.Api;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Tests.Api
{
	public sealed class NameServiceTests
	{
		private readonly NameService _service = new();

		[Fact]
		public void Service_ShouldInitializeCorrectly()
		{
			var name = _service.Generate(Sex.Female);

			Assert.NotNull(name);
		}

		[Theory]
		[InlineData(LanguageId.Ita, Sex.Male)]
		[InlineData(LanguageId.Ron, Sex.Female)]
		[InlineData(LanguageId.Rus, Sex.Male)]
		[InlineData(LanguageId.Spa, Sex.Female)]
		public void Generate_ShouldReturnUsablePersonName(LanguageId language, Sex sex)
		{
			var name = _service.Generate(sex, language);

			Assert.NotNull(name);
			Assert.NotEmpty(name.GivenNames);
			Assert.NotEmpty(name.LastNames);
			Assert.False(string.IsNullOrWhiteSpace(name.FullName));
			Assert.Equal(name.FullName, name.DisplayName);
		}

		[Fact]
		public void Generate_ShouldResolveRandomLanguage()
		{
			var name = _service.Generate(Sex.Female);

			Assert.NotNull(name);
			Assert.NotEmpty(name.GivenNames);
			Assert.NotEmpty(name.LastNames);
		}

		[Fact]
		public void Generate_ShouldUseMaleSex()
		{
			var name = _service.Generate(Sex.Male, LanguageId.Rus, fatherName: "Ivan");

			Assert.Contains("Ivanovich", name.FullName);
		}

		[Fact]
		public void Generate_ShouldUseFemaleSex()
		{
			var name = _service.Generate(Sex.Female, LanguageId.Rus, fatherName: "Ivan");

			Assert.Contains("Ivanovna", name.FullName);
		}

		[Fact]
		public void Service_PublicApi_ShouldOnlyExposeGenerate()
		{
			var publicMethods = typeof(NameService)
				.GetMethods()
				.Where(m => m.DeclaringType == typeof(NameService))
				.Select(m => m.Name)
				.ToArray();

			Assert.Equal(new[] { "Generate" }, publicMethods);
		}
	}
}
