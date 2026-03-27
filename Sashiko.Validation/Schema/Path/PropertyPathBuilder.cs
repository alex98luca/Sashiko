namespace Sashiko.Validation.Schema.Path
{
	internal static class PropertyPathBuilder
	{
		public static string Combine(string prefix, string name)
		{
			if (string.IsNullOrEmpty(prefix))
				return name;

			return prefix + "." + name;
		}
	}
}
