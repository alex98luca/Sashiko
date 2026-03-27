using Sashiko.Validation.Schema.Comparison;
using Sashiko.Validation.Schema.Model;

namespace Sashiko.Validation.Tests.Schema.Comparison
{
	public sealed class SchemaComparerTests
	{
		// ------------------------------------------------------------
		// Helpers
		// ------------------------------------------------------------

		private static SchemaNode Leaf(string name, bool required = true)
			=> new SchemaNode(name, required, SchemaNodeKind.Leaf);

		private static SchemaNode Obj(string name, bool required, params (string, SchemaNode)[] fields)
			=> new SchemaNode(
				name,
				required,
				SchemaNodeKind.Object,
				fields.ToDictionary(f => f.Item1, f => f.Item2));

		private static SchemaNode Arr(string name, bool required, SchemaNode element)
			=> new SchemaNode(
				name,
				required,
				SchemaNodeKind.Array,
				fields: null,
				element: element);

		// ------------------------------------------------------------
		// Matching schemas
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenSchemasMatch_ReturnsMatch()
		{
			var expected = Obj("Root", true,
				("Id", Leaf("Id", true)),
				("Name", Leaf("Name", false))
			);

			var actual = Obj("Root", true,
				("Id", Leaf("Id", true)),
				("Name", Leaf("Name", false))
			);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.True(diff.IsMatch);
			Assert.Empty(diff.Missing);
			Assert.Empty(diff.Extra);
		}

		// ------------------------------------------------------------
		// Missing required fields
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenRequiredFieldMissing_ReportsMissing()
		{
			var expected = Obj("Root", true,
				("Id", Leaf("Id", true)),
				("Name", Leaf("Name", false))
			);

			var actual = Obj("Root", true,
				("Name", Leaf("Name", false))
			);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.False(diff.IsMatch);
			Assert.Contains("Root.Id", diff.Missing);
		}

		// ------------------------------------------------------------
		// Extra fields
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenExtraFieldPresent_ReportsExtra()
		{
			var expected = Obj("Root", true,
				("Id", Leaf("Id", true))
			);

			var actual = Obj("Root", true,
				("Id", Leaf("Id", true)),
				("Extra", Leaf("Extra", true))
			);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.False(diff.IsMatch);
			Assert.Contains("Root.Extra", diff.Extra);
		}

		// ------------------------------------------------------------
		// Nested objects
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenNestedFieldMissing_ReportsNestedPath()
		{
			var expected = Obj("Root", true,
				("Info", Obj("Info", false,
					("Id", Leaf("Id", true)),
					("Name", Leaf("Name", false))
				))
			);

			var actual = Obj("Root", true,
				("Info", Obj("Info", false,
					("Name", Leaf("Name", false))
				))
			);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.False(diff.IsMatch);
			Assert.Contains("Root.Info.Id", diff.Missing);
		}

		// ------------------------------------------------------------
		// Arrays
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenArrayElementSchemaMismatch_ReportsMissingAndExtra()
		{
			var expected = Arr("Items", true,
				Obj("Item", true,
					("Id", Leaf("Id", true))
				)
			);

			var actual = Arr("Items", true,
				Obj("Item", true,
					("Name", Leaf("Name", true))
				)
			);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.False(diff.IsMatch);
			Assert.Contains("Items[].Id", diff.Missing);
			Assert.Contains("Items[].Name", diff.Extra);
		}

		// ------------------------------------------------------------
		// Kind mismatch
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenKindMismatch_ReportsMissing()
		{
			var expected = Leaf("Root", true);
			var actual = Obj("Root", true);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.False(diff.IsMatch);
			Assert.Contains("Root", diff.Missing);
		}

		// ------------------------------------------------------------
		// Recursion handling
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenRecursiveSchema_DoesNotCrash()
		{
			var expected = Obj("Node", true,
				("Child", Obj("Child", false)) // empty object due to recursion
			);

			var actual = Obj("Node", true,
				("Child", Obj("Child", false))
			);

			var diff = SchemaComparer.Compare(expected, actual);

			Assert.True(diff.IsMatch);
		}

		// ------------------------------------------------------------
		// IgnoreCase = true → matching should succeed
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenIgnoreCaseTrue_DifferentCaseFields_ShouldMatch()
		{
			var expected = Obj("Root", true,
				("Id", Leaf("Id", true)),
				("Name", Leaf("Name", false))
			);

			var actual = Obj("Root", true,
				("id", Leaf("id", true)),
				("name", Leaf("name", false))
			);

			var diff = SchemaComparer.Compare(expected, actual, ignoreCase: true);

			Assert.True(diff.IsMatch);
		}

		// ------------------------------------------------------------
		// IgnoreCase = false → matching should fail
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenIgnoreCaseFalse_DifferentCaseFields_ShouldFail()
		{
			var expected = Obj("Root", true,
				("Id", Leaf("Id", true))
			);

			var actual = Obj("Root", true,
				("id", Leaf("id", true))
			);

			var diff = SchemaComparer.Compare(expected, actual, ignoreCase: false);

			Assert.False(diff.IsMatch);
			Assert.Contains("Root.Id", diff.Missing);
			Assert.Contains("Root.id", diff.Extra);
		}

		// ------------------------------------------------------------
		// Nested ignoreCase
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenIgnoreCaseTrue_NestedDifferentCase_ShouldMatch()
		{
			var expected = Obj("Root", true,
				("Info", Obj("Info", false,
					("Id", Leaf("Id", true)),
					("Name", Leaf("Name", false))
				))
			);

			var actual = Obj("Root", true,
				("info", Obj("info", false,
					("id", Leaf("id", true)),
					("name", Leaf("name", false))
				))
			);

			var diff = SchemaComparer.Compare(expected, actual, ignoreCase: true);

			Assert.True(diff.IsMatch);
		}

		// ------------------------------------------------------------
		// Extra field suppressed by ignoreCase
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenIgnoreCaseTrue_ExtraFieldSameNameDifferentCase_ShouldNotReportExtra()
		{
			var expected = Obj("Root", true,
				("Id", Leaf("Id", true))
			);

			var actual = Obj("Root", true,
				("ID", Leaf("ID", true))
			);

			var diff = SchemaComparer.Compare(expected, actual, ignoreCase: true);

			Assert.True(diff.IsMatch);
			Assert.Empty(diff.Extra);
		}

		// ------------------------------------------------------------
		// Missing field suppressed by ignoreCase
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenIgnoreCaseTrue_MissingFieldDifferentCase_ShouldNotReportMissing()
		{
			var expected = Obj("Root", true,
				("Count", Leaf("Count", true))
			);

			var actual = Obj("Root", true,
				("count", Leaf("count", true))
			);

			var diff = SchemaComparer.Compare(expected, actual, ignoreCase: true);

			Assert.True(diff.IsMatch);
			Assert.Empty(diff.Missing);
		}

		// ------------------------------------------------------------
		// Array ignoreCase
		// ------------------------------------------------------------

		[Fact]
		public void Compare_WhenIgnoreCaseTrue_ArrayElementDifferentCase_ShouldMatch()
		{
			var expected = Arr("Items", true,
				Obj("Item", true,
					("Id", Leaf("Id", true))
				)
			);

			var actual = Arr("Items", true,
				Obj("Item", true,
					("id", Leaf("id", true))
				)
			);

			var diff = SchemaComparer.Compare(expected, actual, ignoreCase: true);

			Assert.True(diff.IsMatch);
		}
	}
}
