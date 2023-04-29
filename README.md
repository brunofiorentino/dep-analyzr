# DepAnalyzr

Really poor man's NDepend, full of bugs -- and excuse to play with the Mono.Cecil library. It provides a simple CLI to generate component dependency graphs.

## Prerequisites

- .NET 6 SDK
- Graphviz (`dot` command) => `apt install graphviz`
- Firefox (or an alternative svg viewer)
- Note: working only on Linux for now

## Installation

```./pack.sh``` 

```./install.sh``` # Install as a global .NET Tool.

## Examples

### Analyse Dapper core assembly

```
git clone git@github.com:DapperLib/Dapper.git
dotnet build ./Dapper/Dapper --self-contained --use-current-runtime --output ./dapper-build
cd dapper-build
depanalyzr types graph -a Dapper -f svg > dependencies.svg
firefox dependencies.svg
```
