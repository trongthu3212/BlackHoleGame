using BlackHole.Data;
using BlackHole.Spawner;
using Unity.Serialization.Json;

namespace BlackHole.Interfaces
{
    public interface ISuckableSpawnLogic
    {
        public void Execute(SuckableSpawnArgument argument);
        
        public void DrawGizmos(SuckableSpawnArgument argument);

        public SuckableSpawnSerializeEntry SerializeJson();
        public void DeserializeFromJson(SuckableSpawnSerializeEntry data);
    }
}