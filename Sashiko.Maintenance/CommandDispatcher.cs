using Sashiko.Maintenance.Commands;

namespace Sashiko.Maintenance
{
	internal static class CommandDispatcher
	{
		public static async Task DispatchAsync(string[] args)
		{
			Console.WriteLine("Sashiko Maintenance Tool");

			if (args.Length == 0)
			{
				PrintHelp();
				return;
			}

			var category = args[0].ToLowerInvariant();
			var command = args.Length > 1 ? args[1].ToLowerInvariant() : null;

			switch (category)
			{
				case "languages":
					await LanguageCommands.DispatchAsync(command);
					break;

				default:
					Console.WriteLine($"Unknown category: {category}");
					PrintHelp();
					break;
			}
		}

		private static void PrintHelp()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  languages update");
		}
	}
}
