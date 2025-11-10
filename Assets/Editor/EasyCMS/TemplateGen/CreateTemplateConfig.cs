#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateTemplateConfig
{
    const string TEMPLATE_CONFIG_PATH = "Assets/Editor/EasyCMS/Templates/config.txt";

    [MenuItem("Assets/EasyCMS/Templates/Create Config Template (CMS21)", priority = 5)]
    static void CreateConfigsFromSelection()
    {
        Object[] selectionObjects = Selection.objects;
        foreach (Object selectedObject in selectionObjects) 
        {
            CreateConfig(selectedObject);
        }
    }

    public static void CreateConfig(Object selectedObject)
    {
        TextAsset templateConfigFile = AssetDatabase.LoadAssetAtPath<TextAsset>(TEMPLATE_CONFIG_PATH);
        string templateString = templateConfigFile.text;

        string filePath = AssetDatabase.GetAssetPath(selectedObject);
        string[] splitPath = filePath.Split('/');
        if (!(filePath.StartsWith("Assets/Cars") && splitPath.Length == 3))
        {
            Debug.LogError("[CreateTemplateConfig.CreateConfig] | Please select a folder inside the \"Assets/Cars\" directory: " + filePath);
            return;
        }
        splitPath[splitPath.Length - 1] = splitPath[splitPath.Length - 1].Replace(" ", "_");
        string carName = splitPath[splitPath.Length - 1];

        string outputPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + carName);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        string configFilePath = outputPath + "/config.txt";
        if (File.Exists(configFilePath))
        {
            //Debug.LogError($"[CreateTemplateConfig.CreateConfig] | Please remove the existing config file before generating a new one ({configFilePath})");
            if (!EditorUtility.DisplayDialog("File conflict", "The car config already exists for this car. Replace it?", "Yes", "No"))
            {
                return;
            }
        }
        string outConfigString = string.Format(templateString, carName);
        File.WriteAllText(configFilePath, outConfigString);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif