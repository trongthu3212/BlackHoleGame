using System;
using System.Collections.Generic;
using BlackHole.Interfaces;
using SaintsField.Playa;
using UnityEngine;

namespace BlackHole
{
    public class SuckableObjectController: MonoBehaviour, ISuckable
    {
        [SerializeField] private Rigidbody providedRigidbody;
        [SerializeField] private LayerMask defaultCollisionLayers;
        [SerializeField] private List<Collider> providedColliders;

        private SuckableObjectCurrentState _currentState = SuckableObjectCurrentState.Idle;
        public SuckableObjectCurrentState CurrentState => _currentState;

        #if UNITY_EDITOR
        public SuckableObjectCurrentState CurrentStateMirror;
        #endif
        
        public event Action<ISuckable, SuckableObjectCurrentState> OnSuckableStateChanged;
        
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
            
            _currentState = SuckableObjectCurrentState.Idle;
        }

        private void OnDestroy()
        {
            ChangeState(SuckableObjectCurrentState.Gone);
        }

        private void ChangeState(SuckableObjectCurrentState newState)
        {
            if (_currentState != newState)
            {
                _currentState = newState;
                OnSuckableStateChanged?.Invoke(this, _currentState);
                
                #if UNITY_EDITOR
                CurrentStateMirror = _currentState;
                #endif
            }
        }
        
        public void AllowSuckBy(LayerMask suckableLayer)
        {
            if (_currentState == SuckableObjectCurrentState.Sucked)
            {
                Debug.LogWarning("Trying to allow suck by " + suckableLayer + " but already sucked by another layer");
                return;
            }
            
            if (_currentState == SuckableObjectCurrentState.Gone)
            {
                Debug.LogWarning("Trying to allow suck by " + suckableLayer + " but object is already gone");
                return;
            }
            
            var targetLayerMask = (providedRigidbody.includeLayers & ~defaultCollisionLayers)| suckableLayer;

            providedRigidbody.includeLayers = targetLayerMask;
            providedRigidbody.excludeLayers = 0;

            providedRigidbody.isKinematic = false;
        
            foreach (var providedCollider in providedColliders)
            {
                providedCollider.includeLayers = targetLayerMask;
                providedCollider.excludeLayers = 0;
            }
            
            ChangeState(SuckableObjectCurrentState.Sucked);
        }

        public void DisableSuckBy(LayerMask suckableLayer)
        {
            if (_currentState != SuckableObjectCurrentState.Sucked)
            {
                Debug.LogWarning("Trying to disable suck by " + suckableLayer + " but not currently sucked by any layer");
                return;
            }
            
            var targetLayerMask = (providedRigidbody.includeLayers & ~suckableLayer) | defaultCollisionLayers;

            providedRigidbody.includeLayers = targetLayerMask;
            providedRigidbody.excludeLayers = 0;
            
            foreach (var providedCollider in providedColliders)
            {
                providedCollider.includeLayers = targetLayerMask;
                providedCollider.excludeLayers = 0;
            }
            
            ChangeState(SuckableObjectCurrentState.Idle);
        }

        public void SetAttract()
        {
            if (_currentState == SuckableObjectCurrentState.Idle)
            {
                ChangeState(SuckableObjectCurrentState.Attracted);
            }
        }
        
        public void SetNoLongerAttract()
        {
            if (_currentState == SuckableObjectCurrentState.Attracted)
            {
                // Revert it to idle
                ChangeState(SuckableObjectCurrentState.Idle);
            }
            else
            {
                // It's being sucked up, dont override the state
            }
        }
        
        [Button]
        public void AutoFillVariables()
        {
            if (providedRigidbody == null)
            {
                providedRigidbody = GetComponent<Rigidbody>();
            }
            
            providedColliders = new List<Collider>(GetComponentsInChildren<Collider>());
        }
    }
}