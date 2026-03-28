# Sashiko.Registries

**Sashiko.Registries** provides a lightweight, expressive, and predictable foundation for loading structured registry data in Sashiko‑based applications.
It is designed for ecosystems that rely on declarative, JSON‑driven registries — such as languages, names, biomes, items, or any domain where structured data defines behavior.

This library focuses on:

- **Clear boundaries** between trusted (embedded) and untrusted (external) data
- **Consistent validation semantics** powered by `ISchemaValidator`
- **Option‑aware JSON loading** using `System.Text.Json`
- **Rich diagnostics** through `RegistryLoadException` and `ValidationContext`
- **Composable, future‑proof abstractions** for additional formats (YAML, TOML, etc.)

It is intentionally small, focused, and designed to be extended.

---

## ✨ Features

### Two loading modes

- **Embedded loading**
    
    Fast, trusted, and unwrapped. Ideal for internal resources shipped with the application.

- **External loading**
    
    Validated, safe, and wrapped in RegistryLoadException for predictable error handling.

### Schema validation

External JSON is validated using an `ISchemaValidator`, which receives a rich `ValidationContext` including:

- Source file name
- Case‑sensitivity settings
- JSON options
- Registry type
- Arbitrary metadata

### Option‑aware JSON parsing

Both object and list loaders respect:

- `AllowTrailingCommas`
- `ReadCommentHandling`
- `PropertyNameCaseInsensitive`
- Any other `JsonSerializerOptions` you provide

### Custom exception type

`RegistryLoadException` provides:

- A clear signal that the failure occurred during registry loading
- The source file
- The registry type
- The underlying exception

### Symmetric loaders

- `JsonRegistryObjectLoader<T>`
- `JsonRegistryListLoader<T>`

Both follow the same semantics, making the API predictable and easy to reason about.

---

## 📦 Installation

```bash
dotnet add package Sashiko.Registries
```

## 🚀 Quick Start

### 1. Implement a schema validator

```csharp
public sealed class MySchemaValidator : ISchemaValidator
{
    public void Validate<T>(object input, ValidationContext? context = null)
    {
        // Validate JSON against your schema
        // Throw ValidationException on failure
    }
}
```

### 2. Create a registry loader

```csharp
var validator = new MySchemaValidator();

var loader = new JsonRegistryObjectLoader<MyModel>(
    schemaValidator: validator,
    embeddedOptions: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
    externalOptions: new JsonSerializerOptions { AllowTrailingCommas = true }
);
```

### 3. Load embedded or external JSON

```csharp
var model = loader.LoadEmbedded(jsonString, "MyModel.json");
```

```csharp
try
{
    var model = loader.LoadExternal(jsonString, "MyModel.json");
}
catch (RegistryLoadException ex)
{
    Console.WriteLine($"Failed to load registry: {ex.SourceName}");
}
```

## 🧩 API Overview

### `JsonRegistryObjectLoader<T>`

Loads a single JSON object.

- `LoadEmbedded(string json, string source)`
- `LoadExternal(string json, string source)`

### `JsonRegistryListLoader<T>`

Loads a JSON array of objects.

- `LoadEmbedded(string json, string source)`
- `LoadExternal(string json, string source)`

### `RegistryLoadException`

Thrown when external JSON fails:

- format validation
- deserialization

Schema validation errors (`ValidationException`) are not wrapped.

---

## 🛡 Error Model

| Scenario                 | Exception              |
|--------------------------|------------------------|
| Embedded JSON invalid    | `JsonException`        |
| External JSON invalid    | `RegistryLoadException` |
| Schema mismatch          | `ValidationException`  |
| Deserialization failure  | `RegistryLoadException` |

This model is intentional: embedded JSON is trusted, external JSON is not.

---

## 🧪 Testing
The test suite documents the intended behavior:

- embedded vs external semantics
- JSON option inheritance
- trailing comma handling
- schema validation integration
- ValidationContext metadata
- RegistryLoadException wrapping

Tests are written to be expressive and behavior‑driven, mirroring the design of the library.

---

## 🧱 Roadmap
Sashiko.Registries is the foundation for future registry modules:

- `Sashiko.Registries.Languages`
- `Sashiko.Registries.Names`

Future enhancements may include:

- YAML/TOML/XML loaders
- Built‑in schema validators
- Registry composition utilities
- Source‑mapped diagnostics

---

## ❤️ Philosophy
Sashiko.Registries is built on a few core principles:

- **Clarity over cleverness**
- **Predictability over magic**
- **Explicit boundaries**
- **Expressive error semantics**
- **Composable abstractions**
- **A focus on developer experience**

Every artifact — code, tests, documentation — is crafted to feel intentional.

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
