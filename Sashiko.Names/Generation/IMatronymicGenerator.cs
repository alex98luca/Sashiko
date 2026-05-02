using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface IMatronymicGenerator
	{
		string? Generate(LanguageId language, Sex sex, string motherName);
	}
}
