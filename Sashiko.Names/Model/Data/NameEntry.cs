using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Model.Data
{
	internal sealed record NameEntry(
		LanguageId Language,
		NamePool Pool,
		NameRules Rules
	);
}
