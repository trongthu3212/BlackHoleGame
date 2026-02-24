using System;
using BlackHole.Data;
using BlackHole.Interfaces;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableMonoSpawner : MonoBehaviour
    {
        [SerializeReference, ReferencePicker]
        private ISuckableSpawnLogic spawnLogic;

        [SerializeField] private float scale;
        [SerializeField] private Transform parent;

        [Button]
        private void Execute()
        {
            if (spawnLogic == null)
            {
                Debug.LogError("Spawn logic is not assigned.");
                return;
            }
            
            var argument = new SuckableSpawnArgument
            {
                position = transform.position,
                scale = transform.localScale.x,
                parent = parent
            };
            
            spawnLogic.Execute(argument);
        }
        
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