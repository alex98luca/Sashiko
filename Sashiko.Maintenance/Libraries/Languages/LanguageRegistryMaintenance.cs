using Sashiko.Languages.Model;
using Sashiko.Maintenance.Libraries.Languages.Updating;
using Sashiko.Maintenance.Shared;

namespace Sashiko.Maintenance.Libraries.Languages
{
	internal static class LanguageRegistryMaintenance
	{
		// Entry point for maintenance CLI and tests
		internal static Task UpdateEmbeddedLanguagesAsync() =>
			UpdateEmbeddedLanguagesAsync(
				new LanguageRegistryUpdater(),
				AssemblyProjectLocator.FindProjectRoot
			);

		// Testable overload
		internal static async Task UpdateEmbeddedLanguagesAsync(
			ILanguageRegistryUpdater updater,
			Func<string, Type, string> projectLocator)
		{
			var languagesRoot = projectLocator(
				"Sashiko.Languages",
				typeof(Language)
			);

			var outputPath = Path.Combine(
				languagesRoot,
				"Data",
				"Languages",
				"languages.json"
			);

			await updater.UpdateAsync(outputPath);
		}
	}
}
