# Sashiko.Languages

**Sashiko.Languages** provides a strongly‑typed, embedded registry of human
languages based on the ISO 639‑3 standard.  
It is designed for applications that need reliable, structured language
information without depending on external services at runtime.

The library ships with a pre‑generated `languages.json` file containing all
ISO 639‑3 language entries, mapped into the Sashiko language model.

---

## 🚀 Features

- Embedded, strongly‑typed language registry
- ISO 639‑3 coverage (including macrolanguages and special codes)
- Case‑insensitive lookup by:
  - ISO 639‑3 code
  - ISO 639‑2 code (when available)
  - ISO 639‑1 code (when available)
  - Language name
- Immutable, snapshot‑based registry for predictable behavior
- Zero runtime dependencies on external data sources

---

## 📦 Installation

```bash
dotnet add package Sashiko.Languages
```

---

## 🧩 Usage

### Lookup by ISO 639‑3 code

```csharp
var lang = LanguageRegistry.Get("eng");

Console.WriteLine(lang.Name); // "English"
```

### Lookup by ISO 639‑1 code

```csharp
var lang = LanguageRegistry.GetByIso639_1("fr");

Console.WriteLine(lang.Name); // "French"
```

### Enumerate all languages

```csharp
foreach (var language in LanguageRegistry.All)
{
    Console.WriteLine($"{language.Iso639_3}: {language.Name}");
}
```

---

## 📚 Data Model

Each language entry includes:

- **Iso639_3** — three‑letter ISO 639‑3 code (primary identifier)
- **Iso639_2** — optional three‑letter ISO 639‑2 code (bibliographic/terminologic)
- **Iso639_1** — optional two‑letter ISO 639‑1 code
- **Name** — reference name
- **Scope** — individual, macrolanguage, special
- **Type** — living, extinct, ancient, constructed, etc.

The model is intentionally minimal and stable.

---

## 🔄 Updating the Embedded Registry

The embedded languages.json file is generated using the
**Sashiko Maintenance Tool**:

```bash
dotnet run --project src/Sashiko.Maintenance -- languages update
```

This process:

1. Downloads the official ISO 639‑3 data from SIL International
2. Parses the TSV files
3. Maps them into the Sashiko language model
4. Writes the updated `languages.json` file into this project

The maintenance tool is internal and not required at runtime.

---

## 📄 Source Attribution (Required)

The language data in this library is **derived from** the ISO 639‑3 code tables
provided by **SIL International**.

Required attribution:

> ISO 639‑3 data © SIL International.  
> Used under the terms described at https://iso639-3.sil.org/code_tables/download_tables.

This library redistributes **derived data only**.  
It does **not** include or redistribute the original ISO 639‑3 code tables, and it does not
provide any mechanism to download or mirror the original SIL data.

---

## 🤝 Contributing
Contributions are welcome!  
If you’d like to improve the languages library or propose new features, please see the  
[CONTRIBUTING.md](../CONTRIBUTING.md) file in the repository root.

Feel free to open an issue or submit a pull request!

---

## 📄 License
This project is licensed under the **Apache License 2.0**.  
See the [LICENSE](../LICENSE) file for the full license text.

Copyright © 2026 Alexandru Luca (alex98luca)
