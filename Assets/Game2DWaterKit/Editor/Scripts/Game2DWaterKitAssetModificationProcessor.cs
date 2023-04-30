namespace Game2DWaterKit
{
	using System.IO;
	using UnityEngine;
	using UnityEditor;

	public class Game2DWaterKitAssetModificationProcessor : AssetModificationProcessor
    {
        static void OnWillCreateAsset(string assetName)
        {
            if (assetName.EndsWith(".meta") || !assetName.Contains(".prefab"))
                return;

            foreach (Object item in DragAndDrop.objectReferences)
            {
                if (!(item is GameObject))
                    continue;

                var go = item as GameObject;

                if (go.GetComponent<Game2DWaterKitObject>() != null)
                {
                    var material = go.GetComponent<MeshRenderer>().sharedMaterial;

                    if (material != null)
                        CreateMaterialAsset(material, assetName);
                }
                else if (go.GetComponent<LargeWaterAreaManager>() != null)
                {
                    var waterObject = go.GetComponent<LargeWaterAreaManager>().WaterObject;

                    if (waterObject != null)
                    {
                        var material = waterObject.GetComponent<MeshRenderer>().sharedMaterial;

                        if (material != null)
                            CreateMaterialAsset(material, assetName);
                    }
                }
            }
        }

        private static void CreateMaterialAsset(UnityEngine.Material material, string assetName)
        {
            var prefabName = Path.GetFileNameWithoutExtension(assetName);
            var directorty = Path.GetDirectoryName(assetName);

            var noiseTexture = material.GetTexture("_NoiseTexture");

            if (noiseTexture != null && !AssetDatabase.Contains(noiseTexture))
                AssetDatabase.CreateAsset(noiseTexture, Path.Combine(directorty, prefabName + "_noiseTexture.asset"));

            if (!AssetDatabase.Contains(material))
                AssetDatabase.CreateAsset(material, Path.Combine(directorty, prefabName + ".mat"));
        }
    }
}