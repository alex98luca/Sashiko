using Sashiko.Names.Model;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface INameGenerator
	{
		PersonName Generate(
			LanguageId language,
			Sex sex,
			string? fatherName = null,
			string? motherName = null
		);
	}
}
