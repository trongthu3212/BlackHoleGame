using UnityEngine;

namespace BlackHole.Data
{
    [CreateAssetMenu(fileName = "SuckableObjectEntry", menuName = "BlackHole/SuckableObjectEntry")]
    public class SuckableObjectEntry : ScriptableObject
    {
        public SuckableObjectId objectId;
        public GameObject prefab;
    }
}