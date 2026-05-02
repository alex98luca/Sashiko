using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface ISuffixGenerator
	{
		string? Generate(LanguageId language, Sex sex);
	}
}
