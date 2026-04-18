using System.Text.Json.Serialization;

namespace Sashiko.Names.Model.Data
{
	internal sealed record NamePool
	{
		public IReadOnlyList<string> MaleFirstNames { get; }
		public IReadOnlyList<string> FemaleFirstNames { get; }
		public IReadOnlyList<string> UnisexFirstNames { get; }

		public IReadOnlyList<string> LastNames { get; }

		public IReadOnlyList<string> Prefixes { get; }
		public IReadOnlyList<string> Suffixes { get; }

		[JsonConstructor]
		internal NamePool(
			IReadOnlyList<string> maleFirstNames,
			IReadOnlyList<string> femaleFirstNames,
			IReadOnlyList<string> unisexFirstNames,
			IReadOnlyList<string> lastNames,
			IReadOnlyList<string> prefixes,
			IReadOnlyList<string> suffixes)
		{
			MaleFirstNames = maleFirstNames;
			FemaleFirstNames = femaleFirstNames;
			UnisexFirstNames = unisexFirstNames;

			LastNames = lastNames;

			Prefixes = prefixes;
			Suffixes = suffixes;
		}
	}
}
