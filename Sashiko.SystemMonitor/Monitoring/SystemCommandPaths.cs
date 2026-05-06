namespace Sashiko.SystemMonitor.Monitoring
{
	internal static class SystemCommandPaths
	{
		internal const string LinuxLspci = "/usr/bin/lspci";
		internal const string LinuxNvidiaSmi = "/usr/bin/nvidia-smi";
		internal const string MacPmset = "/usr/bin/pmset";
		internal const string MacPs = "/bin/ps";
		internal const string MacSystemProfiler = "/usr/sbin/system_profiler";
		internal const string MacSysctl = "/usr/sbin/sysctl";
		internal const string MacVmStat = "/usr/bin/vm_stat";

		internal static string WindowsDxDiag => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.Windows),
			"System32",
			"dxdiag.exe"
		);
	}
}
