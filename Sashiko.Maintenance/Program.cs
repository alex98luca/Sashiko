namespace Sashiko.Maintenance
{
	internal static class Program
	{
		private static async Task Main(string[] args)
		{
			await CommandDispatcher.DispatchAsync(args);
		}
	}
}
