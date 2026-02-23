using BlackHole.Editor;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableMonoSpawner : MonoBehaviour
    {
        [SerializeReference, ClassDropdown(typeof(ISuckableSpawnLogic))]
        private ISuckableSpawnLogic spawnLogic;
    }
}