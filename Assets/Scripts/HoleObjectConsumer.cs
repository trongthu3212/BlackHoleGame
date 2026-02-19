using System;
using UnityEngine;

namespace BlackHole
{
    public class HoleObjectConsumer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var suckable = other.GetComponentInParent<Interfaces.ISuckable>();
            if (suckable != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
}