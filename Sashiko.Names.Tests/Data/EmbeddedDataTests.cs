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
		public void EmbeddedPools_ShouldMeetReleaseCoverageFloor()
		{
			var registry = new NameRegistry();

			foreach (var entry in registry.All)
			{
				AssertPoolCount(entry.Language, nameof(entry.Pool.MaleFirstNames), entry.Pool.MaleFirstNames, 150);
				AssertPoolCount(entry.Language, nameof(entry.Pool.FemaleFirstNames), entry.Pool.FemaleFirstNames, 150);

				if (entry.Rules.UsesGenderedLastNames)
				{
					AssertPoolCount(entry.Language, nameof(entry.Pool.MaleLastNames), entry.Pool.MaleLastNames, 200);
					AssertPoolCount(entry.Language, nameof(entry.Pool.FemaleLastNames), entry.Pool.FemaleLastNames, 200);
				}
				else
				{
					AssertPoolCount(entry.Language, nameof(entry.Pool.LastNames), entry.Pool.LastNames, 200);
				}

				var uniqueCoreNames = entry.Pool.MaleFirstNames
					.Concat(entry.Pool.FemaleFirstNames)
					.Concat(entry.Pool.UnisexFirstNames)
					.Concat(entry.Pool.MaleLastNames)
					.Concat(entry.Pool.FemaleLastNames)
					.Concat(entry.Pool.LastNames)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.Count();

				Assert.True(
					uniqueCoreNames >= 500,
					$"{entry.Language} should contain at least 500 unique core names, but contains {uniqueCoreNames}.");
			}
		}

		[Fact]
		public void EmbeddedPools_ShouldBeSortedAndUnique()
		{
			var registry = new NameRegistry();

			foreach (var entry in registry.All)
			{
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.MaleFirstNames), entry.Pool.MaleFirstNames);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.FemaleFirstNames), entry.Pool.FemaleFirstNames);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.UnisexFirstNames), entry.Pool.UnisexFirstNames);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.MaleLastNames), entry.Pool.MaleLastNames);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.FemaleLastNames), entry.Pool.FemaleLastNames);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.LastNames), entry.Pool.LastNames);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.Prefixes), entry.Pool.Prefixes);
				AssertSortedAndUnique(entry.Language, nameof(entry.Pool.Suffixes), entry.Pool.Suffixes);
			}
		}

		[Fact]
		public void EmbeddedPools_ShouldUseLatinReadableNameText()
		{
			var registry = new NameRegistry();

			foreach (var entry in registry.All)
			{
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.MaleFirstNames), entry.Pool.MaleFirstNames);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.FemaleFirstNames), entry.Pool.FemaleFirstNames);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.UnisexFirstNames), entry.Pool.UnisexFirstNames);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.MaleLastNames), entry.Pool.MaleLastNames);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.FemaleLastNames), entry.Pool.FemaleLastNames);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.LastNames), entry.Pool.LastNames);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.Prefixes), entry.Pool.Prefixes);
				AssertLatinReadableValues(entry.Language, nameof(entry.Pool.Suffixes), entry.Pool.Suffixes);
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

		private static void AssertSortedAndUnique(
			LanguageId language,
			string propertyName,
			IReadOnlyList<string> values)
		{
			var expected = values
				.Distinct(StringComparer.InvariantCultureIgnoreCase)
				.OrderBy(value => value, StringComparer.InvariantCultureIgnoreCase)
				.ThenBy(value => value, StringComparer.Ordinal)
				.ToArray();

			Assert.True(
				values.SequenceEqual(expected, StringComparer.Ordinal),
				$"{language}.{propertyName} should be alphabetically sorted and unique.");
		}

		private static void AssertPoolCount(
			LanguageId language,
			string propertyName,
			IReadOnlyList<string> values,
			int minimum)
		{
			Assert.True(
				values.Count >= minimum,
				$"{language}.{propertyName} should contain at least {minimum} values, but contains {values.Count}.");
		}

		private static void AssertLatinReadableValues(
			LanguageId language,
			string propertyName,
			IReadOnlyList<string> values)
		{
			foreach (var value in values)
			{
				Assert.True(
					IsLatinReadableNameText(value),
					$"{language}.{propertyName} contains non-Latin-readable name text: '{value}'.");
			}
		}

		private static bool IsLatinReadableNameText(string value)
			=> value.All(IsLatinReadableNameCharacter);

		private static bool IsLatinReadableNameCharacter(char character)
			=> AllowedLatinReadableNameCharacters.Contains(character)
				|| IsAsciiLatinLetter(character);

		private static bool IsAsciiLatinLetter(char character)
			=> character is >= 'A' and <= 'Z'
				or >= 'a' and <= 'z';

		private static readonly HashSet<char> AllowedLatinReadableNameCharacters =
			[
				' ',
				'\'',
				'-',
				'À',
				'Á',
				'Â',
				'Ã',
				'Ä',
				'Å',
				'Ç',
				'È',
				'É',
				'Ê',
				'Ë',
				'Ì',
				'Í',
				'Î',
				'Ï',
				'Ñ',
				'Ò',
				'Ó',
				'Ô',
				'Õ',
				'Ö',
				'Ù',
				'Ú',
				'Û',
				'Ü',
				'Ý',
				'à',
				'á',
				'â',
				'ã',
				'ä',
				'å',
				'ç',
				'è',
				'é',
				'ê',
				'ë',
				'ì',
				'í',
				'î',
				'ï',
				'ñ',
				'ò',
				'ó',
				'ô',
				'õ',
				'ö',
				'ù',
				'ú',
				'û',
				'ü',
				'ý',
				'Ă',
				'Â',
				'Î',
				'Ș',
				'Ț',
				'ă',
				'â',
				'î',
				'ș',
				'ț'
			];

		private static void AssertProbability(double probability)
		{
			Assert.InRange(probability, 0, 1);
		}
	}
}
