namespace Sashiko.Core.Probability
{
	internal static class ChanceUtility
	{
		internal static bool Of(double probability, Random rng)
		{
			if (probability <= 0) return false;
			if (probability >= 1) return true;

			return rng.NextDouble() < probability;
		}
	}
}
