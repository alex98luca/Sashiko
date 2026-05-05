using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Sashiko.SystemMonitor.Monitoring.Native
{
	[ExcludeFromCodeCoverage(Justification = "Source-generated Windows native interop.")]
	internal static partial class WindowsNativeMethods
	{
		[LibraryImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static partial bool GetSystemTimes(
			out FileTime idleTime,
			out FileTime kernelTime,
			out FileTime userTime);

		[LibraryImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static partial bool GlobalMemoryStatusEx(ref MemoryStatusEx buffer);

		[LibraryImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static partial bool GetSystemPowerStatus(out SystemPowerStatus status);
	}

	internal static class FileTimeExtensions
	{
		public static ulong ToUInt64(this FileTime fileTime)
			=> ((ulong)fileTime.HighDateTime << 32) | fileTime.LowDateTime;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct FileTime
	{
		public uint LowDateTime;
		public uint HighDateTime;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct MemoryStatusEx
	{
		public uint Length;
		public uint MemoryLoad;
		public ulong TotalPhysicalMemory;
		public ulong AvailablePhysicalMemory;
		public ulong TotalPageFile;
		public ulong AvailablePageFile;
		public ulong TotalVirtual;
		public ulong AvailableVirtual;
		public ulong AvailableExtendedVirtual;

		public MemoryStatusEx()
		{
			Length = (uint)Marshal.SizeOf<MemoryStatusEx>();
			MemoryLoad = 0;
			TotalPhysicalMemory = 0;
			AvailablePhysicalMemory = 0;
			TotalPageFile = 0;
			AvailablePageFile = 0;
			TotalVirtual = 0;
			AvailableVirtual = 0;
			AvailableExtendedVirtual = 0;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SystemPowerStatus
	{
		public byte ACLineStatus;
		public byte BatteryFlag;
		public byte BatteryLifePercent;
		public byte SystemStatusFlag;
		public uint BatteryLifeTime;
		public uint BatteryFullLifeTime;
	}
}
