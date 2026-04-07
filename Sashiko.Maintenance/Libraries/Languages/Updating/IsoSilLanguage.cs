namespace Sashiko.Maintenance.Libraries.Languages.Updating
{
	internal sealed record IsoSilLanguage
	{
		internal string Name { get; }
		internal string? Iso639_1 { get; }
		internal string? Iso639_2 { get; }
		internal string Iso639_3 { get; }
		internal string Scope { get; }
		internal string Type { get; }

		internal IsoSilLanguage(
			string name,
			string? iso639_1,
			string? iso639_2,
			string iso639_3,
			string scope,
			string type)
		{
			Name = name;
			Iso639_1 = iso639_1;
			Iso639_2 = iso639_2;
			Iso639_3 = iso639_3;
			Scope = scope;
			Type = type;
		}
	}
}
