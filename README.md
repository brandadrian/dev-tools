# dev-tools
Simple command line application with developer helpers

## Table of contents

- [Features and examples](#features-and-examples)
	- [to-upper](#to-upper)
	- [to-lower](#to-lower)
	- [sync-repos](#sync-repos)
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
dotnet run --project src/DevTools -- to-lower "IgnoreDuplicateCheckIfOlderThan"
```

Output:

```text
hello world
```

### sync-repos

Synchronizes Git repositories defined in a JSON configuration file. Missing repositories are cloned, while existing repositories are checked out to the configured branch and updated with a fast-forward-only pull.

The configuration file can be stored anywhere. Pass its path as the command argument; relative paths are resolved from the repository root when the command is run from there.

Example `repositories.json`:

```json
{
	"metadata": {
		"description": "Repositories used by the development team",
		"version": "1.0"
	},
	"repositories": [
		{
			"gitRepositoryUrl": "git@github.com:example/project.git",
			"name": "project",
			"branch": "main",
			"folder": "./repositories"
		}
	]
}
```

Run the command from the repository root:

```bash
dotnet run --project src/DevTools -- sync-repos ./repositories.json
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
