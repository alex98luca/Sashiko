using System.Text.Json;
using Sashiko.Core.Data.Json;
using Sashiko.Validation;

namespace Sashiko.Registries.Json
{
	public sealed class JsonRegistryObjectLoader<T>
	{
		private readonly ISchemaValidator _schemaValidator;
		private readonly JsonObjectLoader<T> _embeddedLoader;
		private readonly JsonObjectLoader<T> _externalLoader;

		private readonly JsonSerializerOptions _embeddedOptions;
		private readonly JsonSerializerOptions _externalOptions;

		public JsonRegistryObjectLoader(
			ISchemaValidator schemaValidator,
			JsonSerializerOptions? embeddedOptions = null,
			JsonSerializerOptions? externalOptions = null)
		{
			_schemaValidator = schemaValidator;

			// If only one is provided, reuse it
			_embeddedOptions = embeddedOptions ?? externalOptions ?? new JsonSerializerOptions();
			_externalOptions = externalOptions ?? embeddedOptions ?? new JsonSerializerOptions();

			_embeddedLoader = new JsonObjectLoader<T>(_embeddedOptions);
			_externalLoader = new JsonObjectLoader<T>(_externalOptions);
		}

		// ------------------------------------------------------------
		// EMBEDDED (trusted)
		// ------------------------------------------------------------
		public T LoadEmbedded(string json, string source)
		{
			// Embedded JSON is trusted → no validation, no wrapping
			return _embeddedLoader.Load(json);
		}

		// ------------------------------------------------------------
		// EXTERNAL (untrusted)
		// ------------------------------------------------------------
		public T LoadExternal(string json, string source)
		{
			EnsureValidJson(json, source);

			_schemaValidator.Validate<T>(
				json,
				new ValidationContext
				{
					Source = source,
					IgnoreCase = _externalOptions.PropertyNameCaseInsensitive,
					Metadata = new Dictionary<string, object>
					{
						["IsEmbedded"] = false,
						["JsonOptions"] = _externalOptions,
						["RegistryType"] = typeof(T).FullName!
					}
				});

			EnsureDeserializable<T>(json, source);

			return _externalLoader.Load(json);
		}

		// ------------------------------------------------------------
		// Helpers
		// ------------------------------------------------------------

		private void EnsureValidJson(string json, string source)
		{
			try
			{
				var docOptions = new JsonDocumentOptions
				{
					AllowTrailingCommas = _externalOptions.AllowTrailingCommas,
					CommentHandling = _externalOptions.ReadCommentHandling
				};

				JsonDocument.Parse(json, docOptions);
			}
			catch (JsonException ex)
			{
				throw new RegistryLoadException(
						$"Invalid JSON format in '{source}'.",
						source,
						typeof(T),
						ex);
			}
		}

		private void EnsureDeserializable<TModel>(string json, string source)
		{
			try
			{
				var result = JsonSerializer.Deserialize<TModel>(json, _externalOptions);
				if (result == null)
					throw new RegistryLoadException(
						$"Deserialization returned null for '{source}'.",
						source,
						typeof(T));
			}
			catch (RegistryLoadException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new RegistryLoadException(
					$"Deserialization failed for '{source}'.",
					source,
					typeof(T),
					ex);
			}
		}
	}
}
