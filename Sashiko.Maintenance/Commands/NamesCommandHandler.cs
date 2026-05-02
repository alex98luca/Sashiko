using Sashiko.Maintenance.Libraries.Names;

namespace Sashiko.Maintenance.Commands
{
	internal class NamesCommandHandler : ICommandHandler
	{
		internal Func<Task>? PolishHook { get; set; }
			= () => NamePoolMaintenance.PolishEmbeddedNamesAsync();

		public async Task DispatchAsync(string? command)
		{
			switch (command)
			{
				case "polish":
					await (PolishHook?.Invoke() ?? Task.CompletedTask);
					break;

				default:
					Console.WriteLine("Unknown names command.");
					Console.WriteLine("Available: polish");
					break;
			}
		}
	}
}
