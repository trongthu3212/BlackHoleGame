using System;
using System.Collections.Generic;
using SaintsField.Playa;
using UnityEditor;
using UnityEngine;

namespace BlackHole.LevelCreator
{
    public class FloorPolisher: MonoBehaviour
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

        enum GridBlockType
        {
            None = 0,
            Normal = 1,
            
            EdgeTop = 4,
            EdgeBottom = 5,
            EdgeLeft = 6,
            EdgeRight = 7,
            
            CornerTopLeft = 8,
            CornerTopRight = 9,
            CornerBottomLeft = 10,
            CornerBottomRight = 11,
            
            EdgeBlockTypeStarts = EdgeTop,
            EdgeBlockTypeEnds = EdgeRight,
            
            CornerBlockTypeStarts = CornerTopLeft,
            CornerBlockTypeEnds = CornerBottomRight,
            
            DiagonalLeftToTop = 12,
            DiagonalLeftToBottom = 13,
            DiagonalRightToTop = 14,
            DiagonalRightToBottom = 15,
            
            DiagonalBlockTypeStarts = DiagonalLeftToTop,
            DiagonalBlockTypeEnds = DiagonalRightToBottom,
            
            DiagonalSupportLeftToTop = 16,
            DiagonalSupportLeftToBottom = 17,
            DiagonalSupportRightToTop = 18,
            DiagonalSupportRightToBottom = 19,
            
            DiagonalSupportBlockTypeStarts = DiagonalSupportLeftToTop,
            DiagonalSupportBlockTypeEnds = DiagonalSupportRightToBottom,
        }
        
        [Header("Floor Settings")]
        [SerializeField] private Transform floorRoot;
        [SerializeField] private Grid floorGrid;
        [SerializeField] private CornerPrefabsSetup cornerPrefabsSetup;
        [SerializeField] private EdgeBlockPrefabsSetup edgeBlockPrefabsSetup;
        [SerializeField] private CenterBlockPrefabsSetup centerBlockPrefabsSetup;
        [SerializeField] private DiagonalBlockPrefabsSetup diagonalBlockPrefabsSetup;
        [SerializeField] private DiagonalSupportBlockPrefabsSetup diagonalSupportBlockPrefabsSetup;
        
        [Header("General Settings")]
        [SerializeField] private Transform previewRoot;
        [SerializeField] private int seed = 3242563;
        [SerializeField] private bool debugDraw = false;
        
        private GridBlockType[][] _floorGrid;
        private RectInt _floorGridBounds;

        private RectInt IdentifyGridSize()
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minZ = int.MaxValue, maxZ = int.MinValue;
            
            foreach (var childFloor in floorRoot)
            {
                var posOfTile = floorGrid.WorldToCell(((Transform) childFloor).position);
                
                minX = Mathf.Min(minX, posOfTile.x);
                maxX = Mathf.Max(maxX, posOfTile.x);
                minZ = Mathf.Min(minZ, posOfTile.y);
                maxZ = Mathf.Max(maxZ, posOfTile.y);
            }
            
            return new RectInt(minX, minZ, maxX - minX + 1, maxZ - minZ + 1);
        }

        private void BuildFloorGridData()
        {
            _floorGrid = new GridBlockType[_floorGridBounds.width][];
            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                _floorGrid[x] = new GridBlockType[_floorGridBounds.height];
            }
            
            foreach (var childFloor in floorRoot)
            {
                var posOfTile = floorGrid.WorldToCell(((Transform) childFloor).position);
                int gridX = posOfTile.x - _floorGridBounds.xMin;
                int gridZ = posOfTile.y - _floorGridBounds.yMin;
                _floorGrid[gridX][gridZ] = GridBlockType.Normal;
            }
            
            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                for (int z = 0; z < _floorGridBounds.height; z++)
                {
                    if (_floorGrid[x][z] == GridBlockType.Normal)
                    {
                        bool hasTop = z < _floorGridBounds.height - 1 && _floorGrid[x][z + 1] != GridBlockType.None;
                        bool hasBottom = z > 0  && _floorGrid[x][z - 1] != GridBlockType.None;
                        bool hasLeft = x > 0 && _floorGrid[x - 1][z] != GridBlockType.None;
                        bool hasRight = x < _floorGridBounds.width - 1 && _floorGrid[x + 1][z] != GridBlockType.None;

                        if (!hasBottom && !hasLeft)
                            _floorGrid[x][z] = GridBlockType.CornerBottomLeft;
                        else if (!hasBottom && !hasRight)
                            _floorGrid[x][z] = GridBlockType.CornerBottomRight;
                        else if (!hasTop && !hasLeft)
                            _floorGrid[x][z] = GridBlockType.CornerTopLeft;
                        else if (!hasTop && !hasRight)
                            _floorGrid[x][z] = GridBlockType.CornerTopRight;
                        else if (!hasTop)
                            _floorGrid[x][z] = GridBlockType.EdgeTop;
                        else if (!hasBottom)
                            _floorGrid[x][z] = GridBlockType.EdgeBottom;
                        else if (!hasLeft)
                            _floorGrid[x][z] = GridBlockType.EdgeLeft;
                        else if (!hasRight)
                            _floorGrid[x][z] = GridBlockType.EdgeRight;
                    }
                }
            }
            
            // Another run for the diagonals
            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                for (int z = 0; z < _floorGridBounds.height; z++)
                {
                    // If its edge block, we will check to create additional diagonal block for polish
                    if (_floorGrid[x][z] == GridBlockType.EdgeLeft)
                    {
                        if (x > 0 && _floorGrid[x - 1][z] == GridBlockType.None)
                        {
                            if (z < _floorGridBounds.height - 1 && _floorGrid[x - 1][z + 1] == GridBlockType.EdgeBottom)
                            {
                                _floorGrid[x - 1][z] = GridBlockType.DiagonalLeftToBottom; // right to bottom diagonal
                                _floorGrid[x][z] = GridBlockType.DiagonalSupportLeftToBottom;
                                _floorGrid[x - 1][z + 1] = GridBlockType.DiagonalSupportLeftToBottom;
                            }
                            else if (z > 0 && _floorGrid[x - 1][z - 1] == GridBlockType.EdgeTop)
                            {
                                _floorGrid[x - 1][z] = GridBlockType.DiagonalLeftToTop; // right to top diagonal
                                _floorGrid[x][z] = GridBlockType.DiagonalSupportLeftToTop;
                                _floorGrid[x - 1][z - 1] = GridBlockType.DiagonalSupportLeftToTop;
                            }
                        }
                    } else if (_floorGrid[x][z] == GridBlockType.EdgeRight)
                    {
                        if (x < _floorGridBounds.width - 1 && _floorGrid[x + 1][z] == GridBlockType.None)
                        {
                            if (z < _floorGridBounds.height - 1 && _floorGrid[x + 1][z + 1] == GridBlockType.EdgeBottom)
                            {
                                _floorGrid[x + 1][z] = GridBlockType.DiagonalRightToBottom; // left to bottom diagonal
                                _floorGrid[x][z] = GridBlockType.DiagonalSupportRightToBottom;
                                _floorGrid[x + 1][z + 1] = GridBlockType.DiagonalSupportRightToBottom;
                            }
                            else if (z > 0 && _floorGrid[x + 1][z - 1] == GridBlockType.EdgeTop)
                            {
                                _floorGrid[x + 1][z] = GridBlockType.DiagonalRightToTop; // left to top diagonal
                                _floorGrid[x][z] = GridBlockType.DiagonalSupportRightToTop;
                                _floorGrid[x + 1][z - 1] = GridBlockType.DiagonalSupportRightToTop;
                            }
                        }
                    }
                }
            }
        }

        private void InstantiateFloor(int x, int z, GridBlockType blockType)
        {
            var offsetAddSpawnPos = floorGrid.cellSize / 2;
            offsetAddSpawnPos.y = 0;
            var spawnPos =
                floorGrid.CellToWorld(new Vector3Int(x + _floorGridBounds.xMin, z + _floorGridBounds.yMin, 0)) +
                offsetAddSpawnPos;

            if (blockType == GridBlockType.Normal)
            {
                var randomBlockIndex = UnityEngine.Random.Range(0, centerBlockPrefabsSetup.centerBlockPrefabs.Count);
                var randomRotationIndex = UnityEngine.Random.Range(0, 4);
                
                var prefab = centerBlockPrefabsSetup.centerBlockPrefabs[randomBlockIndex];
                var rotation = Quaternion.Euler(0, randomRotationIndex * 90, 0);
                
                Instantiate(prefab, spawnPos, rotation, previewRoot);
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
                
                var obj = Instantiate(prefab, spawnPos, rotation, previewRoot);
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
                
                var obj = Instantiate(prefab, spawnPos, rotation, previewRoot);
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
                
                var obj = Instantiate(prefab, spawnPos, rotation, previewRoot);
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
                
                var obj = Instantiate(prefab, spawnPos, rotation, previewRoot);
                obj.transform.localScale = Vector3.one;
                return;
            }
        }
        
        private void RebuildFloor()
        {
            UnityEngine.Random.InitState(seed);
            
            // Clear previous preview
            for (int i = previewRoot.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(previewRoot.GetChild(i).gameObject);
            }

            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                for (int z = 0; z < _floorGridBounds.height; z++)
                {
                    if (_floorGrid[x][z] != GridBlockType.None)
                    {
                        InstantiateFloor(x, z, _floorGrid[x][z]);
                    }
                }
            }
        }

        [Button]
        public void Polish()
        {
            _floorGridBounds = IdentifyGridSize();
            BuildFloorGridData();
            RebuildFloor();
        }

        private void OnDrawGizmos()
        {
            if (!debugDraw) return;
            if (_floorGrid == null) return;
            
            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                for (int z = 0; z < _floorGridBounds.height; z++)
                {
                    Vector3 worldPos = floorGrid.CellToWorld(new Vector3Int(x + _floorGridBounds.xMin, z + _floorGridBounds.yMin, 0));
                    worldPos += floorGrid.cellSize / 2;
                    
                    if (_floorGrid[x][z] != GridBlockType.None)
                    {
                        switch (_floorGrid[x][z])
                        {
                            case GridBlockType.Normal:
                                Gizmos.color = Color.white;
                                break;
                            case GridBlockType.EdgeTop:
                            case GridBlockType.EdgeBottom:
                            case GridBlockType.EdgeLeft:
                            case GridBlockType.EdgeRight:
                                Gizmos.color = Color.yellow;
                                Handles.Label(worldPos, _floorGrid[x][z].ToString());
                                break;
                            case GridBlockType.CornerTopLeft:
                            case GridBlockType.CornerTopRight:
                            case GridBlockType.CornerBottomLeft:
                            case GridBlockType.CornerBottomRight:
                                Gizmos.color = Color.red;
                                Handles.Label(worldPos, _floorGrid[x][z].ToString());
                                break;
                            case GridBlockType.DiagonalLeftToTop:
                            case GridBlockType.DiagonalLeftToBottom:
                            case GridBlockType.DiagonalRightToTop:
                            case GridBlockType.DiagonalRightToBottom:
                                Gizmos.color = Color.blue;
                                Handles.Label(worldPos, _floorGrid[x][z].ToString());
                                break;
                        }
                        
                        Gizmos.DrawWireCube(worldPos, floorGrid.cellSize);
                    }
                }
            }
        }
    }
}