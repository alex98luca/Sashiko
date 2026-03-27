namespace Sashiko.Validation.Schema.Model
{
	public sealed class SchemaNode
	{
		public string Name { get; }
		public bool Required { get; }
		public SchemaNodeKind Kind { get; }
		public IReadOnlyDictionary<string, SchemaNode>? Fields { get; }
		public SchemaNode? Element { get; } // for arrays/lists

		public SchemaNode(
			string name,
			bool required,
			SchemaNodeKind kind,
			IReadOnlyDictionary<string, SchemaNode>? fields = null,
			SchemaNode? element = null)
		{
			Name = name;
			Required = required;
			Kind = kind;
			Fields = fields;
			Element = element;
		}
	}
}
