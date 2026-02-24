using System.Collections.Generic;
using BlackHole.Interfaces;
using BlackHole.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlackHole.LevelCreator
{
    [System.Serializable]
    public struct SuckableSpawnEntry
    {
        [SerializeReference]
        public ISuckableSpawnLogic spawnLogic;
        public Vector3 position;
        public Vector3 rotation;
    }
    
    [System.Serializable]
    public class LevelData
    {
        public GridBlockType[][] floorGrid;
        public RectInt floorGridBounds;
        public List<SuckableSpawnEntry> suckableSpawnEntries;
    }
}