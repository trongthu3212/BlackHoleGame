using System.Collections.Generic;
using BlackHole.Data;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole.Spawner
{
    [System.Serializable]
    public class SuckableSpawnRandomSingle : ISuckableSpawnLogic
    {
        [SerializeField] private List<SuckableObjectId> targetObjectIds;
        
        public void Execute(SuckableSpawnArgument argument)
        {
            var randomIndex = Random.Range(0, targetObjectIds.Count);
            var targetObjectId = targetObjectIds[randomIndex];
            
            argument.suckableObjectManager.InstantiateSuckableObject(
                targetObjectId,
                argument.position,
                Quaternion.identity,
                argument.scale,
                parent: argument.parent);
        }
        
        public void DrawGizmos(SuckableSpawnArgument argument)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(argument.position, 0.5f);
        }
    }
}