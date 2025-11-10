using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Car Manager", menuName = "EasyCMS/Car Manager", order = 1)]
public class CarImporterSO : ScriptableObject
{
#if UNITY_EDITOR
    [HideInInspector]
    public string BeamNGMaterialsPath = "";
    [HideInInspector]
    public string CMSExePath = "";

    public TextAsset MaterialsFile;
    // Would like this to be a HashSet but the base inspector doesn't support that
    public List<Object> FilesToIgnoreOnExport = new List<Object>();

    int m_LastFileCount = 0;

    private void Awake()
    {
        if(CMSExePath != null && CMSExePath != string.Empty) { return; }

        string[] carImporterAssets = AssetDatabase.FindAssets("t:CarImporterSO", null);
        foreach( string carImporterGUID in carImporterAssets)
        {
            string carImporterPath = AssetDatabase.GUIDToAssetPath(carImporterGUID);
            CarImporterSO carImporter = AssetDatabase.LoadAssetAtPath<CarImporterSO>(carImporterPath);
            if(carImporter == null || carImporter.CMSExePath == null || carImporter.CMSExePath == string.Empty) { continue; }
            CMSExePath = carImporter.CMSExePath;
            return;
        }
    }

    public void Validate()
    {
        List<int> idsToRemove = new List<int>();
        HashSet<Object> existingObjects = new HashSet<Object>();
        for (int i = 0; i < FilesToIgnoreOnExport.Count; i++)
        {
            Object currentObject = FilesToIgnoreOnExport[i];
            if (currentObject == null) continue;
            if (existingObjects.Contains(currentObject))
            {
                idsToRemove.Add(i);
                continue;
            }
            existingObjects.Add(currentObject);
        }

        idsToRemove.Reverse();
        foreach (int id in idsToRemove)
        {
            FilesToIgnoreOnExport[id] = null;
            if (id <= m_LastFileCount - 1)
            {
                FilesToIgnoreOnExport.RemoveAt(id);
            }
        }

        m_LastFileCount = FilesToIgnoreOnExport.Count;
    }

    void OnValidate()
    {
        Validate();
    }
#endif
}
