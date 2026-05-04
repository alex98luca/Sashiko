using Sashiko.Names.Model;

namespace Sashiko.Names.Generation
{
	internal interface INameAssembler
	{
		PersonName Assemble(NameAssemblyRequest request);
	}
}
