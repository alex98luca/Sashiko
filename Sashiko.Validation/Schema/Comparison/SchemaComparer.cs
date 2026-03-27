using Sashiko.Validation.Schema.Model;

namespace Sashiko.Validation.Schema.Comparison
{
	public static class SchemaComparer
	{
		public static SchemaDiff Compare(SchemaNode expected, SchemaNode actual, bool ignoreCase = false)
		{
			var missing = new List<string>();
			var extra = new List<string>();

			CompareNodes(expected, actual, expected.Name ?? "", missing, extra, ignoreCase);

			return new SchemaDiff(missing, extra);
		}

		private static void CompareNodes(
			SchemaNode expected,
			SchemaNode actual,
			string path,
			List<string> missing,
			List<string> extra,
			bool ignoreCase)
		{
			if (expected.Kind != actual.Kind)
			{
				missing.Add(path);
				return;
			}

			if (expected.Kind == SchemaNodeKind.Leaf)
				return;

			if (expected.Kind == SchemaNodeKind.Array)
			{
				if (actual.Element == null)
				{
					missing.Add(path);
					return;
				}

				CompareNodes(expected.Element!, actual.Element!, path + "[]", missing, extra, ignoreCase);
				return;
			}

			// Objects
			var expectedFields = expected.Fields ?? new Dictionary<string, SchemaNode>();
			var actualFields = actual.Fields ?? new Dictionary<string, SchemaNode>();

			// Build case-aware lookup for actual fields
			var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
			var actualLookup = new Dictionary<string, SchemaNode>(comparer);
			foreach (var kv in actualFields)
				actualLookup[kv.Key] = kv.Value;

			// Missing required fields
			foreach (var kv in expectedFields)
			{
				var name = kv.Key;
				var expectedChild = kv.Value;

				if (!actualLookup.TryGetValue(name, out var actualChild))
				{
					if (expectedChild.Required)
						missing.Add(path + "." + name);

					continue;
				}

				CompareNodes(expectedChild, actualChild, path + "." + name, missing, extra, ignoreCase);
			}

			// Extra fields
			// We need to detect actual fields that don't match any expected (case-aware)
			var expectedNamesSet = new HashSet<string>(expectedFields.Keys, comparer);

			foreach (var kv in actualFields)
			{
				if (!expectedNamesSet.Contains(kv.Key))
					extra.Add(path + "." + kv.Key);
			}
		}
	}
}
