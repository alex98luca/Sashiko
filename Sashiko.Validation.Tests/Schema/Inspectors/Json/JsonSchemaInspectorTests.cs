using System.Text.Json;
using Sashiko.Validation.Schema.Inspectors.Json;
using Sashiko.Validation.Schema.Model;

namespace Sashiko.Validation.Tests.Schema.Inspectors.Json
{
	public sealed class JsonSchemaInspectorTests
	{
		private static JsonElement Parse(string json)
			=> JsonDocument.Parse(json).RootElement;

		// ------------------------------------------------------------
		// Leaf values
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenLeafValue_ProducesLeafNode()
		{
			var json = "123";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Value");

			Assert.Equal("Value", schema.Name);
			Assert.Equal(SchemaNodeKind.Leaf, schema.Kind);
			Assert.True(schema.Required);
			Assert.Null(schema.Fields);
			Assert.Null(schema.Element);
		}

		// ------------------------------------------------------------
		// Simple object
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenObject_ProducesObjectNodeWithFields()
		{
			var json = """{ "id": 1, "name": "abc" }""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Root");

			Assert.Equal("Root", schema.Name);
			Assert.Equal(SchemaNodeKind.Object, schema.Kind);
			Assert.True(schema.Required);

			Assert.NotNull(schema.Fields);
			Assert.Equal(2, schema.Fields!.Count);

			Assert.Equal(SchemaNodeKind.Leaf, schema.Fields["id"].Kind);
			Assert.Equal(SchemaNodeKind.Leaf, schema.Fields["name"].Kind);
		}

		// ------------------------------------------------------------
		// Nested object
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenNestedObject_ProducesNestedStructure()
		{
			var json = """{ "info": { "id": 1, "name": "abc" } }""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Root");

			var info = schema.Fields!["info"];

			Assert.Equal(SchemaNodeKind.Object, info.Kind);
			Assert.True(info.Required);

			Assert.Equal(SchemaNodeKind.Leaf, info.Fields!["id"].Kind);
			Assert.Equal(SchemaNodeKind.Leaf, info.Fields!["name"].Kind);
		}

		// ------------------------------------------------------------
		// Arrays
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenArrayOfObjects_ProducesArrayNodeWithElementSchema()
		{
			var json = """[ { "id": 1 }, { "id": 2 } ]""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Items");

			Assert.Equal("Items", schema.Name);
			Assert.Equal(SchemaNodeKind.Array, schema.Kind);
			Assert.True(schema.Required);

			Assert.NotNull(schema.Element);
			Assert.Equal(SchemaNodeKind.Object, schema.Element!.Kind);

			Assert.Equal(SchemaNodeKind.Leaf, schema.Element.Fields!["id"].Kind);
		}

		// ------------------------------------------------------------
		// Array of primitives
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenArrayOfPrimitives_ProducesArrayWithLeafElement()
		{
			var json = """[ 1, 2, 3 ]""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Numbers");

			Assert.Equal(SchemaNodeKind.Array, schema.Kind);
			Assert.Equal(SchemaNodeKind.Leaf, schema.Element!.Kind);
		}

		// ------------------------------------------------------------
		// Empty array
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenArrayIsEmpty_ElementSchemaIsLeaf()
		{
			var json = """[]""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Empty");

			Assert.Equal(SchemaNodeKind.Array, schema.Kind);
			Assert.NotNull(schema.Element);
			Assert.Equal(SchemaNodeKind.Leaf, schema.Element!.Kind);
		}

		// ------------------------------------------------------------
		// Mixed array (first element determines schema)
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenArrayIsMixed_UsesFirstElementSchema()
		{
			var json = """[ { "id": 1 }, 123, "abc" ]""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Mixed");

			Assert.Equal(SchemaNodeKind.Array, schema.Kind);
			Assert.Equal(SchemaNodeKind.Object, schema.Element!.Kind);

			Assert.True(schema.Element.Fields!.ContainsKey("id"));
		}

		// ------------------------------------------------------------
		// Deeply nested structures
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenDeeplyNested_ProducesFullTree()
		{
			var json = """{ "a": { "b": { "c": 123 } } }""";
			var element = Parse(json);

			var schema = JsonSchemaInspector.GetSchema(element, "Root");

			Assert.Equal(SchemaNodeKind.Object, schema.Kind);

			var a = schema.Fields!["a"];
			var b = a.Fields!["b"];
			var c = b.Fields!["c"];

			Assert.Equal(SchemaNodeKind.Object, a.Kind);
			Assert.Equal(SchemaNodeKind.Object, b.Kind);
			Assert.Equal(SchemaNodeKind.Leaf, c.Kind);
		}
	}
}
