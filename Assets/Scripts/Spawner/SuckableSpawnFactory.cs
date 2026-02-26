using BlackHole.Interfaces;

namespace BlackHole.Spawner
{
    public static class SuckableSpawnFactory
    {
        public static ISuckableSpawnLogic CreateFromJson(SuckableSpawnSerializeEntry entry)
        {
            ISuckableSpawnLogic spawnLogic = entry.type switch
            {
                SuckableSpawnType.RandomSingle => new SuckableSpawnRandomSingle(),
                SuckableSpawnType.Single => new SuckableSpawnSingle(),
                SuckableSpawnType.Rect => new SuckableSpawnRect(),
                SuckableSpawnType.Stack => new SuckableSpawnStack(),
                _ => throw new System.ArgumentOutOfRangeException()
            };
            
            spawnLogic.DeserializeFromJson(entry);
            return spawnLogic;
        }
    }
}