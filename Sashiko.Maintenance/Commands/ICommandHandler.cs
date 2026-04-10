namespace Sashiko.Maintenance.Commands
{
	internal interface ICommandHandler
	{
		Task DispatchAsync(string? command);
	}
}
