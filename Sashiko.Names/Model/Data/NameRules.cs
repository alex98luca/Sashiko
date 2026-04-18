using System.Text.Json.Serialization;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Model.Data
{
	internal sealed record NameRules
	{
		// ------------------------------------------------------------
		// Given Names
		// ------------------------------------------------------------

		/// <summary>
		/// Minimum number of given names typically used in this culture.
		/// </summary>
		public int GivenNameCountMin { get; }

		/// <summary>
		/// Maximum number of given names typically used in this culture.
		/// </summary>
		public int GivenNameCountMax { get; }

		/// <summary>
		/// Probability of selecting a unisex name when generating a given name.
		/// </summary>
		public double UnisexFirstNameProbability { get; }

		/// <summary>
		/// Whether this culture uses patronymics (e.g., Russian).
		/// </summary>
		public bool UsesPatronymic { get; }

		public string? PatronymicPatternMale { get; }
		public string? PatronymicPatternFemale { get; }

		// ------------------------------------------------------------
		// Surnames
		// ------------------------------------------------------------

		public bool UsesDoubleLastName { get; }
		public double DoubleLastNameProbability { get; }

		public bool GenderedLastNames { get; }

		// ------------------------------------------------------------
		// Name Order
		// ------------------------------------------------------------

		public NameOrder Order { get; }

		// ------------------------------------------------------------
		// Prefixes / Suffixes
		// ------------------------------------------------------------

		public bool AllowPrefixes { get; }
		public double PrefixProbability { get; }

		public bool AllowSuffixes { get; }
		public double SuffixProbability { get; }

		[JsonConstructor]
		internal NameRules(
			int givenNameCountMin,
			int givenNameCountMax,
			double unisexFirstNameProbability,
			bool usesPatronymic,
			string? patronymicPatternMale,
			string? patronymicPatternFemale,
			bool usesDoubleLastName,
			double doubleLastNameProbability,
			bool genderedLastNames,
			NameOrder order,
			bool allowPrefixes,
			double prefixProbability,
			bool allowSuffixes,
			double suffixProbability)
		{
			GivenNameCountMin = givenNameCountMin;
			GivenNameCountMax = givenNameCountMax;

			UnisexFirstNameProbability = unisexFirstNameProbability;

			UsesPatronymic = usesPatronymic;
			PatronymicPatternMale = patronymicPatternMale;
			PatronymicPatternFemale = patronymicPatternFemale;

			UsesDoubleLastName = usesDoubleLastName;
			DoubleLastNameProbability = doubleLastNameProbability;

			GenderedLastNames = genderedLastNames;

			Order = order;

			AllowPrefixes = allowPrefixes;
			PrefixProbability = prefixProbability;

			AllowSuffixes = allowSuffixes;
			SuffixProbability = suffixProbability;
		}
	}
}
