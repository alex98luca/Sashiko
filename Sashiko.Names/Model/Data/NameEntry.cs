namespace Sashiko.Names.Model.Data
{
	internal sealed record NameEntry(
		string Iso639_3,
		NamePool Pool,
		NameRules Rules
	);
}
