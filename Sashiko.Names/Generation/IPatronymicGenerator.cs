using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface IPatronymicGenerator
	{
		string? Generate(LanguageId language, Sex sex, string fatherName);
	}
}
