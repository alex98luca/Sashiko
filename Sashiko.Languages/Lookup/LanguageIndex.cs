using Sashiko.Languages.Model;

namespace Sashiko.Languages.Lookup
{
	internal sealed class LanguageIndex
	{
		internal IReadOnlyDictionary<string, Language> ByName { get; }
		internal IReadOnlyDictionary<string, Language> ByIso1 { get; }
		internal IReadOnlyDictionary<string, Language> ByIso2 { get; }
		internal IReadOnlyDictionary<string, Language> ByIso3 { get; }

		internal IReadOnlyList<Language> All { get; }

		internal LanguageIndex(IReadOnlyDictionary<string, Language> registry)
		{
			var byName = new Dictionary<string, Language>(StringComparer.OrdinalIgnoreCase);
			var byIso1 = new Dictionary<string, Language>(StringComparer.OrdinalIgnoreCase);
			var byIso2 = new Dictionary<string, Language>(StringComparer.OrdinalIgnoreCase);
			var byIso3 = new Dictionary<string, Language>(StringComparer.OrdinalIgnoreCase);

			foreach (var lang in registry.Values)
			{
				if (!string.IsNullOrWhiteSpace(lang.Name))
					byName[lang.Name] = lang;

				if (!string.IsNullOrWhiteSpace(lang.Iso639_1))
					byIso1[lang.Iso639_1] = lang;

				if (!string.IsNullOrWhiteSpace(lang.Iso639_2))
					byIso2[lang.Iso639_2] = lang;

				if (!string.IsNullOrWhiteSpace(lang.Iso639_3))
					byIso3[lang.Iso639_3] = lang;
			}

			ByName = byName;
			ByIso1 = byIso1;
			ByIso2 = byIso2;
			ByIso3 = byIso3;

			All = byIso3.Values.ToList().AsReadOnly();
		}

		internal bool TryResolve(string input, out Language? lang)
		{
			return ByIso3.TryGetValue(input, out lang)
				|| ByIso2.TryGetValue(input, out lang)
				|| ByIso1.TryGetValue(input, out lang)
				|| ByName.TryGetValue(input, out lang);
		}
	}
}
