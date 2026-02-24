using System;
using System.Collections.Generic;
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

        private List<SuckableSpawnEntry> ListAllSpawnerEntries()
        {
            var entries = new List<SuckableSpawnEntry>();
            if (spawnerTransform == null) return entries;
            
            var spawners = spawnerTransform.GetComponentsInChildren<SuckableMonoSpawner>();
            foreach (var spawner in spawners)
            {
                entries.Add(new SuckableSpawnEntry
                {
                    position = spawner.transform.position,
                    rotation = spawner.transform.rotation.eulerAngles,
                    spawnLogic = spawner.SpawnLogic
                });
            }

            return entries;
        }
        
        [Button]
        private void TrySerialize()
        {
            _floorPolisher.ExecutePolish();
            
            var levelData = new LevelData
            {
                floorGrid = _floorPolisher.FloorGrids,
                floorGridBounds = _floorPolisher.FloorGridBounds,
                suckableSpawnEntries = ListAllSpawnerEntries()
            };

            var result = JsonUtility.ToJson(levelData);
            Debug.Log(result);
            
            var resultCopy2 = JsonUtility.FromJson<LevelData>(result);
            Debug.Log("Im just testing");
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