using System;
using System.IO;
using UnityEngine;

public static class ExperimentResultsFile
{
    private const string FolderName = "ExperimentResults";

    public static string Save(string experimentName, int intervalMilliseconds, string content)
    {
        string fileName = $"{experimentName}_{intervalMilliseconds}ms_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        try
        {
            string folderPath = Path.Combine(Application.persistentDataPath, FolderName);
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }
        catch (Exception exception)
        {
            Debug.LogError($"No se pudo guardar el archivo de resultados: {exception.Message}");
            return string.Empty;
        }
    }
}
