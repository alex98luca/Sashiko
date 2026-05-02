using Sashiko.Maintenance.Libraries.Names;
using Sashiko.Names.Api;

namespace Sashiko.Maintenance.Tests.Libraries.Names
{
	public sealed class NamePoolMaintenanceTests
	{
		private sealed class FakePolisher : INamePoolPolisher
		{
			public string? ReceivedPath { get; private set; }

			public Task<NamePolishResult> PolishAsync(string dataDirectory)
			{
				ReceivedPath = dataDirectory;
				return Task.FromResult(new NamePolishResult(1, 0));
			}
		}

		[Fact]
		public async Task PolishEmbeddedNamesAsync_ShouldCallPolisherWithCorrectPath()
		{
			var polisher = new FakePolisher();

			string FakeLocator(string projectName, Type type)
			{
				Assert.Equal("Sashiko.Names", projectName);
				Assert.Equal(typeof(NameService), type);

				return "/fake/project/root";
			}

			await NamePoolMaintenance.PolishEmbeddedNamesAsync(
				polisher,
				FakeLocator
			);

			Assert.NotNull(polisher.ReceivedPath);
			Assert.Equal(
				"/fake/project/root/Data",
				polisher.ReceivedPath!.Replace("\\", "/")
			);
		}
	}
}
