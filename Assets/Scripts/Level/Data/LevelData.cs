using System.Collections.Generic;
using BlackHole.Interfaces;
using BlackHole.Level;
using BlackHole.Spawner;
using BlackHole.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlackHole.LevelCreator
{
    [System.Serializable]
    public struct SuckableSpawnEntry
    {
        public SuckableSpawnSerializeEntry spawnLogic;
        public JsonFriendlyVector3 position;
        public JsonFriendlyVector3 rotation;
        public float scale;
    }
    
    [System.Serializable]
    public class LevelData
    {
        public GridBlockType[][] floorGrid;
        public JsonFriendlyRectInt floorGridBounds;
        public JsonFriendlyVector2 floorGridCellSize;
        public List<SuckableSpawnEntry> suckableSpawnEntries;
    }
}