namespace Sashiko.Validation.Schema.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RequiredPropertiesAttribute : Attribute
	{
		public IReadOnlyList<string> Properties { get; }
		public RequiredPropertiesAttribute(params string[] properties)
			=> Properties = properties;
	}
}
