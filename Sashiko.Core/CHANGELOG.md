## [0.2.0] - 2026-03-18

### Added
- **JSON utilities**:
  - `JsonListLoader<T>` for strongly-typed list deserialization
  - `JsonObjectLoader<T>` for strongly-typed object deserialization
  - `JsonFileReader` with consistent default options and clear error behavior
  - `JsonFileWriter` with neutral default serialization options

- **Registry utilities**:
  - `RegistrySnapshot` for atomic, case-insensitive dictionary replacement
  - `CreateEmpty<T>` helper for consistent registry initialization

### Improved
- Unified JSON error behavior across loaders and file utilities
- Consistent default `JsonSerializerOptions` across all components
- Case-insensitive semantics clarified and enforced in registry tests

### Testing
- Complete test suite for:
  - JSON list loader
  - JSON object loader
  - JSON file reader
  - JSON file writer
  - Registry snapshot utilities
- Refined error expectation tests to match `System.Text.Json` behavior
- Ensured all snapshot tests use case-insensitive dictionaries

### Notes
This release establishes the foundation for future modules (Registries, Validation, Configuration) by providing stable, predictable, and fully tested JSON and registry primitives.

---

## [0.1.0-alpha] - 2026-03-14

- Initial release of **Sashiko.Core**
- `MemoryConverter` with enum-based conversion:
  - Bytes, Kilobytes, Megabytes, Gigabytes, Terabytes
- `BandwidthConverter` with enum-based conversion:
  - Bits, Kilobits, Megabits, Gigabits
- Strongly-typed `MemoryUnit` and `BandwidthUnit` enums
- Full test suite for all conversion paths
- Project metadata and documentation

---

## [Unreleased]

### Planned
- Temperature conversion utilities
- Time and duration helpers
- Human-readable formatting (e.g., “1.5 GB”)
- Additional numeric abstractions
