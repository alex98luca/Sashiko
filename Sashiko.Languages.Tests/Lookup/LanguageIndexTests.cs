using Sashiko.Languages.Lookup;
using Sashiko.Languages.Registry;

namespace Sashiko.Languages.Tests.Lookup
{
	public sealed class LanguageIndexTests
	{
		private readonly LanguageRegistry _registry;
		private readonly LanguageIndex _index;

		public LanguageIndexTests()
		{
			_registry = new LanguageRegistry();
			_index = new LanguageIndex(_registry.Languages);
		}

		// ------------------------------------------------------------
		// 1. Construction Tests
		// ------------------------------------------------------------

		[Fact]
		public void Index_ShouldInitializeCorrectly()
		{
			Assert.NotNull(_index.ByName);
			Assert.NotNull(_index.ByIso1);
			Assert.NotNull(_index.ByIso2);
			Assert.NotNull(_index.ByIso3);
			Assert.NotNull(_index.All);

			Assert.NotEmpty(_index.All);
		}

		[Fact]
		public void Index_All_ShouldMatchRegistryCount()
		{
			Assert.Equal(_registry.Languages.Count, _index.All.Count);
		}

		// ------------------------------------------------------------
		// 2. Lookup by ISO-3
		// ------------------------------------------------------------

		[Fact]
		public void Index_ShouldResolveIso3()
		{
			var english = _index.ByIso3["eng"];
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void Index_Iso3Lookup_ShouldBeCaseInsensitive()
		{
			var english = _index.ByIso3["ENG"];
			Assert.Equal("English", english.Name);
		}

		// ------------------------------------------------------------
		// 3. Lookup by ISO-2
		// ------------------------------------------------------------

		[Fact]
		public void Index_ShouldResolveIso2()
		{
			// English ISO-2 is "eng" or "en" depending on SIL mapping
			var english = _index.ByIso2["eng"];
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void Index_Iso2Lookup_ShouldBeCaseInsensitive()
		{
			var english = _index.ByIso2["ENG"];
			Assert.Equal("English", english.Name);
		}

		// ------------------------------------------------------------
		// 4. Lookup by ISO-1
		// ------------------------------------------------------------

		[Fact]
		public void Index_ShouldResolveIso1()
		{
			var english = _index.ByIso1["en"];
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void Index_Iso1Lookup_ShouldBeCaseInsensitive()
		{
			var english = _index.ByIso1["EN"];
			Assert.Equal("English", english.Name);
		}

		// ------------------------------------------------------------
		// 5. Lookup by Name
		// ------------------------------------------------------------

		[Fact]
		public void Index_ShouldResolveName()
		{
			var english = _index.ByName["English"];
			Assert.Equal("eng", english.Iso639_3);
		}

		[Fact]
		public void Index_NameLookup_ShouldBeCaseInsensitive()
		{
			var english = _index.ByName["english"];
			Assert.Equal("eng", english.Iso639_3);
		}

		// ------------------------------------------------------------
		// 6. TryResolve Tests
		// ------------------------------------------------------------

		[Fact]
		public void TryResolve_ShouldReturnTrue_ForValidIso3()
		{
			Assert.True(_index.TryResolve("eng", out var lang));
			Assert.Equal("English", lang!.Name);
		}

		[Fact]
		public void TryResolve_ShouldReturnTrue_ForValidIso1()
		{
			Assert.True(_index.TryResolve("en", out var lang));
			Assert.Equal("English", lang!.Name);
		}

		[Fact]
		public void TryResolve_ShouldReturnTrue_ForValidName()
		{
			Assert.True(_index.TryResolve("English", out var lang));
			Assert.Equal("eng", lang!.Iso639_3);
		}

		[Fact]
		public void TryResolve_ShouldReturnFalse_ForInvalidInput()
		{
			Assert.False(_index.TryResolve("not-a-language", out var lang));
			Assert.Null(lang);
		}

		// ------------------------------------------------------------
		// 7. Consistency Tests
		// ------------------------------------------------------------

		[Fact]
		public void Index_All_ShouldContainAllRegistryLanguages()
		{
			var allIso3 = _index.All.Select(l => l.Iso639_3).ToHashSet();
			var registryIso3 = _registry.Languages.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

			Assert.True(registryIso3.SetEquals(allIso3));
		}

		[Fact]
		public void Index_ShouldNotContainDuplicates()
		{
			var all = _index.All;
			Assert.Equal(all.Count, all.Distinct().Count());
		}

		// ------------------------------------------------------------
		// 8. Error Behavior Tests
		// ------------------------------------------------------------

		[Fact]
		public void Index_ByIso3_ShouldThrow_ForUnknownCode()
		{
			Assert.Throws<KeyNotFoundException>(() => _index.ByIso3["zzz"]);
		}

		[Fact]
		public void Index_ByName_ShouldThrow_ForUnknownName()
		{
			Assert.Throws<KeyNotFoundException>(() => _index.ByName["NotALanguage"]);
		}
	}
}
