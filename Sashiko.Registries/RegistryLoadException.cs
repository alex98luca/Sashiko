namespace Sashiko.Registries
{
	public sealed class RegistryLoadException : Exception
	{
		public string SourceName { get; }
		public Type RegistryType { get; }

		public RegistryLoadException(
			string message,
			string sourceName,
			Type registryType,
			Exception? inner = null)
			: base(message, inner)
		{
			SourceName = sourceName;
			RegistryType = registryType;
		}
	}
}
