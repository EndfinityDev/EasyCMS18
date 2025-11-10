#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarImporterSO))]
public class CarImporterEditor : Editor
{
    CarImporterSO m_CarImporter;

    enum PressedButton
    {
        None,
        CMSExecutable,
        BeamNGMaterials,

        ImportMaterialsFile,
        ImportTextures,
        BuildMaterials,

        MakeName,
        MakeConfig,
        MakeBodyConfig,
        MakePartsConfig,

        BuildAssetBundle,
        CopyAssetBundleToGame,

        ImportNameFromGame,
        ImportConfigFromGame,
        ImportBodyConfigFromGame,
        ImportPartsConfigFromGame
    }

    private void OnEnable()
    {
        m_CarImporter = (CarImporterSO)target;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        PressedButton pressedButton = PressedButton.None;

        EditorGUILayout.LabelField("Paths");

        EditorGUILayout.BeginHorizontal();
        m_CarImporter.CMSExePath = EditorGUILayout.TextField("CMS Executable", m_CarImporter.CMSExePath);
        if(GUILayout.Button("...", GUILayout.MaxWidth(20.0f)))
        {
            pressedButton = PressedButton.CMSExecutable;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        m_CarImporter.BeamNGMaterialsPath = EditorGUILayout.TextField("BeamNG Materials", m_CarImporter.BeamNGMaterialsPath);
        if (GUILayout.Button("...", GUILayout.MaxWidth(20.0f)))
        {
            pressedButton = PressedButton.BeamNGMaterials;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Import");
        if(GUILayout.Button("1. Import Materials File"))
        {
            pressedButton = PressedButton.ImportMaterialsFile;
        }
        if (GUILayout.Button("2. Import Textures"))
        {
            pressedButton = PressedButton.ImportTextures;
        }
        if (GUILayout.Button("3. Build Materials"))
        {
            pressedButton = PressedButton.BuildMaterials;
        }
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Templates");
        if (GUILayout.Button("Make Name File"))
        {
            pressedButton = PressedButton.MakeName;
        }
        if (GUILayout.Button("Make Car Config"))
        {
            pressedButton = PressedButton.MakeConfig;
        }
        if (GUILayout.Button("Make Body Config"))
        {
            pressedButton = PressedButton.MakeBodyConfig;
        }
        if (GUILayout.Button("Make Parts Config"))
        {
            pressedButton = PressedButton.MakePartsConfig;
        }
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Export");
        if (GUILayout.Button("Build Asset Bundle"))
        {
            pressedButton = PressedButton.BuildAssetBundle;
        }
        if (GUILayout.Button("Copy Car Assets To Game"))
        {
            pressedButton = PressedButton.CopyAssetBundleToGame;
        }
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Updating Configs");
        if (GUILayout.Button("Import Name File From Game"))
        {
            pressedButton = PressedButton.ImportNameFromGame;
        }
        if (GUILayout.Button("Import Car Config From Game"))
        {
            pressedButton = PressedButton.ImportConfigFromGame;
        }
        if (GUILayout.Button("Import Body Config From Game"))
        {
            pressedButton = PressedButton.ImportBodyConfigFromGame;
        }
        if (GUILayout.Button("Import Parts Config From Game"))
        {
            pressedButton = PressedButton.ImportPartsConfigFromGame;
        }
        EditorGUILayout.Separator();

        // Doing this here so it doesn't throw stupid GUI errors
        switch (pressedButton)
        {
            case PressedButton.CMSExecutable:
                string cmsExePath = EditorUtility.OpenFilePanel("Car Mechanic Simulator Executable", m_CarImporter.CMSExePath, "exe");
                if (cmsExePath != string.Empty)
                {
                    m_CarImporter.CMSExePath = cmsExePath;
                }
                break;
            case PressedButton.BeamNGMaterials:
                string bngMatPath = EditorUtility.OpenFilePanel("BeamNG Car Materials File", m_CarImporter.BeamNGMaterialsPath, "materials.json");
                if (bngMatPath != string.Empty)
                {
                    m_CarImporter.BeamNGMaterialsPath = bngMatPath;
                }
                break;

            case PressedButton.ImportMaterialsFile:
                ImportMaterialsFile();
                break;
            case PressedButton.ImportTextures:
                ImportTextureFiles();
                break;
            case PressedButton.BuildMaterials:
                BuildMaterials();
                break;

            case PressedButton.MakeName:
                CreateTemplateNameFile.CreateNameFile(FindCarFolder());
                break;
            case PressedButton.MakeConfig:
                CreateTemplateConfig.CreateConfig(FindCarFolder());
                break;
            case PressedButton.MakeBodyConfig:
                CreateTemplateBodyconfig.ProcessCarFolder(AssetDatabase.GetAssetPath(FindCarFolder()));
                break;
            case PressedButton.MakePartsConfig:
                CreatePartsTemplate.ProcessCarFolder(AssetDatabase.GetAssetPath(FindCarFolder()));
                break;

            case PressedButton.BuildAssetBundle:
                BuildAssetBundle();
                break;
            case PressedButton.CopyAssetBundleToGame:
                CopyAssetsToGame();
                break;

            case PressedButton.ImportNameFromGame:
            case PressedButton.ImportConfigFromGame:
            case PressedButton.ImportBodyConfigFromGame:
            case PressedButton.ImportPartsConfigFromGame:
                CopyFileFromGame(pressedButton);
                break;
        }

        // Display the asset list (workaround)
        base.OnInspectorGUI();
    }

    Object FindCarFolder()
    {
        string thisPath = AssetDatabase.GetAssetPath(m_CarImporter);
        string[] splitPath = thisPath.Split('/');
        if(!thisPath.StartsWith("Assets/Cars") || splitPath.Length < 3)
        {
            EditorUtility.DisplayDialog("File path error",
                "Failed to validate the car's path. Make sure this file is located in \"Assets/Cars/[car_name]\"", "OK");
            return null;
        }
        string directory = string.Join("/", splitPath.Take(splitPath.Length - 1).ToArray());
        return AssetDatabase.LoadAssetAtPath<Object>(directory);
    }

    void ImportMaterialsFile()
    {
        string carImporterPath = AssetDatabase.GetAssetPath(m_CarImporter);
        string[] splitPath = carImporterPath.Split('/');
        string carImporterFolder = "";
        for (int i = 0; i < splitPath.Length - 1; i++) { carImporterFolder = Path.Combine(carImporterFolder, splitPath[i]); }
        string absoluteMatFolderPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), carImporterFolder);

        string bngMatFileName = Path.GetFileName(m_CarImporter.BeamNGMaterialsPath);
        if (File.Exists(Path.Combine(absoluteMatFolderPath, bngMatFileName)))
        {
            if(!EditorUtility.DisplayDialog("Materials file conflict", "\"" + bngMatFileName + "\" already exists. Replace it?", "Yes", "No"))
            {
                return;
            }
        }

        Object materialsFileObject = CopyMaterialsFile.Copy(m_CarImporter.BeamNGMaterialsPath, absoluteMatFolderPath);
        if(materialsFileObject == null) { return; }
        m_CarImporter.MaterialsFile = materialsFileObject as TextAsset;
        m_CarImporter.FilesToIgnoreOnExport.Add(materialsFileObject);
        m_CarImporter.Validate();
        m_CarImporter.FilesToIgnoreOnExport.RemoveAll(item => item == null);
    }

    void ImportTextureFiles()
    {
        if (m_CarImporter.MaterialsFile == null)
        {
            EditorUtility.DisplayDialog("Missing materials file",
                "No linked materials.json file found. Press \"Import Materials File\" or attach it manually in the inspector", "OK");
            return;
        }
        ImportMaterialJSon.CopyTextures(Path.GetDirectoryName(m_CarImporter.BeamNGMaterialsPath), AssetDatabase.GetAssetPath(m_CarImporter.MaterialsFile));
    }

    void BuildMaterials()
    {
        if (m_CarImporter.MaterialsFile == null)
        {
            EditorUtility.DisplayDialog("Missing materials file",
                "No linked materials.json file found. Press \"Import Materials File\" or attach it manually in the inspector", "OK");
            return;
        }

        HashSet<Object> ignoreFiles = ImportMaterialJSon.ParseMaterialsFile(AssetDatabase.GetAssetPath(m_CarImporter.MaterialsFile));
        foreach (Object obj in ignoreFiles)
        {
            if (!m_CarImporter.FilesToIgnoreOnExport.Contains(obj))
            {
                m_CarImporter.FilesToIgnoreOnExport.Add(obj);
            }
        }

        int fileCount = m_CarImporter.FilesToIgnoreOnExport.Count;
        for(int i = fileCount - 1; i >= 0; i--)
        {
            Object obj = m_CarImporter.FilesToIgnoreOnExport[i];
            if(obj == null)
            {
                m_CarImporter.FilesToIgnoreOnExport.RemoveAt(i);
            }
        }
    }

    void BuildAssetBundle()
    {
        List<string> ignorePaths = new List<string>();
        foreach(Object file in m_CarImporter.FilesToIgnoreOnExport)
        {
            ignorePaths.Add(AssetDatabase.GetAssetPath(file));
        }
        ignorePaths.Add(AssetDatabase.GetAssetPath(m_CarImporter));
        BuildAssetPackage.CreateCarBundle(FindCarFolder(), ignorePaths);
    }

    void CopyAssetsToGame()
    {
        string importerPath = AssetDatabase.GetAssetPath(m_CarImporter);
        string[] splitPath = importerPath.Split('/');
        splitPath[splitPath.Length - 2] = splitPath[splitPath.Length - 2].Replace(" ", "_");
        string carName = splitPath[splitPath.Length - 2];

        string assetsPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + carName);

        AssetCopying.CopyCarAssetsToGame(assetsPath, m_CarImporter.CMSExePath);
    }

    void CopyFileFromGame(PressedButton pressedButton)
    {
        string fileName = "";
        switch (pressedButton)
        {
            case PressedButton.ImportNameFromGame:
                fileName = "name.txt";
                break;
            case PressedButton.ImportConfigFromGame:
                fileName = "config.txt";
                break;
            case PressedButton.ImportBodyConfigFromGame:
                fileName = "bodyconfig.txt";
                break;
            case PressedButton.ImportPartsConfigFromGame:
                fileName = "parts.txt";
                break;
        }

        if(fileName == string.Empty)
        {
            Debug.LogError("[CarImporterEditor.CopyFileFromGame] | An unknown enum value was passed: " + pressedButton.ToString());
            return;
        }

        string importerPath = AssetDatabase.GetAssetPath(m_CarImporter);
        string[] splitPath = importerPath.Split('/');
        splitPath[splitPath.Length - 2] = splitPath[splitPath.Length - 2].Replace(" ", "_");
        string carName = splitPath[splitPath.Length - 2];

        string assetsPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + carName);

        if(File.Exists(Path.Combine(assetsPath, fileName)))
        {
            if(!EditorUtility.DisplayDialog("File Conflict", "File \"" + fileName + "\" already exists in the project files. Overwrite?", "Yes", "No"))
            {
                return;
            }
        }

        AssetCopying.CopyFileFromGame(fileName, assetsPath, m_CarImporter.CMSExePath);
        EditorUtility.DisplayDialog("Import Successful", "File \"" + fileName + "\" has been copied to the project files", "Ok");
    }
}
#endif