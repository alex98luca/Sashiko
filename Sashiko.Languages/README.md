# 🌸 Sashiko.Languages

**Sashiko.Languages** provides a strongly typed, embedded registry of human languages based on the ISO 639-3 standard.

It is designed for applications that need reliable language metadata without calling external services at runtime.

The package ships with a pre-generated `languages.json` file containing ISO 639-3 language entries mapped into the Sashiko language model.

---

## ✨ Features

- Embedded language registry
- ISO 639-3 coverage, including macrolanguages and special codes
- Case-insensitive lookup by:
  - ISO 639-3 code
  - ISO 639-2 code when available
  - ISO 639-1 code when available
  - language name
- Public `LanguageService` API
- Zero runtime dependency on external data sources

---

## 📦 Installation

```bash
dotnet add package Sashiko.Languages
```

---

## 🚀 Usage

### Lookup by ISO 639-3 code

```csharp
using Sashiko.Languages.Api;

var service = new LanguageService();
var language = service.GetIso3("eng");

Console.WriteLine(language.Name); // English
```

### Lookup by ISO 639-1 code

```csharp
using Sashiko.Languages.Api;

var service = new LanguageService();
var language = service.GetIso1("fr");

Console.WriteLine(language.Name); // French
```

### Resolve flexible input

```csharp
using Sashiko.Languages.Api;

var service = new LanguageService();

if (service.TryGet("Italian", out var language))
{
    Console.WriteLine(language.Iso639_3); // ita
}
```

### Enumerate all languages

```csharp
using Sashiko.Languages.Api;

var service = new LanguageService();

foreach (var language in service.All)
{
    Console.WriteLine($"{language.Iso639_3}: {language.Name}");
}
```

---

## 📚 Data Model

Each language entry includes:

- **Iso639_3** — three-letter ISO 639-3 code and primary identifier
- **Iso639_2** — optional three-letter ISO 639-2 code
- **Iso639_1** — optional two-letter ISO 639-1 code
- **Name** — reference language name
- **Scope** — individual, macrolanguage, or special
- **Type** — living, extinct, ancient, historical, constructed, or special

The model is intentionally small and stable.

---

## 🔄 Updating the Embedded Registry

The embedded language registry is maintained with the
**Sashiko Maintenance Tool**:

```bash
dotnet run --project Sashiko.Maintenance -- languages update
```

This process:

1. Downloads the official ISO 639-3 data from SIL International.
2. Parses the source TSV data.
3. Maps it into the Sashiko language model.
4. Writes the updated `languages.json` file into this project.

The maintenance tool is internal and not required at runtime.

---

## 📄 Source Attribution

The language data in this library is derived from the ISO 639-3 code tables provided by **SIL International**.

Required attribution:

> ISO 639-3 data © SIL International.  
> Used under the terms described at https://iso639-3.sil.org/code_tables/download_tables.

This library redistributes derived data only. It does not include or mirror the original SIL code tables.

---

## 🤝 Contributing

Contributions are welcome.  
Please see [CONTRIBUTING.md](../CONTRIBUTING.md) in the repository root.

---

## 📄 License

This project is licensed under the **Apache License 2.0**.  
See [LICENSE](../LICENSE) for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
