namespace Sashiko.Maintenance
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			await CommandDispatcher.DispatchAsync(args);
		}
	}
}
