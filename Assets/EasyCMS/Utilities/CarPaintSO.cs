using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Paint", menuName = "EasyCMS/Utilities/Car Paint Utility", order = 1)]
public class CarPaintSO : ScriptableObject
{
#if UNITY_EDITOR
    public Color CarColor;
    [Range(0.0f, 1.0f)]
    public float Metallic;
    [Range(0.0f, 1.0f)]
    public float Roughness;
    //[Range(0.0f, 1.0f)]
    //public float Clearcoat;

    public List<Material> TargetMaterials = new List<Material>();

    void OnValidate()
    {
        if(TargetMaterials == null) { return; }
        foreach (Material material in TargetMaterials)
        {
            if (material == null) { continue; }
            material.SetColor("_Color", CarColor);
            material.SetFloat("_Metallic", Metallic);
            material.SetFloat("_Roughness", Roughness);
            //material.SetFloat("_ClearCoat", Clearcoat);
        }
    }
#endif
}
