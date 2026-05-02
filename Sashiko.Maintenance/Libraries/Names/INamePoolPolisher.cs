namespace Sashiko.Maintenance.Libraries.Names
{
	internal interface INamePoolPolisher
	{
		Task<NamePolishResult> PolishAsync(string dataDirectory);
	}
}
