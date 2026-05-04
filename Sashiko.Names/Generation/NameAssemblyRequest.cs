using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Generation
{
	internal sealed record NameAssemblyRequest
	{
		public required LanguageId Language { get; init; }
		public required IReadOnlyList<string> GivenNames { get; init; }
		public string? Patronymic { get; init; }
		public string? Matronymic { get; init; }
		public required IReadOnlyList<string> LastNames { get; init; }
		public string? Prefix { get; init; }
		public string? Suffix { get; init; }
	}
}
