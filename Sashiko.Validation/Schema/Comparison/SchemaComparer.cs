namespace Sashiko.Validation.Schema.Comparison
{
	public static class SchemaComparer
	{
		public static SchemaDiff Compare(
			Dictionary<string, SchemaField> expected,
			HashSet<string> actual,
			bool ignoreCase = false)
		{
			var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

			// Normalize both sets using the same comparer
			var expectedKeys = new HashSet<string>(expected.Keys, comparer);
			var actualKeys = new HashSet<string>(actual, comparer);

			// ------------------------------------------------------------
			// Missing required fields
			// ------------------------------------------------------------
			var missing = expected
				.Where(e => e.Value.IsRequired && !actualKeys.Contains(e.Key))
				.Select(e => e.Key)
				.ToList();

			// ------------------------------------------------------------
			// Extra fields not present in expected schema
			// ------------------------------------------------------------
			var extra = actualKeys
				.Where(a => !expectedKeys.Contains(a))
				.ToList();

			return new SchemaDiff(missing, extra);
		}
	}
}
