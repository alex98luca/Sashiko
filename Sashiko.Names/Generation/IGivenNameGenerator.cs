using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface IGivenNameGenerator
	{
		IReadOnlyList<string> Generate(LanguageId language, Sex sex);
	}
}
