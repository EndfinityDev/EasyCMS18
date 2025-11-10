#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CopyMaterialsFile
{
    public static Object Copy(string absFilePath, string absDestinationFolder)
    {
        string absoluteDestinationPath = absDestinationFolder;
        absoluteDestinationPath = absoluteDestinationPath.Replace('\\', '/');
        string projectRootPath = Path.GetDirectoryName(Application.dataPath);
        projectRootPath = projectRootPath.Replace('\\', '/');
        string destinationPath = absoluteDestinationPath.Replace(projectRootPath + "/", "");
        string fileName = Path.GetFileName(absFilePath);
        if (!File.Exists(absFilePath))
        {
            Debug.LogError("[CopyMaterialsFile.Copy] Could not find file " + absFilePath);
            return null;
        }
        File.Copy(absFilePath, Path.Combine(absDestinationFolder, fileName), true);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Object matFileObject = AssetDatabase.LoadAssetAtPath<Object>(Path.Combine(destinationPath, fileName));
        return matFileObject;
    }
}
#endif