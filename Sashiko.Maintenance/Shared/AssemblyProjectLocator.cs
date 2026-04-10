namespace Sashiko.Maintenance.Shared
{
	internal static class AssemblyProjectLocator
	{
		internal static string FindProjectRoot(string projectName, Type anchorType)
		{
			var start = Path.GetDirectoryName(anchorType.Assembly.Location)
				?? throw new InvalidOperationException("Could not determine assembly directory.");

			return FindProjectRoot(projectName, start);
		}

		internal static string FindProjectRoot(string projectName, string startDirectory)
		{
			var dir = startDirectory;

			while (dir != null)
			{
				var candidate = Path.Combine(dir, projectName);
				if (Directory.Exists(candidate))
					return candidate;

				dir = Directory.GetParent(dir)?.FullName;
			}

			throw new InvalidOperationException(
				$"Could not locate project folder '{projectName}'.");
		}
	}
}
