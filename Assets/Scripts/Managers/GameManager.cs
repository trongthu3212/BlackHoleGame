using System;
using BlackHole.LevelCreator;
using Newtonsoft.Json;
using UnityEngine;

namespace BlackHole
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelSpawner.LevelSpawner levelSpawner;

        private void Awake()
        {
            var testLevelData = Resources.Load<TextAsset>("LevelData");
            if (testLevelData == null)
            {
                Debug.LogError("Can't find test level data!");
                return;
            }
            var levelData = JsonConvert.DeserializeObject<LevelData>(testLevelData.text);
            if (levelData == null)
            {
                Debug.LogError("Failed to deserialize level data!");
                return;
            }
            levelSpawner.SpawnLevel(levelData);
        }
    }
}