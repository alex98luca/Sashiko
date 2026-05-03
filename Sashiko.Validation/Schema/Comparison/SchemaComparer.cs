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
				CompareArrayNodes(expected, actual, path, missing, extra, ignoreCase);
				return;
			}

			CompareObjectNodes(expected, actual, path, missing, extra, ignoreCase);
		}

		private static void CompareArrayNodes(
			SchemaNode expected,
			SchemaNode actual,
			string path,
			List<string> missing,
			List<string> extra,
			bool ignoreCase)
		{
			if (actual.Element == null)
			{
				missing.Add(path);
				return;
			}

			CompareNodes(expected.Element!, actual.Element, path + "[]", missing, extra, ignoreCase);
		}

		private static void CompareObjectNodes(
			SchemaNode expected,
			SchemaNode actual,
			string path,
			List<string> missing,
			List<string> extra,
			bool ignoreCase)
		{
			var expectedFields = expected.Fields ?? new Dictionary<string, SchemaNode>();
			var actualFields = actual.Fields ?? new Dictionary<string, SchemaNode>();
			var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
			var actualLookup = new Dictionary<string, SchemaNode>(actualFields, comparer);

			AddMissingRequiredFields(
				expectedFields,
				actualLookup,
				path,
				missing,
				extra,
				ignoreCase);

			AddExtraFields(expectedFields, actualFields, path, extra, comparer);
		}

		private static void AddMissingRequiredFields(
			IReadOnlyDictionary<string, SchemaNode> expectedFields,
			Dictionary<string, SchemaNode> actualLookup,
			string path,
			List<string> missing,
			List<string> extra,
			bool ignoreCase)
		{
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
		}

		private static void AddExtraFields(
			IReadOnlyDictionary<string, SchemaNode> expectedFields,
			IReadOnlyDictionary<string, SchemaNode> actualFields,
			string path,
			List<string> extra,
			StringComparer comparer)
		{
			var expectedNamesSet = new HashSet<string>(expectedFields.Keys, comparer);

			extra.AddRange(
				actualFields
					.Where(kv => !expectedNamesSet.Contains(kv.Key))
					.Select(kv => path + "." + kv.Key));
		}
	}
}
