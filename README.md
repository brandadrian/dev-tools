# dev-tools

Simple command line application with developer helpers

## Table of contents

- [Getting started](#getting-started)
  - [Clone](#clone)
  - [Build](#build)
  - [Install as a global tool](#install-as-a-global-tool)
  - [Uninstall](#uninstall)
- [Features and examples](#features-and-examples)
  - [to-upper](#to-upper)
  - [to-lower](#to-lower)
  - [base64-encode](#base64-encode)
  - [base64-decode](#base64-decode)
  - [sync-repos](#sync-repos)
  - [Help](#help)

## Getting started

### Clone

```bash
git clone https://github.com/<owner>/dev-tools.git
cd dev-tools
```

### Build

From the repository root:

```bash
dotnet build DevTools.sln
```

### Install as a global tool

Pack the CLI project into a NuGet package, then install it as a .NET global tool. This makes the `dev-tools` command available from anywhere on your machine.

```bash
dotnet pack src/DevTools/DevTools.csproj -c Release
dotnet tool install --add-source ./src/DevTools/bin/Release --global DevTools
```

Verify the installation:

```bash
dev-tools --help
```

To upgrade after pulling new changes, pack again and run `dotnet tool update` instead of `install`:

```bash
dotnet pack src/DevTools/DevTools.csproj -c Release
dotnet tool update --add-source ./src/DevTools/bin/Release --global DevTools
```

### Uninstall

```bash
dotnet tool uninstall --global DevTools
```

## Features and examples

Run these commands from the repository root. Quote text that contains spaces.

### base64-encode

Encodes text to Base64.

```bash
dotnet run --project src/DevTools -- base64-encode "Hello world"
```

Output:

```text
SGVsbG8gd29ybGQ=
```

### base64-decode

Decodes Base64 text.

```bash
dotnet run --project src/DevTools -- base64-decode "SGVsbG8gd29ybGQ="
```

Output:

```text
Hello world
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
dotnet run --project src/DevTools -- to-lower "HELLO WORLD"
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
