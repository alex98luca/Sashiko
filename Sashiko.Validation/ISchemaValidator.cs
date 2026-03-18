namespace Sashiko.Validation
{
	public interface ISchemaValidator
	{
		/// <summary>
		/// Validates an input object against the schema of T.
		/// Throws an exception if validation fails.
		/// </summary>
		void Validate<T>(object input, ValidationContext? context = null);
	}
}
