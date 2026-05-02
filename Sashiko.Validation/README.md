# 🌸 Sashiko.Validation

**Sashiko.Validation** provides lightweight structural validation utilities for JSON data, C# models, and registry-style inputs.

It is built to catch shape problems early, before data is deserialized into a domain model or used by higher-level Sashiko packages.

---

## ✨ Features

- C# schema extraction through reflection
- JSON schema extraction through structural inspection
- Schema comparison with clear missing/extra field reporting
- JSON schema validator included out of the box
- Case-sensitive or case-insensitive matching
- Array element validation
- Format-agnostic `ISchemaValidator` abstraction
- Zero external dependencies

---

## 📦 Installation

```bash
dotnet add package Sashiko.Validation
```

---

## 🚀 Usage

### Validate JSON against a C# model

```csharp
using Sashiko.Validation;
using Sashiko.Validation.Validators.Json;

var validator = new JsonSchemaValidator();

validator.Validate<MyConfig>(
    jsonString,
    new ValidationContext { Source = "config.json" });
```

If the JSON structure does not match the target model, a `ValidationException` is thrown with a readable diff.

```text
Schema mismatch in 'config.json'.
Missing required fields:
  Settings.Mode
Unexpected fields:
  Debug.Verbose
```

---

## 🧠 How It Works

Sashiko.Validation compares two schema trees:

- the expected schema extracted from a C# type
- the actual schema extracted from JSON

The comparer reports:

- missing required fields
- unexpected fields
- object, array, and leaf mismatches
- nested mismatches
- array element mismatches

This keeps validation focused on structure while leaving semantic domain rules to higher-level code.

---

## 🧪 Testing

The test suite covers:

- C# schema extraction
- JSON schema extraction
- schema comparison
- nullable and required property behavior
- validator integration
- arrays, nested objects, primitives, and recursion safety

---

## 🗺️ Roadmap

Future versions may include:

- semantic validation helpers
- structured validation error objects
- error codes and diagnostics
- additional format inspectors
- composite validation pipelines

---

## 🤝 Contributing

Contributions are welcome.  
Please see [CONTRIBUTING.md](../CONTRIBUTING.md) in the repository root.

---

## 📄 License

This project is licensed under the **Apache License 2.0**.  
See [LICENSE](../LICENSE) for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
