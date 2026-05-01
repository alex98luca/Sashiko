using System.Text.Json;
using Sashiko.Languages.Api;
using Sashiko.Names.Model.Data;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;
using Sashiko.Validation;
using Sashiko.Validation.Validators.Json;

namespace Sashiko.Names.Tests.Data
{
	public sealed class EmbeddedDataTests
	{
		private readonly JsonSchemaValidator _validator = new();

		// ------------------------------------------------------------
		// 1. Basic JSON Validity
		// ------------------------------------------------------------

		[Theory]
		[MemberData(nameof(EmbeddedResourceNames))]
		public void EmbeddedJson_ShouldBeValidUtf8Object(string resourceName)
		{
			var json = LoadEmbeddedJson(resourceName);
			using var doc = JsonDocument.Parse(json);

			Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
		}

		[Fact]
		public void SupportedLanguages_ShouldBeOfficialIso3Languages()
		{
			var service = new LanguageService();

			foreach (var language in SupportedLanguages)
			{
				var iso3 = language.ToString().ToLowerInvariant();
				var officialLanguage = service.GetIso3(iso3);

				Assert.Equal(iso3, officialLanguage.Iso639_3);
			}
		}

		[Fact]
		public void EmbeddedData_ShouldHaveNamesAndRulesForEachSupportedLanguage()
		{
			var resources = typeof(NameRegistry).Assembly
				.GetManifestResourceNames()
				.ToHashSet(StringComparer.Ordinal);

			foreach (var language in SupportedLanguages)
			{
				var code = language.ToString().ToLowerInvariant();

				Assert.Contains($"Sashiko.Names.Data.{code}.names.json", resources);
				Assert.Contains($"Sashiko.Names.Data.{code}.rules.json", resources);
			}
		}

		// ------------------------------------------------------------
		// 2. Schema Validation
		// ------------------------------------------------------------

		[Theory]
		[MemberData(nameof(NamePoolResourceNames))]
		public void EmbeddedNamesJson_ShouldMatchNamePoolSchema(string resourceName)
		{
			var context = new ValidationContext
			{
				Source = resourceName,
				IgnoreCase = false
			};

			_validator.Validate<NamePool>(LoadEmbeddedJson(resourceName), context);
		}

		[Theory]
		[MemberData(nameof(NameRulesResourceNames))]
		public void EmbeddedRulesJson_ShouldMatchNameRulesSchema(string resourceName)
		{
			var context = new ValidationContext
			{
				Source = resourceName,
				IgnoreCase = false
			};

			_validator.Validate<NameRules>(LoadEmbeddedJson(resourceName), context);
		}

		// ------------------------------------------------------------
		// 3. Product Data Sanity
		// ------------------------------------------------------------

		[Fact]
		public void Registry_ShouldLoadEmbeddedData()
		{
			var registry = new NameRegistry();

			Assert.NotEmpty(registry.All);
		}

		[Fact]
		public void EmbeddedPools_ShouldContainUsableNameData()
		{
			var registry = new NameRegistry();

			foreach (var entry in registry.All)
			{
				AssertUsableValues(entry.Pool.MaleFirstNames);
				AssertUsableValues(entry.Pool.FemaleFirstNames);

				if (entry.Pool.UnisexFirstNames.Count > 0)
					AssertUsableValues(entry.Pool.UnisexFirstNames);

				if (entry.Rules.UsesGenderedLastNames)
				{
					AssertUsableValues(entry.Pool.MaleLastNames);
					AssertUsableValues(entry.Pool.FemaleLastNames);
				}
				else
				{
					AssertUsableValues(entry.Pool.LastNames);
				}

				if (entry.Rules.AllowPrefixes)
					AssertUsableValues(entry.Pool.Prefixes);

				if (entry.Rules.AllowSuffixes)
					AssertUsableValues(entry.Pool.Suffixes);
			}
		}

		[Fact]
		public void EmbeddedRules_ShouldContainValidGenerationSettings()
		{
			var registry = new NameRegistry();

			foreach (var entry in registry.All)
			{
				var rules = entry.Rules;

				Assert.True(rules.GivenNameCountMin > 0);
				Assert.True(rules.GivenNameCountMax >= rules.GivenNameCountMin);
				AssertProbability(rules.UnisexFirstNameProbability);
				AssertProbability(rules.PatronymicProbability);
				AssertProbability(rules.MatronymicProbability);
				AssertProbability(rules.DoubleLastNameProbability);
				AssertProbability(rules.PrefixProbability);
				AssertProbability(rules.SuffixProbability);
				Assert.True(Enum.IsDefined(typeof(NameOrder), rules.Order));

				if (rules.UsesPatronymic)
				{
					Assert.Contains("{father}", rules.PatronymicPatternMale);
					Assert.Contains("{father}", rules.PatronymicPatternFemale);
				}

				if (rules.UsesMatronymic)
				{
					Assert.Contains("{mother}", rules.MatronymicPatternMale);
					Assert.Contains("{mother}", rules.MatronymicPatternFemale);
				}
			}
		}

		public static IEnumerable<object[]> EmbeddedResourceNames()
			=> NamePoolResourceNames().Concat(NameRulesResourceNames());

		public static IEnumerable<object[]> NamePoolResourceNames()
			=> SupportedLanguages.Select(language =>
				new object[] { GetResourceName(language, "names") });

		public static IEnumerable<object[]> NameRulesResourceNames()
			=> SupportedLanguages.Select(language =>
				new object[] { GetResourceName(language, "rules") });

		private static readonly LanguageId[] SupportedLanguages =
			Enum.GetValues<LanguageId>()
				.Where(language => language != LanguageId.Random)
				.ToArray();

		private static string LoadEmbeddedJson(string resourceName)
		{
			var asm = typeof(NameRegistry).Assembly;

			using var stream = asm.GetManifestResourceStream(resourceName)
				?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		private static string GetResourceName(LanguageId language, string resourceKind)
			=> $"Sashiko.Names.Data.{language.ToString().ToLowerInvariant()}.{resourceKind}.json";

		private static void AssertUsableValues(IReadOnlyList<string> values)
		{
			Assert.NotEmpty(values);

			foreach (var value in values)
			{
				Assert.False(string.IsNullOrWhiteSpace(value));
				Assert.Equal(value, value.Trim());
			}
		}

		private static void AssertProbability(double probability)
		{
			Assert.InRange(probability, 0, 1);
		}
	}
}
