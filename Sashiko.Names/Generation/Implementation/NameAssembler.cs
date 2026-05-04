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

		public PersonName Assemble(NameAssemblyRequest request)
		{
			var entry = _registry.Get(request.Language);
			var rules = entry.Rules;

			// Build the ordered list of parts (for FullName)
			var parts = new List<string>();

			if (request.Prefix is not null)
				parts.Add(request.Prefix);

			if (rules.Order == NameOrder.FirstLast)
			{
				parts.AddRange(request.GivenNames);

				if (request.Patronymic is not null)
					parts.Add(request.Patronymic);

				if (request.Matronymic is not null)
					parts.Add(request.Matronymic);

				parts.AddRange(request.LastNames);
			}
			else // NameOrder.LastFirst
			{
				parts.AddRange(request.LastNames);

				parts.AddRange(request.GivenNames);

				if (request.Patronymic is not null)
					parts.Add(request.Patronymic);

				if (request.Matronymic is not null)
					parts.Add(request.Matronymic);
			}

			if (request.Suffix is not null)
				parts.Add(request.Suffix);

			// Construct the structured PersonName
			return new PersonName(new PersonNameParts
			{
				GivenNames = request.GivenNames,
				LastNames = request.LastNames,
				Patronymic = request.Patronymic,
				Matronymic = request.Matronymic,
				Prefix = request.Prefix,
				Suffix = request.Suffix,
				Nickname = null, // nickname not handled here
				Order = rules.Order
			});
		}
	}
}
