using Sashiko.SystemMonitor.Snapshot;

namespace Sashiko.SystemMonitor.Tests.Snapshot
{
	public class SystemSnapshotProviderTests
	{
		[Fact]
		public void Capture_ShouldNotThrow()
		{
			var exception = Record.Exception(() => SystemSnapshotProvider.Capture());

			Assert.Null(exception);
		}

		[Fact]
		public void Capture_ShouldReturnValidSnapshot()
		{
			var snapshot = SystemSnapshotProvider.Capture();

			Assert.NotNull(snapshot);
			Assert.NotNull(snapshot.Os);
			Assert.NotNull(snapshot.Cpu);
			Assert.NotNull(snapshot.Gpu);
			Assert.NotNull(snapshot.Memory);
			Assert.NotNull(snapshot.Disk);
			Assert.NotNull(snapshot.Thermal);
			Assert.NotNull(snapshot.Power);
		}

		[Fact]
		public void Capture_ShouldReturnConsistentStructure()
		{
			var s1 = SystemSnapshotProvider.Capture();
			var s2 = SystemSnapshotProvider.Capture();

			Assert.Equal(s1.GetType(), s2.GetType());
			Assert.Equal(s1.Cpu.GetType(), s2.Cpu.GetType());
			Assert.Equal(s1.Memory.GetType(), s2.Memory.GetType());
		}
	}
}
