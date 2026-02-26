using UnityEngine;

namespace BlackHole.LevelSpawner
{
    public class LevelSpawner : MonoBehaviour
    {
        [SerializeField] private FloorSpawner floorSpawner;
        [SerializeField] private SuckingObjectsSpawner suckingObjectsSpawner;

        [Header("Root")]
        [SerializeField] private Transform floorRoot;

        public void SpawnLevel(LevelCreator.LevelData levelData)
        {
            floorSpawner.RebuildFloor(levelData.floorGrid, levelData.floorGridBounds.ToRectInt(), levelData.floorGridCellSize.ToVector2(), floorRoot);
            suckingObjectsSpawner.SpawnSuckingObjects(levelData.suckableSpawnEntries, null);
            
            DoOptimisePart1();
        }

        private void DoOptimisePart1()
        {
            if (Application.isPlaying)
            {
                StaticBatchingUtility.Combine(floorRoot.gameObject);
            }
        }
    }
}