using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetPackage
{
    [MenuItem("Assets/EasyCMS/Export/Create Car Bundle")]
    static void CreateCarBundleTool()
    {
        var selectionObjects = Selection.objects;
        foreach (UnityEngine.Object selectionObject in selectionObjects)
        {
            CreateCarBundle(selectionObject);
        }
    }
    public static void CreateCarBundle(UnityEngine.Object selectionObject, List<string> ignorePaths = null)
    {
        try
        {
            List<string> pathList = new List<string>();

            string file = AssetDatabase.GetAssetPath(selectionObject);
            string bundleName = file;
            string[] b = bundleName.Split('/');
            b[b.Length - 1] = b[b.Length - 1].Replace(" ", "_");
            bundleName = b[b.Length - 1];
            
            // This path is a directory
            pathList.AddRange(ProcessDirectory(file, ignorePaths));

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = "car_" + bundleName + ".cms";
            buildMap[0].assetNames = pathList.ToArray();

            string outputPath = Path.Combine(Application.streamingAssetsPath, "Cars/" + bundleName);
            
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            if (!Directory.Exists(outputPath + "/PartThumb"))
                Directory.CreateDirectory(outputPath + "/PartThumb");
            if (!Directory.Exists(outputPath + "/Brand"))
                Directory.CreateDirectory(outputPath + "/Brand");

            try
            {
                BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            
                RemoveOldAssetBundle(outputPath + "/car_" + bundleName + ".cms");
                BuildPipeline.BuildAssetBundles(outputPath, buildMap, bundleOptions,
                    EditorUserBuildSettings.activeBuildTarget);
            
                RemoveOldAssetBundle(outputPath + "/" + bundleName);
                RemoveOldAssetBundle(outputPath + "/" + bundleName + ".manifest");
                RemoveOldAssetBundle(
                    outputPath + "/car_" + bundleName + ".cms.manifest");

                byte[] assetData = File.ReadAllBytes(outputPath + "/car_" + bundleName + ".cms");
                assetData[0] -= 2;
                using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath + "/car_" + bundleName + ".cms", FileMode.Create)))
                {
                    writer.Write(assetData);
                }

                Debug.Log("AssetBundle successful created in " + outputPath + "/car_" + bundleName.ToLower() + ".cms");

            }
            catch (Exception ex)
            {
                Debug.LogError("[BuildAssetPackage.CreateCarBundle] | Failed to create asset bundle. Error: " + ex.Message);
            }
            
            AssetDatabase.Refresh();
        }
        catch (Exception ex)
        {
            Debug.LogError("[BuildAssetPackage.CreateCarBundle] | " + ex.Message);
        }
    }

    static void RemoveOldAssetBundle(string assetFilePath)
    {
        if (File.Exists(assetFilePath))
            File.Delete(assetFilePath);
    }

    static List<string> ProcessDirectory(string targetDirectory, List<string> ignorePaths)
    {
        try
        {
            List<string> pathList = new List<string>();

            string[] files = Directory.GetFiles(targetDirectory);
            foreach (string file in files)
            {
                if(!ShouldIgnorePath(file, ignorePaths)) { pathList.Add(file); }
            }

            string[] subDirectories = Directory.GetDirectories(targetDirectory);
            foreach (string subDirectory in subDirectories)
                pathList.AddRange(ProcessDirectory(subDirectory, ignorePaths));

            return pathList;
        }
        catch (Exception ex)
        {
            Debug.LogError("[BuildAssetPackage.ProcessDirectory] | " + ex.Message);
            return null;
        }
    }

    static bool ShouldIgnorePath(string path, List<string> ignorePaths)
    {
        if (ignorePaths == null) { return false; }
        foreach(string ignorePath in ignorePaths)
        {
            if(ComparePaths(path, ignorePath)) { return true; }
        }
        return false;
    }
    static bool ComparePaths(string path1, string path2)
    {
        return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), StringComparison.OrdinalIgnoreCase);
    }
}
