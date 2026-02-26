using BlackHole.Data;
using BlackHole.Interfaces;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace BlackHole.Spawner
{
    [System.Serializable]
    internal struct SuckableSpawnSingleJsonData
    {
        public SuckableObjectId targetObjectId;
    }
    
    public class SuckableSpawnSingle : ISuckableSpawnLogic
    {
        [SerializeField] private SuckableObjectId targetObjectId;
        
        public void Execute(SuckableSpawnArgument argument)
        {
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
            return SuckableSpawnSerializeEntry.Pack(
                SuckableSpawnType.Single,
                new SuckableSpawnSingleJsonData()
                {
                    targetObjectId = targetObjectId
                });
        }

        public void DeserializeFromJson(SuckableSpawnSerializeEntry data)
        {
            var jsonData = JsonConvert.DeserializeObject<SuckableSpawnSingleJsonData>(data.content.ToString());
            targetObjectId = jsonData.targetObjectId;
        }
    }
}