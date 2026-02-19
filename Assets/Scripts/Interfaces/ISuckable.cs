using UnityEngine;

namespace BlackHole.Interfaces
{
    public enum SuckableObjectCurrentState
    {
        Idle,
        Attracted,
        Sucked,
        Gone
    }

    public interface ISuckable
    {
        void AllowSuckBy(LayerMask suckableLayer);
        void DisableSuckBy(LayerMask suckableLayer);
        
        void SetAttract();
        void SetNoLongerAttract();
        
        SuckableObjectCurrentState CurrentState { get; }
        
        event System.Action<ISuckable, SuckableObjectCurrentState> OnSuckableStateChanged;
    }
}