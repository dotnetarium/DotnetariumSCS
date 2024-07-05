# DotnetariumSCS
DotnetariumSCS is a console application designed to provide comprehensive static code analysis for .NET projects and solutions.
A standalone fork of [Security Code Scan](https://github.com/security-code-scan/security-code-scan)

This repo contains only Tools (console apps for .NET Fx and .NET global tool). [Nuget package repo](https://github.com/dotnetarium/Dotnetarium.Analyzers.SCS) with analyzers.

Synked fork (with updated packages and the latest Roslyn) is available [here](https://github.com/dbalikhin/security-code-scan)

## Getting Started


### Prerequisites

- .NET SDK to install the app as a [global tool](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install)

### Supported .NET versions
- .NET 6.0
- .NET 8.0
- .NET 4.7.2 - 4.8

End-of-life .NET versions will be dropped; new stable .NET versions will be added 

### Installation

#### As a .NET Global Tool

To install DotnetariumSCS as a .NET global tool, run:

```sh
dotnet tool install --global dotnetarium-scs
```

#### As a NuGet Package

To install DotnetariumSCS as a NuGet package, add the following package to your project `Dotnetarium.Analyzers.SCS`

#### As a Visual Studio extension

Not supported yet. Continue to use [Security Code Scan version](https://marketplace.visualstudio.com/items?itemName=JaroslavLobacevski.SecurityCodeScanVS2019).
At this point, no changes will affect the Visual Studio extension experience.


## Usage

Run the application from the command line using the required options. Below are the available options:
Required Options

    <solution-or-project-path>
        Description: Specifies the path to the solution or project file.
        Usage: dotnetarium-scs "<path-to-solution-or-project>"

Optional Options

    -w | --excl-warn=<warnings>
        Description: Semicolon delimited list of warnings to exclude.
        Usage: -w "CS0168;CS0219"

    --incl-warn=<warnings>
        Description: Semicolon delimited list of warnings to include.
        Usage: --incl-warn "CS0028;CS0052"

    -p | --excl-proj=<patterns>
        Description: Semicolon delimited list of glob project patterns to exclude.
        Usage: -p "*.Tests;*.Samples"

    --incl-proj=<patterns>
        Description: Semicolon delimited list of glob project patterns to include.
        Usage: --incl-proj "*.Main;*.Core"

    -x | --export=<file-path>
        Description: Path to the SARIF file for exporting analysis results.
        Usage: -x "results.sarif"

    -c | --config=<file-path>
        Description: Path to an additional configuration file.
        Usage: -c "config.json"

    --cwe
        Description: Show CWE IDs in the analysis results.
        Usage: --cwe

    -t | --threads=<number>
        Description: Run analysis in parallel (experimental).
        Usage: -t 4

    --sdk-path=<path>
        Description: Path to the .NET SDK to use.
        Usage: --sdk-path "C:\Program Files\dotnet\sdk"

    --ignore-msbuild-errors
        Description: Do not stop on MSBuild errors.
        Usage: --ignore-msbuild-errors

    --ignore-compiler-errors
        Description: Do not exit with a non-zero code on compilation errors.
        Usage: --ignore-compiler-errors

    -f | --fail-any-warn
        Description: Fail on security warnings with a non-zero exit code.
        Usage: -f

    -n | --no-banner
        Description: Do not show the banner.
        Usage: -n

    -v | --verbose
        Description: Display more diagnostic messages.
        Usage: -v

    -h | -? | --help
        Description: Show this message and exit.
        Usage: -h

Examples
Basic Analysis

```sh
dotnetarium-scs "path/to/solution.sln"
```

Exclude Specific Warnings

```sh
dotnetarium-scs "path/to/project.csproj" -w "CS0168;CS0219"
```

Include Specific Projects Only

```sh
dotnetarium-scs "path/to/solution.sln" --incl-proj "*.Main;*.Core"
```

Export Results to SARIF File

```sh
dotnetarium-scs "path/to/solution.sln" -x "results.sarif"
```

#### Compatibility

DotnetariumSCS is backward compatible with the Security Code Scan project. The Security Code Scan GitHub repository has more details.

#### Contributing

If you would like to contribute to DotnetariumSCS, please fork the repository and submit a pull request. For major changes, please open an issue to discuss what you would like to change.

#### License

DotnetariumSCS is licensed under the LGPL License. See the LICENSE file for more information.

#### Contact

For support or any inquiries, please open an issue on GitHub
