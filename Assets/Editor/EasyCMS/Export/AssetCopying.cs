#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetCopying
{
    static string GetGameCarsFolder(string executablePath)
    {
        string executableDir = Path.GetDirectoryName(executablePath);
        string carsFolder = Path.Combine(executableDir, "cms2018_Data/StreamingAssets/Cars");
        return carsFolder;
    }

    public static void CopyCarAssetsToGame(string carPath, string executablePath)
    {
        bool error = false;
        if(executablePath == null || executablePath == string.Empty)
        {
            EditorUtility.DisplayDialog("Missing Executable Path", "Executable path is not defined. Please define it in the inspector window before running this command", "Ok");
            return;
        }

        string carsFolder = GetGameCarsFolder(executablePath);
        if(!Directory.Exists(carsFolder))
        {
            EditorUtility.DisplayDialog("Game File Validation Error", "Could not find a path for the game's car folder. Please ensure the CMS Executable is set up correctly", "Ok");
            return;
        }

        string[] splitCarPath = carPath.Split('/');
        splitCarPath[splitCarPath.Length - 1] = splitCarPath[splitCarPath.Length - 1].Replace(" ", "_");
        string carName = splitCarPath[splitCarPath.Length - 1];

        if (!Directory.Exists(carPath) || !File.Exists(Path.Combine(carPath, "car_" + carName + ".cms")))
        {
            EditorUtility.DisplayDialog("Missing Car Assets", "Failed to find car asset files. Please ensure the asset bundle has been built", "Ok");
            return;
        }

        string targetPath = Path.Combine(carsFolder, carName);
        if (Directory.Exists(targetPath))
        {
            if(!EditorUtility.DisplayDialog("Car Asset Conflict", "This car already exists in the game's assets. Overwrite?", "Yes", "No")) 
            { 
                return;
            }
        }

        string[] configFiles = { "name.txt", "config.txt", "bodyconfig.txt", "parts.txt" };
        bool[] configFilesOutdated = { false, false, false, false };
        bool anyConfigFileOutdated = false;
        for(uint i = 0; i < configFiles.Length; i++)
        {
            string configFile = configFiles[i];
            bool outDated = CheckIfGameFileNewer(configFile, carPath, executablePath);
            if (outDated)
            {
                anyConfigFileOutdated = true;
                configFilesOutdated[i] = true;
            }
        }
        if(anyConfigFileOutdated)
        {
            if(EditorUtility.DisplayDialog("Outdated Editor Files", "There are game config files that are newer than editor config files. Update editor config files with game config files?", "Yes", "No"))
            {
                for(uint i = 0; i < configFiles.Length; ++i)
                {
                    if(!configFilesOutdated[i]) { continue; }
                    CopyFileFromGame(configFiles[i], carPath, executablePath);
                }
            }
        }

        string[] directories = Directory.GetDirectories(carPath, "*", SearchOption.AllDirectories);
        foreach (string subdir in directories)
        {
            string newDirectory = subdir.Replace(carPath, targetPath);
            try
            {
                Directory.CreateDirectory(newDirectory);
            }
            catch (Exception ex)
            {
                Debug.LogError("[AssetCopying.CreateDirectories] | Failed to create directory. Error: " + ex.Message + "; File: \"" + newDirectory + "\"");
                error = true;
            }
        }

        string[] directoryContents = Directory.GetFiles(carPath);
        foreach(string directoryItem in directoryContents)
        {
            try
            {
                string newPath = directoryItem.Replace(carPath, targetPath);
                File.Copy(directoryItem, newPath, true);
            }
            catch (Exception ex)
            {
                Debug.LogError("[AssetCopying.CopyCarAssetsToGame] | Failed to copy file. Error: " + ex.Message + "; File: \"" + directoryItem + "\"");
                error = true;
            }
        }

        try
        {
            DeleteMetaFiles(targetPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AssetCopying.CopyCarAssetsToGame] | Failed to delete meta files. Error: " + ex.Message + "; File: \"" + targetPath + "\"");
            error = true;
        }

        if (error)
        {
            EditorUtility.DisplayDialog("Error Exporting Assets", "One or more errors happened while exporting assets. Check the console for more information", "Ok");
            return;
        }
        EditorUtility.DisplayDialog("Export Finished", "The asset files have been copied to the game files", "Ok");
    }

    static void DeleteMetaFiles(string directory)
    {
        string[] filesToDelete = Directory.GetFiles(directory, "*.meta", SearchOption.AllDirectories);
        foreach(string file in filesToDelete)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Debug.LogError("[AssetCopying.DeleteMetaFiles] | Failed to delete file. Error: " + ex.Message + "; File: " + file);
            }
        }
    }

    public static void CopyFileFromGame(string fileName, string carPath, string executablePath)
    {
        if (executablePath == null || executablePath == string.Empty)
        {
            EditorUtility.DisplayDialog("Missing Executable Path", "Executable path is not defined. Please define it in the inspector window before running this command", "Ok");
            return;
        }

        string carsFolder = GetGameCarsFolder(executablePath);
        if (!Directory.Exists(carsFolder))
        {
            EditorUtility.DisplayDialog("Game File Validation Error", "Could not find a path for the game's car folder. Please ensure the CMS Executable is set up correctly", "Ok");
            return;
        }

        string[] splitCarPath = carPath.Split('/');
        splitCarPath[splitCarPath.Length - 1] = splitCarPath[splitCarPath.Length - 1].Replace(" ", "_");
        string carName = splitCarPath[splitCarPath.Length - 1];

        string gameCarPath = Path.Combine(carsFolder, carName);
        if(!Directory.Exists(gameCarPath))
        {
            EditorUtility.DisplayDialog("Failed to find car files", "Could not find files for car \"" + carName + "\"", "Ok");
            return;
        }

        string targetFilePath = Path.Combine(gameCarPath, fileName);
        if (!File.Exists(targetFilePath))
        {
            EditorUtility.DisplayDialog("Failed to find car files", "Could not find file \"" + fileName, "Ok");
            return;
        }

        bool error = false;

        string localFilePath = Path.Combine(carPath, fileName);
        try
        {
            File.Copy(targetFilePath, localFilePath, true);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AssetCopying.CopyFileFromGame] | Failed to copy file. Error: " + ex.Message);
            error = true;
        }
        if (error)
        {
            EditorUtility.DisplayDialog("Error Updating Files", "One or more errors happened while updating the file. Check the console for more information", "Ok");
            return;
        }
    }

    public static bool CheckIfGameFileNewer(string fileName, string carPath, string executablePath)
    {
        string[] splitCarPath = carPath.Split('/');
        splitCarPath[splitCarPath.Length - 1] = splitCarPath[splitCarPath.Length - 1].Replace(" ", "_");
        string carName = splitCarPath[splitCarPath.Length - 1];

        string carsFolder = GetGameCarsFolder(executablePath);
        string gameCarPath = Path.Combine(carsFolder, carName);

        string localFilePath = Path.Combine(carPath, fileName);
        string targetFilePath = Path.Combine(gameCarPath, fileName);
        if(!File.Exists(localFilePath) || !File.Exists(targetFilePath))
        {
            return false;
        } 

        DateTime targetFileEditTime = File.GetLastWriteTime(targetFilePath);
        DateTime localFileEditTime = File.GetLastWriteTime(localFilePath);
        if(targetFileEditTime > localFileEditTime)
        {
            return true;
        }
        return false;
    }
}
#endif