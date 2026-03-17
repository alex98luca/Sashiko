using System.Text.Json;

namespace Sashiko.Core.Data.Json
{
	public sealed class JsonObjectLoader<T>
	{
		private readonly JsonSerializerOptions _options;

		public JsonObjectLoader(JsonSerializerOptions? options = null)
		{
			_options = options ?? new JsonSerializerOptions();
		}

		public T Load(string json)
		{
			return JsonSerializer.Deserialize<T>(json, _options)
				?? throw new InvalidOperationException(
					$"Failed to deserialize JSON into {typeof(T).Name}.");
		}
	}
}
