# Changelog — Sashiko Maintenance Tool

This changelog tracks meaningful changes to the internal maintenance tool used
across the Sashiko ecosystem.  
It is intended for contributors.  
The maintenance tool is not versioned or published as a NuGet package.

---

## 2026-04-08
### Added
- Initial implementation of the handler-based command architecture
- `CommandDispatcher` with pluggable handler registry
- `LanguageCommandHandler` with instance-based update hook
- `languages update` command for regenerating the embedded language registry
- Full test suite for dispatcher routing and handler behavior
- Console output isolation and deterministic test setup
- README documenting usage, architecture, and extension model

### Notes
- This is the first official version of the maintenance tool.
- Changes are tracked to support contributors maintaining registry-based libraries.
