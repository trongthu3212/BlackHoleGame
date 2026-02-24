using System;
using BlackHole.Data;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole
{
    public class SuckableObjectManager : MonoBehaviour, ISingleton<SuckableObjectManager>
    {
        [SerializeField] private SuckableObjectCollection suckableObjectCollection;
        
        public static SuckableObjectManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            Instance = this;
        }
        #endif
        
        public GameObject InstantiateSuckableObject(SuckableObjectId objectId, Vector3 basePosition, Quaternion rotation, float scale = 1f, Transform parent = null)
        {
            var entry = suckableObjectCollection.GetEntryById(objectId);
            if (entry == null)
            {
                Debug.LogWarning($"No entry found for object ID: {objectId}");
                return null;
            }
            var prefab = entry.prefab;
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab is null for object ID: {objectId}");
                return null;
            }
            var actualScale = entry.defaultScale * scale;
            var actualSpawnPosition = basePosition + Vector3.up * entry.GetBaselineYOffset(actualScale);
            var instance = Instantiate(prefab, actualSpawnPosition, rotation);
            instance.transform.localScale = Vector3.one * actualScale;
            
            if (parent != null)
            {
                instance.transform.SetParent(parent);
            }
            return instance;
        }
    }
}