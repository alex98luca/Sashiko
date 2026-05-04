using Sashiko.Core.Json;
using Sashiko.Core.Json.Options;
using Sashiko.Names.Model.Data;

namespace Sashiko.Maintenance.Libraries.Names
{
	internal sealed class NamePoolPolisher : INamePoolPolisher
	{
		private static readonly StringComparer NameComparer =
			StringComparer.InvariantCultureIgnoreCase;

		public async Task<NamePolishResult> PolishAsync(string dataDirectory)
		{
			if (!Directory.Exists(dataDirectory))
				throw new DirectoryNotFoundException(
					$"Names data directory does not exist: {dataDirectory}");

			var files = Directory
				.EnumerateFiles(dataDirectory, "names.json", SearchOption.AllDirectories)
				.OrderBy(path => path, StringComparer.Ordinal)
				.ToArray();

			var removed = 0;

			foreach (var file in files)
			{
				removed += await PolishFileAsync(file);
			}

			return new NamePolishResult(files.Length, removed);
		}

		private static async Task<int> PolishFileAsync(string path)
		{
			var json = await File.ReadAllTextAsync(path);
			var pool = JsonFileReader.Deserialize<NamePool>(
				json,
				path,
				JsonWriteOptions.Indented
			);

			var polishedPool = new NamePool
			{
				MaleFirstNames = PolishValues(pool.MaleFirstNames),
				FemaleFirstNames = PolishValues(pool.FemaleFirstNames),
				UnisexFirstNames = PolishValues(pool.UnisexFirstNames),
				MaleLastNames = PolishValues(pool.MaleLastNames),
				FemaleLastNames = PolishValues(pool.FemaleLastNames),
				LastNames = PolishValues(pool.LastNames),
				Prefixes = PolishValues(pool.Prefixes),
				Suffixes = PolishValues(pool.Suffixes)
			};

			var removed = CountRemovedValues(pool, polishedPool);

			JsonFileWriter.Write(path, polishedPool, JsonWriteOptions.Indented);

			return removed;
		}

		private static string[] PolishValues(IReadOnlyList<string> values)
			=> values
				.Select(value => value.Trim())
				.Where(value => value.Length > 0)
				.Distinct(NameComparer)
				.OrderBy(value => value, NameComparer)
				.ThenBy(value => value, StringComparer.Ordinal)
				.ToArray();

		private static int CountRemovedValues(NamePool original, NamePool polished)
			=> CountRemovedValues(original.MaleFirstNames, polished.MaleFirstNames)
				+ CountRemovedValues(original.FemaleFirstNames, polished.FemaleFirstNames)
				+ CountRemovedValues(original.UnisexFirstNames, polished.UnisexFirstNames)
				+ CountRemovedValues(original.MaleLastNames, polished.MaleLastNames)
				+ CountRemovedValues(original.FemaleLastNames, polished.FemaleLastNames)
				+ CountRemovedValues(original.LastNames, polished.LastNames)
				+ CountRemovedValues(original.Prefixes, polished.Prefixes)
				+ CountRemovedValues(original.Suffixes, polished.Suffixes);

		private static int CountRemovedValues(
			IReadOnlyList<string> original,
			IReadOnlyList<string> polished)
			=> original.Count - polished.Count;
	}
}
