## [0.1.0] – 2026‑03‑18

### Added

- **Registry loaders**

  - `JsonRegistryObjectLoader<T>` for strongly‑typed object registries
  - `JsonRegistryListLoader<T>` for strongly‑typed list registries
  - Clear separation between **embedded** (trusted) and **external** (untrusted) loading paths

- **Validation pipeline**

  - Integration with `ISchemaValidator` for external JSON

  - Rich `ValidationContext` including:

    - `Source`
    - `IgnoreCase`
    - `JsonOptions`
    - `RegistryType`
    - `IsEmbedded`

  - Consistent validation semantics across object and list loaders

- **Error model**

  - Introduced `RegistryLoadException` for predictable, domain‑specific error handling
  - External JSON failures (format or deserialization) are wrapped
  - Schema validation errors (`ValidationException`) are preserved
  - Embedded JSON surfaces raw exceptions for clarity

- **Option‑aware JSON loading**

  - Full support for custom `JsonSerializerOptions`
  - External JSON parsing respects:
    - `AllowTrailingCommas`
    - `ReadCommentHandling`
    - `PropertyNameCaseInsensitive`
  - Embedded and external options can be reused or defined independently

### Testing

- Complete test suite for:
  - Object registry loader
  - List registry loader
  - ValidationContext propagation
  - Option inheritance and independence
  - Trailing comma and comment handling
  - RegistryLoadException wrapping behavior
- Behavior‑driven tests documenting the intended API and error model

### Notes

This release establishes the foundation for future registry modules (e.g., Languages, Names) by providing a stable, expressive, and fully tested registry loading pipeline.
