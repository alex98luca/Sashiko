using Sashiko.Maintenance.Libraries.Languages.Updating;

namespace Sashiko.Maintenance.Tests.Libraries.Languages.Updating
{
	public sealed class IsoSilParserTests
	{
		private const string Header =
			"Id\tPart2B\tPart2T\tPart1\tScope\tType\tRef_Name";

		[Fact]
		public void Parse_ShouldReturnEmptyList_WhenOnlyHeader()
		{
			var tsv = Header;
			var result = IsoSilParser.Parse(tsv);
			Assert.Empty(result);
		}

		[Fact]
		public void Parse_ShouldParseValidRow()
		{
			var tsv = Header + "\n" +
					  "eng\teng\teng\ten\tI\tL\tEnglish";

			var result = IsoSilParser.Parse(tsv);

			Assert.Single(result);

			var lang = result[0];
			Assert.Equal("English", lang.Name);
			Assert.Equal("en", lang.Iso639_1);
			Assert.Equal("eng", lang.Iso639_2);
			Assert.Equal("eng", lang.Iso639_3);
			Assert.Equal("I", lang.Scope);
			Assert.Equal("L", lang.Type);
		}

		[Fact]
		public void Parse_ShouldSkipMalformedRows()
		{
			var tsv = Header + "\n" +
					  "too\tfew\tcolumns\n" +
					  "eng\teng\teng\ten\tI\tL\tEnglish";

			var result = IsoSilParser.Parse(tsv);

			Assert.Single(result);
			Assert.Equal("English", result[0].Name);
		}

		[Fact]
		public void Parse_ShouldHandleMissingIso1()
		{
			var tsv = Header + "\n" +
					  "eng\teng\teng\t\tI\tL\tEnglish";

			var result = IsoSilParser.Parse(tsv);

			Assert.Single(result);
			Assert.Null(result[0].Iso639_1);
		}

		[Fact]
		public void Parse_ShouldFallbackIso2_WhenPart2TIsMissing()
		{
			var tsv = Header + "\n" +
					  "eng\teng\t\t\ten\tI\tL\tEnglish";

			var result = IsoSilParser.Parse(tsv);

			Assert.Single(result);
			Assert.Equal("eng", result[0].Iso639_2);
		}
	}
}
