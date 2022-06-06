using System.CommandLine;
using DepAnalyzr.Application;


// Shared options
var assemblyPatternOption = new Option<string>(new[] { "--assembly-pattern", "-a" }) { IsRequired = true };
var dependentPatternOption = new Option<string>(new[] { "--dependent-pattern", "-t" }) { IsRequired = false };
var dependencyPatternOption = new Option<string>(new[] { "--dependency-pattern", "-c" }) { IsRequired = false };
var patternOption = new Option<string>(new[] { "--pattern", "-p" }) { IsRequired = false };
var graphFormatOption = new Option<GraphFormat>(new[] { "--format", "-f" }) { IsRequired = false };


// "depanalyzr" root command
var rootCommand = new RootCommand();


// "depanalyzr assemblies" command
var analyzeAssembliesCommand = new Command("assemblies", "Analyze assemblies");
rootCommand.AddCommand(analyzeAssembliesCommand);


// "depanalyzr assemblies matrix" command
var generateAssemblyDepMatrixCommand = new Command("matrix", "Generate assemblies dependency matrix");
generateAssemblyDepMatrixCommand.AddOption(assemblyPatternOption);
generateAssemblyDepMatrixCommand.AddOption(dependentPatternOption);
generateAssemblyDepMatrixCommand.AddOption(dependencyPatternOption);
generateAssemblyDepMatrixCommand.SetHandler
(
    (assemblyPattern, dependentPattern, dependencyPattern) =>
        new GenerateAssembliesDepMatrixCommand(Console.Out)
            .Execute(assemblyPattern, dependentPattern, dependencyPattern),
    assemblyPatternOption, dependentPatternOption, dependencyPatternOption
);
analyzeAssembliesCommand.AddCommand(generateAssemblyDepMatrixCommand);


// "depanalyzr assemblies graph" command
var generateAssemblyDepGraphCommand = new Command("graph", "Generate assemblies dependency graph");
generateAssemblyDepGraphCommand.AddOption(assemblyPatternOption);
generateAssemblyDepGraphCommand.AddOption(patternOption);
generateAssemblyDepGraphCommand.AddOption(graphFormatOption);
generateAssemblyDepGraphCommand.SetHandler
(
    (assemblyPattern, pattern, format) =>
        new GenerateAssembliesDepGraphCommand(Console.Out)
            .Execute(assemblyPattern, pattern, format),
    assemblyPatternOption, patternOption, graphFormatOption
);
analyzeAssembliesCommand.AddCommand(generateAssemblyDepGraphCommand);


// "depanalyzr types" command
var analyzeTypesCommand = new Command("types", "Analyze Types");
rootCommand.AddCommand(analyzeTypesCommand);


// "depanalyzr types matrix" command
var generateTypeDepMatrixCommand = new Command("matrix", "Generate types dependency matrix");
generateTypeDepMatrixCommand.AddOption(assemblyPatternOption);
generateTypeDepMatrixCommand.AddOption(dependentPatternOption);
generateTypeDepMatrixCommand.AddOption(dependencyPatternOption);
generateTypeDepMatrixCommand.SetHandler
(
    (assemblyPattern, dependentPattern, dependencyPattern) =>
        new GenerateTypesDepMatrixCommand(Console.Out)
            .Execute(assemblyPattern, dependentPattern, dependencyPattern),
    assemblyPatternOption, dependentPatternOption, dependencyPatternOption
);
analyzeTypesCommand.AddCommand(generateTypeDepMatrixCommand);


// "depanalyzr types graph" command
var generateTypeDepGraphCommand = new Command("graph", "Generate types dependency graph");
generateTypeDepGraphCommand.AddOption(assemblyPatternOption);
generateTypeDepGraphCommand.AddOption(patternOption);
generateTypeDepGraphCommand.AddOption(graphFormatOption);
generateTypeDepGraphCommand.SetHandler
(
    (assemblyPattern, pattern, format) =>
        new GenerateTypesDepGraphCommand(Console.Out)
            .Execute(assemblyPattern, pattern, format),
    assemblyPatternOption, patternOption, graphFormatOption
);
analyzeTypesCommand.AddCommand(generateTypeDepGraphCommand);


rootCommand.Invoke(Environment.GetCommandLineArgs().Skip(1).ToArray());