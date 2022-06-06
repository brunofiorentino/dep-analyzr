# dep-analizr
DepAnalyzr is a poor man's NDepend -- actually a "toy project" to explore the great Mono.Cecil library and, who knows, maybe turn into something more serious.

## Commands:

[_] depanlyzr data --assembly-pattern [regex]
[/] depanlyzr assemblies matrix --dependent-pattern [regex] --dependency-pattern [regex] --format [table|csv]
[_] depanlyzr assemblies graph --pattern [regex] --format [dot|svg]
[/] depanlyzr types matrix --dependent-pattern [regex] --dependency-pattern [regex] --format [table|csv]
[_] depanlyzr types graph --pattern [regex] --format [dot|svg]

