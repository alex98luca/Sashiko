using Sashiko.Maintenance.Libraries.Languages;

namespace Sashiko.Maintenance.Commands
{
	internal class LanguageCommandHandler : ICommandHandler
	{
		internal Func<Task>? UpdateHook { get; set; }
			= () => LanguageRegistryMaintenance.UpdateEmbeddedLanguagesAsync();

		public async Task DispatchAsync(string? command)
		{
			switch (command)
			{
				case "update":
					await (UpdateHook?.Invoke() ?? Task.CompletedTask);
					break;

				default:
					Console.WriteLine("Unknown languages command.");
					Console.WriteLine("Available: update");
					break;
			}
		}
	}
}
