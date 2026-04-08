using Sashiko.Languages.Model;
using Sashiko.Maintenance.Libraries.Languages.Updating;

namespace Sashiko.Maintenance.Tests.Libraries.Languages.Updating
{
	public sealed class LanguageMapperTests
	{
		[Fact]
		public void Convert_ShouldMapFieldsCorrectly()
		{
			var external = new[]
			{
				new IsoSilLanguage(
					name: "English",
					iso639_1: "EN",
					iso639_2: "ENG",
					iso639_3: "ENG",
					scope: "I",
					type: "L")
			};

			var result = LanguageMapper.Convert(external);

			Assert.Single(result);

			var lang = result[0];
			Assert.Equal("English", lang.Name);
			Assert.Equal("en", lang.Iso639_1);
			Assert.Equal("eng", lang.Iso639_2);
			Assert.Equal("eng", lang.Iso639_3);
			Assert.Equal(LanguageScope.Individual, lang.Scope);
			Assert.Equal(LanguageType.Living, lang.Type);
		}

		[Fact]
		public void Convert_ShouldNormalizeIsoCodes()
		{
			var external = new[]
			{
				new IsoSilLanguage("Test", " En ", " EnG ", " EnG ", "I", "L")
			};

			var result = LanguageMapper.Convert(external)[0];

			Assert.Equal("en", result.Iso639_1);
			Assert.Equal("eng", result.Iso639_2);
			Assert.Equal("eng", result.Iso639_3);
		}

		[Fact]
		public void Convert_ShouldThrow_OnUnknownScope()
		{
			var external = new[]
			{
				new IsoSilLanguage("Test", null, null, "tst", "X", "L")
			};

			Assert.Throws<InvalidOperationException>(() =>
				LanguageMapper.Convert(external));
		}

		[Fact]
		public void Convert_ShouldThrow_OnUnknownType()
		{
			var external = new[]
			{
				new IsoSilLanguage("Test", null, null, "tst", "I", "X")
			};

			Assert.Throws<InvalidOperationException>(() =>
				LanguageMapper.Convert(external));
		}
	}
}
