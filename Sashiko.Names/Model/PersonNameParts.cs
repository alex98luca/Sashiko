using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Model
{
	internal sealed record PersonNameParts
	{
		public required IEnumerable<string> GivenNames { get; init; }
		public required IEnumerable<string> LastNames { get; init; }
		public string? Patronymic { get; init; }
		public string? Matronymic { get; init; }
		public string? Prefix { get; init; }
		public string? Suffix { get; init; }
		public string? Nickname { get; init; }
		public required NameOrder Order { get; init; }
	}
}
