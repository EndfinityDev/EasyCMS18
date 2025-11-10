#if UNITY_EDITOR

using UnityEditor;

public class ImportModelConfig : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        var modelImporter = assetImporter as ModelImporter;
        modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
        modelImporter.importAnimation = false;
        modelImporter.addCollider = true;
        modelImporter.animationType = ModelImporterAnimationType.None;
        modelImporter.importBlendShapes = false;
        //modelImporter.importLights = false;
        //modelImporter.importVisibility = false;
        //modelImporter.importCameras = false;
        //modelImporter.normalCalculationMode = ModelImporterNormalCalculationMode.AreaAndAngleWeighted;
        modelImporter.isReadable = true;
        //modelImporter.indexFormat = ModelImporterIndexFormat.Auto;
        //modelImporter.materialLocation = ModelImporterMaterialLocation.External;
        modelImporter.importMaterials = true;
        modelImporter.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
        modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
    }
}
#endif