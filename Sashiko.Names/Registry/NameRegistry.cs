using System.Reflection;
using Sashiko.Core.Json.Options;
using Sashiko.Names.Model.Data;
using Sashiko.Names.Model.Enums;
using Sashiko.Registries.Json;
using Sashiko.Validation.Validators.Json;

namespace Sashiko.Names.Registry
{
	internal sealed class NameRegistry : INameRegistry
	{
		private readonly IReadOnlyDictionary<LanguageId, NameEntry> _entries;

		internal IReadOnlyDictionary<LanguageId, NameEntry> Entries => _entries;

		internal NameRegistry()
		{
			_entries = LoadEmbeddedNames();
		}

		// ------------------------------------------------------------
		// EMBEDDED LOADING
		// ------------------------------------------------------------

		private static Dictionary<LanguageId, NameEntry> LoadEmbeddedNames()
		{
			var asm = Assembly.GetExecutingAssembly();
			var resources = asm.GetManifestResourceNames();

			var loaderPool = new JsonRegistryObjectLoader<NamePool>(
				schemaValidator: new JsonSchemaValidator(),
				embeddedOptions: JsonReadOptions.Strict
			);

			var loaderRules = new JsonRegistryObjectLoader<NameRules>(
				schemaValidator: new JsonSchemaValidator(),
				embeddedOptions: JsonReadOptions.Strict
			);

			var dict = new Dictionary<LanguageId, NameEntry>();

			// Detect languages by folder name: Data.<lang>.names.json
			var nameResources = resources
				.Where(r => r.EndsWith("names.json", StringComparison.OrdinalIgnoreCase))
				.ToList();

			foreach (var nameRes in nameResources)
			{
				var iso = ExtractLanguageCode(nameRes);

				// Convert ISO → enum
				if (!Enum.TryParse<LanguageId>(iso, ignoreCase: true, out var lang))
				{
					throw new InvalidOperationException(
						$"Unsupported language '{iso}' found in embedded resources. " +
						$"Add it to LanguageId enum or remove the folder."
					);
				}

				var rulesRes = nameRes.Replace("names.json", "rules.json");

				if (!resources.Contains(rulesRes))
					throw new InvalidOperationException(
						$"Missing rules.json for language '{iso}' (expected: '{rulesRes}').");

				// Load names.json
				var poolJson = ReadResource(asm, nameRes);
				var pool = loaderPool.LoadEmbedded(poolJson, nameRes);

				// Load rules.json
				var rulesJson = ReadResource(asm, rulesRes);
				var rules = loaderRules.LoadEmbedded(rulesJson, rulesRes);

				dict[lang] = new NameEntry(lang, pool, rules);
			}

			return dict;
		}

		private static string ReadResource(Assembly asm, string resourceName)
		{
			using var stream = asm.GetManifestResourceStream(resourceName)
				?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		private static string ExtractLanguageCode(string resourceName)
		{
			// Example: Sashiko.Names.Data.ita.names.json
			var parts = resourceName.Split('.');

			for (int i = parts.Length - 2; i >= 0; i--)
			{
				if (parts[i + 1].Equals("names", StringComparison.OrdinalIgnoreCase))
					return parts[i].ToLowerInvariant();
			}

			throw new InvalidOperationException(
				$"Unable to extract language code from resource name '{resourceName}'.");
		}

		// ------------------------------------------------------------
		// PUBLIC API
		// ------------------------------------------------------------

		public NameEntry Get(LanguageId language)
		{
			if (!_entries.TryGetValue(language, out var entry))
				throw new KeyNotFoundException($"Name entry '{language}' not found in NameRegistry.");

			return entry;
		}

		public bool TryGet(LanguageId language, out NameEntry? entry)
			=> _entries.TryGetValue(language, out entry);

		public IEnumerable<NameEntry> All => _entries.Values;
	}
}
