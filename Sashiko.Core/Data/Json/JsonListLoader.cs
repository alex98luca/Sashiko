using System.Text.Json;

namespace Sashiko.Core.Data.Json
{
	public sealed class JsonListLoader<T>
	{
		private readonly JsonSerializerOptions _options;

		public JsonListLoader(JsonSerializerOptions? options = null)
		{
			_options = options ?? new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
		}

		public IReadOnlyList<T> Load(string json)
		{
			return JsonSerializer.Deserialize<List<T>>(json, _options)
				?? throw new InvalidOperationException(
					$"Failed to deserialize JSON list into List<{typeof(T).Name}>.");
		}
	}
}
