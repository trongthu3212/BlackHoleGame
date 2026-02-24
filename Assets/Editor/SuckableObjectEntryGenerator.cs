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

                    // Create output folder if it doesn't exist
                    if (!AssetDatabase.IsValidFolder(OUTPUT_FOLDER))
                    {
                        AssetDatabase.CreateFolder("Assets", "Data");
                    }

                    AssetDatabase.CreateAsset(entry, assetPath);
                    Debug.Log($"Created: {assetPath}");
                }
                else
                {
                    // Update existing entry
                    existingEntry.objectId = id;
                    existingEntry.prefab = prefab;
                    EditorUtility.SetDirty(existingEntry);
                    Debug.Log($"Updated: {assetPath}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("SuckableObjectEntry generation complete!");
        }
    }
}

