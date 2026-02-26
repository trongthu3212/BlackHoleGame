using BlackHole.Utilities;
using Newtonsoft.Json.Linq;

namespace BlackHole.Spawner
{
    public enum SuckableSpawnType
    {
        RandomSingle = 0,
        Single = 1,
        Rect = 2,
        Stack = 3
    }
    
    [System.Serializable]
    public struct SuckableSpawnSerializeEntry
    {
        public SuckableSpawnType type;
        public JObject content;
        
        public static SuckableSpawnSerializeEntry Pack<T>(SuckableSpawnType spawnType, T data) where T : struct
        {
            return new SuckableSpawnSerializeEntry()
            {
                type = spawnType,
                content = JObject.FromObject(data)
            };
        }
    }
}