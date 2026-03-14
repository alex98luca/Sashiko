# 📡 Sashiko.SystemMonitor

**Sashiko.SystemMonitor** is a lightweight, cross‑platform .NET library for retrieving real‑time system information such as CPU usage, memory consumption, disk activity, network throughput, and more.  
It is the first official package of the **Sashiko** ecosystem — a growing collection of reusable utilities designed to make development faster, cleaner, and more enjoyable.

Sashiko.SystemMonitor provides a simple, unified API that works across Windows, Linux, and macOS, making it ideal for:

- dashboards  
- monitoring tools  
- diagnostics  
- simulations  
- performance analysis  
- custom engines  
- prototyping  

---

## ✨ Features

- Cross‑platform system monitoring  
- CPU usage (per core and total)  
- Memory usage (available, used, total)  
- Disk usage and activity  
- Network throughput  
- Battery information (when available)  
- Snapshot‑based API for easy polling  
- Zero external dependencies  
- Lightweight and modular  

---

## 🚀 Getting Started

### Install via NuGet

```bash
dotnet add package Sashiko.SystemMonitor
```

---

## 📊 Basic Usage

```csharp
using Sashiko.SystemMonitor;

// Create a monitor instance
var monitor = new SystemMonitor();

// Retrieve a snapshot
var snapshot = monitor.GetSnapshot();

// Access system metrics
Console.WriteLine($"CPU Usage: {snapshot.Cpu.TotalUsagePercent}%");
Console.WriteLine($"Memory Used: {snapshot.Memory.UsedMegabytes} MB");
Console.WriteLine($"Disk Read: {snapshot.Disk.ReadBytesPerSecond} B/s");
Console.WriteLine($"Network Upload: {snapshot.Network.UploadBytesPerSecond} B/s");
```

Snapshots are lightweight and designed for periodic polling (e.g., every 250–1000 ms).

---

## 🧩 API Overview
SystemMonitor
The main entry point.
Provides a unified interface for retrieving system metrics.

SystemSnapshot
A structured snapshot containing:

- Cpu
- Memory
- Disk
- Network
- Battery (when supported)

Each category exposes platform‑independent properties.

---

## 🧪 Testing & Reliability
Sashiko.SystemMonitor is built with clarity and reliability in mind.
All platform‑specific logic is isolated, and the snapshot model ensures consistent behavior across operating systems.

---

## 🌸 Part of the Sashiko Ecosystem
Sashiko is an open‑source ecosystem of modular .NET libraries designed to simplify development and accelerate prototyping.
SystemMonitor is the first package, and many more utilities are planned:

- Core helpers
- Validation utilities
- Name generators
- Custom test kit
- JSON and configuration helpers
- Performance and environment tools

You can learn more in the main Sashiko repository.

---

## 🤝 Contributing
Contributions are welcome!  
If you’d like to improve SystemMonitor or propose new features, please check our [Contributing Guidelines](..\CONTRIBUTING.md).  
Feel free to open an issue or submit a pull request!

---

## 📄 License
This project is licensed under the **Apache License 2.0**.  
See the [LICENSE](..\LICENSE.txt) file for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
