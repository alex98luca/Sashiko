# Sashiko.Validation

**Sashiko.Validation** is the structural validation engine of the Sashiko ecosystem.
It provides a lightweight, modular, and format‑agnostic framework for validating configuration files, data models, and runtime inputs **before** deserialization or domain processing.

At its core, Sashiko.Validation extracts **hierarchical schemas** from C# types and JSON documents, compares them structurally, and reports clear, actionable diffs.

---

## ✨ Features

- Hierarchical C# schema extraction via reflection
- Hierarchical JSON schema extraction via structural inspection
- Tree‑based schema comparison with precise diff reporting
- Format‑agnostic validation abstraction (`ISchemaValidator`)
- JSON schema validator included out of the box
- Case‑sensitive or case‑insensitive matching
- Clear, human‑friendly error messages
- Zero external dependencies
- Modular architecture designed for future validators (YAML, XML, semantic rules)

---

## 📦 Installation

```bash
dotnet add package Sashiko.Validation
```

---

## 🔧 Usage

Validate JSON against a C# model

```csharp
using Sashiko.Validation;
using Sashiko.Validation.Validators.Json;

var validator = new JsonSchemaValidator();

validator.Validate<MyConfig>(
    jsonString,
    new ValidationContext { Source = "config.json" }
);
```

If the JSON structure does not match the schema of `MyConfig`,
a `ValidationException` is thrown with a detailed diff:

```Code
Schema mismatch in 'config.json'.
Missing required fields:
  Settings.Mode
Unexpected fields:
  Debug.Verbose
```

---

## 🧠 How it works

Sashiko.Validation is built on three core components:

1. C# Schema Extraction
`CSharpSchemaInspector` walks the public instance properties of a type and produces a **hierarchical schema tree:**

```Code
Root
 ├── Database
 │    ├── Host
 │    └── Port
 └── Logging
      └── Level
```

It understands:

- nested objects
- collections and element schemas
- nullable vs non‑nullable types
- required vs optional fields
- leaf vs complex types
- recursion (with safe cycle detection)

---

2. JSON Schema Extraction
`JsonSchemaInspector` walks a `JsonElement` and produces a **parallel hierarchical schema:**

```Code
root
 ├── database
 │    ├── host
 │    └── port
 └── logging
      └── level
```

It handles:

- objects
- arrays
- arrays of primitives
- arrays of objects
- mixed structures
- empty arrays (with inferred leaf element schema)

---

3. Schema Comparison
`SchemaComparer` performs a **structural diff** between two schema trees.

It detects:

- missing required fields
- unexpected fields
- kind mismatches (object vs array vs leaf)
- nested mismatches
- array element mismatches
- case‑sensitive or case‑insensitive differences

It returns a `SchemaDiff`:

```csharp
IReadOnlyList<string> Missing
IReadOnlyList<string> Extra
bool IsMatch
```

---

4. Validation
`JsonSchemaValidator` ties everything together:

- parses JSON
- extracts JSON schema
- extracts C# schema
- compares them structurally
- validates each array element individually
- throws `ValidationException` on mismatch

This ensures your configuration or input data is structurally correct before deserialization.

---

## 🧪 Testing

The validation engine is fully testable in isolation.

Included test suites cover:

- C# schema extraction
- JSON schema extraction
- schema comparison (including ignore‑case behavior)
- validator integration
- arrays, nested objects, primitives, recursion

A typical test verifies:

- missing fields
- extra fields
- nested mismatches
- array element mismatches
- case‑insensitive matching

---

## 🧱 Roadmap
Future versions of Sashiko.Validation may include:

- YAML schema validator
- XML schema validator
- semantic validation (ranges, enums, patterns)
- domain‑specific validators
- composite validation pipelines
- structured validation error objects
- error codes and diagnostics

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
