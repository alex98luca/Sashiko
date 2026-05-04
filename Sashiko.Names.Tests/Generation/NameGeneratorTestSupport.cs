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
			=> new()
			{
				MaleFirstNames = MaleFirstNames,
				FemaleFirstNames = FemaleFirstNames,
				UnisexFirstNames = UnisexFirstNames,
				MaleLastNames = MaleLastNames,
				FemaleLastNames = FemaleLastNames,
				LastNames = LastNames,
				Prefixes = prefixes ?? Prefixes,
				Suffixes = suffixes ?? Suffixes
			};

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
			=> new()
			{
				GivenNameCountMin = givenNameCountMin,
				GivenNameCountMax = givenNameCountMax,
				UnisexFirstNameProbability = unisexFirstNameProbability,
				UsesPatronymic = usesPatronymic,
				PatronymicPatternMale = patronymicPatternMale,
				PatronymicPatternFemale = patronymicPatternFemale,
				PatronymicProbability = patronymicProbability,
				UsesMatronymic = usesMatronymic,
				MatronymicPatternMale = matronymicPatternMale,
				MatronymicPatternFemale = matronymicPatternFemale,
				MatronymicProbability = matronymicProbability,
				UsesDoubleLastName = usesDoubleLastName,
				DoubleLastNameProbability = doubleLastNameProbability,
				UsesGenderedLastNames = usesGenderedLastNames,
				Order = order,
				AllowPrefixes = allowPrefixes,
				PrefixProbability = prefixProbability,
				AllowSuffixes = allowSuffixes,
				SuffixProbability = suffixProbability
			};

		public static readonly string[] MaleFirstNames = ["Marco", "Luca"];
		public static readonly string[] FemaleFirstNames = ["Anna", "Maria"];
		public static readonly string[] UnisexFirstNames = ["Alex"];
		public static readonly string[] MaleLastNames = ["Ivanov"];
		public static readonly string[] FemaleLastNames = ["Ivanova"];
		public static readonly string[] LastNames = ["Rossi", "Bianchi"];
		public static readonly string[] Prefixes = ["Dr."];
		public static readonly string[] Suffixes = ["Jr."];
		public static readonly string[] DoubleMaleFirstNames = ["Marco", "Marco"];
		public static readonly string[] SingleMaleFirstName = ["Marco"];
		public static readonly string[] SingleFemaleFirstName = ["Anna"];
		public static readonly string[] SingleUnisexFirstName = ["Alex"];
		public static readonly string[] SingleMaleLastName = ["Ivanov"];
		public static readonly string[] SingleFemaleLastName = ["Ivanova"];
		public static readonly string[] SingleLastName = ["Rossi"];
		public static readonly string[] DoubleLastNames = ["Rossi", "Bianchi"];
		public static readonly bool[] FailedChance = [false];
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
