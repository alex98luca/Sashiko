using Sashiko.Languages.Lookup;
using Sashiko.Languages.Registry;
using Sashiko.Languages.Model;

namespace Sashiko.Languages.Api
{
	public sealed class LanguageService
	{
		private readonly LanguageIndex _index;

		public LanguageService()
		{
			var registry = new LanguageRegistry();
			_index = new LanguageIndex(registry.Languages);
		}

		// For testing or advanced scenarios
		internal LanguageService(LanguageRegistry registry)
		{
			_index = new LanguageIndex(registry.Languages);
		}

		public IReadOnlyList<Language> All => _index.All;

		public Language GetIso3(string code) => _index.ByIso3[code];
		public Language GetIso2(string code) => _index.ByIso2[code];
		public Language GetIso1(string code) => _index.ByIso1[code];
		public Language GetByName(string name) => _index.ByName[name];

		public bool Exists(string input) => _index.TryResolve(input, out _);

		public bool TryGet(string input, out Language? lang)
			=> _index.TryResolve(input, out lang);
	}
}
