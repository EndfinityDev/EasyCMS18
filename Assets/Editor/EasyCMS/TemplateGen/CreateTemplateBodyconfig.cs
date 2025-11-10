#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateTemplateBodyconfig
{
    // part to be mounted : part to mount to
    static readonly Dictionary<string, string> s_UnmountTable = new Dictionary<string, string>
    {
        {"window_door_front_left", "door_front_left"},
        {"mirror_left", "door_front_left"},

        {"window_door_front_right", "door_front_right"},
        {"mirror_right", "door_front_right"},

        {"window_door_rear_right", "door_rear_right"},
        {"window_door_rear_left", "door_rear_left"},

        {"license_plate_front", "bumper_front"},
        {"license_plate_rear", "trunk"},

        {"bonusPart1", "trunk"},
        {"window_trunk", "trunk" }
    };

    const string MOD_LICENSEPLATE_POS = "\r\n\r\n[bumper_front2-lpf]\r\npos=0,0.2195,2.374\r\nrot=359.4388,0,0\r\nscale=1.0\r\n\r\n[bumper_front3-lpf]\r\npos=0,0.1741,2.3673\r\nrot=359.4388,0,0\r\nscale=1.0";

    public static void ProcessCarFolder(string carFolderPath)
    {
        string[] splitPath = carFolderPath.Split('/');
        if (!(carFolderPath.StartsWith("Assets/Cars") && splitPath.Length == 3))
        {
            Debug.LogError("[CreateTemplateBodyconfig.ProcessCarFolder] | Please select a folder inside the \"Assets/Cars\" directory: " + carFolderPath);
            return;
        }
        string modelFilePath = carFolderPath + "/model.fbx";
        if (!File.Exists(modelFilePath))
        {
            Debug.LogError("[CreateTemplateBodyconfig.ProcessCarFolder] | No model.fbx file found: " + modelFilePath);
            return;
        }

        splitPath[splitPath.Length - 1] = splitPath[splitPath.Length - 1].Replace(" ", "_");
        string carName = splitPath[splitPath.Length - 1];

        Dictionary<string, List<string>> unmountMap = new Dictionary<string, List<string>>();

        GameObject modelFile = AssetDatabase.LoadAssetAtPath<GameObject>(modelFilePath);
        MeshFilter[] meshFilters = modelFile.GetComponentsInChildren<MeshFilter>();
        List<string> meshNames = new List<string>();
        foreach(MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;
            meshNames.Add(mesh.name);
        }
        meshNames.Add("license_plate_front");
        meshNames.Add("license_plate_rear");

        foreach (string meshName in meshNames)
        {
            string mountToMesh;
            if (s_UnmountTable.TryGetValue(meshName, out mountToMesh))
            {
                if (!unmountMap.ContainsKey(mountToMesh))
                {
                    unmountMap[mountToMesh] = new List<string>();
                }
                unmountMap[mountToMesh].Add(meshName);
            }
        }

        string outConfigString = "[unmount_with]\r\n";
        foreach(KeyValuePair<string,  List<string>> pair in unmountMap)
        {
            outConfigString += pair.Key + "=" + string.Join(",", pair.Value.ToArray()) + "\r\n";
        }

        outConfigString += MOD_LICENSEPLATE_POS;

        string outputPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + carName);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        string configFilePath = outputPath + "/bodyconfig.txt";
        if (File.Exists(configFilePath))
        {
            //Debug.LogError($"[CreateTemplateBodyconfig.ProcessCarFolder] | Please remove the existing config file before generating a new one ({configFilePath})");
            if (!EditorUtility.DisplayDialog("File conflict", "The body config already exists for this car. Replace it?", "Yes", "No"))
            {
                return;
            }
        }
        //if (File.Exists(configFilePath))
        //{
        //    File.Delete(configFilePath);
        //}
        File.WriteAllText(configFilePath, outConfigString);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/EasyCMS/Templates/Create Body Config Template (CMS21)", priority = 8)]
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