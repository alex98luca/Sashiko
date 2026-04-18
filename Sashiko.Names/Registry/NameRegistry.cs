using System.Reflection;
using Sashiko.Core.Json.Options;
using Sashiko.Names.Model.Data;
using Sashiko.Registries.Json;
using Sashiko.Validation.Validators.Json;

namespace Sashiko.Names.Registry
{
	internal sealed class NameRegistry
	{
		private readonly IReadOnlyDictionary<string, NameEntry> _entries;

		internal IReadOnlyDictionary<string, NameEntry> Entries => _entries;

		internal NameRegistry()
		{
			_entries = LoadEmbeddedNames();
		}

		// ------------------------------------------------------------
		// EMBEDDED LOADING
		// ------------------------------------------------------------

		private static IReadOnlyDictionary<string, NameEntry> LoadEmbeddedNames()
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

			var dict = new Dictionary<string, NameEntry>(StringComparer.OrdinalIgnoreCase);

			// We detect languages by folder name: Data.<lang>.names.json
			var nameResources = resources
				.Where(r => r.EndsWith("names.json", StringComparison.OrdinalIgnoreCase))
				.ToList();

			foreach (var nameRes in nameResources)
			{
				var lang = ExtractLanguageCode(nameRes);

				var rulesRes = nameRes.Replace("names.json", "rules.json");

				if (!resources.Contains(rulesRes))
					throw new InvalidOperationException(
						$"Missing rules.json for language '{lang}' (expected: '{rulesRes}').");

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

			// Find the segment before "names"
			for (int i = 0; i < parts.Length - 1; i++)
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

		public NameEntry Get(string iso639_3)
		{
			if (!_entries.TryGetValue(iso639_3, out var entry))
				throw new KeyNotFoundException($"Name entry '{iso639_3}' not found in NameRegistry.");

			return entry;
		}

		public bool TryGet(string iso639_3, out NameEntry entry)
			=> _entries.TryGetValue(iso639_3, out entry);

		public IEnumerable<NameEntry> All => _entries.Values;
	}
}
