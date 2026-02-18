using System;
using System.Collections.Generic;
using BlackHole.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace BlackHole
{
    public class SuckableObjectController: MonoBehaviour, ISuckable
    {
        [SerializeField] private Rigidbody providedRigidbody;
        [SerializeField] private LayerMask defaultCollisionLayers;
        [SerializeField] private List<Collider> providedColliders;

        private void Awake()
        {
            providedRigidbody.includeLayers = defaultCollisionLayers;
            providedRigidbody.excludeLayers = 0;
            
            providedRigidbody.isKinematic = true;
            
            foreach (var providedCollider in providedColliders)
            {
                providedCollider.enabled = true;
                providedCollider.includeLayers = defaultCollisionLayers;
                providedCollider.excludeLayers = 0;
            }
        }
        public void AllowSuckBy(LayerMask suckableLayer)
        {
            var targetLayerMask = (providedRigidbody.includeLayers & ~defaultCollisionLayers)| suckableLayer;

            providedRigidbody.includeLayers = targetLayerMask;
            providedRigidbody.excludeLayers = 0;

            providedRigidbody.isKinematic = false;
        
            foreach (var providedCollider in providedColliders)
            {
                providedCollider.includeLayers = targetLayerMask;
                providedCollider.excludeLayers = 0;
            }
        }

        public void DisableSuckBy(LayerMask suckableLayer)
        {
            var targetLayerMask = (providedRigidbody.includeLayers & ~suckableLayer) | defaultCollisionLayers;

            providedRigidbody.includeLayers = targetLayerMask;
            providedRigidbody.excludeLayers = 0;
            
            foreach (var providedCollider in providedColliders)
            {
                providedCollider.includeLayers = targetLayerMask;
                providedCollider.excludeLayers = 0;
            }
        }
        
        [Button]
        public void AutoFillRigidbody()
        {
            if (providedRigidbody == null)
            {
                providedRigidbody = GetComponent<Rigidbody>();
            }
            
            providedColliders = new List<Collider>(GetComponentsInChildren<Collider>());
        }
    }
}