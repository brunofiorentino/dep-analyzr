namespace DepAnalyzr;

public static class DependencyMatrixView
{
    public static string[,] CreateForAssemblies(DependencyAnalysisResult analysisResult)
    {
        var dependenciesByKey = analysisResult.AssemblyDefDependenciesByKey;
        var defsByKey = analysisResult.IndexedDefinitions.AssemblyDefsByKey;
        var depMatrix = Create(dependenciesByKey, x => defsByKey[x].Name.Name);

        return depMatrix;
    }

    public static string[,] CreateForTypes(DependencyAnalysisResult analysisResult)
    {
        var dependenciesByKey = analysisResult.TypeDefDependenciesByKey;
        var defsByKey = analysisResult.IndexedDefinitions.TypeDefsByKey;
        var depMatrix = Create(dependenciesByKey, x => defsByKey[x].FullName);

        return depMatrix;
    }

    private static string[,] Create
    (
        IReadOnlyDictionary<string, IReadOnlySet<string>> dependenciesByKey,
        Func<string, string> labelByKey
    )
    {
        var sortedKeys = dependenciesByKey.Keys.OrderBy(x => x).ToArray();
        var dependencyData = CreateDependencyData(sortedKeys, dependenciesByKey);
        var dependencyDataView = CreateDependencyDataView(sortedKeys, dependencyData, labelByKey);

        return dependencyDataView;
    }

    private static bool[,] CreateDependencyData
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

    private static string[,] CreateDependencyDataView
    (
        string[] sortedKeys,
        bool[,] dependencyData,
        Func<string, string> labelByKey
    )
    {
        var lengthConsideringLabels = sortedKeys.Length + 1;
        var dependencyDataView = new string[lengthConsideringLabels, lengthConsideringLabels];

        for (var keyIndex = 0; keyIndex < sortedKeys.Length; keyIndex++)
        {
            var key = sortedKeys[keyIndex];
            var label = labelByKey(key);

            dependencyDataView[0, keyIndex + 1] = label;
            dependencyDataView[keyIndex + 1, 0] = label;
        }

        for (var keyIndex = 0; keyIndex < dependencyData.GetLength(0); keyIndex++)
        for (var dependencyIndex = 0; dependencyIndex < dependencyData.GetLength(1); dependencyIndex++)
            dependencyDataView[keyIndex + 1, dependencyIndex + 1] =
                dependencyData[keyIndex, dependencyIndex] ? "y" : "n";

        return dependencyDataView;
    }
}