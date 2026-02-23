using BlackHole.Data;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableSpawnSingle : ISuckableSpawnLogic
    {
        [SerializeField] private SuckableObjectId targetObjectId;
        
        public void Execute(SuckableSpawnArgument argument)
        {
            var prefab = SuckableObjectManager.Instance.GetSuckableObjectPrefab(targetObjectId);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found for object ID: {targetObjectId}");
                return;
            }
            
            var obj = Object.Instantiate(prefab, argument.position, Quaternion.identity, parent: argument.parent);
            if (obj != null)
            {
                obj.transform.localPosition = Vector3.one * argument.scale;
            }
        }
    }
}