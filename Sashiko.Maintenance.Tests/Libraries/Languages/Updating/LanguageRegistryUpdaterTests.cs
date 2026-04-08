using System.Text.Json;
using Sashiko.Maintenance.Libraries.Languages.Updating;

namespace Sashiko.Maintenance.Tests.Libraries.Languages.Updating
{
	public sealed class LanguageRegistryUpdaterTests
	{
		private const string Header =
			"Id\tPart2B\tPart2T\tPart1\tScope\tType\tRef_Name";

		private static string SampleTsv =>
			Header + "\n" +
			"eng\teng\teng\ten\tI\tL\tEnglish\n" +
			"ita\tita\tita\tit\tI\tL\tItalian";

		private static HttpClient CreateFakeHttp(string tsv)
		{
			var handler = new FakeHttpMessageHandler(tsv);
			return new HttpClient(handler);
		}

		[Fact]
		public async Task UpdateAsync_ShouldWriteValidJson()
		{
			// Arrange
			var http = CreateFakeHttp(SampleTsv);
			var updater = new LanguageRegistryUpdater(http);

			var tempFile = Path.GetTempFileName();

			try
			{
				// Act
				await updater.UpdateAsync(tempFile);

				// Assert
				Assert.True(File.Exists(tempFile));

				var json = await File.ReadAllTextAsync(tempFile);
				Assert.False(string.IsNullOrWhiteSpace(json));

				var doc = JsonDocument.Parse(json);
				Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);

				var items = doc.RootElement.EnumerateArray().ToList();
				Assert.Equal(2, items.Count);

				var english = items.First(x => x.GetProperty("Iso639_3").GetString() == "eng");
				Assert.Equal("English", english.GetProperty("Name").GetString());
				Assert.Equal("en", english.GetProperty("Iso639_1").GetString());
				Assert.Equal("eng", english.GetProperty("Iso639_2").GetString());
				Assert.Equal("Individual", english.GetProperty("Scope").GetString());
				Assert.Equal("Living", english.GetProperty("Type").GetString());
			}
			finally
			{
				File.Delete(tempFile);
			}
		}

		[Fact]
		public async Task UpdateAsync_ShouldBeDeterministic()
		{
			// Arrange
			var http = CreateFakeHttp(SampleTsv);
			var updater = new LanguageRegistryUpdater(http);

			var file1 = Path.GetTempFileName();
			var file2 = Path.GetTempFileName();

			try
			{
				// Act
				await updater.UpdateAsync(file1);
				await updater.UpdateAsync(file2);

				// Assert
				var json1 = await File.ReadAllTextAsync(file1);
				var json2 = await File.ReadAllTextAsync(file2);

				Assert.Equal(json1, json2);
			}
			finally
			{
				File.Delete(file1);
				File.Delete(file2);
			}
		}
	}

	internal sealed class FakeHttpMessageHandler : HttpMessageHandler
	{
		private readonly string _response;

		public FakeHttpMessageHandler(string response)
		{
			_response = response;
		}

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			return Task.FromResult(new HttpResponseMessage
			{
				StatusCode = System.Net.HttpStatusCode.OK,
				Content = new StringContent(_response)
			});
		}
	}
}
