using BlackHole.Data;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableSpawnStack : ISuckableSpawnLogic
    {
        [SerializeField] private SuckableObjectId objectId;
        [SerializeField] private int stackCount;
        
        [Header("Debug preview")]
        [SerializeField] float perElementHeight = 1f;
        
        public void Execute(SuckableSpawnArgument argument)
        {
            var prefab = SuckableObjectManager.Instance.GetSuckableObjectPrefab(objectId);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found for object ID: {objectId}");
                return;
            }

            float lastObjHeight = 0f;
            
            for (int i = 0; i < stackCount; i++)
            {
                var spawnPosition = argument.position + Vector3.up * lastObjHeight;
                var rotation = Quaternion.Euler(argument.initialRotate);
                var obj = Object.Instantiate(prefab, spawnPosition, rotation, parent: argument.parent);
                if (obj != null)
                {
                    obj.transform.localScale = Vector3.one * argument.scale;
                    var collider = obj.GetComponent<Collider>();
                    if (collider != null)
                    {
                        lastObjHeight += collider.bounds.size.y * argument.scale;
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
    }
}