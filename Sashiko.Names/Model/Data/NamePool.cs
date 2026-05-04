namespace Sashiko.Names.Model.Data
{
	internal sealed record NamePool
	{
		public required IReadOnlyList<string> MaleFirstNames { get; init; }
		public required IReadOnlyList<string> FemaleFirstNames { get; init; }
		public required IReadOnlyList<string> UnisexFirstNames { get; init; }

		// NEW: Gendered surname support
		public required IReadOnlyList<string> MaleLastNames { get; init; }
		public required IReadOnlyList<string> FemaleLastNames { get; init; }

		// Fallback / non‑gendered surnames
		public required IReadOnlyList<string> LastNames { get; init; }

		public required IReadOnlyList<string> Prefixes { get; init; }
		public required IReadOnlyList<string> Suffixes { get; init; }
	}
}
