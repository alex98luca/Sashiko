using Sashiko.Maintenance.Shared;
using Sashiko.Names.Api;

namespace Sashiko.Maintenance.Libraries.Names
{
	internal static class NamePoolMaintenance
	{
		internal static async Task PolishEmbeddedNamesAsync()
		{
			var result = await PolishEmbeddedNamesAsync(
				new NamePoolPolisher(),
				AssemblyProjectLocator.FindProjectRoot
			);

			Console.WriteLine(
				$"Polished {result.FilesProcessed} names.json file(s). " +
				$"Removed {result.ValuesRemoved} duplicate or blank value(s)."
			);
		}

		internal static Task<NamePolishResult> PolishEmbeddedNamesAsync(
			INamePoolPolisher polisher,
			Func<string, Type, string> projectLocator)
		{
			var namesRoot = projectLocator(
				"Sashiko.Names",
				typeof(NameService)
			);

			var dataDirectory = Path.Combine(namesRoot, "Data");

			return polisher.PolishAsync(dataDirectory);
		}
	}
}
