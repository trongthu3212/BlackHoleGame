using System.Collections.Generic;
using BlackHole.Data;
using BlackHole.Interfaces;
using BlackHole.Utilities;
using Newtonsoft.Json;
using Unity.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace BlackHole.Spawner
{
    [System.Serializable]
    internal struct SuckableSpawnRandomSingleJsonData
    {
        public List<SuckableObjectId> targetObjectIds;
    }
    
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

        public SuckableSpawnSerializeEntry SerializeJson()
        {
            var jsonData = new SuckableSpawnRandomSingleJsonData()
            {
                targetObjectIds = targetObjectIds
            };
            
            return SuckableSpawnSerializeEntry.Pack(
                SuckableSpawnType.RandomSingle,
                jsonData);
        }

        public void DeserializeFromJson(SuckableSpawnSerializeEntry data)
        {
            var jsonData = JsonConvert.DeserializeObject<SuckableSpawnRandomSingleJsonData>(data.content.ToString());
            targetObjectIds = jsonData.targetObjectIds;
        }
    }
}