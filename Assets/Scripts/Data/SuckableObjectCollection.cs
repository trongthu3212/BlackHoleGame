using UnityEngine;

namespace BlackHole.Data
{
    [CreateAssetMenu(fileName = "SuckableObjectCollection", menuName = "BlackHole/SuckableObjectCollection")]
    public class SuckableObjectCollection : ScriptableObject
    {
        public SuckableObjectEntry[] entries;
    }
}