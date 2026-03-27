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
