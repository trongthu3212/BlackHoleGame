using BlackHole.Data;
using UnityEngine;

namespace BlackHole.Spawner
{
    public class SuckableSpawnRect : MonoBehaviour
    {
        [SerializeField] private GameObject targetGameObject;
        [SerializeField] private SuckableObjectId objectIdArgument;
    }
}