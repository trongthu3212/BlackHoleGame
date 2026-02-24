using System;
using BlackHole.Interfaces;
using BlackHole.Spawner;
using SaintsField;
using SaintsField.Playa;
using UnityEditor;
using UnityEngine;

namespace BlackHole.LevelCreator
{
    public class LevelCreator : MonoBehaviour
    {
        [Header("Transform settings")]
        [Header("Floor")]
        [SerializeField] private Transform floorTransform;
        [SerializeField] private Grid floorGrid;
        [SerializeField] private Transform spawnerTransform;

        private FloorPolisher _floorPolisher;

        private void OnValidate()
        {
            _floorPolisher = new();
            _floorPolisher.Initialize(floorTransform, floorGrid);
        }

        [Button]
        private void AutoSetup()
        {
            floorTransform = transform.Find("Floor");
            if (floorTransform == null)
            {
                Debug.Log("Can't find floor root transform. Creating one.");
                floorTransform = new GameObject("Floor").transform;
                floorTransform.parent = transform;
            }
            spawnerTransform = transform.Find("Spawners");
            if (spawnerTransform == null)
            {
                Debug.Log("Can't find spawner root transform. Creating one.");
                spawnerTransform = new GameObject("Spawners").transform;
                spawnerTransform.parent = transform;
            }

            floorGrid = GetComponent<Grid>();
            if (floorGrid == null)
            {
                Debug.LogError("Can't find Grid component for floor!");
            }
        }
        
        [Button]
        private void CreateNewSpawner(string spawnerName, ISuckableSpawnLogic spawnLogic)
        {
            if (spawnerTransform == null)
            {
                Debug.LogError("Can't find spawner root transform. Please run AutoSetup first.");
                return;
            }
            
            var spawnerObj = new GameObject(spawnerName);
            spawnerObj.transform.parent = spawnerTransform;
            
            var spawner = spawnerObj.AddComponent<SuckableMonoSpawner>();
            spawner.SpawnLogic = spawnLogic;
            
            Selection.activeGameObject = spawnerObj;
        }

        [Button]
        private void TryDoPolish()
        {
            if (_floorPolisher == null)
            {
                Debug.LogError("Floor polisher is not initialized. Please check the floor transform and grid settings.");
                return;
            }
            
            _floorPolisher.ExecutePolish();
        }
        
        private void OnDrawGizmos()
        {
            // Draw Center
            Handles.Label(transform.position, "Level Creator Center");

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1f);

            if (_floorPolisher != null)
            {
                _floorPolisher.TryDrawGizmos();
            }
        }
    }
}