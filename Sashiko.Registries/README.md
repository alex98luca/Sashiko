# 🌸 Sashiko.Registries

**Sashiko.Registries** provides lightweight helpers for loading structured registry data from JSON.

It is designed for packages and applications that rely on declarative data: languages, names, biomes, items, rules, or any domain where structured data defines behavior.

---

## ✨ Features

- JSON object registry loader
- JSON list registry loader
- Separate loading behavior for trusted embedded data and untrusted external data
- Schema validation integration through `ISchemaValidator`
- Option-aware JSON parsing with `System.Text.Json`
- Predictable `RegistryLoadException` wrapping for external input failures
- Clear source/type diagnostics for easier debugging

---

## 📦 Installation

```bash
dotnet add package Sashiko.Registries
```

---

## 🚀 Usage

### Create a registry loader

```csharp
using System.Text.Json;
using Sashiko.Registries.Json;
using Sashiko.Validation.Validators.Json;

var loader = new JsonRegistryObjectLoader<MyRegistryEntry>(
    schemaValidator: new JsonSchemaValidator(),
    embeddedOptions: new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = false
    },
    externalOptions: new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    });
```

### Load trusted embedded JSON

```csharp
var entry = loader.LoadEmbedded(json, "MyRegistryEntry.json");
```

### Load and validate external JSON

```csharp
try
{
    var entry = loader.LoadExternal(json, "MyRegistryEntry.json");
}
catch (RegistryLoadException ex)
{
    Console.WriteLine($"Failed to load registry data from {ex.SourceName}");
}
```

---

## 🛡️ Error Model

| Scenario | Exception |
|----------|-----------|
| Embedded JSON invalid | `JsonException` |
| External JSON invalid | `RegistryLoadException` |
| External schema mismatch | `ValidationException` |
| External deserialization failure | `RegistryLoadException` |

Embedded data is treated as trusted package data. External data is validated and wrapped so callers can handle registry-loading failures consistently.

---

## 🧪 Testing

The test suite covers:

- embedded vs external loading behavior
- JSON option inheritance
- trailing comma handling
- schema validation integration
- validation context metadata
- exception wrapping semantics

---

## 🗺️ Roadmap

Future versions may include:

- additional structured-data formats
- richer registry composition helpers
- source-mapped diagnostics
- built-in validator integrations

---

## 🤝 Contributing

Contributions are welcome.  
Please see [CONTRIBUTING.md](../CONTRIBUTING.md) in the repository root.

---

## 📄 License

This project is licensed under the **Apache License 2.0**.  
See [LICENSE](../LICENSE) for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
