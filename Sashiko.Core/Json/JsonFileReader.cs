using System.Text.Json;

namespace Sashiko.Core.Json
{
	public static class JsonFileReader
	{
		public static T Deserialize<T>(
			string json,
			string source,
			JsonSerializerOptions? options = null)
		{
			try
			{
				return JsonSerializer.Deserialize<T>(json, options ?? new JsonSerializerOptions())
					?? throw new InvalidOperationException($"Invalid JSON in: {source}");
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException($"Invalid JSON in: {source}", ex);
			}
		}
	}
}
