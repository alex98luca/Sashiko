using Sashiko.Core.Probability.Selection;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation
{
	internal sealed record NameGeneratorDependencies
	{
		public required INameRegistry Registry { get; init; }
		public required IRandomPicker Picker { get; init; }
		public required IGivenNameGenerator Given { get; init; }
		public required IPatronymicGenerator Patronymic { get; init; }
		public required IMatronymicGenerator Matronymic { get; init; }
		public required ILastNameGenerator Last { get; init; }
		public required IPrefixGenerator Prefix { get; init; }
		public required ISuffixGenerator Suffix { get; init; }
		public required INameAssembler Assembler { get; init; }
	}
}
