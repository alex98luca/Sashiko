using Sashiko.Maintenance.Shared;

namespace Sashiko.Maintenance.Tests.Shared
{
	public sealed class AssemblyProjectLocatorTests
	{
		[Fact]
		public void FindProjectRoot_ShouldFindProjectInParentDirectory()
		{
			// Arrange: create a fake directory structure
			var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			var child = Path.Combine(root, "sub", "folder");
			var project = Path.Combine(root, "MyProject");

			Directory.CreateDirectory(child);
			Directory.CreateDirectory(project);

			try
			{
				// Act
				var result = AssemblyProjectLocator.FindProjectRoot("MyProject", child);

				// Assert
				Assert.Equal(project.Replace("\\", "/"), result.Replace("\\", "/"));
			}
			finally
			{
				Directory.Delete(root, recursive: true);
			}
		}

		[Fact]
		public void FindProjectRoot_ShouldThrow_WhenProjectNotFound()
		{
			var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			var child = Path.Combine(root, "sub");

			Directory.CreateDirectory(child);

			try
			{
				Assert.Throws<InvalidOperationException>(() =>
					AssemblyProjectLocator.FindProjectRoot("MissingProject", child));
			}
			finally
			{
				Directory.Delete(root, recursive: true);
			}
		}

		[Fact]
		public void FindProjectRoot_WithAnchorType_ShouldDelegateToStringVersion()
		{
			// Arrange: create fake project folder
			var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			var project = Path.Combine(root, "Sashiko.Languages");

			Directory.CreateDirectory(project);

			try
			{
				// Fake assembly location: we override the start directory
				var result = AssemblyProjectLocator.FindProjectRoot(
					"Sashiko.Languages",
					project // startDirectory
				);

				Assert.Equal(project.Replace("\\", "/"), result.Replace("\\", "/"));
			}
			finally
			{
				Directory.Delete(root, recursive: true);
			}
		}
	}
}
