# 🌸 Sashiko.Core

**Sashiko.Core** is the foundational utility package of the Sashiko ecosystem.

It provides small, reusable building blocks used by higher-level Sashiko packages: conversions, JSON file helpers, text normalization, probability helpers, and shared primitives.

---

## ✨ Features

- Strongly typed memory conversions
- Strongly typed bandwidth conversions
- JSON file read/write helpers
- Text normalization helpers
- Probability utilities and random selection helpers
- Lightweight environment/runtime information
- Zero external dependencies

---

## 📦 Installation

```bash
dotnet add package Sashiko.Core
```

---

## 🚀 Usage

### Memory conversions

```csharp
using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;

var gigabytes = MemoryConverter.Convert(
    1024,
    MemoryUnit.Megabytes,
    MemoryUnit.Gigabytes);

Console.WriteLine(gigabytes); // 1
```

### Bandwidth conversions

```csharp
using Sashiko.Core.Conversions;
using Sashiko.Core.Models.Enums;

var megabits = BandwidthConverter.Convert(
    1000,
    BandwidthUnit.Kilobits,
    BandwidthUnit.Megabits);

Console.WriteLine(megabits); // 1
```

---

## 🧪 Testing

The test suite covers:

- identity conversions
- cross-unit conversions
- round-trip conversions
- JSON file read/write behavior
- registry snapshot helpers
- probability helpers
- text normalization

---

## 🗺️ Roadmap

Future versions may include:

- temperature conversions
- time and duration helpers
- human-readable formatting helpers
- additional numeric abstractions

---

## 🤝 Contributing

Contributions are welcome.  
Please see [CONTRIBUTING.md](../CONTRIBUTING.md) in the repository root.

---

## 📄 License

This project is licensed under the **Apache License 2.0**.  
See [LICENSE](../LICENSE) for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
