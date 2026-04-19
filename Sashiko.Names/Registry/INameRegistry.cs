using Sashiko.Names.Model.Data;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Registry
{
	internal interface INameRegistry
	{
		NameEntry Get(LanguageId language);
		bool TryGet(LanguageId language, out NameEntry entry);
		IEnumerable<NameEntry> All { get; }
	}
}
