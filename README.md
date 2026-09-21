# dev-tools
Simple command line application with developer helpers

## Table of contents

- [Features and examples](#features-and-examples)
	- [to-upper](#to-upper)
	- [to-lower](#to-lower)
	- [Help](#help)
- [Build](#build)

## Features and examples

Run these commands from the repository root. Quote text that contains spaces.

### to-upper

Converts text to uppercase.

```bash
dotnet run --project src/DevTools -- to-upper "Hello world"
```

Output:

```text
HELLO WORLD
```

### to-lower

Converts text to lowercase.

```bash
dotnet run --project src/DevTools -- to-lower "Hello WORLD"
```

Output:

```text
hello world
```

### Help

List available commands or show help for a specific command:

```bash
dotnet run --project src/DevTools -- --help
dotnet run --project src/DevTools -- to-upper --help
```

## Build

From the repository root:

```bash
dotnet build src/DevTools.sln
```
