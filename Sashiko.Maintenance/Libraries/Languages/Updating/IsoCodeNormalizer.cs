namespace Sashiko.Maintenance.Libraries.Languages.Updating
{
	internal static class IsoCodeNormalizer
	{
		internal static string? Normalize(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			return value.Trim().ToLowerInvariant();
		}
	}
}
