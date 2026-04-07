namespace Sashiko.Maintenance.Libraries.Languages.Updating
{
	internal static class IsoSilParser
	{
		internal static IReadOnlyList<IsoSilLanguage> Parse(string tsv)
		{
			var lines = tsv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			var list = new List<IsoSilLanguage>();

			foreach (var line in lines.Skip(1))
			{
				var cols = line.Split('\t');
				if (cols.Length < 7)
					continue;

				list.Add(new IsoSilLanguage(
					name: cols[6].Trim(),
					iso639_1: Normalize(cols[3]),
					iso639_2: Normalize(cols[2]) ?? Normalize(cols[1]),
					iso639_3: cols[0].Trim(),
					scope: cols[4].Trim(),
					type: cols[5].Trim()
				));
			}

			return list;
		}

		private static string? Normalize(string? value) =>
			string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}
}
