using Sashiko.Validation.Schema.Inspectors.CSharp;
using Sashiko.Validation.Schema.Model;

namespace Sashiko.Validation.Tests.Schema.Inspectors.CSharp
{
	public sealed class CSharpSchemaInspectorTests
	{
#nullable enable
		private sealed class SimpleModel
		{
			public int Id { get; set; }                 // required
			public string Name { get; set; } = "";      // optional
			public string? OptionalDescription { get; set; } // optional
		}

		private sealed class NestedModel
		{
			public SimpleModel Info { get; set; } = new(); // optional
			public int Count { get; set; }                 // required
		}

		private sealed class CollectionModel
		{
			public List<SimpleModel> Items { get; set; } = new();
			public int[] Values { get; set; } = Array.Empty<int>();
		}

		private sealed class RecursiveModel
		{
			public string Name { get; set; } = "";
			public RecursiveModel? Child { get; set; }
		}
#nullable restore

		// ------------------------------------------------------------
		// Leaf model
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenLeafModel_ProducesLeafNodes()
		{
			var schema = CSharpSchemaInspector.GetSchema(typeof(int));

			Assert.Equal(SchemaNodeKind.Leaf, schema.Kind);
			Assert.True(schema.Required);
			Assert.Null(schema.Fields);
			Assert.Null(schema.Element);
		}

		// ------------------------------------------------------------
		// Simple object
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenSimpleModel_ProducesObjectWithCorrectFields()
		{
			var schema = CSharpSchemaInspector.GetSchema(typeof(SimpleModel));

			Assert.Equal(SchemaNodeKind.Object, schema.Kind);
			Assert.True(schema.Required);

			Assert.NotNull(schema.Fields);
			Assert.Equal(3, schema.Fields!.Count);

			Assert.True(schema.Fields["Id"].Required);
			Assert.False(schema.Fields["Name"].Required);
			Assert.False(schema.Fields["OptionalDescription"].Required);

			Assert.Equal(SchemaNodeKind.Leaf, schema.Fields["Id"].Kind);
		}

		// ------------------------------------------------------------
		// Nested object
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenNestedModel_ProducesNestedStructure()
		{
			var schema = CSharpSchemaInspector.GetSchema(typeof(NestedModel));

			Assert.Equal(SchemaNodeKind.Object, schema.Kind);

			var info = schema.Fields!["Info"];
			var count = schema.Fields!["Count"];

			Assert.False(info.Required);  // reference type → optional
			Assert.True(count.Required);  // value type → required

			Assert.Equal(SchemaNodeKind.Object, info.Kind);
			Assert.Equal(SchemaNodeKind.Leaf, count.Kind);

			Assert.True(info.Fields!["Id"].Required);
			Assert.False(info.Fields!["Name"].Required);
		}

		// ------------------------------------------------------------
		// Collections
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenCollectionModel_ProducesArrayNodes()
		{
			var schema = CSharpSchemaInspector.GetSchema(typeof(CollectionModel));

			var items = schema.Fields!["Items"];
			var values = schema.Fields!["Values"];

			Assert.Equal(SchemaNodeKind.Array, items.Kind);
			Assert.Equal(SchemaNodeKind.Array, values.Kind);

			Assert.NotNull(items.Element);
			Assert.NotNull(values.Element);

			Assert.Equal(SchemaNodeKind.Object, items.Element!.Kind);
			Assert.Equal(SchemaNodeKind.Leaf, values.Element!.Kind);
		}

		// ------------------------------------------------------------
		// Recursion
		// ------------------------------------------------------------

		[Fact]
		public void GetSchema_WhenRecursiveModel_StopsRecursion()
		{
			var schema = CSharpSchemaInspector.GetSchema(typeof(RecursiveModel));

			var child = schema.Fields!["Child"];

			Assert.Equal(SchemaNodeKind.Object, child.Kind);
			Assert.Empty(child.Fields!); // recursion stops here
		}
	}
}
