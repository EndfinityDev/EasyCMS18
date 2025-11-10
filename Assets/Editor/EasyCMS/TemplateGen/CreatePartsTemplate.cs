#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreatePartsTemplate
{
    static readonly string[] s_AllowedParts = {
        "hood",
        "door_front_left",
        "door_front_right",
        "door_rear_left",
        "door_rear_right",
        "trunk",
        "fender_front_left",
        "fender_front_right",
        "bumper_front",
        "bumper_rear",
        "headlight_left",
        "headlight_right",
        "taillight_left",
        "taillight_right",
        "taillight_left_trunk",
        "taillight_right_trunk",
        "fender_rear_left",
        "fender_rear_right",
        "mirror_left",
        "mirror_right",
        "front_end",
        "window_front",
        "window_back",
        "window_door_front_left",
        "window_door_front_right",
        "window_door_rear_left",
        "window_door_rear_right",
        "window_body_left_1",
        "window_body_right_1",
        "window_trunk",
        "clamshell_front_openable",
        "clamshell_rear_openable",
        "clamshell_front",
        "clamshell_rear",
        "engine_cover",
        "engine_cover_openable",
    };

    static bool CheckPartAllowed(string partName)
    {
        bool result = false;
        // This should be at least 3 lifelong sentences in federal prison
        foreach(string part in s_AllowedParts)
        {
            if (partName.StartsWith(part))
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public static void ProcessCarFolder(string carFolderPath)
    {
        string[] splitPath = carFolderPath.Split('/');
        if (!(carFolderPath.StartsWith("Assets/Cars") && splitPath.Length == 3))
        {
            Debug.LogError("[CreatePartsTemplate.ProcessCarFolder] | Please select a folder inside the \"Assets/Cars\" directory: " + carFolderPath);
            return;
        }

        splitPath[splitPath.Length - 1] = splitPath[splitPath.Length - 1].Replace(" ", "_");
        string carName = splitPath[splitPath.Length - 1];

        HashSet<string> pricedPartNames = new HashSet<string>();

        string[] assetFiles = Directory.GetFiles(carFolderPath);
        foreach(string assetFile in assetFiles)
        {
            if(!assetFile.EndsWith(".fbx")) { continue; }
            GameObject fileGO = AssetDatabase.LoadAssetAtPath<GameObject>(assetFile);
            MeshFilter[] meshFilters = fileGO.GetComponentsInChildren<MeshFilter>();
            
            //List<string> meshNames = new List<string>();
            foreach (MeshFilter meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.sharedMesh;
                string meshName = mesh.name;
                if(CheckPartAllowed(meshName))
                {
                    pricedPartNames.Add(meshName);
                }
            }
        }

        string outPartsString = "";
        foreach(string part in pricedPartNames)
        {
            outPartsString += "car_" + carName + "-" + part + "=300\r\n";
        }

        string outputPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + carName);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        string partsFilePath = outputPath + "/parts.txt";
        if (File.Exists(partsFilePath))
        {
            //Debug.LogError($"[CreatePartsTemplate.ProcessCarFolder] | Please remove the existing config file before generating a new one ({partsFilePath})");
            if (!EditorUtility.DisplayDialog("File conflict", "The parts config already exists for this car. Replace it?", "Yes", "No"))
            {
                return;
            }
        }
        //if (File.Exists(configFilePath))
        //{
        //    File.Delete(configFilePath);
        //}
        File.WriteAllText(partsFilePath, outPartsString);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/EasyCMS/Templates/Create Parts Template", priority = 8)]
    static void CreateBodyConfigFile()
    {
        Object[] selectionObjects = Selection.objects;
        foreach (Object selectedObject in selectionObjects)
        {
            ProcessCarFolder(AssetDatabase.GetAssetPath(selectedObject));
        }
    }
}
#endif