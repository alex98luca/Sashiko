using Sashiko.Names.Model;
using Sashiko.Names.Model.Enums;
using Sashiko.Names.Registry;

namespace Sashiko.Names.Generation.Implementation
{
	internal sealed class NameAssembler : INameAssembler
	{
		private readonly INameRegistry _registry;

		public NameAssembler(INameRegistry registry)
		{
			_registry = registry;
		}

		public PersonName Assemble(
			LanguageId language,
			Sex sex,
			IReadOnlyList<string> givenNames,
			string? patronymic,
			string? matronymic,
			IReadOnlyList<string> lastNames,
			string? prefix,
			string? suffix)
		{
			var entry = _registry.Get(language);
			var rules = entry.Rules;

			// Build the ordered list of parts (for FullName)
			var parts = new List<string>();

			if (prefix is not null)
				parts.Add(prefix);

			if (rules.Order == NameOrder.FirstLast)
			{
				parts.AddRange(givenNames);

				if (patronymic is not null)
					parts.Add(patronymic);

				if (matronymic is not null)
					parts.Add(matronymic);

				parts.AddRange(lastNames);
			}
			else // NameOrder.LastFirst
			{
				parts.AddRange(lastNames);

				parts.AddRange(givenNames);

				if (patronymic is not null)
					parts.Add(patronymic);

				if (matronymic is not null)
					parts.Add(matronymic);
			}

			if (suffix is not null)
				parts.Add(suffix);

			// Construct the structured PersonName
			return new PersonName(
				givenNames: givenNames,
				lastNames: lastNames,
				patronymic: patronymic,
				matronymic: matronymic,
				prefix: prefix,
				suffix: suffix,
				nickname: null, // nickname not handled here
				order: rules.Order
			);
		}
	}
}
