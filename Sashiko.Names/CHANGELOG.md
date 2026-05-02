# Changelog — Sashiko.Names

All notable changes to this project will be documented in this file.  
The format follows [Semantic Versioning](https://semver.org/).

---

## [0.1.0] - 2026-05-02

### Added
- Embedded, culturally aware person name registry and generator
- Public `NameService` API for generating structured `PersonName` values
- Strongly typed options for:
  - Supported language selection
  - Random language selection
  - Sex-specific name generation
- Structured `PersonName` model exposing:
  - Given names
  - Last names
  - Patronymics
  - Matronymics
  - Prefixes and suffixes
  - Computed `FullName` and `DisplayName`
- Initial support for:
  - Mandarin Chinese (`cmn`)
  - English (`eng`)
  - French (`fra`)
  - Hindi (`hin`)
  - Icelandic (`isl`)
  - Italian (`ita`)
  - Romanian (`ron`)
  - Russian (`rus`)
  - Spanish (`spa`)
- Language-specific generation rules for:
  - Name ordering
  - Given-name count
  - Double surnames
  - Gendered surnames
  - Patronymic and matronymic patterns
- Embedded `names.json` and `rules.json` data for each supported language
- Internal maintenance command for polishing embedded name pools
- Documentation covering usage, supported languages, data guarantees, maintenance, and source policy

### Data Quality
- Release-floor validation requiring each supported language to provide:
  - At least 150 male given names
  - At least 150 female given names
  - At least 200 active surname values
  - At least 500 unique core name values
- Validation that embedded name pools are:
  - Sorted alphabetically
  - Duplicate-free
  - Trimmed and non-empty
  - Latin-readable using plain Latin or accented Latin characters
- Source policy for maintaining name data from public civil-registration,
  statistical, national name-list, or culturally reviewed sources

### Testing
- Test coverage for:
  - Public name generation through `NameService`
  - Registry loading and supported-language discovery
  - Given-name generation
  - Last-name generation
  - Patronymic generation
  - Matronymic generation
  - Prefix and suffix generation
  - Name assembly
  - Embedded JSON validity and schema validation
  - Embedded data quality guarantees

### Notes
This is the first official release of **Sashiko.Names**.  
It establishes a strongly typed, embedded, dependency-light foundation for
realistic person name generation in the Sashiko ecosystem.
