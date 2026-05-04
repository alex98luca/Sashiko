using Sashiko.Languages.Model;
using Sashiko.Languages.Registry;

namespace Sashiko.Languages.Tests.Registry
{
	public sealed class LanguageRegistryTests
	{
		private const string ResourceName =
			"Sashiko.Languages.Data.Languages.languages.json";

		// ------------------------------------------------------------
		// 1. Embedded Resource Tests
		// ------------------------------------------------------------

		[Fact]
		public void EmbeddedResource_ShouldExist()
		{
			var asm = typeof(LanguageRegistry).Assembly;
			var stream = asm.GetManifestResourceStream(ResourceName);

			Assert.NotNull(stream);
		}

		[Fact]
		public void EmbeddedResource_ShouldBeReadable()
		{
			var asm = typeof(LanguageRegistry).Assembly;
			using var stream = asm.GetManifestResourceStream(ResourceName);

			Assert.NotNull(stream);

			using var reader = new StreamReader(stream!);
			var json = reader.ReadToEnd();

			Assert.False(string.IsNullOrWhiteSpace(json));
		}

		// ------------------------------------------------------------
		// 2. Registry Loading Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldLoadSuccessfully()
		{
			var registry = new LanguageRegistry();
			Assert.NotEmpty(registry.Languages);
		}

		[Fact]
		public void Registry_ShouldLoadAllLanguagesWithRequiredFields()
		{
			var registry = new LanguageRegistry();

			foreach (var lang in registry.Languages.Values)
			{
				Assert.False(string.IsNullOrWhiteSpace(lang.Name));
				Assert.False(string.IsNullOrWhiteSpace(lang.Iso639_3));
			}
		}

		// ------------------------------------------------------------
		// 3. Uniqueness Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldHaveUniqueIso3Codes()
		{
			var registry = new LanguageRegistry();
			var iso3 = registry.Languages.Values.Select(l => l.Iso639_3);

			Assert.Equal(iso3.Count(), iso3.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		}

		[Fact]
		public void Registry_ShouldHaveUniqueNames()
		{
			var registry = new LanguageRegistry();
			var names = registry.Languages.Values.Select(l => l.Name);

			Assert.Equal(names.Count(), names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		}

		// ------------------------------------------------------------
		// 4. Normalization Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_IsoCodes_ShouldBeLowercase()
		{
			var registry = new LanguageRegistry();

			foreach (var lang in registry.Languages.Values)
			{
				Assert.Equal(lang.Iso639_3, lang.Iso639_3.ToLowerInvariant());

				if (lang.Iso639_1 != null)
					Assert.Equal(lang.Iso639_1, lang.Iso639_1.ToLowerInvariant());

				if (lang.Iso639_2 != null)
					Assert.Equal(lang.Iso639_2, lang.Iso639_2.ToLowerInvariant());
			}
		}

		[Fact]
		public void Registry_IsoCodes_ShouldBeTrimmed()
		{
			var registry = new LanguageRegistry();

			foreach (var lang in registry.Languages.Values)
			{
				Assert.Equal(lang.Iso639_3, lang.Iso639_3.Trim());

				if (lang.Iso639_1 != null)
					Assert.Equal(lang.Iso639_1, lang.Iso639_1.Trim());

				if (lang.Iso639_2 != null)
					Assert.Equal(lang.Iso639_2, lang.Iso639_2.Trim());
			}
		}

		// ------------------------------------------------------------
		// 5. Enum Validation Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldContainValidEnumValues()
		{
			var registry = new LanguageRegistry();

			foreach (var lang in registry.Languages.Values)
			{
				Assert.True(Enum.IsDefined(lang.Scope));
				Assert.True(Enum.IsDefined(lang.Type));
			}
		}

		// ------------------------------------------------------------
		// 6. Sanity Tests
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldContainEnglish()
		{
			var registry = new LanguageRegistry();

			Assert.True(registry.Languages.TryGetValue("eng", out var english));
			Assert.Equal("English", english!.Name);
		}

		[Fact]
		public void Registry_ShouldNotContainNullValues()
		{
			var registry = new LanguageRegistry();

			foreach (var lang in registry.Languages.Values)
			{
				Assert.NotNull(lang.Name);
				Assert.NotNull(lang.Iso639_3);
			}
		}
	}
}
