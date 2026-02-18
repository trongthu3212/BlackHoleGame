using System;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    [SerializeField] private float speedMove = 3f;
    [SerializeField] private Rigidbody rb;

    private Vector3 _moveVector;
    
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
        rb.MovePosition(rb.position + _moveVector * (speedMove * Time.fixedDeltaTime));
        _moveVector = Vector3.zero;
    }
}
