using Sashiko.Languages.Api;
using Sashiko.Languages.Model;
using Sashiko.Languages.Registry;

namespace Sashiko.Languages.Tests.Api
{
	public sealed class LanguageServiceTests
	{
		private readonly LanguageService _service;

		public LanguageServiceTests()
		{
			_service = new LanguageService();
		}

		// ------------------------------------------------------------
		// 1. Construction Tests
		// ------------------------------------------------------------

		[Fact]
		public void Service_ShouldInitializeCorrectly()
		{
			Assert.NotNull(_service.All);
			Assert.NotEmpty(_service.All);
		}

		[Fact]
		public void Service_All_ShouldBeImmutable()
		{
			var list = _service.All;

			Assert.Throws<NotSupportedException>(() =>
			{
				// Attempt to cast to List<T> and modify
				var cast = (IList<Language>)list;
				cast.Add(list[0]);
			});
		}

		[Fact]
		public void Service_All_ShouldBeReadOnlyCollection()
		{
			var list = _service.All;
			Assert.IsType<System.Collections.ObjectModel.ReadOnlyCollection<Language>>(list);
		}

		// ------------------------------------------------------------
		// 2. Lookup by ISO-3
		// ------------------------------------------------------------

		[Fact]
		public void GetIso3_ShouldReturnCorrectLanguage()
		{
			var english = _service.GetIso3("eng");
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void GetIso3_ShouldBeCaseInsensitive()
		{
			var english = _service.GetIso3("ENG");
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void GetIso3_ShouldThrow_ForUnknownCode()
		{
			Assert.Throws<KeyNotFoundException>(() => _service.GetIso3("zzz"));
		}

		// ------------------------------------------------------------
		// 3. Lookup by ISO-2
		// ------------------------------------------------------------

		[Fact]
		public void GetIso2_ShouldReturnCorrectLanguage()
		{
			var english = _service.GetIso2("eng"); // SIL maps ISO-2 to "eng"
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void GetIso2_ShouldBeCaseInsensitive()
		{
			var english = _service.GetIso2("ENG");
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void GetIso2_ShouldThrow_ForUnknownCode()
		{
			Assert.Throws<KeyNotFoundException>(() => _service.GetIso2("xx"));
		}

		// ------------------------------------------------------------
		// 4. Lookup by ISO-1
		// ------------------------------------------------------------

		[Fact]
		public void GetIso1_ShouldReturnCorrectLanguage()
		{
			var english = _service.GetIso1("en");
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void GetIso1_ShouldBeCaseInsensitive()
		{
			var english = _service.GetIso1("EN");
			Assert.Equal("English", english.Name);
		}

		[Fact]
		public void GetIso1_ShouldThrow_ForUnknownCode()
		{
			Assert.Throws<KeyNotFoundException>(() => _service.GetIso1("xx"));
		}

		// ------------------------------------------------------------
		// 5. Lookup by Name
		// ------------------------------------------------------------

		[Fact]
		public void GetByName_ShouldReturnCorrectLanguage()
		{
			var english = _service.GetByName("English");
			Assert.Equal("eng", english.Iso639_3);
		}

		[Fact]
		public void GetByName_ShouldBeCaseInsensitive()
		{
			var english = _service.GetByName("english");
			Assert.Equal("eng", english.Iso639_3);
		}

		[Fact]
		public void GetByName_ShouldThrow_ForUnknownName()
		{
			Assert.Throws<KeyNotFoundException>(() => _service.GetByName("NotALanguage"));
		}

		// ------------------------------------------------------------
		// 6. Exists Tests
		// ------------------------------------------------------------

		[Fact]
		public void Exists_ShouldReturnTrue_ForValidIso3()
		{
			Assert.True(_service.Exists("eng"));
		}

		[Fact]
		public void Exists_ShouldReturnTrue_ForValidIso1()
		{
			Assert.True(_service.Exists("en"));
		}

		[Fact]
		public void Exists_ShouldReturnTrue_ForValidName()
		{
			Assert.True(_service.Exists("English"));
		}

		[Fact]
		public void Exists_ShouldReturnFalse_ForInvalidInput()
		{
			Assert.False(_service.Exists("not-a-language"));
		}

		// ------------------------------------------------------------
		// 7. TryGet Tests
		// ------------------------------------------------------------

		[Fact]
		public void TryGet_ShouldReturnTrue_AndOutputLanguage()
		{
			var result = _service.TryGet("eng", out var lang);

			Assert.True(result);
			Assert.NotNull(lang);
			Assert.Equal("English", lang!.Name);
		}

		[Fact]
		public void TryGet_ShouldReturnFalse_ForInvalidInput()
		{
			var result = _service.TryGet("invalid", out var lang);

			Assert.False(result);
			Assert.Null(lang);
		}

		// ------------------------------------------------------------
		// 8. Consistency Tests
		// ------------------------------------------------------------

		[Fact]
		public void All_ShouldContainEnglish()
		{
			var english = _service.All.FirstOrDefault(l => l.Iso639_3 == "eng");
			Assert.NotNull(english);
			Assert.Equal("English", english!.Name);
		}

		[Fact]
		public void All_ShouldContainAllLanguages()
		{
			var registry = new LanguageRegistry();
			Assert.Equal(registry.Languages.Count, _service.All.Count);
		}
	}
}
