using System.Reflection;
using Sashiko.Core.Json.Options;
using Sashiko.Languages.Model;
using Sashiko.Registries.Json;
using Sashiko.Validation.Validators.Json;

namespace Sashiko.Languages.Registry
{
	internal sealed class LanguageRegistry
	{
		private readonly IReadOnlyDictionary<string, Language> _languages;

		internal IReadOnlyDictionary<string, Language> Languages => _languages;

		internal LanguageRegistry()
		{
			_languages = LoadEmbeddedLanguages();
		}

		private static Dictionary<string, Language> LoadEmbeddedLanguages()
		{
			var asm = Assembly.GetExecutingAssembly();
			var resourceName = "Sashiko.Languages.Data.Languages.languages.json";

			using var stream = asm.GetManifestResourceStream(resourceName)
				?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

			using var reader = new StreamReader(stream);
			var json = reader.ReadToEnd();

			var loader = new JsonRegistryListLoader<Language>(
				schemaValidator: new JsonSchemaValidator(),
				embeddedOptions: JsonReadOptions.Strict
			);

			var list = loader.LoadEmbedded(json, resourceName);

			return list.ToDictionary(
				l => l.Iso639_3!,
				l => l,
				StringComparer.OrdinalIgnoreCase
			);
		}
	}
}
