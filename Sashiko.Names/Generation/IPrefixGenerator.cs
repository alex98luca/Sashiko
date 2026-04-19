using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface IPrefixGenerator
	{
		string? Generate(LanguageId language, Sex sex);
	}
}
