using UnityEditor;
using UnityEngine;
using System.IO;

namespace BlackHole.Editor
{
    public class FoodPrefabPositioner
    {
        private const string PREFAB_FOLDER = "Assets/Prefabs/Food";
        private const string PREFAB_PREFIX = "PFB_Food_";

        [MenuItem("BlackHole/Align Food Prefabs to Y=0")]
        public static void AlignFoodPrefabsToBaseline()
        {
            // Get all prefab files in the Food folder
            string[] guids = AssetDatabase.FindAssets("PFB_Food_", new[] { PREFAB_FOLDER });

            int processedCount = 0;
            int failedCount = 0;

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Only process prefabs
                if (!assetPath.EndsWith(".prefab"))
                {
                    continue;
                }

                // Load the prefab
                GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefabRoot == null)
                {
                    Debug.LogWarning($"Failed to load prefab: {assetPath}");
                    failedCount++;
                    continue;
                }

                // Temporarily instantiate to calculate bounds
                GameObject tempInstance = PrefabUtility.InstantiatePrefab(prefabRoot) as GameObject;

                if (tempInstance == null)
                {
                    Debug.LogWarning($"Failed to instantiate prefab: {assetPath}");
                    failedCount++;
                    continue;
                }

                try
                {
                    // Get the renderer bounds
                    Bounds bounds = CalculateBounds(tempInstance);

                    // If we have valid bounds, calculate the offset needed
                    if (bounds.size.magnitude > 0)
                    {
                        float lowestPoint = bounds.min.y;
                        float offsetY = -lowestPoint;

                        // Apply the offset to the prefab root in the scene
                        tempInstance.transform.position += Vector3.up * offsetY;

                        // Save the modified prefab
                        PrefabUtility.SaveAsPrefabAsset(tempInstance, assetPath);
                        Debug.Log($"Aligned {assetPath} - Offset Y: {offsetY:F3}");
                        processedCount++;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not calculate bounds for: {assetPath}");
                        failedCount++;
                    }
                }
                finally
                {
                    // Clean up temporary instance
                    Object.DestroyImmediate(tempInstance);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Food Prefab Positioning Complete! Processed: {processedCount}, Failed: {failedCount}");
        }

        private static Bounds CalculateBounds(GameObject obj)
        {
            Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
            bool hasBounds = false;

            // Get bounds from all renderers
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
                else
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
            }

            // If no renderers, try mesh filters
            if (!hasBounds)
            {
                MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();

                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter.sharedMesh != null)
                    {
                        Vector3 meshBoundsMin = meshFilter.sharedMesh.bounds.min;
                        Vector3 meshBoundsMax = meshFilter.sharedMesh.bounds.max;

                        Vector3 worldMin = meshFilter.transform.TransformPoint(meshBoundsMin);
                        Vector3 worldMax = meshFilter.transform.TransformPoint(meshBoundsMax);

                        if (hasBounds)
                        {
                            bounds.Encapsulate(worldMin);
                            bounds.Encapsulate(worldMax);
                        }
                        else
                        {
                            bounds = new Bounds((worldMin + worldMax) / 2, worldMax - worldMin);
                            hasBounds = true;
                        }
                    }
                }
            }

            return bounds;
        }
    }
}

