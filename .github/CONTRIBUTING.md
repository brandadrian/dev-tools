# Contributing to dev-tools

Thanks for your interest in contributing! This document covers how to get set up
and the conventions used in this repo.

## Getting started

```bash
git clone https://github.com/brandadrian/dev-tools.git
cd dev-tools
dotnet tool restore
dotnet build DevTools.sln
```

## Running tests

```bash
dotnet test DevTools.sln
```

## Project structure

Each CLI feature lives in its own folder under `src/DevTools/<Feature>Command/`,
following a consistent pattern:

- `Commands/` (or root) — `System.CommandLine` `Command` subclasses, one per CLI verb.
- `<Feature>CommandProcessor.cs` — orchestrates request/response, writes to console.
- `Models/` — request/result POCOs and enums.
- `Services/` — `I<Feature>Service` + implementation, does the actual work.
- `ServiceCollectionExtensions.cs` — `Add<Feature>()` registers services as singletons.

New feature modules must be registered in `src/DevTools/Program.cs` via `.Add<Feature>()`.

## Adding a new command

1. Create the feature folder following the structure above.
2. Add unit tests under `tests/DevTools.Tests/<Feature>Command/`.
3. Register the feature's services in `Program.cs`.
4. Update the [README](README.md) with a usage example.

## Code style

- The repo uses an [`.editorconfig`](.editorconfig) — most IDEs apply it automatically.
- Run `dotnet format` before committing to ensure consistent formatting.

## Submitting changes

1. Fork the repo and create a feature branch.
2. Make your changes with tests covering new behavior.
3. Ensure `dotnet build` and `dotnet test` both pass locally.
4. Open a pull request describing the change and why it's needed.

## Versioning

This repo uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)
for automatic semantic versioning based on git history — you don't need to manually
bump version numbers in pull requests.

## Code of Conduct

Participation in this project is governed by our [Code of Conduct](CODE_OF_CONDUCT.md).

## Reporting security issues

Please see [SECURITY.md](SECURITY.md) instead of opening a public issue.
