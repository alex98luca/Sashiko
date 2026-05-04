using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Model.Data
{
	internal sealed record NameRules
	{
		// ------------------------------------------------------------
		// Given Names
		// ------------------------------------------------------------
		public required int GivenNameCountMin { get; init; }
		public required int GivenNameCountMax { get; init; }
		public required double UnisexFirstNameProbability { get; init; }

		// ------------------------------------------------------------
		// Patronymics
		// ------------------------------------------------------------
		public required bool UsesPatronymic { get; init; }
		public required string? PatronymicPatternMale { get; init; }
		public required string? PatronymicPatternFemale { get; init; }
		public required double PatronymicProbability { get; init; }

		// ------------------------------------------------------------
		// Matronymics
		// ------------------------------------------------------------
		public required bool UsesMatronymic { get; init; }
		public required string? MatronymicPatternMale { get; init; }
		public required string? MatronymicPatternFemale { get; init; }
		public required double MatronymicProbability { get; init; }

		// ------------------------------------------------------------
		// Surnames
		// ------------------------------------------------------------
		public required bool UsesDoubleLastName { get; init; }
		public required double DoubleLastNameProbability { get; init; }

		public required bool UsesGenderedLastNames { get; init; }

		// ------------------------------------------------------------
		// Name Order
		// ------------------------------------------------------------
		public required NameOrder Order { get; init; }

		// ------------------------------------------------------------
		// Prefixes / Suffixes
		// ------------------------------------------------------------
		public required bool AllowPrefixes { get; init; }
		public required double PrefixProbability { get; init; }

		public required bool AllowSuffixes { get; init; }
		public required double SuffixProbability { get; init; }
	}
}
