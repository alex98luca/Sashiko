namespace Sashiko.Validation.Schema.Comparison
{
	public sealed record SchemaDiff(
		IReadOnlyList<string> Missing,
		IReadOnlyList<string> Extra)
	{
		public bool IsMatch => Missing.Count == 0 && Extra.Count == 0;
	}
}
