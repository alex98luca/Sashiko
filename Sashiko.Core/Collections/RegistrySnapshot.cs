using System.Collections.Concurrent;

namespace Sashiko.Core.Collections
{
	public static class RegistrySnapshot
	{
		public static ConcurrentDictionary<string, TValue> CreateEmpty<TValue>()
		{
			return new ConcurrentDictionary<string, TValue>(
				StringComparer.OrdinalIgnoreCase
			);
		}

		public static void Replace<TValue>(
			ConcurrentDictionary<string, TValue> target,
			ConcurrentDictionary<string, TValue> snapshot)
		{
			target.Clear();

			foreach (var kv in snapshot)
				target[kv.Key] = kv.Value;
		}
	}
}
