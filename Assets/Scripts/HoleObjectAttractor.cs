using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace BlackHole
{
    public class HoleObjectAttractor : MonoBehaviour
    {
        [SerializeField] private LayerMask attractTargetLayer;
        [SerializeField] private float attractForce = 10f;
        [SerializeField] private float attractCenterY = -0.5f;
        [SerializeField] private Rigidbody selfRigidbody;
        [SerializeField] private AnimationCurve suckScaleCurve;
        [SerializeField] private float maxAttractDistance = 5f;
        
        private readonly HashSet<Rigidbody> _targetAttractObjects = new HashSet<Rigidbody>();

        [Button]
        private void FillVariables()
        {
            selfRigidbody = GetComponentInParent<Rigidbody>();
            var sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
                maxAttractDistance = sphereCollider.radius * transform.localScale.x;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ((attractTargetLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                var rigidBody = other.attachedRigidbody;
                if (rigidBody != null)
                {
                    _targetAttractObjects.Add(rigidBody);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if ((attractTargetLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                if (other.attachedRigidbody != null)
                {
                    _targetAttractObjects.Remove(other.attachedRigidbody);
                }
            }
        }
        
        private void FixedUpdate()
        {
            HashSet<Rigidbody> removeAttractRbs = new HashSet<Rigidbody>();
            
            foreach (var targetAttractRb in _targetAttractObjects)
            {
                if (targetAttractRb == null)
                {
                    removeAttractRbs.Add(targetAttractRb);
                }
            }
            
            foreach (var removeAttractObject in removeAttractRbs)
            {
                _targetAttractObjects.Remove(removeAttractObject);
            }
            
            if (_targetAttractObjects.Count > 0)
            {
                foreach (var targetRb in _targetAttractObjects)
                {
                    if (targetRb == null)
                    {
                        continue;
                    }

                    targetRb.isKinematic = false;

                    var attractToPos = selfRigidbody.position;
                    attractToPos.y = attractCenterY;

                    var distanceToHoleV = targetRb.transform.position - attractToPos;
                    distanceToHoleV.y = 0;
                    
                    var distanceToHole = distanceToHoleV.magnitude;
                    var samplePos = Mathf.Clamp01(distanceToHole / maxAttractDistance);
                    var attractScale = suckScaleCurve.Evaluate(samplePos);
                    var forceApply = attractForce * attractScale;
                    
                    Vector3 directionToHole = (attractToPos - targetRb.transform.position).normalized;
                    targetRb.AddForce(directionToHole * forceApply, ForceMode.Acceleration);
                    
                    Debug.DrawLine(targetRb.transform.position, attractToPos, Color.red);
                }
            }
        }
    }
}