using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface ILastNameGenerator
	{
		IReadOnlyList<string> Generate(LanguageId language, Sex sex);
	}
}
