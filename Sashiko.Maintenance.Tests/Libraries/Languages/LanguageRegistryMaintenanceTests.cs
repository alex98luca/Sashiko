using Sashiko.Languages.Model;
using Sashiko.Maintenance.Libraries.Languages;
using Sashiko.Maintenance.Libraries.Languages.Updating;

namespace Sashiko.Maintenance.Tests.Libraries.Languages
{
	public sealed class LanguageRegistryMaintenanceTests
	{
		private sealed class FakeUpdater : ILanguageRegistryUpdater
		{
			public string? ReceivedPath { get; private set; }

			public Task UpdateAsync(string outputPath)
			{
				ReceivedPath = outputPath;
				return Task.CompletedTask;
			}
		}

		[Fact]
		public async Task UpdateEmbeddedLanguagesAsync_ShouldCallUpdaterWithCorrectPath()
		{
			// Arrange
			var updater = new FakeUpdater();

			string FakeLocator(string projectName, Type type)
			{
				Assert.Equal("Sashiko.Languages", projectName);
				Assert.Equal(typeof(Language), type);

				// Simulate project root
				return "/fake/project/root";
			}

			// Act
			await LanguageRegistryMaintenance.UpdateEmbeddedLanguagesAsync(
				updater,
				FakeLocator
			);

			// Assert
			Assert.NotNull(updater.ReceivedPath);
			Assert.Equal(
				"/fake/project/root/Data/Languages/languages.json",
				updater.ReceivedPath!.Replace("\\", "/")
			);
		}
	}
}
