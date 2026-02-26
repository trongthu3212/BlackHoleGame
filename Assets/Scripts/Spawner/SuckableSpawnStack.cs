using BlackHole.Data;
using BlackHole.Interfaces;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace BlackHole.Spawner
{
    [System.Serializable]
    internal struct SuckableSpawnStackJsonData
    {
        public SuckableObjectId objectId;
        public int stackCount;
        public float offsetSizeY;
    }
    
    public class SuckableSpawnStack : ISuckableSpawnLogic
    {
        [SerializeField] private SuckableObjectId objectId;
        [SerializeField] private int stackCount;
        [SerializeField] private float offsetSizeY = 0.1f;

        [Header("Debug preview")]
        [SerializeField] private float perElementHeight = 1f;
        
        public void Execute(SuckableSpawnArgument argument)
        {
            float lastObjHeight = 0f;
            
            for (int i = 0; i < stackCount; i++)
            {
                var spawnPosition = argument.position + Vector3.up * lastObjHeight;
                var rotation = Quaternion.Euler(argument.initialRotate);
                var obj = argument.suckableObjectManager.InstantiateSuckableObject(
                    objectId,
                    spawnPosition,
                    rotation,
                    argument.scale,
                    parent: argument.parent);
                if (obj != null)
                {
                    var collider = obj.GetComponent<Collider>();
                    if (collider != null)
                    {
                        lastObjHeight += collider.bounds.size.y * obj.transform.localScale.y + offsetSizeY;
                    }
                }
            }
        }

        public void DrawGizmos(SuckableSpawnArgument argument)
        {
            Gizmos.color = Color.blue;
            float totalHeight = perElementHeight * stackCount;
            Gizmos.DrawWireCube(argument.position + Vector3.up * totalHeight / 2f, new Vector3(1f, totalHeight, 1f));
        }

        public SuckableSpawnSerializeEntry SerializeJson()
        {
            var jsonData = new SuckableSpawnStackJsonData()
            {
                objectId = objectId,
                stackCount = stackCount,
                offsetSizeY = offsetSizeY
            };
            
            return SuckableSpawnSerializeEntry.Pack(
                SuckableSpawnType.Stack,
                jsonData);
        }

        public void DeserializeFromJson(SuckableSpawnSerializeEntry data)
        {
            var jsonData = JsonConvert.DeserializeObject<SuckableSpawnStackJsonData>(data.content.ToString());
            objectId = jsonData.objectId;
            stackCount = jsonData.stackCount;
            offsetSizeY = jsonData.offsetSizeY;
        }
    }
}