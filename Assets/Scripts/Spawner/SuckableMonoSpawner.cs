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

        [Header("Spawn initial configuration")]
        [SerializeField] private float scale;
        [SerializeField] private Transform parent;
        [SerializeField] private int seed = 54535353;

        public ISuckableSpawnLogic SpawnLogic
        {
            get => spawnLogic;
            set => spawnLogic = value;
        }
        
        [Button]
        private void Execute()
        {
            if (spawnLogic == null)
            {
                Debug.LogError("Spawn logic is not assigned.");
                return;
            }
            
            UnityEngine.Random.InitState(seed);
            
            var argument = new SuckableSpawnArgument
            {
                position = transform.position,
                scale = transform.localScale.x,
                parent = parent,
                suckableObjectManager = SuckableObjectManager.Instance
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
                parent = transform,
                suckableObjectManager = SuckableObjectManager.Instance
            };
            
            UnityEngine.Random.InitState(seed);
            
            spawnLogic.DrawGizmos(argument);
        }
    }
}