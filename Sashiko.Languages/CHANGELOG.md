# Changelog — Sashiko.Languages

All notable changes to this project will be documented in this file.  
The format follows [Semantic Versioning](https://semver.org/).

---

## [0.1.1] - 2026-05-04

### Improved
- Refined registry loading internals to satisfy SonarQube/Roslyn performance guidance while preserving the public read-only contract.
- Cleaned test helpers and enum assertions reported by SonarQube/Roslyn analysis.
- Updated the package to depend on **Sashiko.Registries** `0.1.1`, bringing in the latest stable Sashiko registry, validation, and core improvements.

### Notes
This patch release does not change the public API or language dataset of **Sashiko.Languages**. It keeps the package aligned with the repository quality gate and the latest stable Sashiko building blocks.

---

## [0.1.0] - 2026-04-09

### Added
- Embedded, strongly-typed language registry based on ISO 639‑3
- Support for:
  - ISO 639‑3 codes (primary identifiers)
  - ISO 639‑2 codes (bibliographic/terminologic)
  - ISO 639‑1 codes (when available)
  - Case-insensitive lookups by code or name
- Immutable snapshot-based registry for predictable behavior
- Public `LanguageRegistry` API for:
  - Lookup by ISO 639‑3
  - Lookup by ISO 639‑2
  - Lookup by ISO 639‑1
  - Enumeration of all languages
- Pre-generated `languages.json` file derived from SIL ISO 639‑3 data
- Documentation covering usage, data model, update process, and attribution

### Testing
- Complete test suite for:
  - ISO 639‑3 lookup
  - ISO 639‑2 lookup
  - ISO 639‑1 lookup
  - Case-insensitive matching
  - Registry enumeration and snapshot behavior
- Validation of derived JSON structure and required fields

### Notes
This is the first stable release of **Sashiko.Languages**.  
It provides a complete, strongly-typed language registry derived from the ISO 639‑3
standard, with correct attribution to SIL International and no redistribution of
original code tables.
