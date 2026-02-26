using System.Collections.Generic;
using System.Linq;
using BlackHole.Data;
using BlackHole.Interfaces;
using BlackHole.Utilities;
using Newtonsoft.Json;
using SaintsField;
using Unity.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace BlackHole.Spawner
{
    [System.Serializable]
    internal struct SuckableSpawnRectJsonData
    {
        public SuckableSpawnSerializeEntry[] elements;
        public float xSpacing;
        public float zSpacing;
        public bool isInterleaved;
        public float sizeX;
        public float sizeZ;
        public float elementScale;
    }
    
    public class SuckableSpawnRect : ISuckableSpawnLogic
    {
        [SerializeReference, ReferencePicker] private List<ISuckableSpawnLogic> elements;
        [SerializeField] private float xSpacing;
        [SerializeField] private float zSpacing;
        [SerializeField] private bool isInterleaved;
        [SerializeField] private Vector2 size;
        [SerializeField] private float elementScale;
        
        public void Execute(SuckableSpawnArgument argument)
        {
            if (elements == null)
            {
                return;
            }
            
            var countX = Mathf.RoundToInt(size.x * argument.scale / xSpacing);
            var countZ = Mathf.RoundToInt(size.y * argument.scale / zSpacing);
            
            for (var i = 0; i < countX; i++)
            {
                for (var j = 0; j < countZ; j++)
                {
                    var randomElementIndex = Random.Range(0, elements.Count);
                    var eachElement = elements[randomElementIndex];
                    
                    var offsetX = (i + 0.5f) * xSpacing - size.x * argument.scale / 2 + (isInterleaved ? (j % 2) * xSpacing / 2 : 0);
                    var offsetZ = (j + 0.5f) * zSpacing - size.y * argument.scale / 2;
                    
                    var elementArgument = new SuckableSpawnArgument
                    {
                        position = argument.position + new Vector3(offsetX, 0, offsetZ),
                        scale = elementScale,
                        parent = argument.parent,
                        initialRotate = argument.initialRotate,
                        suckableObjectManager = argument.suckableObjectManager
                    };
                    
                    eachElement.Execute(elementArgument);
                }
            }
        }
        
        public void DrawGizmos(SuckableSpawnArgument argument)
        {
            // Draw rectangle boundary
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(argument.position, new Vector3(size.x * argument.scale, 0.1f, size.y * argument.scale));
            
            if (elements == null)
            {
                return;
            }

            Gizmos.color = Color.blue;
            
            var countX = Mathf.RoundToInt(size.x * argument.scale / xSpacing);
            var countZ = Mathf.RoundToInt(size.y * argument.scale / zSpacing);

            var bounds = new Bounds(argument.position, new Vector3(size.x * argument.scale, 0.1f, size.y * argument.scale));
            
            for (var i = 0; i < countX; i++)
            {
                for (var j = 0; j < countZ; j++)
                {
                    var randomElementIndex = Random.Range(0, elements.Count);
                    var eachElement = elements[randomElementIndex];

                    var offsetX = (i + 0.5f) * xSpacing - size.x * argument.scale / 2 + (isInterleaved ? (j % 2) * xSpacing / 2 : 0);
                    var offsetZ = (j + 0.5f) * zSpacing - size.y * argument.scale / 2;
                    
                    if (!bounds.Contains(argument.position + new Vector3(offsetX, 0, offsetZ)))
                    {
                        continue;
                    }
                    
                    var elementArgument = new SuckableSpawnArgument
                    {
                        position = argument.position + new Vector3(offsetX, 0, offsetZ),
                        scale = elementScale,
                        parent = argument.parent,
                        initialRotate = argument.initialRotate,
                        suckableObjectManager = argument.suckableObjectManager
                    };

                    eachElement.DrawGizmos(elementArgument);
                }
            }
        }

        public SuckableSpawnSerializeEntry SerializeJson()
        {
            var data = new SuckableSpawnRectJsonData
            {
                elements = elements.Select(x => x.SerializeJson()).ToArray(),
                xSpacing = xSpacing,
                zSpacing = zSpacing,
                isInterleaved = isInterleaved,
                sizeX = size.x,
                sizeZ = size.y,
                elementScale = elementScale
            };
            
            return SuckableSpawnSerializeEntry.Pack(SuckableSpawnType.Rect, data);
        }

        public void DeserializeFromJson(SuckableSpawnSerializeEntry data)
        {
            var jsonData = JsonConvert.DeserializeObject<SuckableSpawnRectJsonData>(data.content.ToString());
            xSpacing = jsonData.xSpacing;
            zSpacing = jsonData.zSpacing;
            isInterleaved = jsonData.isInterleaved;
            size = new Vector2(jsonData.sizeX, jsonData.sizeZ);
            elementScale = jsonData.elementScale;
            elements = new List<ISuckableSpawnLogic>();
            foreach (var obj in jsonData.elements)
            {
                elements.Add(SuckableSpawnFactory.CreateFromJson(obj));
            }
        }
    }
}