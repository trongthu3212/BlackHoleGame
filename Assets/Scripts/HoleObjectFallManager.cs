using System;
using System.Collections.Generic;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole
{
    public class HoleObjectFallManager : MonoBehaviour
    {
        private readonly HashSet<GameObject> _exitedObjects = new HashSet<GameObject>();
        
        [SerializeField] private LayerMask suckableLayer;
        [SerializeField] private LayerMask holeLayer;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_exitedObjects.Contains(other.gameObject))
            {
                _exitedObjects.Remove(other.gameObject);
                return;
            }
            
            if ((suckableLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                var suckable = other.GetComponent<ISuckable>();
                if (suckable != null)
                {
                    suckable.AllowSuckBy(holeLayer);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if ((suckableLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                var suckable = other.GetComponent<ISuckable>();
                
                if (suckable != null)
                {
                    _exitedObjects.Add(other.gameObject);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_exitedObjects.Count > 0)
            {
                foreach (var exitedObject in _exitedObjects)
                {
                    if (exitedObject == null)
                    {
                        continue;
                    }
                    var suckable = exitedObject.GetComponent<ISuckable>();
                    if (suckable != null)
                    {
                        suckable.DisableSuckBy(holeLayer);
                    }
                }
                _exitedObjects.Clear();
            }
        }
    }
}