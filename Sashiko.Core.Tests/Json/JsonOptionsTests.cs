using System.Text.Json;
using System.Text.Json.Serialization;
using Sashiko.Core.Json.Options;

namespace Sashiko.Core.Tests.Json
{
	public sealed class JsonOptionsTests
	{
		[Fact]
		public void StrictReadOptions_ShouldUseStrictPropertyMatching()
		{
			var options = JsonReadOptions.Strict;

			Assert.False(options.PropertyNameCaseInsensitive);
			Assert.Equal(JsonCommentHandling.Skip, options.ReadCommentHandling);
			Assert.False(options.AllowTrailingCommas);
			Assert.Contains(options.Converters, converter => converter is JsonStringEnumConverter);
		}

		[Fact]
		public void ForgivingReadOptions_ShouldAllowFlexibleJsonInput()
		{
			var options = JsonReadOptions.Forgiving;

			Assert.True(options.PropertyNameCaseInsensitive);
			Assert.Equal(JsonCommentHandling.Skip, options.ReadCommentHandling);
			Assert.True(options.AllowTrailingCommas);
			Assert.Contains(options.Converters, converter => converter is JsonStringEnumConverter);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void ToDocumentOptions_ShouldMirrorSerializerReadOptions(bool allowTrailingCommas)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = allowTrailingCommas,
				ReadCommentHandling = JsonCommentHandling.Skip
			};

			var documentOptions = JsonReadOptions.ToDocumentOptions(options);

			Assert.Equal(allowTrailingCommas, documentOptions.AllowTrailingCommas);
			Assert.Equal(JsonCommentHandling.Skip, documentOptions.CommentHandling);
		}

		[Fact]
		public void IndentedWriteOptions_ShouldUseIndentedFormatting()
		{
			var options = JsonWriteOptions.Indented;

			Assert.True(options.WriteIndented);
			Assert.Equal(JsonIgnoreCondition.WhenWritingNull, options.DefaultIgnoreCondition);
			Assert.NotNull(options.Encoder);
			Assert.Contains(options.Converters, converter => converter is JsonStringEnumConverter);
		}

		[Fact]
		public void CompactWriteOptions_ShouldUseCompactFormatting()
		{
			var options = JsonWriteOptions.Compact;

			Assert.False(options.WriteIndented);
			Assert.Equal(JsonIgnoreCondition.WhenWritingNull, options.DefaultIgnoreCondition);
			Assert.NotNull(options.Encoder);
			Assert.Contains(options.Converters, converter => converter is JsonStringEnumConverter);
		}
	}
}
