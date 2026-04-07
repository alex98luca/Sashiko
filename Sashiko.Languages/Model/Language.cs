using System.Text.Json.Serialization;
using Sashiko.Validation.Schema.Attributes;

namespace Sashiko.Languages.Model
{
	public sealed record Language
	{
		[Required]
		public string Name { get; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[Optional]
		public string? Iso639_1 { get; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[Optional]
		public string? Iso639_2 { get; }

		[Required]
		public string Iso639_3 { get; }

		[Required]
		public LanguageScope Scope { get; }

		[Required]
		public LanguageType Type { get; }

		[JsonConstructor]
		internal Language(
			string name,
			string? iso639_1,
			string? iso639_2,
			string iso639_3,
			LanguageScope scope,
			LanguageType type)
		{
			Name = name;
			Iso639_1 = iso639_1;
			Iso639_2 = iso639_2;
			Iso639_3 = iso639_3;
			Scope = scope;
			Type = type;
		}
	}
}
