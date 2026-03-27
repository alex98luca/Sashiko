namespace Sashiko.Validation.Schema.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OptionalPropertiesAttribute : Attribute
	{
		public IReadOnlyList<string> Properties { get; }
		public OptionalPropertiesAttribute(params string[] properties)
			=> Properties = properties;
	}
}
