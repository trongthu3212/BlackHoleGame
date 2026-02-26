using System;
using System.Collections.Generic;
using BlackHole.Level;
using SaintsField.Playa;
using UnityEditor;
using UnityEngine;

namespace BlackHole.LevelSpawner
{
    public class FloorSpawner: MonoBehaviour
    {
        [System.Serializable]
        struct CornerPrefabsSetup
        {
            public List<GameObject> cornerPrefabs;
            public int topLeftCornerRotate;
            public int topRightCornerRotate;
            public int bottomLeftCornerRotate;
            public int bottomRightCornerRotate;
        }

        [System.Serializable]
        struct EdgeBlockPrefabsSetup
        {
            public List<GameObject> edgeBlockPrefabs;
            public int topEdgeRotate;
            public int bottomEdgeRotate;
            public int leftEdgeRotate;
            public int rightEdgeRotate;
        }

        [System.Serializable]
        struct CenterBlockPrefabsSetup
        {
            public List<GameObject> centerBlockPrefabs;
        }

        [System.Serializable]
        struct DiagonalBlockPrefabsSetup
        {
            public List<GameObject> diagonalBlockPrefabs;
            public int rightToBottomRotate;
            public int rightToTopRotate;
            public int leftToTopRotate;
            public int leftToBottomRotate;
        }
        
        [System.Serializable]
        struct DiagonalSupportBlockPrefabsSetup
        {
            public List<GameObject> diagonalSupportBlockPrefabs;
            public int rightToBottomRotate;
            public int rightToTopRotate;
            public int leftToTopRotate;
            public int leftToBottomRotate;
        }
        
        [Header("Floor Settings")]
        [SerializeField] private CornerPrefabsSetup cornerPrefabsSetup;
        [SerializeField] private EdgeBlockPrefabsSetup edgeBlockPrefabsSetup;
        [SerializeField] private CenterBlockPrefabsSetup centerBlockPrefabsSetup;
        [SerializeField] private DiagonalBlockPrefabsSetup diagonalBlockPrefabsSetup;
        [SerializeField] private DiagonalSupportBlockPrefabsSetup diagonalSupportBlockPrefabsSetup;
        
        [Header("General Settings")]
        [SerializeField] private bool debugDraw = false;
        
        private RectInt _floorGridBounds;
        
        private Transform _floorRoot;
        private Vector2 _floorCellSize;
        
        private Vector3 GetCellPosition(int x, int z)
        {
            var offsetAddSpawnPos = new Vector3(_floorCellSize.x / 2, 0, _floorCellSize.y / 2)
            {
                y = 0
            };
            var basePos = new Vector3((x + _floorGridBounds.xMin) * _floorCellSize.x, 0, (z + _floorGridBounds.yMin) * _floorCellSize.y);
            return _floorRoot.transform.position + basePos + offsetAddSpawnPos;
        }
        
        private void InstantiateFloor(int x, int z, GridBlockType blockType)
        {
            var spawnPos = GetCellPosition(x, z);

            if (blockType == GridBlockType.Normal)
            {
                var randomBlockIndex = UnityEngine.Random.Range(0, centerBlockPrefabsSetup.centerBlockPrefabs.Count);
                var randomRotationIndex = UnityEngine.Random.Range(0, 4);
                
                var prefab = centerBlockPrefabsSetup.centerBlockPrefabs[randomBlockIndex];
                var rotation = Quaternion.Euler(0, randomRotationIndex * 90, 0);
                
                Instantiate(prefab, spawnPos, rotation, _floorRoot);
                return;
            }

            if (blockType >= GridBlockType.EdgeBlockTypeStarts && blockType <= GridBlockType.EdgeBlockTypeEnds)
            {
                var randomBlockIndex = UnityEngine.Random.Range(0, edgeBlockPrefabsSetup.edgeBlockPrefabs.Count);
                int rotationY = 0;
                switch (blockType)
                {
                    case GridBlockType.EdgeTop:
                        rotationY = edgeBlockPrefabsSetup.topEdgeRotate;
                        break;
                    case GridBlockType.EdgeBottom:
                        rotationY = edgeBlockPrefabsSetup.bottomEdgeRotate;
                        break;
                    case GridBlockType.EdgeLeft:
                        rotationY = edgeBlockPrefabsSetup.leftEdgeRotate;
                        break;
                    case GridBlockType.EdgeRight:
                        rotationY = edgeBlockPrefabsSetup.rightEdgeRotate;
                        break;
                }
                
                var prefab = edgeBlockPrefabsSetup.edgeBlockPrefabs[randomBlockIndex];
                var rotation = Quaternion.Euler(0, rotationY, 0);
                
                var obj = Instantiate(prefab, spawnPos, rotation, _floorRoot);
                obj.transform.localScale = Vector3.one;
                
                return;
            }
            
            if (blockType >= GridBlockType.CornerBlockTypeStarts && blockType <= GridBlockType.CornerBlockTypeEnds)
            {
                var randomBlockIndex = UnityEngine.Random.Range(0, cornerPrefabsSetup.cornerPrefabs.Count);
                int rotationY = 0;
                switch (blockType)
                {
                    case GridBlockType.CornerTopLeft:
                        rotationY = cornerPrefabsSetup.topLeftCornerRotate;
                        break;
                    case GridBlockType.CornerTopRight:
                        rotationY = cornerPrefabsSetup.topRightCornerRotate;
                        break;
                    case GridBlockType.CornerBottomLeft:
                        rotationY = cornerPrefabsSetup.bottomLeftCornerRotate;
                        break;
                    case GridBlockType.CornerBottomRight:
                        rotationY = cornerPrefabsSetup.bottomRightCornerRotate;
                        break;
                }
                
                var prefab = cornerPrefabsSetup.cornerPrefabs[randomBlockIndex];
                var rotation = Quaternion.Euler(0, rotationY, 0);
                
                var obj = Instantiate(prefab, spawnPos, rotation, _floorRoot);
                obj.transform.localScale = Vector3.one;
                return;
            }
            
            if (blockType >= GridBlockType.DiagonalBlockTypeStarts && blockType <= GridBlockType.DiagonalBlockTypeEnds)
            {
                var randomBlockIndex = UnityEngine.Random.Range(0, diagonalBlockPrefabsSetup.diagonalBlockPrefabs.Count);
                int rotationY = 0;
                switch (blockType)
                {
                    case GridBlockType.DiagonalLeftToTop:
                        rotationY = diagonalBlockPrefabsSetup.leftToTopRotate;
                        break;
                    case GridBlockType.DiagonalLeftToBottom:
                        rotationY = diagonalBlockPrefabsSetup.leftToBottomRotate;
                        break;
                    case GridBlockType.DiagonalRightToTop:
                        rotationY = diagonalBlockPrefabsSetup.rightToTopRotate;
                        break;
                    case GridBlockType.DiagonalRightToBottom:
                        rotationY = diagonalBlockPrefabsSetup.rightToBottomRotate;
                        break;
                }
                
                var prefab = diagonalBlockPrefabsSetup.diagonalBlockPrefabs[randomBlockIndex];
                var rotation = Quaternion.Euler(0, rotationY, 0);
                
                var obj = Instantiate(prefab, spawnPos, rotation, _floorRoot);
                obj.transform.localScale = Vector3.one;
                return;
            }
            
            if (blockType >= GridBlockType.DiagonalSupportBlockTypeStarts && blockType <= GridBlockType.DiagonalSupportBlockTypeEnds)
            {
                var randomBlockIndex = UnityEngine.Random.Range(0, diagonalSupportBlockPrefabsSetup.diagonalSupportBlockPrefabs.Count);
                int rotationY = 0;
                switch (blockType)
                {
                    case GridBlockType.DiagonalSupportLeftToTop:
                        rotationY = diagonalSupportBlockPrefabsSetup.leftToTopRotate;
                        break;
                    case GridBlockType.DiagonalSupportLeftToBottom:
                        rotationY = diagonalSupportBlockPrefabsSetup.leftToBottomRotate;
                        break;
                    case GridBlockType.DiagonalSupportRightToTop:
                        rotationY = diagonalSupportBlockPrefabsSetup.rightToTopRotate;
                        break;
                    case GridBlockType.DiagonalSupportRightToBottom:
                        rotationY = diagonalSupportBlockPrefabsSetup.rightToBottomRotate;
                        break;
                }
                
                var prefab = diagonalSupportBlockPrefabsSetup.diagonalSupportBlockPrefabs[randomBlockIndex];
                var rotation = Quaternion.Euler(0, rotationY, 0);
                
                var obj = Instantiate(prefab, spawnPos, rotation, _floorRoot);
                obj.transform.localScale = Vector3.one;
                return;
            }
        }
        
        public void RebuildFloor(GridBlockType[][] floorGrid, RectInt floorGridBounds, Vector2 floorCellSize, Transform spawnRoot)
        {
            _floorGridBounds = floorGridBounds;
            _floorCellSize = floorCellSize;
            _floorRoot = spawnRoot;
            
            // Clear previous preview
            for (int i = spawnRoot.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(spawnRoot.GetChild(i).gameObject);
            }

            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                for (int z = 0; z < _floorGridBounds.height; z++)
                {
                    if (floorGrid[x][z] != GridBlockType.None)
                    {
                        InstantiateFloor(x, z, floorGrid[x][z]);
                    }
                }
            }
        }
        
    }
}