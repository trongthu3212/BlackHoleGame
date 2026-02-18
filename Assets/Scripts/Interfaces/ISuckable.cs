using UnityEngine;

namespace BlackHole.Interfaces
{
    public interface ISuckable
    {
        void AllowSuckBy(LayerMask suckableLayer);
        void DisableSuckBy(LayerMask suckableLayer);
    }
}