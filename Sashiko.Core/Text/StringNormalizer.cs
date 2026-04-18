namespace Sashiko.Core.Text
{
	public static class StringNormalizer
	{
		public static string? NormalizeOptional(string? value)
			=> string.IsNullOrWhiteSpace(value) ? null : value.Trim();

		public static IReadOnlyList<string> NormalizeCollection(IEnumerable<string> values)
		{
			if (values is null)
				throw new ArgumentException("Value collection cannot be null.");

			var list = values
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim())
				.ToList();

			if (list.Count == 0)
				throw new ArgumentException("Value collection cannot be empty.");

			return list.AsReadOnly();
		}
	}
}
