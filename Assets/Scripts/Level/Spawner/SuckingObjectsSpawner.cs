using System.Collections.Generic;
using BlackHole.Data;
using BlackHole.LevelCreator;
using BlackHole.Spawner;
using UnityEngine;

namespace BlackHole.LevelSpawner
{
    public class SuckingObjectsSpawner : MonoBehaviour
    {
        public void SpawnSuckingObjects(List<SuckableSpawnEntry> spawnEntries, Transform parent = null)
        {
            foreach (var entry in spawnEntries)
            {
                var spawnLogic = SuckableSpawnFactory.CreateFromJson(entry.spawnLogic);
                spawnLogic.Execute(new SuckableSpawnArgument()
                {
                    initialRotate = entry.rotation.ToVector3(),
                    position = entry.position.ToVector3(),
                    scale = entry.scale,
                    parent = parent,
                    suckableObjectManager = SuckableObjectManager.Instance
                });
            }
        }
    }
}