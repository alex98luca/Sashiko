using Sashiko.Languages.Model;

namespace Sashiko.Maintenance.Libraries.Languages.Updating
{
	internal static class LanguageMapper
	{
		internal static IReadOnlyList<Language> Convert(IReadOnlyList<IsoSilLanguage> external)
		{
			return external.Select(x => new Language(
				name: x.Name,
				iso639_1: IsoCodeNormalizer.Normalize(x.Iso639_1),
				iso639_2: IsoCodeNormalizer.Normalize(x.Iso639_2),
				iso639_3: IsoCodeNormalizer.Normalize(x.Iso639_3),
				scope: MapScope(x.Scope),
				type: MapType(x.Type)
			)).ToList();
		}

		private static LanguageScope MapScope(string code) => code switch
		{
			"I" => LanguageScope.Individual,
			"M" => LanguageScope.Macrolanguage,
			"S" => LanguageScope.Special,
			_ => throw new InvalidOperationException($"Unknown scope code: {code}")
		};

		private static LanguageType MapType(string code) => code switch
		{
			"L" => LanguageType.Living,
			"E" => LanguageType.Extinct,
			"A" => LanguageType.Ancient,
			"H" => LanguageType.Historical,
			"C" => LanguageType.Constructed,
			"S" => LanguageType.Special,
			_ => throw new InvalidOperationException($"Unknown type code: {code}")
		};
	}
}
