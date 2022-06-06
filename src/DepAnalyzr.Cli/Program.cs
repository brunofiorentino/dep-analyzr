using System.CommandLine;
using DepAnalyzr.Application;

var rootCommand = new RootCommand();
var assemblyPatternOption = new Option<string>(new[] { "--assembly-pattern", "-a" }) { IsRequired = true };
var dependentPatternOption = new Option<string>(new[] { "--dependent-pattern", "-t" }) { IsRequired = false };
var dependencyPatternOption = new Option<string>(new[] { "--dependency-pattern", "-c" }) { IsRequired = false };


var analyzeAssembliesCommand = new Command("assemblies", "Analyze Assemblies");
analyzeAssembliesCommand.AddOption(assemblyPatternOption);
analyzeAssembliesCommand.AddOption(dependentPatternOption);
analyzeAssembliesCommand.AddOption(dependencyPatternOption);
analyzeAssembliesCommand.SetHandler
(
    (assemblyPattern, dependentPattern, dependencyPattern) =>
        new AnalyzeAssemblyCommand(Console.Out)
            .Execute(assemblyPattern, dependentPattern, dependencyPattern),
    assemblyPatternOption, dependentPatternOption, dependencyPatternOption
);
rootCommand.AddCommand(analyzeAssembliesCommand);


var analyzeTypesCommand = new Command("types", "Analyze Types");
analyzeTypesCommand.AddOption(assemblyPatternOption);
analyzeTypesCommand.AddOption(dependentPatternOption);
analyzeTypesCommand.AddOption(dependencyPatternOption);
analyzeTypesCommand.SetHandler
(
    (assemblyPattern, dependentPattern, dependencyPattern) =>
        new AnalyzeTypesCommand(Console.Out)
            .Execute(assemblyPattern, dependentPattern, dependencyPattern),
    assemblyPatternOption, dependentPatternOption, dependencyPatternOption
);
rootCommand.AddCommand(analyzeTypesCommand);

rootCommand.Invoke(Environment.GetCommandLineArgs().Skip(1).ToArray());