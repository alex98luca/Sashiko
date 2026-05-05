using System.Reflection;
using System.Runtime.InteropServices;
using Sashiko.SystemMonitor.Monitoring;
using Sashiko.SystemMonitor.Monitoring.Native;

namespace Sashiko.SystemMonitor.Tests.Monitoring
{
	public class WindowsInteropModelTests
	{
		[Fact]
		public void FileTime_ShouldConvertToUnsignedInteger()
		{
			var fileTime = new FileTime
			{
				HighDateTime = 0x01234567,
				LowDateTime = 0x89ABCDEF
			};

			Assert.Equal(0x0123456789ABCDEFUL, fileTime.ToUInt64());
		}

		[Fact]
		public void MemoryStatusEx_ShouldInitializeLength()
		{
			var statusType = typeof(MemoryStatusEx);

			Assert.NotNull(statusType);

			var status = Activator.CreateInstance(statusType);
			var length = statusType.GetField("Length")?.GetValue(status);

			Assert.Equal((uint)Marshal.SizeOf(statusType), length);
		}
	}
}
