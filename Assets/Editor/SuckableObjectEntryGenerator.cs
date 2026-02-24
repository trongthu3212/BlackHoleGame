using UnityEngine;
using UnityEditor;
using BlackHole.Data;
using System.IO;

namespace BlackHole.Editor
{
    public class SuckableObjectEntryGenerator
    {
        private const string OUTPUT_FOLDER = "Assets/Data";
        private const string PREFAB_FOLDER = "Assets/Prefabs/Food";

        [MenuItem("BlackHole/Generate SuckableObjectEntry Assets")]
        public static void GenerateSuckableObjectEntries()
        {
            // Get all enum values from SuckableObjectId
            var enumValues = System.Enum.GetValues(typeof(SuckableObjectId));

            foreach (SuckableObjectId id in enumValues)
            {
                // Convert enum name to prefab name pattern (PFB_Food_{id})
                string prefabName = $"PFB_Food_{id.ToString().ToLower()}";
                string prefabPath = $"{PREFAB_FOLDER}/{prefabName}.prefab";

                // Load the prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab == null)
                {
                    Debug.LogWarning($"Prefab not found: {prefabPath}");
                    continue;
                }

                // Calculate baseline Y offset
                float baselineYOffset = CalculateBaselineYOffset(prefab);

                // Create the entry asset
                string assetName = $"SO_{id}";
                string assetPath = $"{OUTPUT_FOLDER}/{assetName}.asset";

                // Check if asset already exists
                SuckableObjectEntry existingEntry = AssetDatabase.LoadAssetAtPath<SuckableObjectEntry>(assetPath);

                if (existingEntry == null)
                {
                    // Create new entry
                    SuckableObjectEntry entry = ScriptableObject.CreateInstance<SuckableObjectEntry>();
                    entry.objectId = id;
                    entry.prefab = prefab;
                    entry.baselineYOffset = baselineYOffset;

                    // Create output folder if it doesn't exist
                    if (!AssetDatabase.IsValidFolder(OUTPUT_FOLDER))
                    {
                        AssetDatabase.CreateFolder("Assets", "Data");
                    }

                    AssetDatabase.CreateAsset(entry, assetPath);
                    Debug.Log($"Created: {assetPath} (BaselineY: {baselineYOffset:F3})");
                }
                else
                {
                    // Update existing entry
                    existingEntry.objectId = id;
                    existingEntry.prefab = prefab;
                    existingEntry.baselineYOffset = baselineYOffset;
                    EditorUtility.SetDirty(existingEntry);
                    Debug.Log($"Updated: {assetPath} (BaselineY: {baselineYOffset:F3})");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("SuckableObjectEntry generation complete!");
        }

        private static float CalculateBaselineYOffset(GameObject prefab)
        {
            // Temporarily instantiate to calculate bounds
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            if (tempInstance == null)
            {
                Debug.LogWarning($"Failed to instantiate prefab to calculate baseline: {prefab.name}");
                return 0f;
            }

            try
            {
                Bounds bounds = CalculateBounds(tempInstance);

                // Return the distance from the lowest point to origin (this is the baseline Y offset)
                return -bounds.min.y;
            }
            finally
            {
                Object.DestroyImmediate(tempInstance);
            }
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

