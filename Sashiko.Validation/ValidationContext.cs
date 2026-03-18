namespace Sashiko.Validation
{
	public sealed class ValidationContext
	{
		public string? Source { get; init; }
		public bool IgnoreCase { get; init; }
		public IDictionary<string, object>? Metadata { get; init; }
	}
}
