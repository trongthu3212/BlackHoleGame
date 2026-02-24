using SaintsField.Playa;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace BlackHole.Data
{
    [CreateAssetMenu(fileName = "SuckableObjectCollection", menuName = "BlackHole/SuckableObjectCollection")]
    public class SuckableObjectCollection : ScriptableObject
    {
        public SuckableObjectEntry[] entries;

        public SuckableObjectEntry GetEntryById(SuckableObjectId id)
        {
            foreach (var entry in entries)
            {
                if (entry.objectId == id)
                {
                    return entry;
                }
            }
            return null;
        }
        
        [Button]
        private void SetAllDefaultScale(float scale)
        {
            foreach (var entry in entries)
            {
                entry.defaultScale = scale;
                EditorUtility.SetDirty(entry);
            }
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        
        [Button]
        private void RanBaselineYCalculator()
        {
            foreach (var entry in entries)
            {
                if (entry.prefab == null)
                {
                    Debug.LogWarning($"Prefab is null for object ID: {entry.objectId}");
                    continue;
                }
                
                var renderer = entry.prefab.GetComponentInChildren<Renderer>();
                if (renderer == null)
                {
                    Debug.LogWarning($"Renderer not found in prefab for object ID: {entry.objectId}");
                    continue;
                }
                
                var bounds = renderer.bounds;
                var baselineY = bounds.min.y;
                entry.baselineYOffset = -baselineY;
                
                Debug.Log($"Calculated baseline Y offset for object ID: {entry.objectId} is {entry.baselineYOffset}");
                
                EditorUtility.SetDirty(entry);
            }
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}