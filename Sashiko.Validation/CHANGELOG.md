# Changelog — Sashiko.Validation

All notable changes to this project will be documented in this file.  
The format follows [Semantic Versioning](https://semver.org/).

---

## [0.1.1] - 2026-05-03

### Improved
- Refactored schema comparison internals to reduce cognitive complexity while preserving structural comparison behavior.
- Simplified JSON array validation internals by marking stateless validation helpers as static.
- Cleaned Validation test assertions reported by SonarQube/Roslyn analysis.

### Testing
- Added focused coverage for `ValidationContext` default and init-only property behavior.
- Added focused coverage for `ValidationException` constructors.
- Added nullable-reference detector regression coverage for legacy reference types without nullable metadata.

### Notes
This patch release does not change the public API or runtime contract of **Sashiko.Validation**. It keeps the package aligned with the repository quality gate and strengthens regression coverage around foundational validation components.

---

## [0.1.0] - 2026-03-27

### Added
- **Hierarchical schema model** (`SchemaNode`) replacing the old flattened path-based representation.
- **C# schema inspector** with:
  - reflection-based structural extraction
  - required/optional property detection
  - leaf, object, and collection node support
  - recursion protection for cyclic object graphs
- **JSON schema inspector** with:
  - structural extraction from `JsonElement`
  - object and array handling
  - element schema inference for arrays
  - support for empty arrays
- **Structural schema comparer** with:
  - nested object comparison
  - array element schema comparison
  - kind mismatch detection
  - case-sensitive and case-insensitive matching
- **JSON schema validator** with:
  - per-element array validation
  - case-insensitive validation via `ValidationContext.IgnoreCase`
  - clear, human-friendly error messages
- **Complete test suite** covering:
  - C# schema extraction
  - JSON schema extraction
  - schema comparison (including ignore-case behavior)
  - validator integration
  - recursion and nested structures

### Notes
This is the first official release of **Sashiko.Validation**, establishing the foundation for future Sashiko modules (Configuration, Registries, Semantic Validation).  
The architecture is stable, fully tested, and ready for production use.

---

## [Unreleased]

### Planned
- YAML and XML schema validators  
- Semantic validation (ranges, patterns, enums)  
- Domain-specific validation rules  
- Structured validation diagnostics  
