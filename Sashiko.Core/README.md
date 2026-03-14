# Sashiko.Core

**Sashiko.Core** is the foundational utility library of the Sashiko ecosystem.  
It provides lightweight, reusable building blocks designed to support higher‑level Sashiko packages such as `Sashiko.SystemMonitor`.

This package focuses on **unit conversions**, **common abstractions**, and **shared helpers** that are useful across multiple domains.

---

## ✨ Features

- Strongly‑typed unit conversion APIs  
- Enum‑based conversion system for clarity and extensibility  
- Memory conversions (Bytes, KB, MB, GB, TB)  
- Bandwidth conversions (Bits, Kb, Mb, Gb)  
- Round‑trip safe and fully tested  
- Zero dependencies  
- Cross‑platform and .NET‑friendly

---

## 📦 Installation

```bash
dotnet add package Sashiko.Core
```

## 🔧 Usage

### Memory conversions

```csharp
using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;

double gb = MemoryConverter.Convert(1024, MemoryUnit.Megabytes, MemoryUnit.Gigabytes);
// gb == 1.0
```

### Bandwidth conversions
```csharp
using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;

double mbps = BandwidthConverter.Convert(1000, BandwidthUnit.Kilobits, BandwidthUnit.Megabits);
// mbps == 1.0
```

## 🧪 Testing
All conversion logic is covered by a dedicated test suite under:

```code
Sashiko.Core.Tests/Conversions
```

Tests include:

- identity conversions
- cross‑unit conversions
- round‑trip conversions
- tolerance‑based floating‑point validation

## 🧱 Roadmap
Future versions of Sashiko.Core may include:

- Temperature conversions
- Time and duration utilities
- Human‑friendly formatting helpers
- Additional numeric abstractions

---

## 🤝 Contributing
Contributions are welcome!  
If you’d like to improve SystemMonitor or propose new features, please check our [Contributing Guidelines](https://github.com/alex98luca/Sashiko/blob/master/CONTRIBUTING.md).  
Feel free to open an issue or submit a pull request!

---

## 📄 License
This project is licensed under the **Apache License 2.0**.  
See the [LICENSE](https://github.com/alex98luca/Sashiko/blob/master/LICENSE) file for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
