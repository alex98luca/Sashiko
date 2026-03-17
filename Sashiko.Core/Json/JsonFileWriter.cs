using System.Text.Json;

namespace Sashiko.Core.Json
{
	public static class JsonFileWriter
	{
		public static void Write<T>(
			string path,
			T value,
			JsonSerializerOptions? options = null)
		{
			var dir = Path.GetDirectoryName(path);

			if (dir == null || !Directory.Exists(dir))
				throw new DirectoryNotFoundException(
					$"Target directory does not exist: {dir}"
				);

			var json = JsonSerializer.Serialize(
				value,
				options ?? new JsonSerializerOptions()
			);

			File.WriteAllText(path, json);
		}
	}
}
