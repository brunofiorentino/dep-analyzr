using System.Text.RegularExpressions;
using ConsoleTables;

namespace DepAnalyzr.Core;

internal sealed class DependencyMatrix
{
    private DependencyMatrix(string[,] data) =>
        Data = data;


    public string[,] Data { get; }
    internal const string FirstTableCellLabel = "Dependent-Dependency";


    public void WriteTabularTo(TextWriter output)
    {
        var columnLabels = new List<string> { FirstTableCellLabel };

        for (var columnIndex = 1; columnIndex < Data.GetLength(1); columnIndex++)
            columnLabels.Add(Data[0, columnIndex]);

        var tabularWriterOpts = new ConsoleTableOptions { Columns = columnLabels.ToArray(), OutputTo = output };
        var tabularWriter = new ConsoleTable(tabularWriterOpts);

        for (var rowIndex = 1; rowIndex < Data.GetLength(0); rowIndex++)
        {
            var cells = new List<object>();

            for (var columnIndex = 0; columnIndex < Data.GetLength(1); columnIndex++)
                cells.Add(Data[rowIndex, columnIndex]);

            tabularWriter.AddRow(cells.ToArray());
        }

        tabularWriter.Write();
    }


    public static DependencyMatrix CreateForAssemblies(
        AnalysisResult analysisResult, string? dependentPattern, string? dependencyPattern) =>
        new(CreateDependencyMatrixData(
            analysisResult.AssemblyDefDependenciesByKey,
            dependentPattern, dependencyPattern,
            x => analysisResult.IndexedDefinitions.AssemblyDefsByKey[x].Name.Name));


    public static DependencyMatrix CreateForTypes(
        AnalysisResult analysisResult, string? dependentPattern, string? dependencyPattern) =>
        new(CreateDependencyMatrixData(
            analysisResult.TypeDefDependenciesByKey,
            dependentPattern, dependencyPattern,
            x => analysisResult.IndexedDefinitions.TypeDefsByKey[x].FullName));


    public static DependencyMatrix CreateForMethods(
        AnalysisResult analysisResult, string? dependentPattern, string? dependencyPattern) =>
        new(CreateDependencyMatrixData( // TODO: Figure out a better way to contextualize/display method names
            analysisResult.MethodDefDependenciesByKey,
            dependentPattern, dependencyPattern,
            x => analysisResult.IndexedDefinitions.MethodDefsByKey[x].FullName));


    private static string[,] CreateDependencyMatrixData
    (
        IReadOnlyDictionary<string, IReadOnlySet<string>> dependenciesByKey,
        string? dependentPattern, string? dependencyPattern,
        Func<string, string> labelByKey
    )
    {
        var sortedKeys = dependenciesByKey.Keys.OrderBy(x => x).ToArray();
        var rawDepMatrixData = CreateRawDependencyMatrixData(sortedKeys, dependenciesByKey);
        var friendlyDepMatrixData = CreateFriendlyDependencyMatrixData(sortedKeys, rawDepMatrixData, labelByKey);
        return  ApplyFiltersIfNeeded(friendlyDepMatrixData, dependentPattern, dependencyPattern);
    }


    private static bool[,] CreateRawDependencyMatrixData
    (
        string[] sortedKeys,
        IReadOnlyDictionary<string, IReadOnlySet<string>> dependenciesByKey
    )
    {
        var dependencyData = new bool[sortedKeys.Length, sortedKeys.Length];

        for (var dependentIndex = 0; dependentIndex < dependencyData.GetLength(0); dependentIndex++)
        {
            var dependentKey = sortedKeys[dependentIndex];
            var dependencies = dependenciesByKey[dependentKey];

            for (var dependencyIndex = 0; dependencyIndex < dependencyData.GetLength(1); dependencyIndex++)
            {
                var possibleDependencyKey = sortedKeys[dependencyIndex];
                var depends = dependencies.Contains(possibleDependencyKey);
                dependencyData[dependentIndex, dependencyIndex] = depends;
            }
        }

        return dependencyData;
    }


    // ReSharper disable once InconsistentNaming
    internal const string Yes = nameof(Yes);

    // ReSharper disable once InconsistentNaming
    internal const string No = nameof(No);


    private static string[,] CreateFriendlyDependencyMatrixData
    (
        string[] sortedKeys,
        bool[,] rawDepMatrixData,
        Func<string, string> labelByKey
    )
    {
        var lengthConsideringLabels = sortedKeys.Length + 1;
        var friendlyDepMatrixData = new string[lengthConsideringLabels, lengthConsideringLabels];

        for (var index = 0; index < sortedKeys.Length; index++)
        {
            var key = sortedKeys[index];
            var label = labelByKey(key);

            friendlyDepMatrixData[0, index + 1] = label;
            friendlyDepMatrixData[index + 1, 0] = label;
        }

        for (var dependentIndex = 0; dependentIndex < rawDepMatrixData.GetLength(0); dependentIndex++)
        for (var dependencyIndex = 0; dependencyIndex < rawDepMatrixData.GetLength(1); dependencyIndex++)
            friendlyDepMatrixData[dependentIndex + 1, dependencyIndex + 1] =
                rawDepMatrixData[dependentIndex, dependencyIndex] ? Yes : No;

        return friendlyDepMatrixData;
    }


    private static string[,] ApplyFiltersIfNeeded
    (
        string[,] friendlyDepMatrixData,
        string? dependentPattern,
        string? dependencyPattern
    )
    {
        // TODO: Extract methods

        if (string.IsNullOrEmpty(dependentPattern) && string.IsNullOrEmpty(dependencyPattern))
            return friendlyDepMatrixData;


        // Setup regexes
        const RegexOptions regexOpts = RegexOptions.Compiled | RegexOptions.Singleline;
        static Regex? CreateRegexIfSpecified(string? p) => string.IsNullOrEmpty(p) ? null : new(p, regexOpts);

        var dependentPatternRegex = CreateRegexIfSpecified(dependentPattern);
        var dependencyPatternRegex = CreateRegexIfSpecified(dependencyPattern);


        // Dependencies (columns) filtering (index map) 
        var oldDependencyIndexNewDependencyIndexPairs = new Dictionary<int, int> { { 0, 0 } };

        for (int fixedOldDependentIndex = 0, oldDependencyIndex = 1;
             oldDependencyIndex < friendlyDepMatrixData.GetLength(1);
             oldDependencyIndex++)
        {
            var dependencyLabel = friendlyDepMatrixData[fixedOldDependentIndex, oldDependencyIndex];
            var dependencyPatternMatches = dependencyPatternRegex?.IsMatch(dependencyLabel) ?? true;

            if (dependencyPatternMatches)
                oldDependencyIndexNewDependencyIndexPairs.Add(
                    oldDependencyIndex, oldDependencyIndexNewDependencyIndexPairs.Count);
        }


        // Filter dependents (rows) collecting cell values considering dependency (column) filter.
        var filteredDependents = new List<List<string>>();

        for (int oldDependentIndex = 1, fixedOldDependencyIndex = 0;
             oldDependentIndex < friendlyDepMatrixData.GetLength(0);
             oldDependentIndex++)
        {
            var dependentLabel = friendlyDepMatrixData[oldDependentIndex, fixedOldDependencyIndex];
            var dependentPatternMatches = dependentPatternRegex?.IsMatch(dependentLabel) ?? true;

            if (!dependentPatternMatches)
                continue;

            var cells = new List<string>();

            for (var oldDependencyIndex = 0;
                 oldDependencyIndex < friendlyDepMatrixData.GetLength(1);
                 oldDependencyIndex++)
            {
                if (oldDependencyIndexNewDependencyIndexPairs.ContainsKey(oldDependencyIndex))
                    cells.Add(friendlyDepMatrixData[oldDependentIndex, oldDependencyIndex]);
            }

            filteredDependents.Add(cells);
        }


        // Returns the original matrix if nothing was filtered
        var allDependentsMatched = filteredDependents.Count + 1 == friendlyDepMatrixData.GetLength(0);
        var allDependenciesMatched =
            oldDependencyIndexNewDependencyIndexPairs.Count == friendlyDepMatrixData.GetLength(1);

        if (allDependentsMatched && allDependenciesMatched)
            return friendlyDepMatrixData;


        // New filtered/reduced matrix - preparation
        var newDependentsLength = filteredDependents.Count + 1;
        var newDependenciesLength = oldDependencyIndexNewDependencyIndexPairs.Count;
        var filteredFriendlyDepMatrixData = new string[newDependentsLength, newDependenciesLength];


        // New filtered/reduced matrix -- build dependency labels considering filters
        var filteredNewDependencyIndexOldIndexPairs =
            oldDependencyIndexNewDependencyIndexPairs.ToDictionary(x => x.Value, x => x.Key);

        for (var newDependencyIndex = 0;
             newDependencyIndex < filteredFriendlyDepMatrixData.GetLength(1);
             newDependencyIndex++)
        {
            if (filteredNewDependencyIndexOldIndexPairs.TryGetValue(newDependencyIndex, out var oldDependencyIndex))
                filteredFriendlyDepMatrixData[0, newDependencyIndex] = friendlyDepMatrixData[0, oldDependencyIndex];
        }


        // New filtered/reduced matrix -- add filtered dependents, already matching dependency (column) filtering
        for (var newDependentIndex = 1;
             newDependentIndex < filteredFriendlyDepMatrixData.GetLength(0);
             newDependentIndex++)
        {
            var filteredDependent = filteredDependents[newDependentIndex - 1];

            for (var newDependencyIndex = 0;
                 newDependencyIndex < filteredFriendlyDepMatrixData.GetLength(1);
                 newDependencyIndex++)
            {
                filteredFriendlyDepMatrixData[newDependentIndex, newDependencyIndex] =
                    filteredDependent[newDependencyIndex];
            }
        }

        return filteredFriendlyDepMatrixData;
    }
}