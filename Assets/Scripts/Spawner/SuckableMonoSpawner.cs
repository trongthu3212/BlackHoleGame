using System;
using BlackHole.Data;
using BlackHole.Interfaces;
using SaintsField;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableMonoSpawner : MonoBehaviour
    {
        [SerializeReference, ReferencePicker]
        private ISuckableSpawnLogic spawnLogic;

        private void OnDrawGizmos()
        {
            if (spawnLogic == null) return;
            
            var argument = new SuckableSpawnArgument
            {
                position = transform.position,
                scale = transform.localScale.x,
                parent = transform
            };
            
            spawnLogic.DrawGizmos(argument);
        }
    }
}