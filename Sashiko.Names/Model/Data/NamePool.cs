using System.Text.Json.Serialization;

namespace Sashiko.Names.Model.Data
{
	internal sealed record NamePool
	{
		public IReadOnlyList<string> MaleFirstNames { get; }
		public IReadOnlyList<string> FemaleFirstNames { get; }
		public IReadOnlyList<string> UnisexFirstNames { get; }

		// NEW: Gendered surname support
		public IReadOnlyList<string> MaleLastNames { get; }
		public IReadOnlyList<string> FemaleLastNames { get; }

		// Fallback / non‑gendered surnames
		public IReadOnlyList<string> LastNames { get; }

		public IReadOnlyList<string> Prefixes { get; }
		public IReadOnlyList<string> Suffixes { get; }

		[JsonConstructor]
		internal NamePool(
			IReadOnlyList<string> maleFirstNames,
			IReadOnlyList<string> femaleFirstNames,
			IReadOnlyList<string> unisexFirstNames,
			IReadOnlyList<string> maleLastNames,
			IReadOnlyList<string> femaleLastNames,
			IReadOnlyList<string> lastNames,
			IReadOnlyList<string> prefixes,
			IReadOnlyList<string> suffixes)
		{
			MaleFirstNames = maleFirstNames;
			FemaleFirstNames = femaleFirstNames;
			UnisexFirstNames = unisexFirstNames;

			MaleLastNames = maleLastNames;
			FemaleLastNames = femaleLastNames;
			LastNames = lastNames;

			Prefixes = prefixes;
			Suffixes = suffixes;
		}
	}
}
