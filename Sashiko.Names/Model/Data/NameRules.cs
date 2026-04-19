using System.Text.Json.Serialization;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Model.Data
{
	internal sealed record NameRules
	{
		// ------------------------------------------------------------
		// Given Names
		// ------------------------------------------------------------
		public int GivenNameCountMin { get; }
		public int GivenNameCountMax { get; }
		public double UnisexFirstNameProbability { get; }

		// ------------------------------------------------------------
		// Patronymics
		// ------------------------------------------------------------
		public bool UsesPatronymic { get; }
		public string? PatronymicPatternMale { get; }
		public string? PatronymicPatternFemale { get; }
		public double PatronymicProbability { get; }

		// ------------------------------------------------------------
		// Matronymics
		// ------------------------------------------------------------
		public bool UsesMatronymic { get; }
		public string? MatronymicPatternMale { get; }
		public string? MatronymicPatternFemale { get; }
		public double MatronymicProbability { get; }

		// ------------------------------------------------------------
		// Surnames
		// ------------------------------------------------------------
		public bool UsesDoubleLastName { get; }
		public double DoubleLastNameProbability { get; }

		public bool UsesGenderedLastNames { get; }

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
			double patronymicProbability,
			bool usesMatronymic,
			string? matronymicPatternMale,
			string? matronymicPatternFemale,
			double matronymicProbability,
			bool usesDoubleLastName,
			double doubleLastNameProbability,
			bool usesGenderedLastNames,
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
			PatronymicProbability = patronymicProbability;

			UsesMatronymic = usesMatronymic;
			MatronymicPatternMale = matronymicPatternMale;
			MatronymicPatternFemale = matronymicPatternFemale;
			MatronymicProbability = matronymicProbability;

			UsesDoubleLastName = usesDoubleLastName;
			DoubleLastNameProbability = doubleLastNameProbability;

			UsesGenderedLastNames = usesGenderedLastNames;

			Order = order;

			AllowPrefixes = allowPrefixes;
			PrefixProbability = prefixProbability;

			AllowSuffixes = allowSuffixes;
			SuffixProbability = suffixProbability;
		}
	}
}
