# DepAnalyzr

Inspired by `NDepend`, this is a toy project to explore the `Mono.Cecil` library.

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

### Analyse eShopOnContainers' Catalog Microservice

```
git clone git@github.com:dotnet-architecture/eShopOnContainers.git
dotnet build ./eShopOnContainers/src/Services/Catalog/Catalog.API/ --self-contained --use-current-runtime --output ./catalog-build
cd ./catalog-build
depanalyzr types graph -a Catalog -f svg > dependencies.svg
firefox dependencies.svg

```

