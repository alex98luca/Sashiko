using System.Text.Json;
using Sashiko.Maintenance.Libraries.Names;

namespace Sashiko.Maintenance.Tests.Libraries.Names
{
	public sealed class NamePoolPolisherTests
	{
		[Fact]
		public async Task PolishAsync_ShouldSortTrimAndRemoveDuplicatesFromEachNamesFile()
		{
			var root = CreateTemporaryDirectory();

			try
			{
				var ita = Path.Combine(root, "ita");
				var rus = Path.Combine(root, "rus");
				Directory.CreateDirectory(ita);
				Directory.CreateDirectory(rus);

				var itaPath = Path.Combine(ita, "names.json");
				var rusPath = Path.Combine(rus, "names.json");
				var ignoredPath = Path.Combine(ita, "rules.json");

				await File.WriteAllTextAsync(
					itaPath,
					"""
					{
					  "MaleFirstNames": [ "Zoey", " Adam ", "Adam", "", "Anna" ],
					  "FemaleFirstNames": [ "Mia", "Clara", "Mia" ],
					  "UnisexFirstNames": [],
					  "MaleLastNames": [],
					  "FemaleLastNames": [],
					  "LastNames": [],
					  "Prefixes": [],
					  "Suffixes": []
					}
					""");

				await File.WriteAllTextAsync(
					rusPath,
					"""
					{
					  "MaleFirstNames": [],
					  "FemaleFirstNames": [],
					  "UnisexFirstNames": [],
					  "MaleLastNames": [],
					  "FemaleLastNames": [],
					  "LastNames": [ "Petrov", "Ivanov" ],
					  "Prefixes": [],
					  "Suffixes": []
					}
					""");

				await File.WriteAllTextAsync(ignoredPath, "{ \"Untouched\": true }");

				var polisher = new NamePoolPolisher();

				var result = await polisher.PolishAsync(root);

				Assert.Equal(new NamePolishResult(2, 3), result);

				Assert.Equal(
					new[] { "Adam", "Anna", "Zoey" },
					ReadArray(itaPath, "MaleFirstNames")
				);
				Assert.Equal(
					new[] { "Clara", "Mia" },
					ReadArray(itaPath, "FemaleFirstNames")
				);
				Assert.Equal(
					new[] { "Ivanov", "Petrov" },
					ReadArray(rusPath, "LastNames")
				);
				Assert.Equal("{ \"Untouched\": true }", await File.ReadAllTextAsync(ignoredPath));
			}
			finally
			{
				Directory.Delete(root, recursive: true);
			}
		}

		[Fact]
		public async Task PolishAsync_ShouldThrow_WhenDataDirectoryDoesNotExist()
		{
			var polisher = new NamePoolPolisher();
			var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

			await Assert.ThrowsAsync<DirectoryNotFoundException>(
				() => polisher.PolishAsync(missing));
		}

		private static string CreateTemporaryDirectory()
		{
			var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(path);
			return path;
		}

		private static IReadOnlyList<string> ReadArray(string path, string propertyName)
		{
			using var document = JsonDocument.Parse(File.ReadAllText(path));

			return document.RootElement
				.GetProperty(propertyName)
				.EnumerateArray()
				.Select(value => value.GetString() ?? string.Empty)
				.ToArray();
		}
	}
}
