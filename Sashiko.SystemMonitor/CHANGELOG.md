# Changelog — Sashiko.SystemMonitor

All notable changes to this project will be documented in this file.  
The format follows [Semantic Versioning](https://semver.org/).

---

## [0.1.3-alpha] - 2026-05-05

### Improved
- Replaced the internal SystemMonitor platform detector with the shared `Sashiko.Core` runtime environment model.
- Moved Windows native interop calls into a dedicated internal wrapper, keeping monitor classes focused on monitoring logic.
- Removed source-generator `partial` requirements from the monitor classes themselves.

### Packaging
- Updated the package to depend on `Sashiko.Core` 0.4.0.

### Notes
This alpha builds on the 0.1.2 cleanup by aligning runtime detection with the wider Sashiko ecosystem.

---

## [0.1.2-alpha] - 2026-05-04

### Improved
- Added cached platform detection so snapshot capture reuses the detected operating system instead of repeating OS checks across monitors.
- Updated Windows interop declarations to use source-generated `LibraryImport` APIs.
- Refined Linux and macOS probe fallbacks so unavailable hardware sensors degrade intentionally without noisy exceptions.
- Cleaned CPU, GPU, memory, thermal, and power monitor internals based on SonarQube maintainability findings.

### Testing
- Added coverage for cached platform reuse and OS monitor mapping.

### Packaging
- Updated the package dependency to `Sashiko.Core` 0.3.1.
- Added the official Sashiko NuGet icon metadata.

### Notes
This alpha keeps the public snapshot API unchanged while improving internal maintainability and repeated-capture behavior.

---

## [0.1.1-alpha] - 2026-03-16

### Changed
- Refactored `DiskMonitor` and `MemoryMonitor` to use `Sashiko.Core` conversion APIs
- Updated project to depend on `Sashiko.Core` 0.1.0-alpha
- Improved consistency and readability across monitoring modules

### Fixed
- Corrected namespace in test project
- Updated README references

---

## [0.1.0-alpha] - 2026-03-14
- Initial release of **Sashiko.SystemMonitor**
- Added cross-platform CPU, memory, disk, network, thermal, and power monitoring
- Added `SystemSnapshotProvider` for unified snapshot capture
- Added smoke and structural tests
