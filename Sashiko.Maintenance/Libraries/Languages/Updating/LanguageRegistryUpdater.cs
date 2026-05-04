using Sashiko.Core.Json;
using Sashiko.Core.Json.Options;

namespace Sashiko.Maintenance.Libraries.Languages.Updating
{
	internal sealed class LanguageRegistryUpdater : ILanguageRegistryUpdater
	{
		private static readonly Uri Iso6393Uri = new UriBuilder
		{
			Scheme = Uri.UriSchemeHttps,
			Host = "iso639-3.sil.org",
			Path = "sites/iso639-3/files/downloads/iso-639-3.tab"
		}.Uri;

		private readonly HttpClient _http;

		internal LanguageRegistryUpdater(HttpClient? httpClient = null)
		{
			_http = httpClient ?? new HttpClient();
		}

		public async Task UpdateAsync(string outputPath)
		{
			// 1. Download TSV
			var tsv = await _http.GetStringAsync(Iso6393Uri);

			// 2. Parse TSV → IsoSilLanguage[]
			var external = IsoSilParser.Parse(tsv);

			// 3. Convert → Language[]
			var internalList = LanguageMapper.Convert(external);

			// 4. Write JSON
			JsonFileWriter.Write(outputPath, internalList, JsonWriteOptions.Indented);
		}
	}
}
