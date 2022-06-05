using ConsoleTables;

namespace DepAnalyzr.Core;

public class DependencyMatrix
{
    private DependencyMatrix(string[,] data) =>
        Data = data;

    public string[,] Data { get; }

    public void WriteTabularTo(TextWriter output)
    {
        var columnLabels = new List<string> { "Dependent / Dependency" };

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

    public static DependencyMatrix CreateForAssemblies(DependencyAnalysisResult analysisResult)
    {
        var dependenciesByKey = analysisResult.AssemblyDefDependenciesByKey;
        var defsByKey = analysisResult.IndexedDefinitions.AssemblyDefsByKey;
        var depMatrix = CreateFor(dependenciesByKey, x => defsByKey[x].Name.Name);

        return new DependencyMatrix(depMatrix);
    }

    public static DependencyMatrix CreateForTypes(DependencyAnalysisResult analysisResult)
    {
        var dependenciesByKey = analysisResult.TypeDefDependenciesByKey;
        var defsByKey = analysisResult.IndexedDefinitions.TypeDefsByKey;
        var depMatrix = CreateFor(dependenciesByKey, x => defsByKey[x].FullName);

        return new DependencyMatrix(depMatrix);
    }

    private static string[,] CreateFor
    (
        IReadOnlyDictionary<string, IReadOnlySet<string>> dependenciesByKey,
        Func<string, string> labelByKey
    )
    {
        var sortedKeys = dependenciesByKey.Keys.OrderBy(x => x).ToArray();
        var rawDepMatrixData = CreateRawDependencyMatrixData(sortedKeys, dependenciesByKey);
        var friendlyDepMatrixData = CreateFriendlyDependencyMatrixData(sortedKeys, rawDepMatrixData, labelByKey);

        return friendlyDepMatrixData;
    }

    private static bool[,] CreateRawDependencyMatrixData
    (
        string[] sortedKeys,
        IReadOnlyDictionary<string, IReadOnlySet<string>> dependenciesByKey
    )
    {
        var dependencyData = new bool[sortedKeys.Length, sortedKeys.Length];

        for (var keyIndex = 0; keyIndex < dependencyData.GetLength(0); keyIndex++)
        {
            var key = sortedKeys[keyIndex];
            var dependencies = dependenciesByKey[key];

            for (var dependencyIndex = 0; dependencyIndex < dependencyData.GetLength(1); dependencyIndex++)
            {
                var possibleDependencyKey = sortedKeys[dependencyIndex];
                var depends = dependencies.Contains(possibleDependencyKey);
                dependencyData[keyIndex, dependencyIndex] = depends;
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

        for (var keyIndex = 0; keyIndex < sortedKeys.Length; keyIndex++)
        {
            var key = sortedKeys[keyIndex];
            var label = labelByKey(key);

            friendlyDepMatrixData[0, keyIndex + 1] = label;
            friendlyDepMatrixData[keyIndex + 1, 0] = label;
        }

        for (var keyIndex = 0; keyIndex < rawDepMatrixData.GetLength(0); keyIndex++)
        for (var dependencyIndex = 0; dependencyIndex < rawDepMatrixData.GetLength(1); dependencyIndex++)
            friendlyDepMatrixData[keyIndex + 1, dependencyIndex + 1] =
                rawDepMatrixData[keyIndex, dependencyIndex] ? Yes : No;

        return friendlyDepMatrixData;
    }
}