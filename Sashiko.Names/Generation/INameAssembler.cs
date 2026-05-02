using Sashiko.Names.Model;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal interface INameAssembler
	{
		PersonName Assemble(
			LanguageId language,
			Sex sex,
			IReadOnlyList<string> given,
			string? patronymic,
			string? matronymic,
			IReadOnlyList<string> last,
			string? prefix,
			string? suffix
		);
	}
}
