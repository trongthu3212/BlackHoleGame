using SaintsField.Playa;
using UnityEngine;

namespace BlackHole.Data
{
    [CreateAssetMenu(fileName = "SuckableObjectEntry", menuName = "BlackHole/SuckableObjectEntry")]
    public class SuckableObjectEntry : ScriptableObject
    {
        public SuckableObjectId objectId;
        public GameObject prefab;
        public float baselineYOffset;
        public float defaultScale;
        
        public float GetBaselineYOffset(float scale = 1f)
        {
            return baselineYOffset * scale;
        }
    }
}