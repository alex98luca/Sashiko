using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Tests.Registry
{
	public sealed class NameRegistryTests
	{
		private static readonly LanguageId[] SupportedLanguages =
			Enum.GetValues<LanguageId>()
				.Where(language => language != LanguageId.Random)
				.ToArray();

		// ------------------------------------------------------------
		// 1. Embedded Resource Tests
		// ------------------------------------------------------------

		[Theory]
		[MemberData(nameof(SupportedLanguageCodes))]
		public void EmbeddedResources_ShouldExist(string languageCode)
		{
			var asm = typeof(NameRegistry).Assembly;

			Assert.NotNull(asm.GetManifestResourceStream(GetResourceName(languageCode, "names")));
			Assert.NotNull(asm.GetManifestResourceStream(GetResourceName(languageCode, "rules")));
		}

		[Theory]
		[MemberData(nameof(SupportedLanguageCodes))]
		public void EmbeddedResources_ShouldBeReadable(string languageCode)
		{
			var namesJson = ReadResource(languageCode, "names");
			var rulesJson = ReadResource(languageCode, "rules");

			Assert.False(string.IsNullOrWhiteSpace(namesJson));
			Assert.False(string.IsNullOrWhiteSpace(rulesJson));
		}

		// ------------------------------------------------------------
		// 2. Registry Loading Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldLoadSuccessfully()
		{
			var registry = new NameRegistry();

			Assert.NotEmpty(registry.All);
		}

		[Fact]
		public void Registry_ShouldLoadAllSupportedLanguages()
		{
			var registry = new NameRegistry();
			var languages = registry.All.Select(entry => entry.Language).ToArray();

			Assert.Equal(SupportedLanguages.Length, languages.Length);

			foreach (var language in SupportedLanguages)
				Assert.Contains(language, languages);
		}

		[Fact]
		public void Registry_ShouldLoadEntriesWithRequiredFields()
		{
			var registry = new NameRegistry();

			foreach (var entry in registry.All)
			{
				Assert.NotNull(entry.Pool);
				Assert.NotNull(entry.Rules);
				Assert.NotEmpty(entry.Pool.MaleFirstNames);
				Assert.NotEmpty(entry.Pool.FemaleFirstNames);
			}
		}

		// ------------------------------------------------------------
		// 3. Lookup Tests
		// ------------------------------------------------------------

		[Theory]
		[MemberData(nameof(SupportedLanguagesData))]
		public void Get_ShouldReturnEntry_ForSupportedLanguage(LanguageId language)
		{
			var registry = new NameRegistry();
			var entry = registry.Get(language);

			Assert.Equal(language, entry.Language);
		}

		[Theory]
		[MemberData(nameof(SupportedLanguagesData))]
		public void TryGet_ShouldReturnTrue_ForSupportedLanguage(LanguageId language)
		{
			var registry = new NameRegistry();
			var result = registry.TryGet(language, out var entry);

			Assert.True(result);
			Assert.NotNull(entry);
			Assert.Equal(language, entry!.Language);
		}

		[Fact]
		public void Get_ShouldThrow_ForRandomLanguage()
		{
			var registry = new NameRegistry();

			Assert.Throws<KeyNotFoundException>(() => registry.Get(LanguageId.Random));
		}

		[Fact]
		public void TryGet_ShouldReturnFalse_ForRandomLanguage()
		{
			var registry = new NameRegistry();
			var result = registry.TryGet(LanguageId.Random, out var entry);

			Assert.False(result);
			Assert.Null(entry);
		}

		// ------------------------------------------------------------
		// 4. Uniqueness and Sanity Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldHaveUniqueLanguages()
		{
			var registry = new NameRegistry();
			var languages = registry.All.Select(entry => entry.Language).ToArray();

			Assert.Equal(languages.Length, languages.Distinct().Count());
		}

		[Fact]
		public void Registry_ShouldNotContainRandomLanguage()
		{
			var registry = new NameRegistry();

			Assert.DoesNotContain(registry.All, entry => entry.Language == LanguageId.Random);
		}

		public static IEnumerable<object[]> SupportedLanguageCodes()
			=> SupportedLanguages.Select(language => new object[] { language.ToString().ToLowerInvariant() });

		public static IEnumerable<object[]> SupportedLanguagesData()
			=> SupportedLanguages.Select(language => new object[] { language });

		private static string ReadResource(string languageCode, string resourceKind)
		{
			var asm = typeof(NameRegistry).Assembly;
			var resourceName = GetResourceName(languageCode, resourceKind);

			using var stream = asm.GetManifestResourceStream(resourceName)
				?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		private static string GetResourceName(string languageCode, string resourceKind)
			=> $"Sashiko.Names.Data.{languageCode}.{resourceKind}.json";
	}
}
