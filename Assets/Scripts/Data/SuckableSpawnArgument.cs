using UnityEngine;

namespace BlackHole.Data
{
    [System.Serializable]
    public struct SuckableSpawnArgument
    {
        public Vector3 position;
        public float scale;
        public Transform parent;
        public Vector3 initialRotate;
    }
}