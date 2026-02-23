using BlackHole.Data;
using BlackHole.Spawner;

namespace BlackHole.Interfaces
{
    public interface ISuckableSpawnLogic
    {
        public void Execute(SuckableSpawnArgument argument);
        
        public void DrawGizmos(SuckableSpawnArgument argument);
    }
}