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
        
        public GameObject GetSuckableObjectPrefab(SuckableObjectId objectId)
        {
            foreach (var entry in suckableObjectCollection.entries)
            {
                if (entry.objectId == objectId)
                {
                    return entry.prefab;
                }
            }

            return null;
        }
    }
}