namespace Sashiko.Maintenance.Libraries.Languages.Updating
{
	internal interface ILanguageRegistryUpdater
	{
		Task UpdateAsync(string outputPath);
	}
}
