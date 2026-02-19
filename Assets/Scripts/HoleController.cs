using System.Collections.Generic;
using BlackHole.Interfaces;
using UnityEngine;

namespace BlackHole
{
    public class HoleController : MonoBehaviour
    {
        [SerializeField] private float speedMove = 3f;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private LayerMask suckableLayer;
        [SerializeField] private LayerMask holeLayer;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private float skinWidth = 0.01f;
        
        private Vector3 _moveVector;
        private HashSet<GameObject> _exitedObjects = new HashSet<GameObject>();

        // Update is called once per frame
        void Update()
        {
            _moveVector = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                _moveVector += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _moveVector += Vector3.back;
            }
        
            if (Input.GetKey(KeyCode.A))
            {
                _moveVector += Vector3.left;
            }
        
            if (Input.GetKey(KeyCode.D))
            {
                _moveVector += Vector3.right;
            }
        
            _moveVector.Normalize();
        }

        private void FixedUpdate()
        {
            if (_moveVector != Vector3.zero)
            {
                float moveDistance = speedMove * Time.fixedDeltaTime;
                
                // Check for walls before moving
                if (rb.SweepTest(_moveVector, out RaycastHit hit, moveDistance + skinWidth))
                {
                    // Only stop if we hit a wall layer
                    if ((wallLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                    {
                        moveDistance = Mathf.Max(0f, hit.distance - skinWidth);
                    }
                }
                
                rb.MovePosition(rb.position + _moveVector * moveDistance);
            }
            
            _moveVector = Vector3.zero;
            
            if (_exitedObjects.Count > 0)
            {
                foreach (var exitedObject in _exitedObjects)
                {
                    var suckable = exitedObject.GetComponent<ISuckable>();
                    if (suckable != null)
                    {
                        suckable.DisableSuckBy(holeLayer);
                    }
                }
                _exitedObjects.Clear();
            }
        }

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
    }
}