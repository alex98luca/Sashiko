# 🌸 Sashiko.SystemMonitor

**Sashiko.SystemMonitor** provides a lightweight snapshot API for reading basic system information from .NET applications.

It is useful for diagnostics, local dashboards, development tooling, and applications that need a simple view of the current machine.

This package is still in alpha because hardware monitoring depends heavily on operating system permissions, drivers, and vendor-specific APIs. The public API is intentionally small while the collectors become more mature.

---

## ✨ Features

- Operating system information
- CPU model, core count, load, and clock data
- GPU information when available
- Memory totals and process usage
- Disk totals and free/used space
- Thermal information when available
- Power and battery information when available
- Snapshot-based API
- Cached platform detection for cheaper repeated snapshots
- Zero external dependencies

---

## 📦 Installation

```bash
dotnet add package Sashiko.SystemMonitor
```

---

## 🚀 Usage

```csharp
using Sashiko.SystemMonitor.Snapshot;

var snapshot = SystemSnapshotProvider.Capture();

Console.WriteLine($"OS: {snapshot.Os.Family} {snapshot.Os.Version}");
Console.WriteLine($"CPU: {snapshot.Cpu.Model}");
Console.WriteLine($"CPU Load: {snapshot.Cpu.LoadPercent}%");
Console.WriteLine($"Memory Used: {snapshot.Memory.UsedBySystemGB} GB");
Console.WriteLine($"Disk Free: {snapshot.Disk.FreeGB} GB");
```

Snapshots are designed for simple polling when you need periodic system state.

---

## 🖥️ Platform Notes

SystemMonitor uses built-in operating system APIs and command-line probes only. When a metric is unavailable, blocked by permissions, or unsupported by the current machine, the snapshot returns a neutral value instead of throwing.

- Windows support currently focuses on CPU, memory, disk, and power data available through native APIs.
- Linux support can read richer `/proc` and `/sys` data when those virtual files are exposed by the host.
- macOS support uses native command-line tools where available, with thermal data planned for a future collector.

The operating system is detected once per process through `Sashiko.Core` runtime metadata and reused by the snapshot pipeline, keeping repeated captures lighter and more predictable.

---

## 📚 Data Model

`SystemSnapshot` contains:

- **Os** — operating system family, version, architecture, and mobile flag
- **Cpu** — model, cores, load, and clock information
- **Gpu** — vendor, model, VRAM, and load information when available
- **Memory** — total, available, system-used, and process-used memory
- **Disk** — total, free, and used disk space
- **Thermal** — CPU, GPU, and system temperatures when available
- **Power** — battery and power-state information when available

Availability and precision can vary by platform and hardware.

---

## 🧪 Testing

The test suite verifies that snapshot capture:

- does not throw
- returns a complete snapshot structure
- keeps model types consistent across captures

Platform-specific values may differ by operating system and machine.

---

## 🗺️ Roadmap

Future versions may include:

- richer network metrics
- improved platform-specific collectors
- configurable fallbacks for unavailable sensors
- historical sampling helpers
- configurable polling utilities

---

## 🤝 Contributing

Contributions are welcome.  
Please see [CONTRIBUTING.md](../CONTRIBUTING.md) in the repository root.

---

## 📄 License

This project is licensed under the **Apache License 2.0**.  
See [LICENSE](../LICENSE) for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
