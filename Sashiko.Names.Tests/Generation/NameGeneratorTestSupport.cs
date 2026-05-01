using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Model.Data;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Tests.Generation
{
	internal static class NameGeneratorTestSupport
	{
		public static TestNameRegistry CreateRegistry(
			NamePool? pool = null,
			NameRules? rules = null)
			=> new(new NameEntry(
				LanguageId.Ita,
				pool ?? CreatePool(),
				rules ?? CreateRules()));

		public static NamePool CreatePool(
			IReadOnlyList<string>? prefixes = null,
			IReadOnlyList<string>? suffixes = null)
			=> new(
				maleFirstNames: new[] { "Marco", "Luca" },
				femaleFirstNames: new[] { "Anna", "Maria" },
				unisexFirstNames: new[] { "Alex" },
				maleLastNames: new[] { "Ivanov" },
				femaleLastNames: new[] { "Ivanova" },
				lastNames: new[] { "Rossi", "Bianchi" },
				prefixes: prefixes ?? new[] { "Dr." },
				suffixes: suffixes ?? new[] { "Jr." });

		public static NameRules CreateRules(
			int givenNameCountMin = 1,
			int givenNameCountMax = 1,
			double unisexFirstNameProbability = 0,
			bool usesPatronymic = false,
			string? patronymicPatternMale = null,
			string? patronymicPatternFemale = null,
			double patronymicProbability = 0,
			bool usesMatronymic = false,
			string? matronymicPatternMale = null,
			string? matronymicPatternFemale = null,
			double matronymicProbability = 0,
			bool usesDoubleLastName = false,
			double doubleLastNameProbability = 0,
			bool usesGenderedLastNames = false,
			NameOrder order = NameOrder.FirstLast,
			bool allowPrefixes = false,
			double prefixProbability = 0,
			bool allowSuffixes = false,
			double suffixProbability = 0)
			=> new(
				givenNameCountMin,
				givenNameCountMax,
				unisexFirstNameProbability,
				usesPatronymic,
				patronymicPatternMale,
				patronymicPatternFemale,
				patronymicProbability,
				usesMatronymic,
				matronymicPatternMale,
				matronymicPatternFemale,
				matronymicProbability,
				usesDoubleLastName,
				doubleLastNameProbability,
				usesGenderedLastNames,
				order,
				allowPrefixes,
				prefixProbability,
				allowSuffixes,
				suffixProbability);
	}

	internal sealed class TestNameRegistry : INameRegistry
	{
		private readonly IReadOnlyDictionary<LanguageId, NameEntry> _entries;

		public TestNameRegistry(params NameEntry[] entries)
		{
			_entries = entries.ToDictionary(entry => entry.Language);
		}

		public NameEntry Get(LanguageId language)
			=> _entries[language];

		public bool TryGet(LanguageId language, out NameEntry? entry)
			=> _entries.TryGetValue(language, out entry);

		public IEnumerable<NameEntry> All => _entries.Values;
	}

	internal sealed class DeterministicRandomPicker : IRandomPicker
	{
		private readonly Queue<object> _picks;
		private readonly Queue<bool> _chanceResults;

		public DeterministicRandomPicker(params object[] picks)
			: this(null, picks)
		{
		}

		public DeterministicRandomPicker(
			IEnumerable<bool>? chanceResults = null,
			params object[] picks)
		{
			_picks = new Queue<object>(picks);
			_chanceResults = new Queue<bool>(chanceResults ?? Array.Empty<bool>());
		}

		public T Pick<T>(IEnumerable<T> values)
		{
			var list = values.ToList();

			if (list.Count == 0)
				throw new InvalidOperationException("Cannot pick from an empty list.");

			if (_picks.Count > 0 && _picks.Peek() is T next)
			{
				_picks.Dequeue();
				return next;
			}

			return list[0];
		}

		public T Pick<T>(IReadOnlyList<T> values)
			=> Pick(values.AsEnumerable());

		public KeyValuePair<TKey, TValue> Pick<TKey, TValue>(
			IReadOnlyDictionary<TKey, TValue> values)
			=> Pick(values.ToList());

		public bool Chance(double probability)
		{
			if (_chanceResults.Count > 0)
				return _chanceResults.Dequeue();

			return probability >= 1;
		}
	}
}
