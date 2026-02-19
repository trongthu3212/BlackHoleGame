using System;
using System.Collections.Generic;
using System.Linq;
using BlackHole.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace BlackHole
{
    public class HoleObjectAttractor : MonoBehaviour
    {
        struct AttractEntry : IEquatable<AttractEntry>
        {
            public Rigidbody TargetRb;
            public ISuckable Suckable;

            public bool Equals(AttractEntry other)
            {
                return Equals(TargetRb, other.TargetRb) && Equals(Suckable, other.Suckable);
            }

            public override bool Equals(object obj)
            {
                return obj is AttractEntry other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(TargetRb, Suckable);
            }
        }
        
        [SerializeField] private LayerMask attractTargetLayer;
        [SerializeField] private float attractForce = 10f;
        [SerializeField] private float attractCenterY = -0.5f;
        [SerializeField] private Rigidbody selfRigidbody;
        [SerializeField] private AnimationCurve suckScaleCurve;
        [SerializeField] private float maxAttractDistance = 5f;
        [SerializeField] private bool debugDraw = true;
        
        private readonly List<AttractEntry> _targetAttractObjects = new List<AttractEntry>();

        private void OnSuckableStateChanged(ISuckable suckable, SuckableObjectCurrentState newState)
        {
            if (newState == SuckableObjectCurrentState.Gone)
            {
                var entryIndex = _targetAttractObjects.FindIndex(entry => entry.Suckable == suckable);
                if (entryIndex >= 0)
                {
                    _targetAttractObjects.RemoveAt(entryIndex);
                }
                
                suckable.OnSuckableStateChanged -= OnSuckableStateChanged;
            }
            else if (newState == SuckableObjectCurrentState.Idle)
            {
                if (_targetAttractObjects.Any(x => x.Suckable == suckable))
                {
                    suckable.SetAttract();
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ((attractTargetLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                var rigidBody = other.attachedRigidbody;
                var suckable = other.GetComponentInParent<ISuckable>();
                if (rigidBody != null && suckable != null)
                {
                    _targetAttractObjects.Add(new AttractEntry
                    {
                        TargetRb = rigidBody,
                        Suckable = suckable
                    });
                    
                    suckable.OnSuckableStateChanged += OnSuckableStateChanged;
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if ((attractTargetLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                if (other.attachedRigidbody != null)
                {
                    var entryIndex = _targetAttractObjects.FindIndex(entry => entry.TargetRb == other.attachedRigidbody);
                    if (entryIndex >= 0)
                    {
                        var entry = _targetAttractObjects[entryIndex];
                        _targetAttractObjects.RemoveAt(entryIndex);
                        
                        entry.Suckable.SetNoLongerAttract();
                        entry.Suckable.OnSuckableStateChanged -= OnSuckableStateChanged;
                    }
                }
            }
        }

        private Vector3 CalculateAttractDirection(Rigidbody currentRb, Rigidbody targetRb)
        {
            var attractToPos = currentRb.position;
            attractToPos.y = attractCenterY;

            return (attractToPos - targetRb.transform.position).normalized;
        }
        
        private void FixedUpdate()
        {
            for (int i = _targetAttractObjects.Count - 1; i >= 0; i--)
            {
                var entry = _targetAttractObjects[i];
                if (entry.TargetRb == null || entry.Suckable == null || entry.Suckable.CurrentState == SuckableObjectCurrentState.Gone)
                {
                    _targetAttractObjects.RemoveAt(i);
                    if (entry.Suckable != null)
                    {
                        entry.Suckable.OnSuckableStateChanged -= OnSuckableStateChanged;
                    }
                }
            }
            
            if (_targetAttractObjects.Count > 0)
            {
                foreach (var entry in _targetAttractObjects)
                {
                    var targetRb = entry.TargetRb;
                    
                    if (targetRb == null)
                    {
                        continue;
                    }

                    targetRb.isKinematic = false;

                    var distanceToHoleV = targetRb.position - selfRigidbody.position;
                    distanceToHoleV.y = 0;
                    
                    var distanceToHole = distanceToHoleV.magnitude;
                    var samplePos = Mathf.Clamp01(distanceToHole / maxAttractDistance);
                    var attractScale = suckScaleCurve.Evaluate(samplePos);
                    var forceApply = attractForce * attractScale;
                    
                    Vector3 directionToHole = CalculateAttractDirection(selfRigidbody, targetRb);
                    targetRb.AddForce(directionToHole * forceApply, ForceMode.Acceleration);
                    
                    entry.Suckable.SetAttract();
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!debugDraw)
            {
                return;
            }
            
            Gizmos.color = Color.red;
            foreach (var entry in _targetAttractObjects)
            {
                if (entry.TargetRb != null)
                {
                    Gizmos.DrawLine(entry.TargetRb.position, selfRigidbody.position);
                }
            }
        }
        
        [Button]
        private void FillVariables()
        {
            selfRigidbody = GetComponentInParent<Rigidbody>();
            var sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
                maxAttractDistance = sphereCollider.radius * transform.localScale.x;
        }
    }
}