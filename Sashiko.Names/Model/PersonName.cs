using Sashiko.Core.Text;
using Sashiko.Names.Model.Enums;

namespace Sashiko.Names.Model
{
	public sealed record PersonName
	{
		public string? Prefix { get; }
		public IReadOnlyList<string> GivenNames { get; }
		public string? Patronymic { get; }
		public string? Matronymic { get; }
		public IReadOnlyList<string> LastNames { get; }
		public string? Suffix { get; }
		public string? Nickname { get; }

		internal NameOrder Order { get; }

		internal PersonName(
			IEnumerable<string> givenNames,
			IEnumerable<string> lastNames,
			string? patronymic = null,
			string? matronymic = null,
			string? prefix = null,
			string? suffix = null,
			string? nickname = null,
			NameOrder order = NameOrder.FirstLast)
		{
			GivenNames = StringNormalizer.NormalizeCollection(givenNames);
			LastNames = StringNormalizer.NormalizeCollection(lastNames);

			Patronymic = StringNormalizer.NormalizeOptional(patronymic);
			Matronymic = StringNormalizer.NormalizeOptional(matronymic);
			Prefix = StringNormalizer.NormalizeOptional(prefix);
			Suffix = StringNormalizer.NormalizeOptional(suffix);
			Nickname = StringNormalizer.NormalizeOptional(nickname);

			Order = order;
		}

		// ------------------------------------------------------------
		// Computed Variants
		// ------------------------------------------------------------

		public string FullName => BuildFullName();
		public string DisplayName => Nickname ?? FullName;

		private string BuildFullName()
		{
			var parts = new List<string>();

			if (Prefix is not null)
				parts.Add(Prefix);

			switch (Order)
			{
				case NameOrder.FirstLast:
					parts.AddRange(GivenNames);
					if (Patronymic is not null) parts.Add(Patronymic);
					if (Matronymic is not null) parts.Add(Matronymic);
					parts.AddRange(LastNames);
					break;

				case NameOrder.LastFirst:
					parts.AddRange(LastNames);
					parts.AddRange(GivenNames);
					if (Patronymic is not null) parts.Add(Patronymic);
					if (Matronymic is not null) parts.Add(Matronymic);
					break;
			}

			if (Suffix is not null)
				parts.Add(Suffix);

			return string.Join(" ", parts);
		}

		public bool Equals(PersonName? other)
		{
			if (other is null) return false;

			return Prefix == other.Prefix
				&& Patronymic == other.Patronymic
				&& Matronymic == other.Matronymic
				&& Suffix == other.Suffix
				&& Nickname == other.Nickname
				&& Order == other.Order
				&& GivenNames.SequenceEqual(other.GivenNames)
				&& LastNames.SequenceEqual(other.LastNames);
		}

		public override int GetHashCode()
		{
			HashCode hash = new();
			hash.Add(Prefix);
			hash.Add(Patronymic);
			hash.Add(Matronymic);
			hash.Add(Suffix);
			hash.Add(Nickname);
			hash.Add(Order);

			foreach (var g in GivenNames) hash.Add(g);
			foreach (var l in LastNames) hash.Add(l);

			return hash.ToHashCode();
		}
	}
}
