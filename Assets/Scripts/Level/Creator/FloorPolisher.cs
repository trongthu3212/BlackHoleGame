using System;
using System.Collections.Generic;
using BlackHole.Level;
using SaintsField.Playa;
using UnityEditor;
using UnityEngine;

namespace BlackHole.LevelCreator
{
    public class FloorPolisher
    {
        private GridBlockType[][] _floorGrid;
        private RectInt _floorGridBounds;
        private Transform _floorRoot;
        private Grid _floorGridComp;
        
        public GridBlockType[][] FloorGrids => _floorGrid;
        public RectInt FloorGridBounds => _floorGridBounds;

        public void Initialize(Transform floorRoot, Grid floorGrid)
        {
            _floorRoot = floorRoot;
            _floorGridComp = floorGrid;
        }
        
        private RectInt IdentifyGridSize()
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minZ = int.MaxValue, maxZ = int.MinValue;
            
            foreach (var childFloor in _floorRoot)
            {
                var posOfTile = _floorGridComp.WorldToCell(((Transform) childFloor).position);
                
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
            
            foreach (var childFloor in _floorRoot)
            {
                var posOfTile = _floorGridComp.WorldToCell(((Transform) childFloor).position);
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
        
        public void ExecutePolish()
        {
            _floorGridBounds = IdentifyGridSize();
            BuildFloorGridData();
        }

        public void TryDrawGizmos()
        {
            if (_floorGrid == null) return;
            
            for (int x = 0; x < _floorGridBounds.width; x++)
            {
                for (int z = 0; z < _floorGridBounds.height; z++)
                {
                    Vector3 worldPos = _floorGridComp.CellToWorld(new Vector3Int(x + _floorGridBounds.xMin, z + _floorGridBounds.yMin, 0));
                    worldPos += _floorGridComp.cellSize / 2;
                    
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
                        
                        Gizmos.DrawWireCube(worldPos, _floorGridComp.cellSize);
                    }
                }
            }
        }
    }
}