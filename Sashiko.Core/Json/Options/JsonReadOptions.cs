using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sashiko.Core.Json.Options
{
	public static class JsonReadOptions
	{
		public static readonly JsonSerializerOptions Strict = new()
		{
			PropertyNameCaseInsensitive = false,
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = false,
			Converters = { new JsonStringEnumConverter() }
		};

		public static readonly JsonSerializerOptions Forgiving = new()
		{
			PropertyNameCaseInsensitive = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true,
			Converters = { new JsonStringEnumConverter() }
		};

		public static JsonDocumentOptions ToDocumentOptions(JsonSerializerOptions options)
		{
			return new JsonDocumentOptions
			{
				AllowTrailingCommas = options.AllowTrailingCommas,
				CommentHandling = options.ReadCommentHandling
			};
		}
	}
}
