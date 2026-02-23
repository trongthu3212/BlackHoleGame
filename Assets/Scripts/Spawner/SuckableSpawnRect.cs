using BlackHole.Data;
using BlackHole.Interfaces;
using SaintsField;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableSpawnRect : ISuckableSpawnLogic
    {
        [SerializeReference, ReferencePicker] private ISuckableSpawnLogic eachElement;
        [SerializeField] private float xSpacing;
        [SerializeField] private float zSpacing;
        [SerializeField] private bool isInterleaved;
        [SerializeField] private Vector2 size;
        
        public void Execute(SuckableSpawnArgument argument)
        {
            if (eachElement == null)
            {
                return;
            }
            var countX = Mathf.RoundToInt(size.x * argument.scale / xSpacing);
            var countZ = Mathf.RoundToInt(size.y * argument.scale / zSpacing);
            
            for (var i = 0; i < countX; i++)
            {
                for (var j = 0; j < countZ; j++)
                {
                    var offsetX = i * xSpacing - size.x * argument.scale / 2 + (isInterleaved ? (j % 2) * xSpacing / 2 : 0);
                    var offsetZ = j * zSpacing - size.y * argument.scale / 2;
                    
                    var elementArgument = new SuckableSpawnArgument
                    {
                        position = argument.position + new Vector3(offsetX, 0, offsetZ),
                        scale = 1f,
                        parent = argument.parent,
                        initialRotate = argument.initialRotate
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
            
            if (eachElement == null)
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
                    var offsetX = (i + 0.5f) * xSpacing - size.x * argument.scale / 2 + (isInterleaved ? (j % 2) * xSpacing / 2 : 0);
                    var offsetZ = (j + 0.5f) * zSpacing - size.y * argument.scale / 2;
                    
                    if (!bounds.Contains(argument.position + new Vector3(offsetX, 0, offsetZ)))
                    {
                        continue;
                    }
                    
                    var elementArgument = new SuckableSpawnArgument
                    {
                        position = argument.position + new Vector3(offsetX, 0, offsetZ),
                        scale = 1f,
                        parent = argument.parent,
                        initialRotate = argument.initialRotate
                    };

                    eachElement.DrawGizmos(elementArgument);
                }
            }
        }
    }
}