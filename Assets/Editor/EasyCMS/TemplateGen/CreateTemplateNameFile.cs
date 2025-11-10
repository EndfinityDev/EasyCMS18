#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateTemplateNameFile
{
    [MenuItem("Assets/EasyCMS/Templates/Create Name File Template", priority = 7)]
    static void CreateNameFilesFromSelection()
    {
        Object[] selectionObjects = Selection.objects;
        foreach (Object selectedObject in selectionObjects)
        {
            CreateNameFile(selectedObject);
        }

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public static void CreateNameFile(Object carFolder)
    {
        string filePath = AssetDatabase.GetAssetPath(carFolder);
        string[] splitPath = filePath.Split('/');
        if (!(filePath.StartsWith("Assets/Cars") && splitPath.Length == 3))
        {
            Debug.LogError("[CreateTemplateNameFile.CreateNameFile] | Please select a folder inside the \"Assets/Cars\" directory");
            return;
        }
        splitPath[splitPath.Length - 1] = splitPath[splitPath.Length - 1].Replace(" ", "_");
        string carName = splitPath[splitPath.Length - 1];

        string outputPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + carName);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        string nameFilePath = outputPath + "/name.txt";
        if (File.Exists(nameFilePath))
        {
            //File.Delete(nameFilePath);
            if (!EditorUtility.DisplayDialog("File conflict", "The name file already exists for this car. Replace it?", "Yes", "No"))
            {
                return;
            }
        }
        File.WriteAllText(nameFilePath, carName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif